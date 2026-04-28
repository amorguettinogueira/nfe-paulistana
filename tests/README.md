# Nfe.Paulistana — Testes

Este documento descreve a estratégia de testes, a organização da suíte e como executá-la.

---

## Estratégia de testes em três níveis

| Nível | Onde vive | Certificado | Rede | Propósito |
|---|---|---|---|---|
| **Tier 1 — Unitários** | `tests/` | — | — | Valor Objects, builders, serviços com HttpClient mockado |
| **Tier 2 — Integração estrutural** | `tests/Integration/` | Auto-assinado (gerado em memória) | — | Pipeline completo: build → assinar → validar XSD |
| **Tier 3 — Integração real** | `integration-tests/` | A1 pessoal (User Secrets) | ✔ endpoint da Prefeitura | Regressão fim-a-fim antes de cada release |

Os **Tier 1 e 2** rodam no CI sem nenhuma configuração adicional. O **Tier 3** é local e salta automaticamente quando os User Secrets não estão presentes.

---

## Como executar

```bash
# Tier 1 + 2 — todos os testes do projeto principal
dotnet test tests/Nfe.Paulistana.Tests.csproj

# Com relatório de cobertura (requer coverlet)
dotnet test tests/Nfe.Paulistana.Tests.csproj --collect:"XPlat Code Coverage"

# Filtro por categoria
dotnet test tests/Nfe.Paulistana.Tests.csproj --filter "FullyQualifiedName~V2.Models.DataTypes"

# Tier 3 — integração real (requer User Secrets configurados; pula graciosamente se ausentes)
dotnet test integration-tests/Nfe.Paulistana.IntegrationTests.csproj -c Release
```

No Visual Studio: **Test Explorer → Run All** executa Tier 1 e 2. O Tier 3 aparece como **Skipped** até que os User Secrets sejam preenchidos.

---

## Números

| Métrica | Valor |
|---|---|
| Total de testes (Tier 1 + 2) | > 2.6k |
| Framework | xUnit 2.9.3 / .NET 8, 9, 10 |
| Assertions | xUnit `Assert.*` exclusivamente |
| Testes Tier 3 | 4 (1 por combinação V1/V2 × envio/conectividade) |

> ⚠️ **FluentAssertions não é utilizado neste projeto.** A partir da versão 8.x a biblioteca adotou licença comercial Xceed, incompatível com uso comercial. Use `Assert.*` do xUnit em todas as contribuições.

---

## Estrutura de pastas

A estrutura de pastas dos testes espelha a do projeto `src/`, facilitando a navegação:

```
tests/                           ← Tier 1 (unitários) + Tier 2 (integração estrutural)
├── Diagnostics/             # SoapDiagnosticsHandlerTests
├── Exceptions/              # NfeRequestExceptionTests
├── Extensions/              # ServiceCollectionExtensionsTests, StringExtensionsTests
├── Fixtures/                # CertificadoFixture — gera cert RSA-2048 auto-assinado em memória
├── Helpers/                 # Dados de teste compartilhados (CPFs e CNPJs válidos)
├── Infrastructure/          # CertificadoNfePaulistanaTests, ElementSignatureGeneratorBaseTests, MensagemXmlTests
├── Integration/             # ← Tier 2: pipeline completo sem rede
│   ├── V1/
│   │   ├── EnvioRpsPipelineTests.cs      # build → assinar → validar XSD (RPS unitário V1)
│   │   └── EnvioLoteRpsPipelineTests.cs  # build → assinar → validar XSD (lote V1)
│   └── V2/
│       ├── EnvioRpsPipelineTests.cs      # idem para V2, inclui campos IBS/CBS e NBS
│       └── EnvioLoteRpsPipelineTests.cs  # idem para lote V2
├── Models/
│   ├── DataTypes/           # Testes de Value Objects compartilhados
│   └── Validators/          # TributacaoDataEmissaoValidatorTests
├── Options/                 # CertificadoTests (opções de configuração)
├── Xml/                     # SchemaProviderTests, ValidationHelperTests
├── V1/
│   ├── Builders/            # RpsBuilderTests, TomadorBuilderTests, etc.
│   ├── Helpers/             # CNPJs válidos específicos de V1
│   ├── Infrastructure/      # RpsSignatureGeneratorTests
│   ├── Models/Domain/       # Testes de entidades de domínio V1
│   ├── Models/DataTypes/    # Testes de Value Objects V1
│   └── Services/            # Testes de serviço V1 (com HttpClient mockado)
└── V2/
    ├── Builders/            # RpsBuilderTests, EventoBuilderTests, etc.
    ├── Helpers/             # CNPJs válidos específicos de V2
    ├── Infrastructure/      # CancelamentoSignatureGeneratorTests, RpsSignatureGeneratorTests
    │   └── Envelope/        # EnvioLoteRpsRequest/ResponseTests, TesteEnvioLoteRpsRequest/ResponseTests
    ├── Models/Domain/       # Testes de entidades de domínio V2
    ├── Models/DataTypes/    # Testes de Value Objects V2
    └── Services/            # Testes de serviço V2 (com HttpClient mockado)

integration-tests/               ← Tier 3: integração real com endpoint da Prefeitura
├── Configuration/           # IntegrationTestSettings — chaves dos User Secrets documentadas
├── Fixtures/                # LocalIntegrationFixture — carrega User Secrets e constrói DI
├── V1/
│   ├── TesteEnvioLoteRpsTests.cs   # envia RPS real via TesteEnvioLoteRPS (modoTeste: true)
│   └── ConectividadeTests.cs       # ConsultaCNPJ V1 — smoke test de autenticação mTLS
└── V2/
    ├── TesteEnvioLoteRpsTests.cs   # idem para V2, com campos IBS/CBS
    └── ConectividadeTests.cs       # ConsultaCNPJ V2
```

