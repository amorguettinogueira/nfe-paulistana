using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoEnvioLoteRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoEnvioLoteRps
{
    public RetornoEnvioLoteRps() { }

    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoInformacoesLote? Cabecalho { get; set; }

    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }

    [XmlElement("ChaveNFeRPS", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public ChaveNFeRps[]? ChavesNFeRps { get; set; }
}
