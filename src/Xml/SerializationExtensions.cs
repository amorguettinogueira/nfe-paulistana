using Nfe.Paulistana.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Xml;

/// <summary>
/// Extensões internas para serialização, validação e persistência de objetos como documentos XML.
/// </summary>
internal static class SerializationExtensions
{
    /// <summary>
    /// Suprime as declarações padrão <c>xmlns:xsd</c> e <c>xmlns:xsi</c> que o <see cref="XmlSerializer"/> adiciona
    /// por padrão, garantindo que o XML serializado para assinatura seja idêntico ao transmitido.
    /// </summary>
    private static readonly XmlSerializerNamespaces _emptyNamespaces = new([XmlQualifiedName.Empty]);

    /// <summary>
    /// Serializa o objeto em um <see cref="XmlDocument"/> usando <see cref="XmlSerializer"/>.
    /// </summary>
    /// <remarks>
    /// Utiliza <see cref="_emptyNamespaces"/> para suprimir <c>xmlns:xsd</c> e <c>xmlns:xsi</c>,
    /// garantindo que o XML canônico usado no cálculo de <c>DigestValue</c> e <c>SignatureValue</c>
    /// seja idêntico ao XML transmitido via <c>MensagemXml</c> (que também usa <c>EmptyNamespaces</c>).
    /// Sem isso, o servidor recalcula a assinatura sobre um XML diferente e rejeita com erro 1057.
    /// </remarks>
    public static XmlDocument ToXmlDocument<T>(this T serializableObject) where T : class
    {
        XmlSerializer serializer = new(typeof(T));
        XmlDocument xmlDocument = new();

        using StringWriter stringWriter = new();
        serializer.Serialize(stringWriter, serializableObject, _emptyNamespaces);
        xmlDocument.LoadXml(stringWriter.ToString());

        return xmlDocument;
    }

    /// <summary>
    /// Valida o objeto serializado contra o XSD definido em <see cref="IXmlValidatableSchema.ValidationSchema"/>.
    /// </summary>
    /// <param name="serializableObject">Objeto a ser validado.</param>
    /// <param name="error">Mensagem de erro se inválido; <see langword="null"/> se válido.</param>
    /// <returns><see langword="true"/> se válido; <see langword="false"/> se inválido.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="serializableObject"/> for nulo.</exception>
    public static bool IsValidXsd<T>(this T serializableObject, [NotNullWhen(false)] out string? error)
        where T : class, IXmlValidatableSchema
        => ValidationHelper.Validate(serializableObject, out error);

    /// <summary>
    /// Serializa o objeto em um arquivo XML com a codificação e namespaces fornecidos.
    /// </summary>
    /// <param name="serializableObject">Objeto a ser serializado.</param>
    /// <param name="xmlFile">Caminho completo do arquivo XML de destino.</param>
    /// <param name="encoding">Codificação do arquivo.</param>
    /// <param name="namespaces">Namespaces XML a incluir na serialização. Opcional.</param>
    public static void SaveXmlFile<T>(this T serializableObject,
                                      string xmlFile,
                                      Encoding encoding,
                                      XmlSerializerNamespaces? namespaces) where T : class
    {
        XmlSerializer serializer = new(typeof(T));
        using var streamWriter = new StreamWriter(xmlFile, false, encoding);
        using var xmlWriter = XmlWriter.Create(streamWriter);
        xmlWriter.WriteStartDocument(true);
        serializer.Serialize(xmlWriter, serializableObject, namespaces);
    }
}