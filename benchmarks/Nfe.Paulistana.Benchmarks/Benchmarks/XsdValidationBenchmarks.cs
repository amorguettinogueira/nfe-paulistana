using BenchmarkDotNet.Attributes;
using Nfe.Paulistana.Xml;
using V1Pedido = Nfe.Paulistana.V1.Models.Operations.PedidoEnvioLote;
using V2Pedido = Nfe.Paulistana.V2.Models.Operations.PedidoEnvioLote;

namespace Nfe.Paulistana.Benchmarks.Benchmarks;

/// <summary>
/// Mede o custo de validação XSD do payload de envio em lote de RPS,
/// comparando V1 e V2 do schema da NF-e Paulistana.
/// O benchmark exercita o caminho completo: serialização para XmlDocument
/// seguida da validação contra o XmlSchemaSet embutido no assembly.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class XsdValidationBenchmarks
{
    private V1Pedido _v1Pedido = null!;
    private V2Pedido _v2Pedido = null!;

    [GlobalSetup]
    public void Setup()
    {
        _v1Pedido = new V1Pedido();
        _v2Pedido = new V2Pedido();
    }

    [Benchmark(Baseline = true, Description = "XSD Validate V1")]
    public bool ValidateV1()
    {
        _v1Pedido.IsValidXsd(out _);
        return true;
    }

    [Benchmark(Description = "XSD Validate V2")]
    public bool ValidateV2()
    {
        _v2Pedido.IsValidXsd(out _);
        return true;
    }
}