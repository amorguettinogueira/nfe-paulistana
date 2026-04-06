using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Nfe.Paulistana.Xml;

/// <summary>
/// Coleta erros e avisos de validação XSD e expõe um ponto único de validação via <see cref="Validate{T}"/>.
/// </summary>
internal sealed class ValidationHelper
{
    private const string FormatoErro = "Erro: {0}";
    private const string FormatoAviso = "Aviso: {0}";

    private readonly StringBuilder _errorMessages = new();
    private readonly ValidationEventHandler _eventHandler;

    private ValidationHelper() => _eventHandler = new(OnValidationEvent);

    private bool HasErrors => _errorMessages.Length > 0;
    private string ErrorMessages => _errorMessages.ToString();

    /// <summary>
    /// Serializa o objeto e valida contra o <see cref="XmlSchemaSet"/> definido em <see cref="IXmlValidatableSchema.ValidationSchema"/>.
    /// </summary>
    /// <typeparam name="T">Tipo que implementa <see cref="IXmlValidatableSchema"/>.</typeparam>
    /// <param name="objectToValidate">Objeto a ser serializado e validado.</param>
    /// <param name="error">Mensagem de erro se inválido; <see langword="null"/> se válido.</param>
    /// <returns><see langword="true"/> se válido; <see langword="false"/> se inválido.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="objectToValidate"/> for nulo.</exception>
    public static bool Validate<T>(T objectToValidate, [NotNullWhen(false)] out string? error)
        where T : class, IXmlValidatableSchema
    {
        ArgumentNullException.ThrowIfNull(objectToValidate);

        XmlDocument document;

        // Quando o objeto possui XML assinado, valida diretamente o conteúdo
        // que será transmitido, garantindo fidelidade entre validação e envio.
        if (objectToValidate is ISignedXmlFile signedFile && signedFile.SignedXmlContent != null)
        {
            document = new XmlDocument();
            document.LoadXml(signedFile.SignedXmlContent);
        }
        else
        {
            document = objectToValidate.ToXmlDocument();
        }

        document.Schemas = objectToValidate.ValidationSchema;

        ValidationHelper helper = new();
        document.Validate(helper._eventHandler);

        if (helper.HasErrors)
        {
            error = helper.ErrorMessages;
            return false;
        }

        error = null;
        return true;
    }

    private void OnValidationEvent(object? sender, ValidationEventArgs e) =>
        _errorMessages.AppendLine((e.Severity == XmlSeverityType.Error
            ? FormatoErro
            : FormatoAviso).Format(e.Message));
}