using Nfe.Paulistana.Models;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// <para>
/// Defines the contract for generating signatures for domain objects that implement ISignedXmlFile.
/// </para>
/// <para>
/// This generic service interface encapsulates the complete signing process: converting a domain object
/// into a standardized signing text format, computing the digital signature, and storing it in the object.
/// </para>
/// </summary>
/// <typeparam name="T">
/// The type of object to sign. Must implement ISignedXmlFile to participate in signing operations.
/// </typeparam>
/// <remarks>
/// <para>
/// <strong>Architectural Design:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Generic Type Parameter:</strong> The interface is generic to allow different domain objects
/// (Rps, Cabecalho, PedidoEnvio, etc.) to have specialized signing implementations without code duplication.
/// </item>
/// <item>
/// <strong>Centralized Logic:</strong> All knowledge about signing text format for a specific type T is
/// encapsulated in implementations of this interface. Format changes affect only the service, not the domain model.
/// </item>
/// <item>
/// <strong>Versioning Support:</strong> Different signing text versions (V1, V2, etc.) can coexist as
/// separate IObjectSignatureGenerator&lt;T&gt; implementations. For example:
/// <list type="bullet">
/// <item>RpsSigningTextGeneratorV1 - Uses numeric type indicators ("1", "2", "3")</item>
/// <item>RpsSigningTextGeneratorV2 - Uses letter type indicators ("A", "B", "C")</item>
/// </list>
/// </item>
/// <item>
/// <strong>Single Responsibility:</strong> This interface focuses exclusively on generating and applying
/// digital signatures. Domain models (implementing ISignedXmlFile) remain free of signing logic.
/// </item>
/// <item>
/// <strong>Specification-Driven:</strong> Implementations are guided by official specifications from
/// the São Paulo city hall (Nota do Milhão) for NFe signing requirements.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Usage example for signing an RPS
/// var rps = new RpsBuilder().Build();
/// var generator = new RpsSigningTextGenerator();
/// var certificate = LoadCertificate();
///
/// generator.Sign(rps, certificate);
/// // At this point, rps.Assinatura contains the digital signature
/// </code>
/// </example>
internal interface IXmlFileSignatureGenerator<T> where T : ISignedXmlFile
{
    /// <summary>
    /// Signs the provided object using the specified certificate.
    /// </summary>
    /// <param name="objectToSign">
    /// The domain object to sign. Must not be null. Upon completion, its Assinatura property
    /// will contain the generated digital signature (or null if signing failed).
    /// </param>
    /// <param name="certificate">
    /// The X509 certificate containing the private key for signature generation.
    /// The certificate must have a valid private key accessible to the current context.
    /// </param>
    /// <remarks>
    /// <para>
    /// <strong>Signing Process:</strong>
    /// </para>
    /// <list type="number">
    /// <item>Generates signing text by formatting all relevant object properties</item>
    /// <item>Encodes the signing text as UTF-8 bytes</item>
    /// <item>Computes SHA1 digest of the signing text</item>
    /// <item>Signs the digest using the certificate's private key with RSA/PKCS#1 padding</item>
    /// <item>Stores the signature bytes in the object's Assinatura property</item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">If objectToSign or certificate is null.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// If the certificate lacks a private key or the signing operation fails.
    /// </exception>
    public void Sign(T objectToSign, X509Certificate2 certificate);
}