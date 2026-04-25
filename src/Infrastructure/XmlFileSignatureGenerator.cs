using Nfe.Paulistana.Constants;
using Nfe.Paulistana.Models;
using Nfe.Paulistana.Xml;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Serializa qualquer objeto <see cref="ISignedXmlFile"/> em um documento XML,
/// computa a assinatura XMLDSig via <see cref="SignedXml"/> e armazena
/// o XML assinado em <see cref="ISignedXmlFile.SignedXmlContent"/> para
/// transmissão direta, garantindo que o conteúdo enviado ao servidor
/// seja idêntico ao que foi assinado digitalmente.
/// </summary>
/// <typeparam name="T">Tipo que implementa <see cref="ISignedXmlFile"/>.</typeparam>
[SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Requisito do modelo NF-e Paulistana que utiliza XMLDSig com SHA1")]
internal sealed class XmlFileSignatureGenerator<T> : IXmlFileSignatureGenerator<T> where T : class, ISignedXmlFile
{
    private const string ChavePrivadaNaoEncontrada = "Chave privada não encontrada no certificado.";

    /// <summary>
    /// Assina o objeto XML serializado com o certificado fornecido utilizando <see cref="SignedXml"/>,
    /// populando <see cref="ISignedXmlFile.SignedXmlContent"/> com o XML completo assinado.
    /// </summary>
    /// <param name="xmlToSign">Objeto a ser serializado em XML e assinado.</param>
    /// <param name="certificate">Certificado X509 com chave privada para assinatura RSA-SHA1.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="xmlToSign"/> ou <paramref name="certificate"/> for nulo.</exception>
    /// <exception cref="CryptographicException">Se o certificado não contiver chave privada RSA.</exception>
    public void Sign(T xmlToSign, X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(xmlToSign);
        ArgumentNullException.ThrowIfNull(certificate);

        xmlToSign.SignedXmlContent = null;

        // Serializa o documento sem assinatura
        XmlDocument doc = xmlToSign.ToXmlDocument();
        doc.PreserveWhitespace = true;

        // Remove a declaração XML (MensagemXml transmite sem declaração)
        if (doc.FirstChild is XmlDeclaration xmlDecl)
        {
            _ = doc.RemoveChild(xmlDecl);
        }

        // Obtém a chave privada RSA do certificado
        using RSA privateKey = certificate.GetRSAPrivateKey()
            ?? throw new CryptographicException(ChavePrivadaNaoEncontrada);

        // Configura a assinatura XMLDSig via SignedXml
        SignedXml signedXml = new(doc) { SigningKey = privateKey };

        Reference reference = new(string.Empty) { DigestMethod = Uris.Sha1XmlDSignature };
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform(false));
        signedXml.AddReference(reference);

        if (signedXml.SignedInfo == null)
        {
            throw new InvalidOperationException("SignedInfo não pode ser nulo após adicionar referência.");
        }

        signedXml.SignedInfo.CanonicalizationMethod = Uris.CanonicalXml;
        signedXml.SignedInfo.SignatureMethod = Uris.RsaSha1XmlDSignature;

        KeyInfo keyInfo = new();
        keyInfo.AddClause(new KeyInfoX509Data(certificate));
        signedXml.KeyInfo = keyInfo;

        // Computa DigestValue e SignatureValue de forma atômica
        signedXml.ComputeSignature();

        // Insere o elemento Signature no documento
        XmlElement sigElement = signedXml.GetXml();
        _ = doc.DocumentElement!.AppendChild(doc.ImportNode(sigElement, true));

        // Armazena o XML assinado para transmissão direta (garante identidade com o assinado)
        xmlToSign.SignedXmlContent = doc.OuterXml;
    }
}