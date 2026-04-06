using System.Xml.Schema;

namespace Nfe.Paulistana.Models;

/// <summary>
/// Contrato de infraestrutura que delega ao objeto implementador a responsabilidade
/// de fornecer o <see cref="XmlSchemaSet"/> que valida a sua própria representação XML.
/// </summary>
/// <remarks>
/// <para>
/// Esta interface é consumida exclusivamente por
/// <c>XmlExtensions.IsValidXsd&lt;T&gt;</c>, que serializa o objeto para XML e
/// executa a validação contra o schema retornado por <see cref="ValidationSchema"/>.
/// </para>
/// <para>
/// O padrão de auto-schema (o próprio tipo conhece seu XSD) mantém o acoplamento
/// entre modelo e schema dentro do mesmo tipo, evitando que o código de validação
/// precise conhecer os schemas de cada tipo individualmente.
/// </para>
/// </remarks>
internal interface IXmlValidatableSchema
{
    /// <summary>
    /// Retorna o <see cref="XmlSchemaSet"/> contendo o XSD que descreve a estrutura
    /// XML válida para este objeto.
    /// </summary>
    /// <value>
    /// Um <see cref="XmlSchemaSet"/> já compilado e pronto para uso em
    /// <see cref="System.Xml.XmlDocument.Validate"/>.
    /// </value>
    public XmlSchemaSet ValidationSchema { get; }
}