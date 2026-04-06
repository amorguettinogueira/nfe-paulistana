# ADR 0001 — Convenção de nomenclatura e organização de namespaces/pastas

**Status:** Aceito
**Data:** 2025-01-01

---

## Contexto

O repositório contém três projetos com responsabilidades distintas:

- **`src/`** (`Nfe.Paulistana`) — biblioteca principal distribuída via NuGet
- **`tests/`** (`Nfe.Paulistana.Tests`) — suíte de testes unitários
- **`samples/`** (`Nfe.Paulistana.Integration.Sample`) — aplicação console de referência para integração manual

Para garantir coerência, descoberta de código e aplicação consistente de Clean Architecture nos três projetos, é necessária uma convenção explícita e documentada.

---

## Decisão

Adotar as convenções abaixo como padrão obrigatório em todo o repositório.

### Regras gerais

- `AssemblyName`, `RootNamespace` e nome de pasta raiz seguem o mesmo valor por projeto.
- Namespace = `RootNamespace` + caminho relativo da pasta (cada segmento em **PascalCase**).
- Pastas em PascalCase; arquivos em PascalCase com sufixo que expressa responsabilidade.
- Nenhuma abreviação não consagrada nos nomes de tipo.

---

### Projeto `src/` — `Nfe.Paulistana`

| Pasta | Namespace | Conteúdo |
|---|---|---|
| `Abstractions/` | `Nfe.Paulistana.Abstractions` | Interfaces de alto nível (`INfeService`) |
| `Constants/` | `Nfe.Paulistana.Constants` | URIs e constantes de operação WSDL |
| `Diagnostics/` | `Nfe.Paulistana.Diagnostics` | `SoapDiagnosticsHandler`, `SoapExchange` |
| `Exceptions/` | `Nfe.Paulistana.Exceptions` | Exceções tipadas (`NfeRequestException`) |
| `Extensions/` | `Nfe.Paulistana.Extensions` | `ServiceCollectionExtensions`, `HttpClientBuilderExtensions`, `StringExtensions` |
| `Infrastructure/` | `Nfe.Paulistana.Infrastructure` | `SoapClient`, `SoapEnvelope`, `SoapBody`, base de assinatura |
| `Models/DataTypes/` | `Nfe.Paulistana.Models.DataTypes` | Value Objects compartilhados entre V1 e V2 |
| `Models/Enums/` | `Nfe.Paulistana.Models.Enums` | Enumerações compartilhadas |
| `Models/Validators/` | `Nfe.Paulistana.Models.Validators` | Validadores transversais |
| `Options/` | `Nfe.Paulistana.Options` | `NfeOptions`, `Certificado` |
| `Xml/` | `Nfe.Paulistana.Xml` | `SchemaProvider`, `ValidationHelper`, `SerializationExtensions` |
| `V1/**` | `Nfe.Paulistana.V1.<SubPasta>` | Implementação completa do schema v01 |
| `V2/**` | `Nfe.Paulistana.V2.<SubPasta>` | Implementação completa do schema v02 |

**Sufixos de arquivo:**

| Responsabilidade | Sufixo |
|---|---|
| Interface de serviço público | `I<Operação>Service` |
| Implementação de serviço | `<Operação>Service` |
| Builder fluente (interface) | `IRpsSet<Etapa>` |
| Builder fluente (implementação) | `RpsBuilder`, `EventoBuilder`, etc. |
| Factory de pedido | `Pedido<Operação>Factory` |
| Value Object | nome do campo fiscal (sem sufixo) |
| Envelope SOAP | `<Operação>Request` / `<Operação>Response` |

---

### Projeto `tests/` — `Nfe.Paulistana.Tests`

- A estrutura de pastas **espelha `src/`** exatamente.
- Namespace = `Nfe.Paulistana.Tests` + caminho relativo.
- Todo arquivo de teste termina em `Tests` (ex.: `CodigoPaisISOTests.cs`).
- Helpers de dados de teste ficam em `Helpers/` e não contêm assertions.

---

### Projeto `samples/` — `Nfe.Paulistana.Integration.Sample`

| Pasta | Namespace | Conteúdo |
|---|---|---|
| `Actions/` | `...Integration.Sample.Actions` | `IIntegrationAction`, `*Action`, `ActionCatalog` |
| `Configuration/` | `...Integration.Sample.Configuration` | Classes de options, `AppSettings` |
| `Host/` | `...Integration.Sample.Host` | `AppSettingsLoader`, `ServiceConfigurator` |
| `Infrastructure/` | `...Integration.Sample.Infrastructure` | `HttpWsdlFetcher` e clients externos |
| `Presentation/Console/` | `...Integration.Sample.Presentation.Console` | `ConsoleMenuHost`, `ConsolePresenter`, `ConsolePrinter` |

---

## Consequências

**Positivas:**
- Navegação intuitiva: conhecer o namespace resolve imediatamente a pasta.
- Novos colaboradores localizam código sem auxílio.
- Aplicação uniforme de Clean Architecture em todos os projetos.

**Negativas:**
- Refactoring inicial necessário quando a convenção não é seguida.
- Estrutura mais verbosa que projetos simples — justificável pela complexidade do domínio fiscal.

---

## Observações

Para renomear pastas/namespaces em larga escala, use o script PowerShell com `git mv` + substituição de namespace para preservar o histórico Git:

```powershell
git mv src\OldFolder src\NewFolder
Get-ChildItem src\NewFolder -Recurse -Filter "*.cs" |
    ForEach-Object { (Get-Content $_.FullName) -replace 'OldNamespace', 'NewNamespace' | Set-Content $_.FullName }
```
