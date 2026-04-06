using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoEnvioLoteFactory"/>:
/// guard clauses, assinatura de todos os RPS, e verificação das totalizações
/// automáticas de <see cref="CabecalhoLote"/>.
/// </summary>
public class PedidoEnvioLoteFactoryTests
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

    private static Rps CriarRps(DateTime dataEmissao, decimal valorServicos) =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.NotaFiscalConjugada,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(dataEmissao), (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)valorServicos)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(TomadorPadrao)
            .Build();

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_ConfiguracaoNula_ThrowsArgumentNullException()
    {
        Certificado? config = null;

        _ = Assert.Throws<ArgumentNullException>(() => new PedidoEnvioLoteFactory(config!));
    }

    // ============================================
    // NewCpf — guard clauses
    // ============================================

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(cpf!, false, [CriarRps(DateTime.Today, 1000m)]));
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsListaNula_ThrowsArgumentNullException(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(new Cpf(cpfNumber), false, null!));
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsListaVazia_ThrowsArgumentException(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        _ = Assert.Throws<ArgumentException>(() => factory.NewCpf(new Cpf(cpfNumber), false, []));
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsListaComElementoNulo_ThrowsArgumentException(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        _ = Assert.Throws<ArgumentException>(() => factory.NewCpf(new Cpf(cpfNumber), false, [null!]));
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [CriarRps(DateTime.Today, 1000m)]);
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_CabecalhoContemCpfCorreto(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCpf(cpf, false, [CriarRps(DateTime.Today, 1000m)]);

        Assert.Equal(cpf.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cpf?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_ArgumentosValidos_CabecalhoNaoContemCnpj(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [CriarRps(DateTime.Today, 1000m)]);
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cnpj);
    }

    // ============================================
    // NewCnpj — guard clauses
    // ============================================

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Cnpj? cnpj = null;
        Rps rps = CriarRps(DateTime.Today, 1000m);

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj(cnpj!, false, [rps]));
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_RpsNulo_ThrowsArgumentNullException(long cnpjNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps? rps = null;

        _ = Assert.Throws<ArgumentException>(() => factory.NewCnpj(new Cnpj(cnpjNumber), false, [rps!]));
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida(long cnpjNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCnpj(new Cnpj(cnpjNumber), false, [CriarRps(DateTime.Today, 1000m)]);
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_CabecalhoContemCnpjCorreto(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCnpj(cnpj, false, [CriarRps(DateTime.Today, 1000m)]);

        Assert.Equal(cnpj.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cnpj?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_ArgumentosValidos_CabecalhoNaoContemCpf(long cnpjNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        PedidoEnvioLote resultado = factory.NewCnpj(new Cnpj(cnpjNumber), false, [CriarRps(DateTime.Today, 1000m)]);
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cpf);
    }

    // ============================================
    // NewCpf — assinatura
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsValido_LoteTemAssinaturaPreenchida(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps = CriarRps(new DateTime(2024, 1, 20), 1000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps]);

        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_RpsValido_RpsContidoTemAssinaturaPreenchida(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps = CriarRps(new DateTime(2024, 1, 20), 1000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps]);

        Assert.All(resultado.Rps!, r => Assert.NotNull(r.Assinatura));
    }

    // ============================================
    // NewCpf — totalizações automáticas
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_DoisRps_QtdRpsIgualADois(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps1 = CriarRps(new DateTime(2024, 1, 1), 1000m);
        Rps rps2 = CriarRps(new DateTime(2024, 1, 31), 2000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps1, rps2]);

        Assert.Equal("2", resultado.Cabecalho?.QtdRps?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_DoisRps_ValorTotalServicosEhSomaDosValores(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps1 = CriarRps(new DateTime(2024, 1, 1), 1000m);
        Rps rps2 = CriarRps(new DateTime(2024, 1, 31), 2000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps1, rps2]);

        Assert.Equal(new Valor(3000m).ToString(), resultado.Cabecalho?.ValorTotalServicos?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_SemDeducoes_ValorTotalDeducoesEhNulo(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps = CriarRps(new DateTime(2024, 1, 20), 1000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps]);

        Assert.Null(resultado.Cabecalho?.ValorTotalDeducoes);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_DoisRps_DtInicioEhDataMaisAntiga(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var dataInicio = new DateTime(2024, 1, 1);
        Rps rps1 = CriarRps(dataInicio, 1000m);
        Rps rps2 = CriarRps(new DateTime(2024, 1, 31), 2000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps1, rps2]);

        Assert.Equal(new DataXsd(dataInicio).ToString(), resultado.Cabecalho?.DtInicio?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_DoisRps_DtFimEhDataMaisRecente(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var dataFim = new DateTime(2024, 1, 31);
        Rps rps1 = CriarRps(new DateTime(2024, 1, 1), 1000m);
        Rps rps2 = CriarRps(dataFim, 2000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps1, rps2]);

        Assert.Equal(new DataXsd(dataFim).ToString(), resultado.Cabecalho?.DtFim?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_Transacao_PropagadaParaCabecalho(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps = CriarRps(new DateTime(2024, 1, 20), 1000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), true, [rps]);

        Assert.True(resultado.Cabecalho?.Transacao);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_DoisRps_ArrayRpsContemAmbos(long cpfNumber)
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        Rps rps1 = CriarRps(new DateTime(2024, 1, 1), 1000m);
        Rps rps2 = CriarRps(new DateTime(2024, 1, 31), 2000m);

        PedidoEnvioLote resultado = factory.NewCpf(new Cpf(cpfNumber), false, [rps1, rps2]);

        Assert.Equal(2, resultado.Rps?.Length);
    }
}