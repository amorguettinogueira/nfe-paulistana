using BenchmarkDotNet.Attributes;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using V1Factory = Nfe.Paulistana.V1.Builders.PedidoEnvioLoteFactory;
using V1InscricaoMunicipal = Nfe.Paulistana.V1.Models.DataTypes.InscricaoMunicipal;
using V1Pedido = Nfe.Paulistana.V1.Models.Operations.PedidoEnvioLote;
using V1Rps = Nfe.Paulistana.V1.Models.Domain.Rps;
using V1RpsBuilder = Nfe.Paulistana.V1.Builders.RpsBuilder;
using V1StatusNfe = Nfe.Paulistana.Models.Enums.StatusNfe;
using V1Tomador = Nfe.Paulistana.V1.Models.Domain.Tomador;
using V1TomadorBuilder = Nfe.Paulistana.V1.Builders.TomadorBuilder;
using V2Factory = Nfe.Paulistana.V2.Builders.PedidoEnvioLoteFactory;
using V2IbsCbsBuilder = Nfe.Paulistana.V2.Builders.InformacoesIbsCbsBuilder;
using V2InformacoesIbsCbs = Nfe.Paulistana.V2.Models.Domain.InformacoesIbsCbs;
using V2InscricaoMunicipal = Nfe.Paulistana.V2.Models.DataTypes.InscricaoMunicipal;
using V2Pedido = Nfe.Paulistana.V2.Models.Operations.PedidoEnvioLote;
using V2Rps = Nfe.Paulistana.V2.Models.Domain.Rps;
using V2RpsBuilder = Nfe.Paulistana.V2.Builders.RpsBuilder;
using V2Tomador = Nfe.Paulistana.V2.Models.Domain.Tomador;
using V2TomadorBuilder = Nfe.Paulistana.V2.Builders.TomadorBuilder;

namespace Nfe.Paulistana.Benchmarks.Benchmarks;

/// <summary>
/// Mede o custo de construção e assinatura de um lote de RPS via
/// <see cref="V1Factory"/> e <see cref="V2Factory"/>, comparando V1 e V2
/// do schema da NF-e Paulistana.
/// <para>
/// O benchmark exercita o caminho crítico de cada chamada à library:
/// N assinaturas RSA-2048 (uma por RPS) mais uma assinatura do documento de lote,
/// construção de documentos XML para assinatura XmlDsig e acumulação de totalizações.
/// O parâmetro <see cref="TamanhoLote"/> controla quantos RPS são incluídos no lote,
/// permitindo observar como o custo escala com N+1 assinaturas.
/// </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class LoteSigningBenchmarks
{
    private const long CpfValido = 63596780047L;

    [Params(1, 10, 50)]
    public int TamanhoLote { get; set; }

    private V1Factory _v1Factory = null!;
    private V2Factory _v2Factory = null!;
    private Cpf _cpf = null!;
    private V1Rps[] _v1Rps = null!;
    private V2Rps[] _v2Rps = null!;

    [GlobalSetup]
    public void Setup()
    {
        var cert = CriarCertificado();
        _cpf = new Cpf(CpfValido);
        _v1Factory = new V1Factory(cert);
        _v2Factory = new V2Factory(cert);

        var v1Tomador = V1TomadorBuilder.NewCpf(_cpf).Build();
        var v2Tomador = V2TomadorBuilder.NewCpf(_cpf).Build();
        var ibsCbs = V2IbsCbsBuilder.New()
            .SetUsoOuConsumoPessoal(new NaoSim(false))
            .SetCodigoOperacaoFornecimento(new CodigoOperacao("010101"))
            .SetClassificacaoTributaria(new ClassificacaoTributaria("010101"))
            .Build();

        _v1Rps = [.. Enumerable.Range(0, TamanhoLote).Select(i => CriarRpsV1(v1Tomador, i + 1))];
        _v2Rps = [.. Enumerable.Range(0, TamanhoLote).Select(i => CriarRpsV2(v2Tomador, ibsCbs, i + 1))];
    }

    /// <summary>
    /// Assina um lote usando o schema V1 (baseline).
    /// Custo: 1 construção de X509Certificate2 + N assinaturas RSA-2048 de RPS + 1 assinatura do lote.
    /// </summary>
    [Benchmark(Baseline = true, Description = "Sign Lote V1")]
    public V1Pedido SignV1()
        => _v1Factory.NewCpf(_cpf, false, _v1Rps);

    /// <summary>
    /// Assina um lote usando o schema V2 (inclui IBS/CBS e totalizações adicionais).
    /// Custo: 1 construção de X509Certificate2 + N assinaturas RSA-2048 de RPS + 1 assinatura do lote.
    /// </summary>
    [Benchmark(Description = "Sign Lote V2")]
    public V2Pedido SignV2()
        => _v2Factory.NewCpf(_cpf, false, _v2Rps);

    private static Certificado CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Benchmark", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var cert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
        return new Certificado
        {
            RawData = new ReadOnlyCollection<byte>(cert.Export(X509ContentType.Pfx))
        };
    }

    private static V1Rps CriarRpsV1(V1Tomador tomador, int numero) =>
        V1RpsBuilder.New(
                new V1InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(numero),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', V1StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(tomador)
            .Build();

    private static V2Rps CriarRpsV2(V2Tomador tomador, V2InformacoesIbsCbs ibsCbs, int numero) =>
        V2RpsBuilder.New(
                new V2InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(numero),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', new NaoSim(false), new NaoSim(false))
            .SetServico(new CodigoServico(7617), new CodigoNBS("123456789"))
            .SetIss((Aliquota)0.05m, false)
            .SetIbsCbs(ibsCbs)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)3550308)
            .SetTomador(tomador)
            .Build();
}
