using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Xml;

/// <summary>
/// Testes unitários para <see cref="SerializationExtensions"/>:
/// <see cref="SerializationExtensions.ToXmlDocument{T}"/>,
/// <see cref="SerializationExtensions.IsValidXsd{T}"/> e
/// <see cref="SerializationExtensions.SaveXmlFile{T}"/>.
/// </summary>
public class SerializationExtensionsTests
{
    private static Certificado CriarConfiguracao()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return new Certificado
        {
            Certificate = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1))
        };
    }

    private static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf(new Cpf(new ValidCpfNumber().Min())).Build();

    private static Rps CriarRps() =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.NotaFiscalConjugada,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(TomadorPadrao)
            .Build();

    private static PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        return factory.NewCpf(new Cpf(new ValidCpfNumber().Min()), false, [CriarRps()]);
    }

    // ============================================
    // ToXmlDocument
    // ============================================

    [Fact]
    public void ToXmlDocument_ObjetoValido_RetornaXmlDocumentComRaiz()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();

        var doc = lote.ToXmlDocument();

        Assert.NotNull(doc);
        Assert.NotNull(doc.DocumentElement);
    }

    [Fact]
    public void ToXmlDocument_ObjetoValido_NaoContemXmlnsXsdNemXsi()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();

        var doc = lote.ToXmlDocument();
        string xml = doc.OuterXml;

        Assert.DoesNotContain("xmlns:xsd", xml);
        Assert.DoesNotContain("xmlns:xsi", xml);
    }

    [Fact]
    public void ToXmlDocument_ObjetoValido_ElementoRaizCorreto()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();

        var doc = lote.ToXmlDocument();

        Assert.Equal("PedidoEnvioLoteRPS", doc.DocumentElement!.LocalName);
    }

    // ============================================
    // IsValidXsd
    // ============================================

    [Fact]
    public void IsValidXsd_LoteValido_RetornaTrueSemErro()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();

        bool resultado = lote.IsValidXsd(out string? erro);

        Assert.True(resultado);
        Assert.Null(erro);
    }

    [Fact]
    public void IsValidXsd_LoteSemAssinatura_RetornaFalseComErro()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();
        lote.SignedXmlContent = null;

        bool resultado = lote.IsValidXsd(out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }

    [Fact]
    public void IsValidXsd_SignedXmlContentInvalido_RetornaFalseComErro()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();
        lote.SignedXmlContent = "<PedidoEnvioLoteRPS xmlns=\"http://www.prefeitura.sp.gov.br/nfe\"/>";

        bool resultado = lote.IsValidXsd(out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }

    // ============================================
    // SaveXmlFile
    // ============================================

    [Fact]
    public void SaveXmlFile_SemNamespaces_CriaArquivoComXmlValido()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();
        string path = Path.GetTempFileName();
        try
        {
            lote.SaveXmlFile(path, Encoding.UTF8, null);

            string conteudo = File.ReadAllText(path, Encoding.UTF8);
            Assert.Contains("<?xml", conteudo);
            Assert.Contains("PedidoEnvioLoteRPS", conteudo);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SaveXmlFile_ComNamespaces_ArquivoContemPrefixoNamespace()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();
        string path = Path.GetTempFileName();
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("nfe", "http://www.prefeitura.sp.gov.br/nfe");
        try
        {
            lote.SaveXmlFile(path, Encoding.UTF8, namespaces);

            string conteudo = File.ReadAllText(path, Encoding.UTF8);
            Assert.Contains("nfe:", conteudo);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SaveXmlFile_ArquivoContemDeclaracaoStandalone()
    {
        PedidoEnvioLote lote = CriarLoteAssinado();
        string path = Path.GetTempFileName();
        try
        {
            lote.SaveXmlFile(path, Encoding.UTF8, null);

            string conteudo = File.ReadAllText(path, Encoding.UTF8);
            Assert.Contains("standalone=\"yes\"", conteudo);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
