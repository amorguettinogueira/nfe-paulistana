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
    private string _v1ResponseXml = null!;
    private string _v2ResponseXml = null!;

    [GlobalSetup]
    public void Setup()
    {
        _v1ResponseXml = SoapClient.SerializeEnvelope(new SoapEnvelope<V1Response>(new V1Response()));
        _v2ResponseXml = SoapClient.SerializeEnvelope(new SoapEnvelope<V2Response>(new V2Response()));
    }

    [Benchmark(Baseline = true, Description = "Deserialize V1")]
    public SoapEnvelope<V1Response> DeserializeV1() =>
        SoapClient.DeserializeEnvelope<V1Response>(_v1ResponseXml);

    [Benchmark(Description = "Deserialize V2")]
    public SoapEnvelope<V2Response> DeserializeV2() =>
        SoapClient.DeserializeEnvelope<V2Response>(_v2ResponseXml);
}