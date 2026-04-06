namespace Nfe.Paulistana.Constants;

/// <summary>
/// Centraliza os namespaces XML e URIs de algoritmos criptográficos utilizados
/// nos documentos SOAP e assinaturas digitais da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Os valores refletem as especificações do XSD oficial da Prefeitura de São Paulo
/// e do padrão W3C XML Digital Signature (XMLDSig). Qualquer alteração aqui afeta
/// diretamente a serialização XML e a geração de assinaturas digitais.
/// </remarks>
internal static class Uris
{
    // -------------------------------------------------------
    // Namespaces da NF-e Paulistana
    // -------------------------------------------------------

    /// <summary>Namespace principal dos documentos NF-e da Prefeitura de São Paulo.</summary>
    public const string Nfe = "http://www.prefeitura.sp.gov.br/nfe";

    /// <summary>Namespace dos tipos auxiliares do XSD da NF-e Paulistana.</summary>
    public const string NfeTipos = "http://www.prefeitura.sp.gov.br/nfe/tipos";

    // -------------------------------------------------------
    // Namespaces W3C padrão
    // -------------------------------------------------------

    /// <summary>Namespace do XML Schema Instance (xsi).</summary>
    public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>Namespace do XML Schema Definition (xsd).</summary>
    public const string Xsd = "http://www.w3.org/2001/XMLSchema";

    /// <summary>Namespace do envelope SOAP 1.1.</summary>
    public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";

    // -------------------------------------------------------
    // URIs do W3C XML Digital Signature (XMLDSig)
    // -------------------------------------------------------

    /// <summary>Namespace raiz do padrão W3C XML Digital Signature.</summary>
    public const string XmlDSignature = "http://www.w3.org/2000/09/xmldsig#";

    /// <summary>URI do algoritmo de digest SHA-1 conforme XMLDSig.</summary>
    public const string Sha1XmlDSignature = "http://www.w3.org/2000/09/xmldsig#sha1";

    /// <summary>URI do algoritmo de assinatura RSA com SHA-1 conforme XMLDSig.</summary>
    public const string RsaSha1XmlDSignature = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

    /// <summary>URI do algoritmo de canonicalização XML exclusiva (Exc-C14N).</summary>
    public const string ExclusiveXmlCanonicalization = "http://www.w3.org/2001/10/xml-exc-c14n#";

    /// <summary>URI da transformação de assinatura envelopada conforme XMLDSig.</summary>
    public const string EnvelopedSignature = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

    /// <summary>URI do algoritmo de canonicalização XML canônica (C14N).</summary>
    public const string CanonicalXml = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
}