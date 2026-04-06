using System.ComponentModel;
using System.Net.Mail;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa um endereço de e-mail, com no máximo 75 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpEmail</c>, Linha: 146.
/// </remarks>
[Serializable]
public sealed class Email : ConstrainedString
{
    private const string InvalidEmail = "O email informado não é válido.";
    private const int MaxLength = 75;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Email()
    { }

    /// <summary>
    /// Inicializa o Value Object com o endereço de e-mail informado.
    /// </summary>
    /// <param name="value">Endereço de e-mail (máximo 75 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio, não for um e-mail válido ou exceder 75 caracteres.
    /// </exception>
    public Email(string value) : base(EnsureValidEmail(value))
    { }

    private static string EnsureValidEmail(string value) =>
        !IsValidEmail(value) ? throw new ArgumentException(InvalidEmail) : value;

    private static bool IsValidEmail(string email) =>
        MailAddress.TryCreate(email, out MailAddress? _);

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Email"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Endereço de e-mail.</param>
    /// <returns>Nova instância de <see cref="Email"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Email FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Email"/>.
    /// </summary>
    /// <param name="value">Endereço de e-mail.</param>
    public static explicit operator Email(string value) => FromString(value);
}