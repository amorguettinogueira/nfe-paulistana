# Nfe.Paulistana — Biblioteca

Este documento descreve a arquitetura interna, os padrões de design e a organização de namespaces da biblioteca `Nfe.Paulistana`.

---

## Estrutura de pastas

```
src/
├── Abstractions/          # Interfaces públicas de alto nível (INfeService, INfeLoteRpsService)
├── Constants/             # URIs de endpoint e constantes de operação WSDL
├── Diagnostics/           # Pipeline de diagnóstico SOAP (SoapExchange, SoapDiagnosticsHandler)
├── Exceptions/            # Exceções tipadas do domínio (NfeRequestException)
├── Extensions/            # Extensões de IServiceCollection, IHttpClientBuilder, string
├── Infrastructure/        # Cliente SOAP, envelope XML, assinatura digital (base)
├── Models/
│   ├── DataTypes/         # Value Objects compartilhados (Cpf, Cnpj, Email, Valor, etc.)
│   ├── Enums/             # Enumerações compartilhadas (StatusNfe, TipoRps)
│   ├── Validators/        # Validadores de regras de negócio transversais
│   └── Interfaces/        # Contratos internos (ICpfOrCnpj, ISignedElement, IXmlValidatableSchema)
├── Options/               # Classes de configuração (NfeOptions, Certificado)
├── Xml/                   # Validação XSD (SchemaProvider, ValidationHelper, SerializationExtensions)
├── V1/                    # Implementação completa do schema v01
│   ├── Builders/          # Builders fluentes de pedido (RpsBuilder, TomadorBuilder, etc.)
│   ├── Infrastructure/    # Envelopes SOAP e geradores de assinatura específicos da V1
│   ├── Models/            # DTOs de request/response, domínio e operações do schema v01
│   └── Services/          # Implementações de serviço V1 + interfaces públicas
└── V2/                    # Implementação completa do schema v02 (espelha a estrutura da V1)
    ├── Builders/
    ├── Infrastructure/
    ├── Models/
    └── Services/
```

---

## Namespaces

| Namespace | Conteúdo |
|---|---|
| `Nfe.Paulistana` | Ponto de entrada — extensões de DI (`ServiceCollectionExtensions`) |
| `Nfe.Paulistana.Abstractions` | `INfeService`, `INfeLoteRpsService` |
| `Nfe.Paulistana.Models.DataTypes` | Value Objects compartilhados entre V1 e V2 |
| `Nfe.Paulistana.V1.Services` | Interfaces e implementações de serviço V1 |
| `Nfe.Paulistana.V1.Builders` | Factories e builders fluentes para pedidos V1 |
| `Nfe.Paulistana.V2.Services` | Interfaces e implementações de serviço V2 |
| `Nfe.Paulistana.V2.Builders` | Factories e builders fluentes para pedidos V2 |
| `Nfe.Paulistana.Options` | `NfeOptions`, `Certificado` |
| `Nfe.Paulistana.Diagnostics` | `SoapDiagnosticsHandler`, `SoapExchange` |

---

## Padrões de design

### Value Objects

Todos os campos fiscais são representados como tipos de valor herdando de `XmlSerializableDataType<T>`. Cada tipo encapsula sua regra de validação no construtor e é imutável.

```
XmlSerializableDataType<T>
    └─ ConstrainedString<T>          ← strings com regex/tamanho
        └─ CodigoNBS                 ← [0-9]{9}
        └─ CodigoPaisISO             ← [A-Z]{2}
        └─ IdentificacaoObra         ← [0-9A-Z]{30}
        └─ ...
    └─ ConstrainedDecimal<T>         ← decimais com escala e intervalo
        └─ Valor, Aliquota, ...
    └─ ModulusElevenValidatedNumber  ← CPF e CNPJ com dígito verificador
        └─ Cpf, Cnpj
```

Nenhum `string` nu circula no domínio. Invariantes são garantidas na construção, não nos consumidores. Veja [ADR 0002](../docs/adr/0002-value-objects-tipagem-forte.md).

### Builder fluente

Pedidos complexos são construídos via interface fluente que guia o chamador pelos campos obrigatórios em sequência segura — a ausência de qualquer etapa é um erro de compilação:

```csharp
var rps = RpsBuilder
    .New(inscricaoPrestador, tipoRps, numero, discriminacao, serie)
    .SetNFe(/* ... */)
    .SetServico(/* ... */)
    .SetIss(/* ... */)
    .SetTomador(/* ... */)
    .Build();
```

Cada `.Set*()` retorna a próxima interface da cadeia (`IRpsSetNfe → IRpsSetServico → ...`).

### Pipeline SOAP

```
Chamador
  └─ IXxxService.SendAsync(pedido)
      └─ ValidationHelper.Validate(pedido)    ← XSD embarcado
      └─ IElementSignatureGenerator.Sign()    ← xmldsig com certificado A1/A3
      └─ SoapClient.SendAsync(envelope)       ← HttpClient typed com mTLS
          └─ SoapDiagnosticsHandler           ← intercepta para logging/diagnóstico
      └─ XmlSerializer.Deserialize(response)  ← tipado para Retorno*
```

### Suporte dual V1/V2

V1 e V2 compartilham os Value Objects de `Models/DataTypes/` mas têm namespaces, builders e services completamente independentes. O consumidor registra somente a versão necessária. Veja [ADR 0003](../docs/adr/0003-suporte-dual-v1-v2.md).

---

## Extensibilidade

### Diagnóstico / logging

```csharp
builder.Services.AddNfePaulistanaV2(
    options => options.Certificado.FilePath = "/certs/empresa.pfx",
    configureClient: b => b.AddNfePaulistanaDiagnostics(exchange =>
    {
        logger.LogDebug("SOAP {Operation} → {Status} ({Duration}ms)",
            exchange.OperationName, exchange.StatusCode, exchange.ElapsedMs);
    }));
```

### Endpoint customizado

```csharp
options.EndpointUrl = new Uri("https://mock-nfe.intranet/lotenfe.asmx");
```

Útil para apontar a um mock server (WireMock, Mockoon) em testes de integração.

---

## Recursos embarcados

Os schemas XSD de V1 e V2 são compilados como `EmbeddedResource` no assembly. O `SchemaProvider` carrega e armazena em cache cada schema por nome na primeira utilização, eliminando dependências de arquivos externos em qualquer ambiente.
