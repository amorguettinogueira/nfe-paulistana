using Nfe.Paulistana.Options;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.Fixtures;

/// <summary>
/// Fixture compartilhada que cria um certificado auto-assinado uma única vez por classe de teste.
/// </summary>
public sealed class CertificadoFixture : IDisposable
{
    /// <summary>Certificado X.509 auto-assinado para uso nos testes.</summary>
    public X509Certificate2 Certificate { get; }

    /// <summary>Wrapper <see cref="Certificado"/> com o certificado X.509 embutido.</summary>
    public Certificado Certificado { get; }

    public CertificadoFixture()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        Certificate = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
        Certificado = new Certificado { RawData = new ReadOnlyCollection<byte>(Certificate.Export(X509ContentType.Pfx)) };
    }

    public void Dispose() =>
        Certificate.Dispose();
}
