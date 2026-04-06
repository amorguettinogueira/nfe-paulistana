namespace Nfe.Paulistana.Integration.Sample.Presentation.Console;

/// <summary>
/// Utilitário de baixo nível para saída colorida no console.
/// Salva e restaura <see cref="System.Console.ForegroundColor"/> via try/finally
/// a cada chamada. Não é thread-safe; projetado para execução serial.
/// </summary>
internal static class ConsolePresenter
{
    private static void WithColor(ConsoleColor color, Action action)
    {
        var prev = System.Console.ForegroundColor;
        try
        {
            System.Console.ForegroundColor = color;
            action();
        }
        finally
        {
            System.Console.ForegroundColor = prev;
        }
    }

    /// <summary>Exibe <paramref name="text"/> em cinza (informação geral).</summary>
    public static void Info(string text) =>
        WithColor(ConsoleColor.Gray, () => System.Console.WriteLine(text));

    /// <summary>Exibe <paramref name="text"/> em verde (operação bem-sucedida).</summary>
    public static void Success(string text) =>
        WithColor(ConsoleColor.Green, () => System.Console.WriteLine(text));

    /// <summary>Exibe <paramref name="text"/> em amarelo (alerta).</summary>
    public static void Warning(string text) =>
        WithColor(ConsoleColor.Yellow, () => System.Console.WriteLine(text));

    /// <summary>Exibe <paramref name="text"/> em vermelho (erro).</summary>
    public static void Error(string text) =>
        WithColor(ConsoleColor.Red, () => System.Console.WriteLine(text));

    /// <summary>Exibe <paramref name="text"/> em ciano (cabeçalho de seção).</summary>
    public static void Header(string text) =>
        WithColor(ConsoleColor.Cyan, () => System.Console.WriteLine(text));

    /// <summary>
    /// Exibe <paramref name="text"/> em verde se <paramref name="success"/> for
    /// <see langword="true"/>, ou em vermelho caso contrário.
    /// </summary>
    public static void Outcome(string text, bool success)
    {
        if (success)
        {
            Success(text);
        }
        else
        {
            Error(text);
        }
    }
}