using Nfe.Paulistana.Extensions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Nfe.Paulistana.Xml;

/// <summary>
/// Provedor de <see cref="XmlSchemaSet"/> com cache por identificador de recurso,
/// combinando os XSDs base (TiposNFe, XMLDSig) com o schema variável de cada tipo.
/// </summary>
internal static class SchemaProvider
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    private const string ResourcesPathV01 = "Nfe.Paulistana.Schemas.V1.{0}";
    private const string ResourcesPathV02 = "Nfe.Paulistana.Schemas.V2.{0}";
    private const string XsdNaoEncontrado = "XSD embutido não encontrado: {0}";

    private static readonly KeyValuePair<string, string>[] CoreSchemasV01 = [
        new(ResourcesPathV01.Format("TiposNFe_v01.xsd"), Constants.Uris.NfeTipos),
        new(ResourcesPathV01.Format("xmldsig-core-schema_v01.xsd"), Constants.Uris.XmlDSignature),
    ];

    private static readonly KeyValuePair<string, string>[] CoreSchemasV02 = [
        new(ResourcesPathV02.Format("TiposNFe_v02.xsd"), Constants.Uris.NfeTipos),
        new(ResourcesPathV02.Format("xmldsig-core-schema_v02.xsd"), Constants.Uris.XmlDSignature),
    ];

    // cache por resourceName; o mesmo XSD pode ser reutilizado por múltiplos tipos XML
    private static readonly ConcurrentDictionary<string, Lazy<XmlSchemaSet>> Cache = new();

    /// <summary>
    /// Retorna o <see cref="XmlSchemaSet"/> compilado para o namespace e recurso indicados
    /// usando os schemas base da versão 1. Cache por <paramref name="resourceName"/>.
    /// </summary>
    /// <param name="targetNamespace">Namespace XML alvo do schema variável.</param>
    /// <param name="resourceName">Nome do arquivo XSD embutido (sem o prefixo do caminho base).</param>
    /// <returns><see cref="XmlSchemaSet"/> compilado e pronto para validação.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="targetNamespace"/> ou <paramref name="resourceName"/> forem nulos.</exception>
    /// <exception cref="InvalidOperationException">Se algum XSD embutido não for encontrado na assembly.</exception>
    public static XmlSchemaSet GetSchemaSetV1(string targetNamespace, string resourceName)
    {
        ArgumentNullException.ThrowIfNull(targetNamespace);
        ArgumentNullException.ThrowIfNull(resourceName);

        return Cache.GetOrAdd(resourceName, key => new Lazy<XmlSchemaSet>(
            () => BuildSchemaSet(targetNamespace, key, CoreSchemasV01, ResourcesPathV01), true)).Value;
    }

    /// <summary>
    /// Retorna o <see cref="XmlSchemaSet"/> compilado para o namespace e recurso indicados
    /// usando os schemas base da versão 2. Cache por <paramref name="resourceName"/>.
    /// </summary>
    /// <param name="targetNamespace">Namespace XML alvo do schema variável.</param>
    /// <param name="resourceName">Nome do arquivo XSD embutido (sem o prefixo do caminho base).</param>
    /// <returns><see cref="XmlSchemaSet"/> compilado e pronto para validação.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="targetNamespace"/> ou <paramref name="resourceName"/> forem nulos.</exception>
    /// <exception cref="InvalidOperationException">Se algum XSD embutido não for encontrado na assembly.</exception>
    public static XmlSchemaSet GetSchemaSetV2(string targetNamespace, string resourceName)
    {
        ArgumentNullException.ThrowIfNull(targetNamespace);
        ArgumentNullException.ThrowIfNull(resourceName);

        return Cache.GetOrAdd(resourceName, key => new Lazy<XmlSchemaSet>(
            () => BuildSchemaSet(targetNamespace, key, CoreSchemasV02, ResourcesPathV02), true)).Value;
    }

    private static XmlSchemaSet BuildSchemaSet(
        string targetNamespace,
        string resourceName,
        KeyValuePair<string, string>[] coreSchemas,
        string resourcesPath)
    {
        XmlSchemaSet set = new();

        foreach ((string name, string @namespace) in coreSchemas)
        {
            AddSchema(set, @namespace, name);
        }

        AddSchema(set, targetNamespace, resourcesPath.Format(resourceName));

        set.Compile();
        return set;
    }

    private static void AddSchema(XmlSchemaSet set, string targetNamespace, string resourceName)
    {
        using Stream stream = Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(XsdNaoEncontrado.Format(resourceName));

        using var reader = XmlReader.Create(stream);

        _ = set.Add(targetNamespace, reader);
    }
}