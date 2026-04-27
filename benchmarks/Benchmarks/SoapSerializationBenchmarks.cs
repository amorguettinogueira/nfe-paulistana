using BenchmarkDotNet.Attributes;
using Nfe.Paulistana.Infrastructure;
using V1Pedido = Nfe.Paulistana.V1.Models.Operations.PedidoEnvioLote;
using V1Request = Nfe.Paulistana.V1.Infrastructure.Envelope.EnvioLoteRpsRequest;
using V2Pedido = Nfe.Paulistana.V2.Models.Operations.PedidoEnvioLote;
using V2Request = Nfe.Paulistana.V2.Infrastructure.Envelope.EnvioLoteRpsRequest;

namespace Nfe.Paulistana.Benchmarks.Benchmarks;

/// <summary>
/// Mede o custo de serialização do envelope SOAP para envio em lote de RPS,
/// comparando V1 e V2 do schema da NF-e Paulistana.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class SoapSerializationBenchmarks
{
    private SoapEnvelope<V1Request> _v1Envelope = null!;
    private SoapEnvelope<V2Request> _v2Envelope = null!;

    [GlobalSetup]
    public void Setup()
    {
        var v1Request = new V1Request(new MensagemXml<V1Pedido>(new V1Pedido()));
        _v1Envelope = new SoapEnvelope<V1Request>(v1Request);

        var v2Request = new V2Request(new MensagemXml<V2Pedido>(new V2Pedido()));
        _v2Envelope = new SoapEnvelope<V2Request>(v2Request);
    }

    [Benchmark(Baseline = true, Description = "Serialize V1")]
    public void SerializeV1()
    {
        using MemoryStream stream = SoapClient.SerializeEnvelope(_v1Envelope);
    }

    [Benchmark(Description = "Serialize V2")]
    public void SerializeV2()
    {
        using MemoryStream stream = SoapClient.SerializeEnvelope(_v2Envelope);
    }
}