# Nfe.Paulistana — Testes

Este documento descreve a estratégia de testes, a organização da suíte e como executá-la.

---

## Como executar

```bash
# Todos os testes
dotnet test

# Com relatório de cobertura (requer coverlet)
dotnet test --collect:"XPlat Code Coverage"

# Filtro por categoria
dotnet test --filter "FullyQualifiedName~V2.Models.DataTypes"
```

No Visual Studio: **Test Explorer → Run All**.

---

## Números

| Métrica | Valor |
|---|---|
| Total de testes | 1590 |
| Framework | xUnit 2.9+ / .NET 8 |
| Biblioteca de mock | NSubstitute |
| Assertions | xUnit `Assert.*` exclusivamente |

> ⚠️ **FluentAssertions não é utilizado neste projeto.** A partir da versão 8.x a biblioteca adotou licença comercial Xceed, incompatível com uso comercial. Use `Assert.*` do xUnit em todas as contribuições.

---

## Estrutura de pastas

A estrutura de pastas dos testes espelha a do projeto `src/`, facilitando a navegação:

```
tests/
├── Diagnostics/           # SoapDiagnosticsHandlerTests
├── Exceptions/            # NfeRequestExceptionTests
├── Extensions/            # ServiceCollectionExtensionsTests, StringExtensionsTests
├── Helpers/               # Dados de teste compartilhados (CPFs e CNPJs válidos)
├── Infrastructure/        # CertificadoNfePaulistanaTests
├── Models/
│   ├── DataTypes/         # Testes de Value Objects compartilhados
│   └── Validators/        # TributacaoDataEmissaoValidatorTests
├── Xml/                   # SchemaProviderTests, ValidationHelperTests
├── V1/
│   ├── Builders/          # RpsBuilderTests, TomadorBuilderTests, etc.
│   ├── Helpers/           # CNPJs válidos específicos de V1
│   ├── Models/Domain/     # Testes de entidades de domínio V1
│   ├── Models/DataTypes/  # Testes de Value Objects V1
│   └── Services/          # Testes de serviço V1 (com HttpClient mockado)
└── V2/
    ├── Builders/          # RpsBuilderTests, EventoBuilderTests, etc.
    ├── Helpers/           # CNPJs válidos específicos de V2
    ├── Models/Domain/     # Testes de entidades de domínio V2
    ├── Models/DataTypes/  # Testes de Value Objects V2
    └── Services/          # Testes de serviço V2 (com HttpClient mockado)
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
| **Services** | `SendAsync` com `HttpClient` mockado via NSubstitute — resposta de sucesso, erro HTTP, exceção de negócio |
| **Infrastructure** | Carregamento de certificado, geração de assinatura XML |
| **Xml** | Carregamento de schemas XSD, validação de documentos válidos e inválidos |
| **Diagnostics** | Callback de diagnóstico — isolamento de falha no callback, métricas de tempo |
| **Extensions** | Registro correto de dependências no container de DI |

## O que NÃO é testado aqui

| Camada | Motivo |
|---|---|
| Integração real com o webservice da Prefeitura | Requer certificado digital e credenciais reais — coberto pelo projeto `samples/` |
| Autenticação mTLS em nível de rede | Responsabilidade do `HttpClient` runtime do .NET |
