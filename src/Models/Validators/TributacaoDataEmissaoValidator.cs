using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Models.Validators;

/// <summary>
/// Validador de regras para o campo 'TributacaoNfe' conforme a data de emissão da NFS-e.
/// </summary>
/// <remarks>
/// <para>Garante que o valor informado para 'TributacaoNfe' está de acordo com as regras vigentes para o período da emissão.</para>
/// <para>Emitidas até 22/02/2015: T, F, I, J. A partir de 23/02/2015: T, F, A, B, D, M, N, R, S, X, V, P.</para>
/// </remarks>
internal static class TributacaoDataEmissaoValidator
{
    private static readonly HashSet<char> PermitidosAte2015 = ['T', 'F', 'I', 'J'];
    private static readonly HashSet<char> PermitidosApos2015 = ['T', 'F', 'A', 'B', 'D', 'M', 'N', 'R', 'S', 'X', 'V', 'P'];
    private static readonly DateTime DataCorte = new(2015, 2, 23);

    /// <summary>
    /// Valida o campo 'TributacaoNfe' conforme as regras do período da emissão,
    /// lançando exceção em caso de violação.
    /// </summary>
    /// <param name="tributacaoNfe">Valor a ser validado.</param>
    /// <param name="dataEmissao">Data de emissão da NFS-e.</param>
    /// <exception cref="ArgumentNullException">Lançado quando qualquer argumento for nulo.</exception>
    /// <exception cref="ArgumentException">Lançado quando o valor informado não for permitido para o período da emissão.</exception>
    public static void ThrowIfInvalid(TributacaoNfe tributacaoNfe, DataXsd dataEmissao)
    {
        ArgumentNullException.ThrowIfNull(tributacaoNfe);
        ArgumentNullException.ThrowIfNull(dataEmissao);

        string valor = tributacaoNfe.ToString() ?? string.Empty;

        if (valor.Length != 1)
        {
            throw new ArgumentException("O campo 'TributacaoNfe' deve ter exatamente 1 caractere.", nameof(tributacaoNfe));
        }

        char c = valor[0];

        if ((DateTime)dataEmissao < DataCorte)
        {
            if (!PermitidosAte2015.Contains(c))
            {
                throw new ArgumentException(
                    $"Para a data de emissão {dataEmissao}, o campo 'TributacaoNfe' deve ser preenchido com um dos valores permitidos: T, F, I, J.",
                    nameof(tributacaoNfe));
            }
        }
        else
        {
            if (!PermitidosApos2015.Contains(c))
            {
                throw new ArgumentException(
                    $"Para a data de emissão {dataEmissao}, o campo 'TributacaoNfe' deve ser preenchido com um dos valores permitidos: T, F, A, B, D, M, N, R, S, X, V, P.",
                    nameof(tributacaoNfe));
            }
        }
    }
}