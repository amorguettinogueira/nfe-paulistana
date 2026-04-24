using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Classe base abstrata para Value Objects de texto do XSD da NF-e Paulistana.
/// Aplica a normalização de caracteres especiais exigida pelo web service e
/// restringe o comprimento máximo da string resultante.
/// </summary>
/// <remarks>
/// <para>
/// A normalização converte os cinco caracteres reservados do XML
/// (<c>&amp;</c>, <c>&gt;</c>, <c>&lt;</c>, <c>"</c>, <c>'</c>)
/// para suas entidades de escape antes de armazenar o valor.
/// </para>
/// <para>
/// Fonte: Manual NF-e Web Service v2.8.1, seção 3.4.5 —
/// <em>Tratamento de caracteres especiais no texto de XML</em>.
/// </para>
/// <para>
/// Subclasses devem sobrescrever <see cref="GetMaxLength"/> para definir o
/// comprimento máximo específico do campo. O padrão é 10 caracteres.
/// </para>
/// </remarks>
public abstract class ConstrainedString : XmlSerializableDataType
{
    private const string InvalidValue = "O tipo '{0}' não deve estar em branco ou ter mais do que {1} caracteres.";
    private const int MaxLength = 10;

    /// <summary>
    /// Construtor sem parâmetros protegido exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected ConstrainedString() { }

    /// <summary>
    /// Normaliza os caracteres especiais reservados do XML para suas entidades de escape.
    /// Retorna <c>null</c> se a entrada for nula, vazia ou apenas espaços.
    /// </summary>
    private static string? NormalizeString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        ReadOnlySpan<char> span = value.AsSpan().Trim();
        var sb = new StringBuilder(span.Length);

        foreach (char c in span)
        {
            _ = sb.Append(c switch
            {
                '&' => "&amp;",
                '>' => "&gt;",
                '<' => "&lt;",
                '"' => "&quot;",
                '\'' => "&apos;",
                _ => c,
            });
        }

        return sb.ToString();
    }

    /// <summary>
    /// Inicializa o Value Object normalizando os caracteres especiais e validando o comprimento.
    /// </summary>
    /// <param name="value">Valor textual do campo.</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio, resultar em string vazia após normalização,
    /// ou exceder o comprimento máximo definido por <see cref="GetMaxLength"/>.
    /// </exception>
    protected ConstrainedString(string value)
    {
        string normalizedValue = NormalizeString(value) ?? string.Empty;

        if (!IsValidString(normalizedValue))
        {
            throw new ArgumentException(InvalidValue.Format(GetType().Name, GetMaxLengthOrDefault()));
        }

        Value = normalizedValue;
    }

    private bool IsValidString(string? normalizedValue) =>
        !string.IsNullOrWhiteSpace(normalizedValue) &&
            normalizedValue.Length <= GetMaxLengthOrDefault();

    private int GetMaxLengthOrDefault()
    {
        int maxLength = GetMaxLength();
        return maxLength == default ? MaxLength : maxLength;
    }

    /// <summary>
    /// Retorna o comprimento máximo permitido para este campo.
    /// Subclasses devem sobrescrever para definir o limite específico do campo.
    /// </summary>
    /// <returns>Comprimento máximo em caracteres. O padrão é <c>10</c>.</returns>
    protected virtual int GetMaxLength() =>
        MaxLength;

    /// <summary>
    /// Hook chamado após desserialização XML (via IXmlSerializable.ReadXml).
    /// Valida o valor desserializado do XML.
    /// </summary>
    protected override void OnXmlDeserialized()
    {
        try
        {
            if (!IsValidString(Value))
            {
                throw new SerializationException($"O valor desserializado do campo '{GetType().Name}' não é válido.");
            }
        }
        catch (SerializationException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            // Converte ArgumentException para SerializationException para consistência
            throw new SerializationException(ex.Message, ex);
        }
    }

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, invoca <paramref name="factory"/> para construir a instância.
    /// </summary>
    /// <remarks>
    /// Use este método nas subclasses para expor um <c>ParseIfPresent</c> público com tipagem forte.
    /// </remarks>
    /// <typeparam name="T">Tipo derivado de <see cref="ConstrainedString"/>.</typeparam>
    /// <param name="value">String de entrada, possivelmente nula ou vazia.</param>
    /// <param name="factory">Fábrica que cria a instância a partir de uma string não vazia.</param>
    /// <returns>Instância de <typeparamref name="T"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="factory"/> for <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Propagada pela <paramref name="factory"/> se o valor não satisfizer as regras do tipo.</exception>
    protected static T? ParseIfPresent<T>(string? value, Func<string, T> factory) where T : ConstrainedString
    {
        ArgumentNullException.ThrowIfNull(factory);
        return string.IsNullOrWhiteSpace(value) ? null : factory(value);
    }
}