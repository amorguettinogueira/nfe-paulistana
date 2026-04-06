using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Representa uma NFS-e (Nota Fiscal de Serviço Eletrônica) retornada pelo webservice
/// da NF-e Paulistana (<c>tpNFe</c>).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo complexo <c>tpNFe</c>.
/// Utilizado como elemento filho no <see cref="RetornoConsulta"/>.
/// Utiliza tipos primitivos para garantir desserialização resiliente,
/// independente das regras de validação aplicadas no envio.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
public sealed class NfeModel
{
    /// <summary>Inicializa uma nova instância de <see cref="NfeModel"/>.</summary>
    [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public NfeModel() { }

    /// <summary>Assinatura digital da NFS-e. Opcional.</summary>
    [XmlElement("Assinatura", Namespace = "")]
    public string? Assinatura { get; set; }

    /// <summary>Chave de identificação da NFS-e.</summary>
    [XmlElement("ChaveNFe", Namespace = "")]
    public ChaveNFe? ChaveNFe { get; set; }

    /// <summary>Data e hora de emissão da NFS-e.</summary>
    [XmlElement("DataEmissaoNFe", Namespace = "")]
    public DateTime DataEmissaoNFe { get; set; }

    /// <summary>Número do lote gerador da NFS-e. Opcional.</summary>
    [XmlElement("NumeroLote", Namespace = "")]
    public string? NumeroLote { get; set; }

    /// <summary>Chave do RPS que originou a NFS-e. Opcional.</summary>
    [XmlElement("ChaveRPS", Namespace = "")]
    public ChaveRps? ChaveRps { get; set; }

    /// <summary>Tipo do RPS emitido. Opcional.</summary>
    [XmlElement("TipoRPS", Namespace = "")]
    public string? TipoRps { get; set; }

    /// <summary>Data de emissão do RPS que originou a NFS-e. Opcional.</summary>
    [XmlElement("DataEmissaoRPS", Namespace = "")]
    public string? DataEmissaoRps { get; set; }

    /// <summary>Data do fato gerador da NFS-e.</summary>
    [XmlElement("DataFatoGeradorNFe", Namespace = "")]
    public DateTime DataFatoGeradorNFe { get; set; }

    /// <summary>CPF ou CNPJ do prestador do serviço.</summary>
    [XmlElement("CPFCNPJPrestador", Namespace = "")]
    public CpfCnpj? CpfCnpjPrestador { get; set; }

    /// <summary>Nome ou razão social do prestador.</summary>
    [XmlElement("RazaoSocialPrestador", Namespace = "")]
    public string? RazaoSocialPrestador { get; set; }

    /// <summary>Endereço do prestador.</summary>
    [XmlElement("EnderecoPrestador", Namespace = "")]
    public EnderecoNfe? EnderecoPrestador { get; set; }

    /// <summary>E-mail do prestador. Opcional.</summary>
    [XmlElement("EmailPrestador", Namespace = "")]
    public string? EmailPrestador { get; set; }

    /// <summary>Status da NFS-e.</summary>
    [XmlElement("StatusNFe", Namespace = "")]
    public string? StatusNFe { get; set; }

    /// <summary>Data de cancelamento da NFS-e. Opcional.</summary>
    [XmlElement("DataCancelamento", Namespace = "")]
    public string? DataCancelamento { get; set; }

    /// <summary>Tributação da NFS-e (código de tributação).</summary>
    [XmlElement("TributacaoNFe", Namespace = "")]
    public string? TributacaoNFe { get; set; }

    /// <summary>Opção pelo Simples Nacional.</summary>
    [XmlElement("OpcaoSimples", Namespace = "")]
    public string? OpcaoSimples { get; set; }

    /// <summary>Número da guia vinculada à NFS-e. Opcional.</summary>
    [XmlElement("NumeroGuia", Namespace = "")]
    public string? NumeroGuia { get; set; }

    /// <summary>Data de quitação da guia vinculada à NFS-e. Opcional.</summary>
    [XmlElement("DataQuitacaoGuia", Namespace = "")]
    public string? DataQuitacaoGuia { get; set; }

    /// <summary>Valor dos serviços prestados.</summary>
    [XmlElement("ValorServicos", Namespace = "")]
    public string? ValorServicos { get; set; }

    /// <summary>Valor das deduções. Opcional.</summary>
    [XmlElement("ValorDeducoes", Namespace = "")]
    public string? ValorDeducoes { get; set; }

    /// <summary>Valor da retenção do PIS. Opcional.</summary>
    [XmlElement("ValorPIS", Namespace = "")]
    public string? ValorPis { get; set; }

    /// <summary>Valor da retenção do COFINS. Opcional.</summary>
    [XmlElement("ValorCOFINS", Namespace = "")]
    public string? ValorCofins { get; set; }

    /// <summary>Valor da retenção do INSS. Opcional.</summary>
    [XmlElement("ValorINSS", Namespace = "")]
    public string? ValorInss { get; set; }

    /// <summary>Valor da retenção do IR. Opcional.</summary>
    [XmlElement("ValorIR", Namespace = "")]
    public string? ValorIr { get; set; }

    /// <summary>Valor da retenção do CSLL. Opcional.</summary>
    [XmlElement("ValorCSLL", Namespace = "")]
    public string? ValorCsll { get; set; }

    /// <summary>Código do serviço prestado.</summary>
    [XmlElement("CodigoServico", Namespace = "")]
    public string? CodigoServico { get; set; }

    /// <summary>Alíquota dos serviços.</summary>
    [XmlElement("AliquotaServicos", Namespace = "")]
    public string? AliquotaServicos { get; set; }

    /// <summary>Valor do ISS.</summary>
    [XmlElement("ValorISS", Namespace = "")]
    public string? ValorIss { get; set; }

    /// <summary>Valor do crédito gerado.</summary>
    [XmlElement("ValorCredito", Namespace = "")]
    public string? ValorCredito { get; set; }

    /// <summary>Indica se o ISS foi retido.</summary>
    [XmlElement("ISSRetido", Namespace = "")]
    public bool IssRetido { get; set; }

    /// <summary>CPF ou CNPJ do tomador do serviço. Opcional.</summary>
    [XmlElement("CPFCNPJTomador", Namespace = "")]
    public CpfCnpj? CpfCnpjTomador { get; set; }

    /// <summary>Inscrição Municipal do tomador. Opcional.</summary>
    [XmlElement("InscricaoMunicipalTomador", Namespace = "")]
    public string? InscricaoMunicipalTomador { get; set; }

    /// <summary>Inscrição Estadual do tomador. Opcional.</summary>
    [XmlElement("InscricaoEstadualTomador", Namespace = "")]
    public string? InscricaoEstadualTomador { get; set; }

    /// <summary>Nome ou razão social do tomador. Opcional.</summary>
    [XmlElement("RazaoSocialTomador", Namespace = "")]
    public string? RazaoSocialTomador { get; set; }

    /// <summary>Endereço do tomador. Opcional.</summary>
    [XmlElement("EnderecoTomador", Namespace = "")]
    public EnderecoNfe? EnderecoTomador { get; set; }

    /// <summary>E-mail do tomador. Opcional.</summary>
    [XmlElement("EmailTomador", Namespace = "")]
    public string? EmailTomador { get; set; }

    /// <summary>CPF ou CNPJ do intermediário de serviço. Opcional.</summary>
    [XmlElement("CPFCNPJIntermediario", Namespace = "")]
    public CpfCnpj? CpfCnpjIntermediario { get; set; }

    /// <summary>Inscrição Municipal do intermediário de serviço. Opcional.</summary>
    [XmlElement("InscricaoMunicipalIntermediario", Namespace = "")]
    public string? InscricaoMunicipalIntermediario { get; set; }

    /// <summary>Indica retenção do ISS pelo intermediário de serviço. Opcional.</summary>
    [XmlElement("ISSRetidoIntermediario", Namespace = "")]
    public string? IssRetidoIntermediario { get; set; }

    /// <summary>E-mail do intermediário de serviço. Opcional.</summary>
    [XmlElement("EmailIntermediario", Namespace = "")]
    public string? EmailIntermediario { get; set; }

    /// <summary>Discriminação dos serviços prestados.</summary>
    [XmlElement("Discriminacao", Namespace = "")]
    public string? Discriminacao { get; set; }

    /// <summary>Valor da carga tributária total em R$. Opcional.</summary>
    [XmlElement("ValorCargaTributaria", Namespace = "")]
    public string? ValorCargaTributaria { get; set; }

    /// <summary>Valor percentual da carga tributária. Opcional.</summary>
    [XmlElement("PercentualCargaTributaria", Namespace = "")]
    public string? PercentualCargaTributaria { get; set; }

    /// <summary>Fonte de informação da carga tributária. Opcional.</summary>
    [XmlElement("FonteCargaTributaria", Namespace = "")]
    public string? FonteCargaTributaria { get; set; }

    /// <summary>Código do CEI — Cadastro Específico do INSS. Opcional.</summary>
    [XmlElement("CodigoCEI", Namespace = "")]
    public string? CodigoCei { get; set; }

    /// <summary>Matrícula da obra. Opcional.</summary>
    [XmlElement("MatriculaObra", Namespace = "")]
    public string? MatriculaObra { get; set; }

    /// <summary>Código do município de prestação do serviço. Opcional.</summary>
    [XmlElement("MunicipioPrestacao", Namespace = "")]
    public string? MunicipioPrestacao { get; set; }

    /// <summary>Número do encapsulamento. Opcional.</summary>
    [XmlElement("NumeroEncapsulamento", Namespace = "")]
    public string? NumeroEncapsulamento { get; set; }

    /// <summary>Valor total recebido. Opcional.</summary>
    [XmlElement("ValorTotalRecebido", Namespace = "")]
    public string? ValorTotalRecebido { get; set; }
}