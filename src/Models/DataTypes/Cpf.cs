using System.ComponentModel;
using System.Runtime.Serialization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// <para>
/// Armazena um CPF como um número com validação automática via módulo 11.
/// Value Object que garante sempre estar em estado válido.
/// </para>
/// <para>
/// Fonte: https://notadomilhao.prefeitura.sp.gov.br/empresas/informacoes-gerais/manuais-arquivos/nfe_web_service.pdf/@@download/file/NFe_Web_Service%20v2.8.1.pdf
/// Página: 15
/// Seção: 3.4.4. Regras de preenchimento dos campos
/// </para>
/// </summary>
[Serializable]
public sealed class Cpf : ModulusElevenValidatedNumber
{
    /// <summary>
    /// Os pesos utilizados no cálculo do módulo 11 para CPF.
    /// </summary>
    protected override byte[] ValidationWeights => [2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

    /// <summary>
    /// Valor mínimo aceito para CPF (inclusivo).
    /// </summary>
    protected override long MinValueInclusive => 100;

    /// <summary>
    /// Valor máximo aceito para CPF (exclusivo).
    /// </summary>
    protected override long MaxValueExclusive => 99_999_999_999;

    /// <summary>
    /// Nome da entidade para mensagens de erro, usado na classe base para formatação de mensagens.
    /// </summary>
    protected override string EntityNameForErrorMessage => "CPF";

    /// <summary>
    /// Construtor parameterless privado para desserialização apenas.
    /// Uso normal deve usar os construtores públicos.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Cpf() { }

    /// <summary>
    /// Cria um novo CPF a partir de um valor numérico com validação automática.
    /// </summary>
    /// <param name="value">O valor numérico do CPF a ser validado.</param>
    /// <exception cref="ArgumentException">Se o valor não for um CPF válido.</exception>
    public Cpf(long value) : base(value) { }

    /// <summary>
    /// Cria um novo CPF a partir de uma string com validação automática.
    /// </summary>
    /// <param name="value">O valor em string do CPF (pode conter formatação como "123.456.789-10").</param>
    /// <exception cref="ArgumentException">Se a string não contém um CPF válido.</exception>
    public Cpf(string value) : base(value) { }

    /// <summary>
    /// Factory method para criar CPF a partir de string.
    /// </summary>
    /// <param name="value">O valor em string do CPF.</param>
    /// <returns>Uma nova instância de Cpf validado.</returns>
    /// <exception cref="ArgumentException">Se a string não contém um CPF válido.</exception>
    public static Cpf FromString(string value) => new(value);

    /// <summary>
    /// Factory method para criar CPF a partir de número long.
    /// </summary>
    /// <param name="value">O valor numérico do CPF.</param>
    /// <returns>Uma nova instância de Cpf validado.</returns>
    /// <exception cref="ArgumentException">Se o número não é um CPF válido.</exception>
    public static Cpf FromInt64(long value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em CPF.
    /// </summary>
    /// <param name="value">O valor em string do CPF.</param>
    /// <returns>Uma nova instância de Cpf validado.</returns>
    public static explicit operator Cpf(string value) => FromString(value);

    /// <summary>
    /// Converte explicitamente um número em CPF.
    /// </summary>
    /// <param name="value">O valor numérico do CPF.</param>
    /// <returns>Uma nova instância de Cpf validado.</returns>
    public static explicit operator Cpf(long value) => FromInt64(value);

    /// <summary>
    /// Hook chamado após desserialização para validar o CPF.
    /// </summary>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => ValidateAfterDeserialization();

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Cpf"/>.
    /// </summary>
    /// <param name="value">CPF como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="Cpf"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Cpf? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Cpf"/>.
    /// </summary>
    /// <param name="value">CPF como inteiro long, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Cpf"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static Cpf? ParseIfPresent(long? value) =>
        value == null ? null : FromInt64(value.Value);
}