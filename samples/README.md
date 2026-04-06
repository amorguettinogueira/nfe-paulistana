# Nfe.Paulistana — Sample de integração

Este repositório demonstra como usar a biblioteca [`Nfe.Paulistana`](https://github.com/amorguettinogueira/Nfe.Paulistana) para se comunicar com os **Web Services SOAP da NFS-e da Prefeitura de São Paulo**. O objetivo é servir como referência prática: qualquer desenvolvedor pode explorar as ações, ler o código-fonte e replicar os padrões no seu próprio projeto.

> Este projeto **não é** uma suíte de testes automatizada. Todas as chamadas são feitas manualmente via menu interativo de console.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Certificado digital A1 (`.pfx` / e-CNPJ) válido e registrado na Prefeitura de SP
- CNPJ e Inscrição Municipal válidos e cadastrados na Prefeitura de SP
- Acesso à internet (o endpoint SOAP da Prefeitura é público)

## Quick Start

```bash
# 1. Clone o repositório
git clone https://github.com/amorguettinogueira/Nfe.Paulistana.integration.test
cd Nfe.Paulistana.integration.test/Nfe.Paulistana.Integration.Sample

# 2. Inicialize o User Secrets
dotnet user-secrets init

# 3. Abra o secrets.json.example, copie o conteúdo e cole no seu User Secrets
#    No Visual Studio: botão direito no projeto → "Manage User Secrets"
#    No terminal: o comando abaixo abre o caminho do arquivo
dotnet user-secrets list

# 4. Configure os campos obrigatórios (ajuste com seus dados reais)
dotnet user-secrets set "MeuCnpj"                    "00.000.000/0001-00"
dotnet user-secrets set "CnpjDaMinhaFilial"          "00.000.000/0001-00"
dotnet user-secrets set "MinhaInscricaoMunicipal"    "0.000.000-0"
dotnet user-secrets set "Certificado:CaminhoArquivo" "C:\caminho\certificado.pfx"
dotnet user-secrets set "Certificado:Senha"          "sua-senha"
# ... preencha também os campos de cada ação (veja docs/configuracao.md)

# 5. Execute
dotnet run
```

> **Dica:** o arquivo `secrets.json.example` contém todos os campos com valores de exemplo e comentários. Cole o conteúdo inteiro no seu `secrets.json` (User Secrets) e ajuste apenas os campos necessários. Veja [docs/configuracao.md](docs/configuracao.md) para detalhes de cada campo.

## Como funciona

Ao executar, o aplicativo:

1. Carrega e valida todas as configurações dos User Secrets (`AppSettingsLoader`) — encerra com mensagem de erro se algum campo obrigatório estiver ausente
2. Constrói o container de DI e registra os serviços da biblioteca (`ServiceConfigurator`) — configura mTLS com o certificado `.pfx` e pipeline de resiliência
3. Exibe um menu interativo no console (`ConsoleMenuHost`)
4. Executa a ação escolhida e imprime parâmetros enviados e resposta recebida no terminal

```
Program.cs
  └─ AppSettingsLoader.LoadAndValidate()       ← falha → imprime erros e encerra
  └─ ServiceConfigurator.RegisterServices()    ← registra Nfe.Paulistana (mTLS + resiliência)
  └─ ConsoleMenuHost.RunAsync()                ← loop de menu → executa IIntegrationAction
```

## Ações disponíveis

Cada ação (implementação de `IIntegrationAction`) demonstra o uso de um serviço da API. V1 e V2 representam versões distintas do Web Service da Prefeitura. Para entender o payload de cada chamada, leia a classe correspondente em `Actions/` — elas são o principal material de referência deste projeto.

| Ação (Menu) | Classes |
|---|---|
| Cancelamento NFS-e | `CancelNfeV1Action` / `CancelNfeV2Action` |
| Consulta CNPJ | `QueryCnpjV1Action` / `QueryCnpjV2Action` |
| Consulta Informações Lote | `QueryBatchInfoV1Action` / `QueryBatchInfoV2Action` |
| Consulta Lote | `QueryBatchV1Action` / `QueryBatchV2Action` |
| Consulta NFS-e | `QueryNfeV1Action` / `QueryNfeV2Action` |
| Consulta NFS-e Emitidas | `QueryIssuedNfeV1Action` / `QueryIssuedNfeV2Action` |
| Consulta NFS-e Recebidas | `QueryReceivedNfeV1Action` / `QueryReceivedNfeV2Action` |
| Download WSDL | `DownloadWsdlAction` |
| Envio RPS (teste) | `SendRpsTestV1Action` / `SendRpsTestV2Action` |

## Estrutura de pastas

| Pasta | Responsabilidade |
|---|---|
| `Actions/` | Uma classe por operação de integração (`IIntegrationAction`) e o catálogo de ordem do menu (`ActionCatalog`) |
| `Configuration/` | Classes de options fortemente tipadas, mapeadas diretamente dos User Secrets |
| `Host/` | Startup: carregamento e validação de configuração (`AppSettingsLoader`) e wiring do container de DI (`ServiceConfigurator`) |
| `Infrastructure/` | Clientes HTTP auxiliares (ex.: `HttpWsdlFetcher` para download do WSDL) |
| `Presentation/Console/` | Menu interativo (`ConsoleMenuHost`) e utilitários de impressão formatada no terminal |

> Veja o [ADR 0001](docs/adr/0001-convenção-de-nomenclatura.md) para a convenção de nomenclatura e organização de namespaces adotada.

## Usando a biblioteca no seu projeto

Este projeto referencia `Nfe.Paulistana` via referência de projeto direta (conveniência de desenvolvimento). No seu projeto, referencie o pacote NuGet:

```bash
dotnet add package Nfe.Paulistana
```

## Diagnóstico de chamadas SOAP

A biblioteca expõe `SoapDiagnosticsHandler`, um `DelegatingHandler` que intercepta cada troca SOAP e invoca um callback com os XMLs de request/response, o tempo decorrido e o status da operação.

```csharp
services.AddNfePaulistanaAll(options => ...,
    configureClient: b => b
        .AddNfePaulistanaDiagnostics(exchange =>
        {
            Console.WriteLine($"[{exchange.SoapAction}] {exchange.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine(exchange.RequestXml);
            Console.WriteLine(exchange.ResponseXml);
        }));
```

**No Visual Studio:** selecione o perfil **`(Diagnósticos SOAP)`** no dropdown de execução da barra de ferramentas — isso ativa o nível `Debug` no logger `SoapDiagnostics` automaticamente, sem alterar código.

Para integrar com `ILogger` ou personalizar o comportamento, veja [Como inspecionar o XML SOAP](docs/troubleshooting.md#como-inspecionar-o-xml-soap).

## Documentação complementar

- [Configuração detalhada do secrets.json](docs/configuracao.md) — todos os campos com explicações, exemplos e links de referência
- [Troubleshooting](docs/troubleshooting.md) — erros comuns e como resolvê-los
- [Como adicionar uma nova ação](docs/como-adicionar-uma-acao.md) — passo a passo para criar uma nova entrada no menu
- [ADR 0001 — Convenção de nomenclatura](docs/adr/0001-convenção-de-nomenclatura.md)

## Contribuição

Pull requests são bem-vindos. Mantenha o estilo de código consistente com o projeto.

## Licença

Consulte o arquivo de licença do repositório principal [`LICENSE`](../LICENSE) para informações sobre uso e distribuição.