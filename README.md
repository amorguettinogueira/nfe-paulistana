# Nfe.Paulistana

[![Build](https://img.shields.io/github/actions/workflow/status/amorguettinogueira/nfe-paulistana/ci.yml?branch=main&label=build)](https://github.com/amorguettinogueira/nfe-paulistana/actions)
[![Coverage](https://codecov.io/github/amorguettinogueira/nfe-paulistana/graph/badge.svg?token=RZY4IPJ49W)](https://codecov.io/github/amorguettinogueira/nfe-paulistana)
[![NuGet](https://img.shields.io/nuget/v/Nfe.Paulistana?label=nuget)](https://www.nuget.org/packages/Nfe.Paulistana)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Nfe.Paulistana)](https://www.nuget.org/packages/Nfe.Paulistana)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8_%7C_9_%7C_10-512BD4)](https://dotnet.microsoft.com/download/dotnet/8.0)

Biblioteca .NET para integração com o webservice SOAP da **Nota Fiscal de Serviços Eletrônica Paulistana (NFS-e)** da Prefeitura de São Paulo.

---

## Índice

- [Instalação](#instalação)
- [Pré-requisitos](#pré-requisitos)
- [Registro de serviços](#registro-de-serviços)
- [Resiliência](#resiliência)
- [Diagnóstico](#diagnóstico)
- [Operações disponíveis](#operações-disponíveis)
- [Exemplos de uso](#exemplos-de-uso)
- [Tratamento de erros](#tratamento-de-erros)
- [Arquitetura](#arquitetura)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

---

## Instalação

```bash
dotnet add package Nfe.Paulistana
```

## Pré-requisitos

- .NET 8 ou superior
- Certificado digital A1 (`.pfx` / `.p12`) habilitado para emissão de NFS-e pela Prefeitura de São Paulo

---

## Registro de serviços

A biblioteca expõe três métodos de extensão sobre `IServiceCollection`. Cada método registra os **serviços** (typed `HttpClient`) e as **factories** (singletons para construção de pedidos assinados) da versão correspondente, além de `CertificadoNfePaulistana` como singleton compartilhado:

| Método | Quando usar |
|---|---|
| `AddNfePaulistanaV1` | Integração exclusivamente com o schema v01 |
| `AddNfePaulistanaV2` | Integração exclusivamente com o schema v02 |
| `AddNfePaulistanaAll` | Migração gradual - V1 e V2 coexistindo no mesmo processo |

### Configuração básica

```csharp
// Program.cs
builder.Services.AddNfePaulistanaV1(options =>
{
    options.Certificado.FilePath = "/run/secrets/certificado.pfx";
    options.Certificado.Password  = "senha-do-certificado";
});
```

### Fontes de certificado

`CertificadoNfePaulistana` aceita quatro fontes alternativas. Configure apenas uma delas:

```csharp
// 1. Caminho de arquivo no sistema de arquivos
options.Certificado.FilePath = "/etc/certs/empresa.pfx";
options.Certificado.Password = "senha";

// 2. Bytes brutos (útil com Azure Key Vault / AWS Secrets Manager)
byte[] certBytes = await secretClient.GetSecretValueAsync("certificado");
options.Certificado.RawData = new ReadOnlyCollection<byte>(certBytes);

// 3. Handle nativo (certificado já carregado pelo sistema operacional)
options.Certificado.PointerHandle = handleNativo;

// 4. Instância já construída (o chamador é responsável pelo ciclo de vida)
options.Certificado.Certificate = x509Certificate2Instance;
```

Exclusivamente para o carregamento via sistema de arquivos controle, opcionalmente, o armazenamento da chave privada:

```csharp
options.Certificado.KeyStorageFlags = X509KeyStorageFlags.EphemeralKeySet; // sem persistência em disco
```

### Configuração de endpoint

O endpoint de produção é configurado automaticamente. Para sobrescrever (ex.: testes de integração contra um mock):

```csharp
options.EndpointUrl = new Uri("https://mock-nfe.intranet.exemplo.com/lotenfe.asmx");
```

---

## Resiliência

Todos os `HttpClient` registrados pela biblioteca são configurados de forma uniforme através do parâmetro opcional `configureClient`, que recebe um `Action<IHttpClientBuilder>` aplicado a **cada cliente**:

### Microsoft.Extensions.Http.Resilience (recomendado para .NET 8+)

```bash
dotnet add package Microsoft.Extensions.Http.Resilience
```

```csharp
builder.Services.AddNfePaulistanaV2(
    options =>
    {
        options.Certificado.FilePath = "/run/secrets/certificado.pfx";
        options.Certificado.Password  = "senha";
    },
    configureClient: b => b.AddStandardResilienceHandler());
```

`AddStandardResilienceHandler` configura automaticamente retry com backoff exponencial, timeout por tentativa, circuit breaker e timeout total — tudo alinhado às recomendações do .NET.

Para personalizar as políticas:

```csharp
builder.Services.AddNfePaulistanaV2(
    options => options.Certificado.FilePath = "/run/secrets/certificado.pfx",
    configureClient: b => b.AddResilienceHandler("nfe-pipeline", pipeline =>
    {
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay            = TimeSpan.FromSeconds(2),
            BackoffType      = DelayBackoffType.Exponential,
        });

        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration        = TimeSpan.FromSeconds(30),
            FailureRatio            = 0.5,
            MinimumThroughput       = 5,
            BreakDuration           = TimeSpan.FromSeconds(15),
        });

        pipeline.AddTimeout(TimeSpan.FromSeconds(10));
    }));
```

### Polly

```bash
dotnet add package Microsoft.Extensions.Http.Polly
```

```csharp
using Polly;
using Polly.Extensions.Http;

IAsyncPolicy<HttpResponseMessage> RetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

builder.Services.AddNfePaulistanaV2(
    options => options.Certificado.FilePath = "/run/secrets/certificado.pfx",
    configureClient: b => b.AddPolicyHandler(RetryPolicy()));
```

### Cenário de migração gradual (V1 + V2 registradas simultaneamente)

```csharp
builder.Services.AddNfePaulistanaAll(
    options =>
    {
        options.Certificado.FilePath = "/run/secrets/certificado.pfx";
        options.Certificado.Password  = "senha";
    },
    configureClient: b => b.AddStandardResilienceHandler());
```

---

## Diagnóstico

A biblioteca expõe um handler de diagnóstico SOAP via `AddNfePaulistanaDiagnostics`, que intercepta cada intercâmbio e entrega um `SoapExchange` com os XMLs completos de requisição e resposta, o tempo decorrido e o status HTTP.

### Logging estruturado via ILogger (recomendado)

```csharp
builder.Services.AddNfePaulistanaV2(
    options => options.Certificado.FilePath = "/run/secrets/certificado.pfx",
    configureClient: b => b
        .AddStandardResilienceHandler()
        .AddNfePaulistanaDiagnostics(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            return exchange => logger.LogDebug(
                "[NFS-e] {SoapAction} → {Status} ({ElapsedMs}ms)",
                exchange.SoapAction,
                exchange.IsSuccess ? "OK" : "FALHA",
                (long)exchange.Elapsed.TotalMilliseconds);
        }));
```

> **Ordem importa:** `AddNfePaulistanaDiagnostics` posicionado **após** `AddStandardResilienceHandler` observa apenas o resultado final da cadeia de retries. Posicionado antes, recebe uma notificação por tentativa — útil para métricas de retry.

### Callback direto (útil em testes de integração)

```csharp
configureClient: b => b.AddNfePaulistanaDiagnostics(exchange =>
{
    Console.WriteLine($"Request:\n{exchange.RequestXml}");
    Console.WriteLine($"Response:\n{exchange.ResponseXml}");
})
```

`SoapExchange` expõe: `SoapAction`, `RequestXml`, `ResponseXml`, `Elapsed` e `IsSuccess`.

---

## Operações disponíveis

### V1 — Schema v01 (`AddNfePaulistanaV1`)

| Interface | Operação WSDL | Descrição |
|---|---|---|
| `IEnvioRpsService` | `EnvioRPS` | Envio de RPS unitário |
| `IEnvioLoteRpsService` | `EnvioLoteRPS` | Envio de lote de RPS |
| `ICancelamentoNFeService` | `CancelamentoNFe` | Cancelamento de NFS-e emitida |
| `IConsultaNFeService` | `ConsultaNFe` | Consulta de NFS-e por chave |
| `IConsultaNFeRecebidasService` | `ConsultaNFeRecebidas` | Consulta de NFS-e recebidas por período |
| `IConsultaNFeEmitidasService` | `ConsultaNFeEmitidas` | Consulta de NFS-e emitidas por período |
| `IConsultaLoteService` | `ConsultaLote` | Consulta de lote pelo número |
| `IConsultaInformacoesLoteService` | `ConsultaInformacoesLote` | Informações do lote de envio |
| `IConsultaCNPJService` | `ConsultaCNPJ` | Inscrições municipais vinculadas a um CNPJ |

Namespaces: `Nfe.Paulistana.V1.Services`, `Nfe.Paulistana.V1.Builders`

### V2 — Schema v02 (`AddNfePaulistanaV2`)

| Interface | Operação WSDL | Diferença em relação à V1 |
|---|---|---|
| `IEnvioRpsService` | `EnvioRPS` | Suporte a **IBS/CBS** (reforma tributária), tomador estrangeiro com NIF e endereço exterior |
| `IEnvioLoteRpsService` | `EnvioLoteRPS` | Suporte a **IBS/CBS** (reforma tributária) e envio de eventos fiscais |
| `ICancelamentoNFeService` | `CancelamentoNFe` | Schema v02 (operação equivalente à V1) |
| `IConsultaNFeService` | `ConsultaNFe` | Schema v02 (operação equivalente à V1) |
| `IConsultaNFeRecebidasService` | `ConsultaNFeRecebidas` | Schema v02 (operação equivalente à V1) |
| `IConsultaNFeEmitidasService` | `ConsultaNFeEmitidas` | Schema v02 (operação equivalente à V1) |
| `IConsultaLoteService` | `ConsultaLote` | Schema v02 (operação equivalente à V1) |
| `IConsultaInformacoesLoteService` | `ConsultaInformacoesLote` | Inscrição municipal de **1 a 12 dígitos** |
| `IConsultaCNPJService` | `ConsultaCNPJ` | Suporte a **CNPJ alfanumérico** (Receita Federal 2026) |

Namespaces: `Nfe.Paulistana.V2.Services`, `Nfe.Paulistana.V2.Builders`

---

## Exemplos de uso

Todos os serviços seguem o mesmo padrão: injete a interface do serviço e a **Factory** correspondente (ambos registrados automaticamente) e chame `SendAsync`.

### Envio de RPS unitário (V1)

```csharp
public class EmissaoNfeService(
    IEnvioRpsService envioRpsService,
    PedidoEnvioFactory factory)
{
    public async Task<RetornoEnvioRps> EmitirAsync(DadosNfe dados, CancellationToken ct = default)
    {
        var rps = RpsBuilder
            .New(
                inscricaoPrestador: (InscricaoMunicipal)dados.InscricaoMunicipal,
                tipoRps: TipoRps.Rps,
                numeroRps: (Numero)dados.NumeroRps,
                discriminacao: (Discriminacao)dados.Discriminacao,
                serieRps: (SerieRps)dados.SerieRps)
            .SetNFe(/* ... */)
            .SetServico(/* ... */)
            .SetIss(/* ... */)
            .SetTomador(/* ... */)
            .Build();

        PedidoEnvio pedido = factory.NewCnpj((Cnpj)dados.Cnpj, rps);

        return await envioRpsService.SendAsync(pedido, ct);
    }
}
```

### Consulta de CNPJ alfanumérico (V2)

```csharp
public class ConsultaCnpjService(
    Nfe.Paulistana.V2.Services.IConsultaCNPJService consultaService,
    Nfe.Paulistana.V2.Builders.PedidoConsultaCNPJFactory factory)
{
    public async Task<RetornoConsultaCNPJ> ConsultarAsync(string cnpjAlfa, CancellationToken ct = default)
    {
        var cnpj = new Nfe.Paulistana.V2.Models.DataTypes.Cnpj(cnpjAlfa);
        var im   = new Nfe.Paulistana.V2.Models.DataTypes.InscricaoMunicipal(39616924);

        PedidoConsultaCNPJ pedido = factory.NewCnpj(cnpj, new CpfOrCnpj(cnpj), im);

        return await consultaService.SendAsync(pedido, ct);
    }
}
```

### Consulta de informações de lote (V2)

```csharp
public class InformacoesLoteService(
    Nfe.Paulistana.V2.Services.IConsultaInformacoesLoteService infoService,
    Nfe.Paulistana.V2.Builders.PedidoInformacoesLoteFactory factory)
{
    public async Task<RetornoInformacoesLote> ConsultarAsync(long numeroLote, CancellationToken ct = default)
    {
        var cpf = new Cpf(46381819618L);
        var im  = new Nfe.Paulistana.V2.Models.DataTypes.InscricaoMunicipal(39616924);

        PedidoInformacoesLote pedido = factory.NewCpf(cpf, im, new Numero(numeroLote));

        return await infoService.SendAsync(pedido, ct);
    }
}
```

> Para exemplos end-to-end executáveis de todas as operações — envio de RPS (incluindo campos V2 como IBS/CBS), cancelamento e consultas — explore o [projeto de amostra](samples/README.md) incluído neste repositório.

---

## Tratamento de erros

Os serviços lançam exceções tipadas:

| Exceção | Causa |
|---|---|
| `ArgumentNullException` | Pedido nulo passado a `SendAsync` |
| `InvalidOperationException` | Falha na validação XSD do pedido, ou resposta sem payload válido |
| `HttpRequestException` | Falha de transporte HTTP (resolvida com política de resiliência) |

A resposta do webservice pode indicar falha de negócio através das coleções `Erro` e `Alerta` presentes em cada tipo `Retorno*`. Verifique `Cabecalho.Sucesso` antes de processar os dados:

```csharp
RetornoConsultaCNPJ retorno = await consultaService.SendAsync(pedido, ct);

if (retorno.Cabecalho?.Sucesso == false)
{
    foreach (var erro in retorno.Erro ?? [])
        logger.LogError("NFS-e erro {Codigo}: {Descricao}", erro.Codigo, erro.Descricao);
}
```

---

## Arquitetura

A biblioteca segue **Clean Architecture** com separação explícita entre domínio, aplicação e infraestrutura. Os pontos de destaque:

- **Value Objects fortemente tipados** — cada campo fiscal possui seu próprio tipo (`InscricaoMunicipal`, `Cnpj`, `CodigoNBS`, etc.) com validação de invariantes no construtor. Erros de tipo são capturados em tempo de compilação. Para campos opcionais, todos os tipos de texto expõem `ParseIfPresent(string? value)`: retorna `null` para entradas ausentes e lança `ArgumentException` para valores presentes mas inválidos.
- **Suporte dual V1/V2** — as duas versões do webservice da Prefeitura coexistem no mesmo pacote com namespaces isolados (`Nfe.Paulistana.V1` / `Nfe.Paulistana.V2`), permitindo migração gradual.
- **Assinatura digital embutida** — o pipeline de assinatura XML (xmldsig) é transparente ao consumidor; a biblioteca assina automaticamente RPS e pedidos de cancelamento usando o certificado configurado.
- **Validação XSD embarcada** — todos os schemas `.xsd` são recursos embarcados (`EmbeddedResource`); a validação ocorre antes do envio, sem dependência de arquivos externos.
- **Builder fluente** — pedidos complexos são construídos via interface fluente (`RpsBuilder`, `EventoBuilder`) que guia o consumidor pelos campos obrigatórios e opcionais.

Para detalhes de implementação e decisões de design, consulte:

| Documento | Conteúdo |
|---|---|
| [`src/README.md`](src/README.md) | Estrutura de pastas, namespaces e padrões da biblioteca |
| [`docs/adr/0002`](docs/adr/0002-value-objects-tipagem-forte.md) | Decisão: Value Objects para campos fiscais |
| [`docs/adr/0003`](docs/adr/0003-suporte-dual-v1-v2.md) | Decisão: suporte dual V1/V2 |
| [`docs/adr/0004`](docs/adr/0004-assinatura-digital-e-validacao-xsd.md) | Decisão: assinatura digital e validação XSD |

---

## Contribuindo

Contribuições são bem-vindas. Antes de abrir um PR, leia:

- [`docs/adr/`](docs/adr/) — decisões arquiteturais registradas; entenda o *porquê* das escolhas antes de propor alterações
- [`tests/README.md`](tests/README.md) — convenções de teste e como executar a suíte
- Toda contribuição deve incluir testes correspondentes seguindo o padrão `Método_Estado_ResultadoEsperado`

> ⚠️ Este repositório exige assinatura de CLA antes de aceitar contribuições externas.

---

## Licença

Este projeto é licenciado sob a [MIT License](LICENSE).
Resumindo: você pode usar livremente, inclusive em projetos comerciais,
desde que mantenha o aviso de copyright.