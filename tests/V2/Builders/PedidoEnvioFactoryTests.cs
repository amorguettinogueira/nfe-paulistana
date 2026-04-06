using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V2.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoEnvioFactory"/> v02:
/// guard clauses do construtor e dos métodos de fábrica, e verificação
/// de que o pedido retornado está corretamente assinado.
/// </summary>
public class PedidoEnvioFactoryTests
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
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', (NaoSim)false, (NaoSim)false, StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), new CodigoNBS("123456789"))
            .SetIss((Aliquota)0.05m, false)
            .SetIbsCbs(new InformacoesIbsCbs())
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)"3550308")
            .SetTomador(TomadorPadrao)
            .Build();

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_ConfiguracaoNula_ThrowsArgumentNullException()
    {
        Certificado? config = null;

        _ = Assert.Throws<ArgumentNullException>(() => new PedidoEnvioFactory(config!));
    }

    // ============================================
    // NewCpf — guard clauses
    // ============================================

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(cpf!, CriarRps()));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsNulo_ThrowsArgumentNullException(long cpfNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());
        Rps? rps = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(new Cpf(cpfNumber), rps!));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida(long cpfNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCpf(new Cpf(cpfNumber), CriarRps());
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_RpsContidoTemAssinaturaPreenchida(long cpfNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCpf(new Cpf(cpfNumber), CriarRps());
        Assert.NotNull(resultado.Rps?.Assinatura);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_CabecalhoContemCpfCorreto(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCpf(cpf, CriarRps());

        Assert.Equal(cpf.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cpf?.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_CabecalhoNaoContemCnpj(long cpfNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCpf(new Cpf(cpfNumber), CriarRps());
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cnpj);
    }

    // ============================================
    // NewCnpj
    // ============================================

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj(cnpj!, CriarRps()));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_RpsNulo_ThrowsArgumentNullException(long cnpjNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());
        Rps? rps = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj(new Cnpj(cnpjNumber.ToString("D14")), rps!));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida(long cnpjNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCnpj(new Cnpj(cnpjNumber.ToString("D14")), CriarRps());
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_RpsContidoTemAssinaturaPreenchida(long cnpjNumber)
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCnpj(new Cnpj(cnpjNumber.ToString("D14")), CriarRps());
        Assert.NotNull(resultado.Rps?.Assinatura);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_CabecalhoContemCnpjCorreto(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber.ToString("D14"));
        var factory = new PedidoEnvioFactory(CriarConfiguracao());

        PedidoEnvio resultado = factory.NewCnpj(cnpj, CriarRps());

        Assert.Equal(cnpj.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cnpj?.ToString());
    }
}