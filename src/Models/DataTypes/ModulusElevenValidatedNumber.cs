using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Classe base abstrata para Value Objects numéricos validados pelo algoritmo
/// de Módulo 11 duplo, como CPF e CNPJ.
/// </summary>
/// <remarks>
/// <para><strong>Validação:</strong></para>
/// <list type="bullet">
/// <item>Verifica que o valor está dentro dos limites definidos por
/// <see cref="MinValueInclusive"/> e <see cref="MaxValueExclusive"/>.</item>
/// <item>Verifica que os dois dígitos verificadores estão corretos conforme
/// o algoritmo de Módulo 11 duplo.</item>
/// <item>Rejeita números com todos os dígitos iguais (ex: <c>111.111.111-11</c>).</item>
/// </list>
/// <para><strong>Padrão de serialização:</strong></para>
/// <para>
/// Para suportar desserialização XML via <see cref="System.Xml.Serialization.IXmlSerializable"/>,
/// subclasses devem:
/// </para>
/// <list type="number">
/// <item>Expor um construtor sem parâmetros protegido (marcado com
/// <see cref="System.ComponentModel.EditorBrowsableAttribute"/> como <c>Never</c>).</item>
/// <item>Implementar <see cref="XmlSerializableDataType.OnXmlDeserialized"/> chamando
/// <see cref="ValidateAfterDeserialization"/>.</item>
/// </list>
/// <para>
/// Esse padrão garante que objetos criados pelo código normal sejam sempre válidos,
/// e que objetos desserializados sejam validados imediatamente após a leitura do XML.
/// </para>
/// </remarks>
public abstract class ModulusElevenValidatedNumber : XmlSerializableDataType
{
    /// <summary>
    /// O nome da entidade para mensagens de erro. Deve ser sobrescrito pelas classes filhas para fornecer mensagens de erro específicas (ex: "CPF", "CNPJ").
    /// </summary>
    protected abstract string EntityNameForErrorMessage { get; }

    /// <summary>
    /// Os pesos utilizados no cálculo do módulo 11. Deve ser sobrescrito pelas classes filhas.
    /// </summary>
    protected abstract IReadOnlyList<byte> ValidationWeights { get; }

    /// <summary>
    /// Valor mínimo aceito (inclusivo). Deve ser sobrescrito pelas classes filhas.
    /// </summary>
    protected abstract long MinValueInclusive { get; }

    /// <summary>
    /// Valor máximo aceito (exclusivo). Deve ser sobrescrito pelas classes filhas.
    /// </summary>
    protected abstract long MaxValueExclusive { get; }

    private static int ValueLength(long value) =>
        value == 0 ? 1 : (int)Math.Floor(Math.Log10(value)) + 1;

    /// <summary>
    /// Construtor parameterless protegido para desserialização APENAS.
    /// PRIVADO: Código normal não pode usar. Apenas frameworks de desserialização via reflection.
    /// </summary>
    [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected ModulusElevenValidatedNumber() { }

    /// <summary>
    /// Valida o valor numérico contra os limites de intervalo e os dígitos verificadores do Módulo 11.
    /// </summary>
    /// <param name="value">Valor numérico a ser validado.</param>
    /// <param name="deserialized"><c>true</c> quando chamado após desserialização, para ajustar a mensagem de erro.</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for menor ou igual a <see cref="MinValueInclusive"/>,
    /// maior que <see cref="MaxValueExclusive"/>, ou não passar na validação de Módulo 11.
    /// </exception>
    private void ValidateValue(long value, bool deserialized)
    {
        if (value <= MinValueInclusive)
        {
            throw new ArgumentException($"O valor{(deserialized ? " desserializado" : string.Empty)} do campo {EntityNameForErrorMessage} deve ter pelo menos {ValueLength(MinValueInclusive)} dígitos significativos.", nameof(value));
        }

        if (value > MaxValueExclusive)
        {
            throw new ArgumentException($"O valor{(deserialized ? " desserializado" : string.Empty)} do campo {EntityNameForErrorMessage} deve ter no máximo {ValueLength(MaxValueExclusive)} dígitos.", nameof(value));
        }

        if (!IsValid(value))
        {
            throw new ArgumentException($"O valor{(deserialized ? " desserializado" : string.Empty)} do campo {EntityNameForErrorMessage} não tem dígitos verificadores válidos.", nameof(value));
        }
    }

    /// <summary>
    /// Cria uma nova instância de número validado. Executa validação automaticamente.
    /// Este é o construtor que código normal deve usar.
    /// </summary>
    /// <param name="value">O valor numérico a ser validado.</param>
    /// <exception cref="ArgumentException">Se o valor for menor que o mínimo, maior que o máximo ou não passar na validação de módulo 11.</exception>
    protected ModulusElevenValidatedNumber(long value)
    {
        ValidateValue(value, false);
        Value = value.ToString(CultureInfo.InvariantCulture).PadLeft(ValueLength(MaxValueExclusive), '0');
    }

    /// <summary>
    /// Cria uma nova instância de número validado a partir de uma string. Remove formatação e valida o valor numérico. Executa validação automaticamente.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="ArgumentException">Se o valor não for numérico, for menor que o mínimo, maior que o máximo ou não passar na validação de módulo 11.</exception>
    protected ModulusElevenValidatedNumber(string value)
    {
        string unformattedValue = value.RemoveFormatting();

        if (!long.TryParse(unformattedValue, out long numericValue))
        {
            throw new ArgumentException($"O valor do campo {EntityNameForErrorMessage} não é um número válido.", nameof(value));
        }

        ValidateValue(numericValue, false);
        Value = numericValue.ToString(CultureInfo.InvariantCulture).PadLeft(ValueLength(MaxValueExclusive), '0');
    }

    /// <summary>
    /// Hook chamado após desserialização para validar o estado.
    /// Subclasses devem chamar isto em [OnDeserialized] se necessário validação custom.
    /// </summary>
    /// <exception cref="System.Runtime.Serialization.SerializationException">
    /// Se o valor desserializado for inválido.
    /// </exception>
    protected void ValidateAfterDeserialization()
    {
        if (string.IsNullOrWhiteSpace(Value) || !long.TryParse(Value, out long numericValue))
        {
            throw new SerializationException($"O valor desserializado do campo {EntityNameForErrorMessage} não é um número válido.");
        }

        ValidateValue(numericValue, true);
    }

    /// <summary>
    /// Hook chamado após desserialização XML (via IXmlSerializable.ReadXml).
    /// Valida o valor desserializado do XML.
    /// </summary>
    protected override void OnXmlDeserialized()
    {
        try
        {
            ValidateAfterDeserialization();
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
    /// Valida se um número é válido usando o algoritmo de módulo 11 duplo
    /// delegando para <see cref="ModulusElevenCalculator"/>.
    /// </summary>
    /// <param name="value">O valor a validar.</param>
    /// <returns><c>true</c> se o valor é válido; <c>false</c> caso contrário.</returns>
    protected bool IsValid(long value)
    {
        if (value <= MinValueInclusive || value > MaxValueExclusive)
        {
            return false;
        }

        int digitCount = ValueLength(value);
        Span<int> digits = stackalloc int[digitCount];

        long temp = value;

        for (int i = digitCount - 1; i >= 0; i--)
        {
            digits[i] = (int)(temp % 10);
            temp /= 10;
        }

        return ModulusElevenCalculator.Validate(digits, ValidationWeights);
    }
}