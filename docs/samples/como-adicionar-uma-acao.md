# Como adicionar uma nova ação

Este guia descreve o passo a passo para criar uma nova entrada no menu interativo do sample. O padrão completo envolve até 5 arquivos, mas apenas 2 são obrigatórios em todos os casos.

## Visão geral dos arquivos

| Arquivo | Obrigatório | Finalidade |
|---|---|---|
| `Actions/ActionCatalog.cs` | ✅ sempre | Define a posição (`Order`) e o rótulo do menu (`Description`) |
| `Actions/MinhaNovaAction.cs` | ✅ sempre | Implementa `IIntegrationAction` — lógica da ação |
| `Configuration/MinhaNovaOptions.cs` | Só se a ação precisar de parâmetros específicos | Classe de options fortemente tipada, mapeada dos User Secrets |
| `Configuration/AppSettings.cs` | Só se criou `*Options.cs` | Agrega as options ao objeto central de configuração |
| `secrets.json.example` + User Secrets | Só se criou `*Options.cs` | Persiste os valores de configuração localmente |

> **Registro automático:** o `ServiceConfigurator` descobre todas as implementações de `IIntegrationAction` via reflexão. Não é necessário registrar a nova classe manualmente no container de DI.

---

## Passo 1 — Registrar no catálogo de menu

Abra `Actions/ActionCatalog.cs` e adicione uma classe aninhada ao final da classe `ActionCatalog`:

```csharp
/// <summary>Constantes para a ação de minha nova operação.</summary>
internal static class MinhaNovaAcao
{
    public const int Order = 17; // deve ser único — o startup valida duplicidades
    public const string Description = "Minha Nova Ação";
}
```

> O valor de `Order` determina a posição no menu. Use o próximo inteiro disponível (atualmente `17`). Valores duplicados causam falha imediata no startup com mensagem de erro clara.

---

## Passo 2 — Criar a classe de options (condicional)

Pule este passo se a ação usar apenas `MeuCnpj` / `MinhaInscricaoMunicipal` — esses campos já estão disponíveis em `AppSettings`.

Crie `Configuration/MinhaNovaOptions.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

internal sealed class MinhaNovaOptions
{
    [Required, Range(1, long.MaxValue)]
    public long MeuParametro { get; init; }
}
```

Siga o padrão das classes existentes: `[Required]` em campos obrigatórios, `[Range]` em numéricos, nome da chave User Secrets documentado em XML summary.

---

## Passo 3 — Registrar em AppSettings (condicional)

Se criou `MinhaNovaOptions`, adicione a propriedade em `Configuration/AppSettings.cs` junto às demais:

```csharp
/// <summary>
/// Parâmetros para a ação minha nova operação.
/// Chave User Secrets raiz: <c>MinhaNovaAcao</c>.
/// </summary>
[Required(ErrorMessage = $"{nameof(MinhaNovaAcao)} é obrigatório.")]
public MinhaNovaOptions MinhaNovaAcao { get; init; } = new();
```

---

## Passo 4 — Adicionar ao secrets.json (condicional)

Se criou `MinhaNovaOptions`, adicione os campos ao `secrets.json.example` com valor de exemplo e comentário, e configure seus próprios valores nos User Secrets:

```bash
dotnet user-secrets set "MinhaNovaAcao:MeuParametro" "123"
```

No Visual Studio: botão direito no projeto → **Manage User Secrets** → adicione a chave manualmente no JSON.

---

## Passo 5 — Implementar a ação

Crie `Actions/MinhaNovaAction.cs`. Consulte `Actions/QueryNfeV1Action.cs` como referência canônica do padrão:

```csharp
using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;

namespace Nfe.Paulistana.Integration.Sample.Actions;

internal sealed class MinhaNovaAction(
    AppSettings settings,
    ILogger<MinhaNovaAction> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.MinhaNovaAcao.Order;
    public string Description => ActionCatalog.MinhaNovaAcao.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            // ConsolePresenter.Info($"  Meu Parâmetro: {settings.MinhaNovaAcao.MeuParametro}");

            // TODO: montar pedido e chamar serviço
            await Task.CompletedTask;

            ConsolePresenter.Outcome("Sucesso", true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar MinhaNovaAction");
            ConsolePresenter.Error($"Erro: {ex.Message}");
        }
    }
}
```

**Restrições importantes:**
- A classe é registrada como **Singleton** — não guarde estado mutável em campos de instância.
- `RunAsync` deve sempre propagar o `cancellationToken` às chamadas assíncronas.
- Injete apenas interfaces/abstrações — nunca `new` dependências externas diretamente.

---

## Checklist rápido

- [ ] `ActionCatalog.cs` — classe aninhada com `Order` único e `Description`
- [ ] `*Options.cs` — criada (se necessário)
- [ ] `AppSettings.cs` — propriedade com `[Required]` adicionada (se necessário)
- [ ] `secrets.json.example` + User Secrets — campos adicionados (se necessário)
- [ ] `*Action.cs` — implementa `IIntegrationAction`, referencia `ActionCatalog`, usa `ConsolePresenter`
- [ ] Build passa sem erros