---

## Convenções

### Nomenclatura

```
Método_Estado_ResultadoEsperado
```

Exemplos:

```
Constructor_ValidValue_ShouldSetValue
Constructor_InvalidValue_ShouldThrowArgumentException
SendAsync_ValidRequest_ShouldReturnSuccessResponse
FromString_NullValue_ShouldThrowArgumentNullException
```

### Estrutura interna (Arrange-Act-Assert)

```csharp
[Fact]
public void Constructor_ValidValue_ShouldSetValue()
{
    // Arrange
    const string value = "AB";

    // Act
    var codigo = new CodigoPaisISO(value);

    // Assert
    Assert.Equal(value, codigo.ToString());
}
```

### Parametrização com `[Theory]`

Casos de erro com entrada `null` usam `Assert.ThrowsAny<ArgumentException>` (aceita tipos derivados como `ArgumentNullException`). Casos sem `null` usam `Assert.Throws<ArgumentException>` (exact match):

```csharp
[Theory]
[InlineData(null)]           // → ArgumentNullException (derivado de ArgumentException)
[InlineData("")]             // → ArgumentException
[InlineData("123")]          // → ArgumentException
public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
{
    Action act = () => _ = new CodigoPaisISO(value!);
    Assert.ThrowsAny<ArgumentException>(act);
}
```

---

## O que é testado

| Camada | O que é coberto |
|---|---|
| **Value Objects** | Happy path, todos os casos de borda (null, vazio, tamanho, regex, dígito verificador), serialização/desserialização XML |
| **Domain** | Construção, validação de invariantes, operadores implícitos/explícitos |
| **Builders** | Fluxo fluente completo, combinações obrigatórias vs. opcionais |
| **Services** | `SendAsync` com `HttpClient` mockado via `FakeHttpMessageHandler` — resposta de sucesso, erro HTTP, exceção de negócio |
| **Infrastructure** | Carregamento de certificado, geração de assinatura XML, serialização de mensagens SOAP |
| **Infrastructure V1/V2** | Geração de assinatura de RPS e cancelamento, serialização/desserialização de envelopes SOAP |
| **Options** | Validação de opções de configuração (`Certificado`) |
| **Xml** | Carregamento de schemas XSD, validação de documentos válidos e inválidos |
| **Diagnostics** | Callback de diagnóstico — isolamento de falha no callback, métricas de tempo |
| **Extensions** | Registro correto de dependências no container de DI |
| **Integration/V1 e V2 (Tier 2)** | Pipeline completo — build → assinatura com cert auto-assinado → validação XSD embutida, campos exclusivos V2 (`<NBS>`, `<IBSCBS>`), assinaturas individuais por RPS no lote |

## O que NÃO é testado no projeto `tests/`

| O quê | Onde está coberto |
|---|---|
| Integração real com o endpoint da Prefeitura | `integration-tests/` — requer User Secrets com certificado A1 pessoal |
| Autenticação mTLS em nível de rede | Responsabilidade do `HttpClient` runtime do .NET |

---

## Tier 3 — Integração real (`integration-tests/`)

O projeto `Nfe.Paulistana.IntegrationTests` envia requisições reais ao endpoint de teste da Prefeitura (`TesteEnvioLoteRPS` com `modoTeste: true`) e verifica a resposta. Como a Prefeitura de SP não disponibiliza ambiente de homologação, esse endpoint é o único ponto de validação fim-a-fim disponível — processa e valida o RPS sem emitir NFS-e definitiva.

As configurações sensíveis (caminho do `.pfx`, senha, CNPJ) são armazenadas em **User Secrets do .NET** e nunca no repositório. Os testes pulam automaticamente quando os segredos estão ausentes.

### Configurar os segredos

```bash
cd integration-tests

dotnet user-secrets set "Certificado:CaminhoArquivo" "C:\caminho\para\cert.pfx"
dotnet user-secrets set "Certificado:Senha"          "sua-senha"
dotnet user-secrets set "CnpjPrestador"              "12345678000190"
dotnet user-secrets set "InscricaoMunicipalPrestador" "12345678"
dotnet user-secrets set "CnpjTomador"                "98765432000110"
dotnet user-secrets set "RazaoSocialTomador"         "Empresa Tomadora Ltda"
```

Os campos de RPS (`V1:NumeroRps`, `V2:NumeroRps`, etc.) têm valores padrão razoáveis e só precisam ser sobrescritos se houver conflito de numeração. Veja a documentação completa das chaves em [`integration-tests/Configuration/IntegrationTestSettings.cs`](../integration-tests/Configuration/IntegrationTestSettings.cs).

### Executar

```bash
dotnet test integration-tests/Nfe.Paulistana.IntegrationTests.csproj -c Release
```

O `tag-release.bat` executa este comando automaticamente antes de criar cada tag de versão.
