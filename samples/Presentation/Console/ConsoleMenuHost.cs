using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Integration.Sample.Actions;

namespace Nfe.Paulistana.Integration.Sample.Presentation.Console;

/// <summary>
/// Host do menu interativo em console para a aplicação NF-e Paulistana.
/// Resolve todas as implementações de <see cref="IIntegrationAction"/> via
/// <paramref name="provider"/>, ordena-as por <see cref="IIntegrationAction.MenuOrder"/>
/// em tempo de construção e as executa serialmente sob demanda do usuário.
/// </summary>
/// <param name="provider">Container de DI usado para resolver e ordenar todas as implementações de <see cref="IIntegrationAction"/>.</param>
internal sealed class ConsoleMenuHost(IServiceProvider provider)
{
    private readonly IList<IIntegrationAction> _actions = [.. provider.GetServices<IIntegrationAction>().OrderBy(a => a.MenuOrder)];

    /// <summary>
    /// Executa o loop interativo do menu até o usuário selecionar "0"/Enter ou o token ser cancelado.
    /// <para><b>Validação de startup:</b> verifica unicidade de <see cref="IIntegrationAction.MenuOrder"/>
    /// antes de renderizar o menu; retorna <c>1</c> imediatamente se houver duplicidade de ordem,
    /// imprimindo os conflitos no console via <see cref="ConsolePresenter.Error"/>.</para>
    /// <para><b>Cancelamento:</b> cria um <see cref="System.Threading.CancellationTokenSource"/> vinculado a
    /// <paramref name="cancellationToken"/> e registra <c>Console.CancelKeyPress</c> para
    /// sinalizar cancelamento gracioso (Ctrl+C) sem encerrar o processo imediatamente
    /// (<c>e.Cancel = true</c>). O handler <b>não é desregistrado</b> ao término;
    /// este método deve ser invocado apenas uma vez por ciclo de vida da aplicação.</para>
    /// <para><b>Execução de ações:</b> serial, uma por vez. O token vinculado é propagado
    /// para <see cref="IIntegrationAction.RunAsync"/>. O tempo decorrido é medido via
    /// <see cref="System.Diagnostics.Stopwatch"/> e exibido ao término de cada ação.</para>
    /// <para><b>Rótulos de menu:</b> <see cref="IIntegrationAction.MenuOrder"/> é convertido
    /// em rótulo alfabético por base-26 bijetiva (0→A, 25→Z, 26→AA…) via <c>LabelFromIndex</c>.
    /// A correspondência na leitura de input é case-insensitive.</para>
    /// </summary>
    /// <param name="cancellationToken">Token externo de cancelamento; vinculado ao CTS interno.</param>
    /// <returns>
    /// <c>0</c> em saída normal (opção "0", Enter vazio ou token cancelado);
    /// <c>1</c> se <see cref="IIntegrationAction.MenuOrder"/> duplicado for detectado.
    /// </returns>
    public async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        // Validate duplicate MenuOrder
        var duplicates = _actions.GroupBy(a => a.MenuOrder).Where(g => g.Count() > 1).ToList();

        if (duplicates.Count > 0)
        {
            ConsolePresenter.Error("Erro: valores duplicados de MenuOrder detectados nas ações:");
            foreach (var g in duplicates)
            {
                ConsolePresenter.Error($"  Ordem {g.Key}: {string.Join(", ", g.Select(x => x.GetType().Name))}");
            }

            return 1;
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        System.Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            System.Console.WriteLine("\nCancelando... aguardando término das operações.");
        };

        while (!cts.IsCancellationRequested)
        {
            ConsolePresenter.Info("");
            ConsolePresenter.Info("╔════════════════════════════════════════════════╗");
            ConsolePresenter.Info("║   NF-e Paulistana — Exemplo de Uso             ║");
            ConsolePresenter.Info("╠════════════════════════════════════════════════╣");

            foreach (var a in _actions)
            {
                var label = LabelFromIndex(a.MenuOrder);
                ConsolePresenter.Info($"║ {label}. {a.Description,-44}║");
            }

            ConsolePresenter.Info("║ 0. Sair                                        ║");
            ConsolePresenter.Info("╚════════════════════════════════════════════════╝");
            ConsolePresenter.Info("Opção: ");

            var option = (System.Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
            if (option is "0" or "")
            {
                ConsolePresenter.Header("Saindo...");
                return 0;
            }

            var action = _actions.FirstOrDefault(a => string.Equals(LabelFromIndex(a.MenuOrder), option, StringComparison.OrdinalIgnoreCase));

            if (action is not null)
            {
                var repeat = (46 - action.Description.Length) / 2;
                ConsolePresenter.Header($"\n{"".PadLeft(repeat, '═')} {action.Description} {"".PadLeft(repeat + ((46 - action.Description.Length) % 2), '═')}");
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await action.RunAsync(cts.Token);
                sw.Stop();
                ConsolePresenter.Info($"\nTempo: {sw.Elapsed.TotalMilliseconds:F0} ms");
                ConsolePresenter.Header("════════════════════════════════════════════════");
            }
            else
            {
                ConsolePresenter.Warning("Opção inválida.");
            }
        }

        return 0;
    }

    /// <summary>
    /// Converte um índice baseado em zero para rótulo alfabético em numeração base-26 bijetiva:
    /// <c>0→"A"</c>, <c>1→"B"</c>, …, <c>25→"Z"</c>, <c>26→"AA"</c>, <c>27→"AB"</c>, etc.
    /// Retorna <c>"?"</c> para índices negativos.
    /// </summary>
    private static string LabelFromIndex(int index)
    {
        if (index < 0)
        {
            return "?";
        }

        var s = string.Empty;
        var i = index + 1;

        while (i > 0)
        {
            var rem = (i - 1) % 26;
            s = (char)('A' + rem) + s;
            i = (i - 1) / 26;
        }

        return s;
    }
}