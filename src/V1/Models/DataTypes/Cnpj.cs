using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Nfe.Paulistana.V1.Models.DataTypes;

/// <summary>
/// <para>
/// Armazena um CNPJ como um número com validação automática via módulo 11.
/// Value Object que garante sempre estar em estado válido.
/// </para>
/// <para>
/// Fonte: https://notadomilhao.prefeitura.sp.gov.br/empresas/informacoes-gerais/manuais-arquivos/nfe_web_service.pdf/@@download/file/NFe_Web_Service%20v2.8.1.pdf
/// Página: 15
/// Seção: 3.4.4. Regras de preenchimento dos campos
/// </para>
/// </summary>
[Serializable]
public sealed class Cnpj : ModulusElevenValidatedNumber
{
    /// <summary>
    /// Os pesos utilizados no cálculo do módulo 11 para CNPJ.
    /// </summary>
    protected override byte[] ValidationWeights => [2, 3, 4, 5, 6, 7, 8, 9];

    /// <summary>
    /// Valor mínimo aceito para CNPJ (inclusivo).
    /// </summary>
    protected override long MinValueInclusive => 1_000_000;

    /// <summary>
    /// Valor máximo aceito para CNPJ (exclusivo).
    /// </summary>
    protected override long MaxValueExclusive => 99_999_999_999_999;

    /// <summary>
    /// Nome da entidade para mensagens de erro, usado na classe base para formatação de mensagens.
    /// </summary>
    protected override string EntityNameForErrorMessage => "CNPJ";

    /// <summary>
    /// Construtor parameterless privado para desserialização apenas.
    /// Uso normal deve usar os construtores públicos.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Cnpj() { }

    /// <summary>
    /// Cria um novo CNPJ a partir de um valor numérico com validação automática.
    /// </summary>
    /// <param name="value">O valor numérico do CNPJ a ser validado.</param>
    /// <exception cref="ArgumentException">Se o valor não for um CNPJ válido.</exception>
    public Cnpj(long value) : base(value) { }

    /// <summary>
    /// Cria um novo CNPJ a partir de uma string com validação automática.
    /// </summary>
    /// <param name="value">O valor em string do CNPJ (pode conter formatação como "12.345.678/0001-90").</param>
    /// <exception cref="ArgumentException">Se a string não contém um CNPJ válido.</exception>
    public Cnpj(string value) : base(value) { }

    /// <summary>
    /// Factory method para criar CNPJ a partir de string.
    /// </summary>
    /// <param name="value">O valor em string do CNPJ.</param>
    /// <returns>Uma nova instância de Cnpj validado.</returns>
    /// <exception cref="ArgumentException">Se a string não contém um CNPJ válido.</exception>
    public static Cnpj FromString(string value) => new(value);

    /// <summary>
    /// Factory method para criar CNPJ a partir de número long.
    /// </summary>
    /// <param name="value">O valor numérico do CNPJ.</param>
    /// <returns>Uma nova instância de Cnpj validado.</returns>
    /// <exception cref="ArgumentException">Se o número não é um CNPJ válido.</exception>
    public static Cnpj FromInt64(long value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em CNPJ.
    /// </summary>
    /// <param name="value">O valor em string do CNPJ.</param>
    /// <returns>Uma nova instância de Cnpj validado.</returns>
    public static explicit operator Cnpj(string value) => FromString(value);

    /// <summary>
    /// Converte explicitamente um número em CNPJ.
    /// </summary>
    /// <param name="value">O valor numérico do CNPJ.</param>
    /// <returns>Uma nova instância de Cnpj validado.</returns>
    public static explicit operator Cnpj(long value) => FromInt64(value);

    /// <summary>
    /// Hook chamado após desserialização para validar o CNPJ.
    /// </summary>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => ValidateAfterDeserialization();

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Cnpj"/>.
    /// </summary>
    /// <param name="value">CNPJ como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="Cnpj"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Cnpj? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Cnpj"/>.
    /// </summary>
    /// <param name="value">CNPJ como inteiro long, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Cnpj"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static Cnpj? ParseIfPresent(long? value) =>
        value == null ? null : FromInt64(value.Value);
}