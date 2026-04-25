using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.Xml;
using System.Text;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Xml;

/// <summary>
/// Testes unitários para <see cref="SerializationExtensions"/>:
/// <see cref="SerializationExtensions.ToXmlDocument{T}"/>,
/// <see cref="SerializationExtensions.IsValidXsd{T}"/> e
/// <see cref="SerializationExtensions.SaveXmlFile{T}"/>.
/// </summary>
public class SerializationExtensionsTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);
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