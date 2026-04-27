using BenchmarkDotNet.Attributes;
using Nfe.Paulistana.Infrastructure;
using V1Response = Nfe.Paulistana.V1.Infrastructure.Envelope.EnvioLoteRpsResponse;
using V2Response = Nfe.Paulistana.V2.Infrastructure.Envelope.EnvioLoteRpsResponse;

namespace Nfe.Paulistana.Benchmarks.Benchmarks;

/// <summary>
/// Mede o custo de deserialização do envelope SOAP da resposta de envio em lote de RPS,
/// comparando V1 e V2 do schema da NF-e Paulistana.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class SoapDeserializationBenchmarks
{
    private byte[] _v1ResponseBytes = null!;
    private byte[] _v2ResponseBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        using MemoryStream v1Stream = SoapClient.SerializeEnvelope(new SoapEnvelope<V1Response>(new V1Response()));
        _v1ResponseBytes = v1Stream.ToArray();

        using MemoryStream v2Stream = SoapClient.SerializeEnvelope(new SoapEnvelope<V2Response>(new V2Response()));
        _v2ResponseBytes = v2Stream.ToArray();
    }

    [Benchmark(Baseline = true, Description = "Deserialize V1")]
    public SoapEnvelope<V1Response> DeserializeV1()
    {
        using var stream = new MemoryStream(_v1ResponseBytes);
        return SoapClient.DeserializeEnvelope<V1Response>(stream);
    }

    [Benchmark(Description = "Deserialize V2")]
    public SoapEnvelope<V2Response> DeserializeV2()
    {
        using var stream = new MemoryStream(_v2ResponseBytes);
        return SoapClient.DeserializeEnvelope<V2Response>(stream);
    }
}