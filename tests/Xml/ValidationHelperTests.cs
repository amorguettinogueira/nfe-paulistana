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

namespace Nfe.Paulistana.Tests.Xml;

/// <summary>
/// Testes unitários para <see cref="ValidationHelper"/>:
/// guard clause de argumento nulo, ramo com <see cref="ISignedXmlFile.SignedXmlContent"/>
/// preenchido, ramo sem <c>SignedXmlContent</c> e acúmulo de erros de validação XSD.
/// </summary>
public class ValidationHelperTests
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
    // Guard clauses
    // ============================================

    [Fact]
    public void Validate_ObjetoNulo_ThrowsArgumentNullException()
    {
        PedidoEnvioLote? pedido = null;

        _ = Assert.Throws<ArgumentNullException>(() => ValidationHelper.Validate(pedido!, out _));
    }

    // ============================================
    // Ramo com SignedXmlContent preenchido
    // ============================================

    [Fact]
    public void Validate_LoteComSignedXmlContent_RetornaTrue()
    {
        PedidoEnvioLote pedido = CriarLoteAssinado();

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.True(resultado);
        Assert.Null(erro);
    }

    // ============================================
    // Ramo sem SignedXmlContent (serialização direta)
    // ============================================

    [Fact]
    public void Validate_LoteSemSignedXmlContent_RetornaFalseComErro()
    {
        // O XSD de PedidoEnvioLoteRPS exige o elemento <Signature>.
        // O XML re-serializado sem assinatura não satisfaz esse requisito,
        // documentando que o lote deve ser assinado antes de ser validado.
        PedidoEnvioLote pedido = CriarLoteAssinado();
        pedido.SignedXmlContent = null;

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }

    // ============================================
    // Acúmulo de erros de validação XSD
    // ============================================

    [Fact]
    public void Validate_SignedXmlContentInvalido_RetornaFalseComErro()
    {
        PedidoEnvioLote pedido = CriarLoteAssinado();
        pedido.SignedXmlContent = "<PedidoEnvioLoteRPS xmlns=\"http://www.prefeitura.sp.gov.br/nfe\"/>";  // Ausência de Cabecalho/RPS obrigatórios

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }
}