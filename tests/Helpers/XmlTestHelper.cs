using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Helpers;

/// <summary>
/// Utilitários de serialização/desserialização XML para uso nos testes.
/// </summary>
internal static class XmlTestHelper
{
    /// <summary>
    /// Serializa um objeto para XML usando <see cref="XmlSerializer"/>.
    /// </summary>
    internal static string SerializarParaXml<T>(T obj)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        serializer.Serialize(writer, obj);
        return writer.ToString();
    }

    /// <summary>
    /// Desserializa um objeto de uma string XML usando <see cref="XmlSerializer"/>.
    /// </summary>
    internal static T? DesserializarDeXml<T>(string xml)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        return (T?)serializer.Deserialize(reader);
    }
}
