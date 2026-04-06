using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V1.Models.DataTypes;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Modelo de dados do RPS (Recibo Provisório de Serviços) para submissão à NF-e Paulistana.
/// Representa uma única solicitação de emissão de nota fiscal junto à Prefeitura de São Paulo.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpRPS</c>, linha 798.
/// Construa instâncias via <see cref="Nfe.Paulistana.Builders.RpsBuilder"/>.
/// </para>
/// <para><strong>Notas Arquiteturais:</strong></para>
/// <list type="bullet">
/// <item>
/// <strong>Modelo de dados puro:</strong> <see cref="Rps"/> é um contêiner de dados
/// que implementa <see cref="Nfe.Paulistana.Models.ISignedElement"/>.
/// Não contém lógica de negócio além da validação dos campos no construtor.
/// </item>
/// <item>
/// <strong>Geração do texto de assinatura:</strong> Delegada a implementações de
/// <c>IObjectSignatureGenerator&lt;Rps&gt;</c>, mantendo o modelo desacoplado
/// do formato de assinatura digital.
/// </item>
/// </list>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class Rps : ISignedElement
{
    private bool? _issRetidoIntermediario;

    public Rps(ChaveRps chaveRps,
               TipoRps tipoRps,
               DataXsd dataEmissao,
               StatusNfe statusNfe,
               TributacaoNfe tributacaoNfe,
               Valor valorServicos,
               Valor valorDeducoes,
               CodigoServico codigoServico,
               Aliquota aliquotaServicos,
               bool issRetido,
               Discriminacao discriminacao,
               Valor? valorPis = null,
               Valor? valorCofins = null,
               Valor? valorInss = null,
               Valor? valorIr = null,
               Valor? valorCsll = null,
               CpfOrCnpj? cpfOrCnpjTomador = null,
               InscricaoMunicipal? inscricaoMunicipalTomador = null,
               InscricaoEstadual? inscricaoEstadualTomador = null,
               RazaoSocial? razaoSocialTomador = null,
               Endereco? enderecoTomador = null,
               Email? emailTomador = null,
               CpfOrCnpj? cpfCnpjIntermediario = null,
               InscricaoMunicipal? inscricaoMunicipalIntermediario = null,
               bool? issRetidoIntermediario = null,
               Email? emailIntermediario = null,
               Valor? valorCargaTributaria = null,
               Percentual? percentualCargaTributaria = null,
               FonteCargaTributaria? fonteCargaTributaria = null,
               Numero? codigoCei = null,
               Numero? matriculaObra = null,
               CodigoIbge? municipioPrestacao = null,
               Numero? numeroEncapsulamento = null,
               Valor? valorTotalRecebido = null)
    {
        ChaveRps = chaveRps ?? throw new ArgumentNullException(nameof(chaveRps));
        TipoRps = tipoRps;
        DataEmissao = dataEmissao ?? throw new ArgumentNullException(nameof(dataEmissao));
        StatusRps = statusNfe;
        TributacaoRps = tributacaoNfe ?? throw new ArgumentNullException(nameof(tributacaoNfe));
        ValorServicos = valorServicos ?? throw new ArgumentNullException(nameof(valorServicos));
        ValorDeducoes = valorDeducoes ?? throw new ArgumentNullException(nameof(valorDeducoes));
        ValorPis = valorPis;
        ValorCofins = valorCofins;
        ValorInss = valorInss;
        ValorIr = valorIr;
        ValorCsll = valorCsll;
        CodigoServico = codigoServico ?? throw new ArgumentNullException(nameof(codigoServico));
        AliquotaServicos = aliquotaServicos ?? throw new ArgumentNullException(nameof(aliquotaServicos));
        IssRetido = issRetido;
        CpfOrCnpjTomador = cpfOrCnpjTomador;
        InscricaoMunicipalTomador = inscricaoMunicipalTomador;
        InscricaoEstadualTomador = inscricaoEstadualTomador;
        RazaoSocialTomador = razaoSocialTomador;
        EnderecoTomador = enderecoTomador;
        EmailTomador = emailTomador;
        CpfCnpjIntermediario = cpfCnpjIntermediario;
        InscricaoMunicipalIntermediario = inscricaoMunicipalIntermediario;
        IssRetidoIntermediario = issRetidoIntermediario;
        EmailIntermediario = emailIntermediario;
        Discriminacao = discriminacao ?? throw new ArgumentNullException(nameof(discriminacao));
        ValorCargaTributaria = valorCargaTributaria;
        PercentualCargaTributaria = percentualCargaTributaria;
        FonteCargaTributaria = fonteCargaTributaria;
        CodigoCei = codigoCei;
        MatriculaObra = matriculaObra;
        MunicipioPrestacao = municipioPrestacao;
        NumeroEncapsulamento = numeroEncapsulamento;
        ValorTotalRecebido = valorTotalRecebido;
    }

    /// <summary>Inicializa uma instância vazia. Exclusivo para desserialização XML via reflection.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Rps() : this(
        new ChaveRps(),
        default,
        new DataXsd(),
        default,
        new TributacaoNfe(),
        new Valor(),
        new Valor(),
        new CodigoServico(),
        new Aliquota(),
        false,
        new Discriminacao())
    { }

    /// <summary>
    /// Assinatura digital do documento em formato Base64, conforme XMLDSig.
    /// Preenchida pelo serviço de assinatura após a construção do objeto —
    /// não deve ser definida manualmente.
    /// </summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "base64Binary")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is only used during XML signing and goal is to be serialized as a base64")]
    public byte[]? Assinatura { get; set; }

    /// <summary>
    /// Chave composta que identifica unicamente o RPS: Inscrição Municipal do prestador,
    /// Série e Número do RPS.
    /// </summary>
    /// <exception cref="ArgumentNullException">Lançado no construtor se <c>chaveRps</c> for nulo.</exception>
    [XmlElement("ChaveRPS", Form = XmlSchemaForm.Unqualified)]
    public ChaveRps? ChaveRps { get; set; }

    /// <summary>Tipo do RPS (ex.: Recibo Provisório de Serviços).</summary>
    [XmlElement("TipoRPS", Form = XmlSchemaForm.Unqualified)]
    public TipoRps? TipoRps { get; set; }

    /// <summary>Data de emissão do RPS.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DataEmissao { get; set; }

    /// <summary>Situação do RPS: Normal ou Cancelado.</summary>
    [XmlElement("StatusRPS", Form = XmlSchemaForm.Unqualified)]
    public StatusNfe? StatusRps { get; set; }

    /// <summary>Regime de tributação aplicado ao serviço.</summary>
    [XmlElement("TributacaoRPS", Form = XmlSchemaForm.Unqualified)]
    public TributacaoNfe? TributacaoRps { get; set; }

    /// <summary>Valor bruto dos serviços prestados em R$.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorServicos { get; set; }

    /// <summary>
    /// Valor das deduções aplicadas sobre o serviço em R$.
    /// Quando não informado via <see cref="Nfe.Paulistana.Builders.RpsBuilder"/>,
    /// o builder injeta <c>0</c> — valor exigido pelo XSD quando não há deduções.
    /// </summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorDeducoes { get; set; }

    /// <summary>Valor da contribuição ao Programa de Integração Social (PIS) em R$. Opcional.</summary>
    [XmlElement("ValorPIS", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorPis { get; set; }

    /// <summary>Valor da Contribuição para o Financiamento da Seguridade Social (COFINS) em R$. Opcional.</summary>
    [XmlElement("ValorCOFINS", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorCofins { get; set; }

    /// <summary>Valor da contribuição ao Instituto Nacional do Seguro Social (INSS) em R$. Opcional.</summary>
    [XmlElement("ValorINSS", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorInss { get; set; }

    /// <summary>Valor do Imposto sobre a Renda Retido na Fonte (IRRF) em R$. Opcional.</summary>
    [XmlElement("ValorIR", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorIr { get; set; }

    /// <summary>Valor da Contribuição Social sobre o Lucro Líquido (CSLL) em R$. Opcional.</summary>
    [XmlElement("ValorCSLL", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorCsll { get; set; }

    /// <summary>Código do serviço prestado conforme lista da Prefeitura de São Paulo.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CodigoServico? CodigoServico { get; set; }

    /// <summary>Alíquota aplicada ao serviço para cálculo do ISS.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Aliquota? AliquotaServicos { get; set; }

    /// <summary>Indica se o ISS foi retido na fonte pelo tomador do serviço.</summary>
    [XmlElement("ISSRetido", Form = XmlSchemaForm.Unqualified)]
    public bool IssRetido { get; set; }

    /// <summary>CPF ou CNPJ do tomador do serviço. Opcional.</summary>
    [XmlElement("CPFCNPJTomador", Form = XmlSchemaForm.Unqualified)]
    public CpfOrCnpj? CpfOrCnpjTomador { get; set; }

    /// <summary>Inscrição Municipal do tomador do serviço na Prefeitura de São Paulo. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? InscricaoMunicipalTomador { get; set; }

    /// <summary>Inscrição Estadual do tomador do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoEstadual? InscricaoEstadualTomador { get; set; }

    /// <summary>Razão social ou nome do tomador do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public RazaoSocial? RazaoSocialTomador { get; set; }

    /// <summary>Endereço completo do tomador do serviço. Opcional.</summary>
    [XmlElement("EnderecoTomador", Form = XmlSchemaForm.Unqualified)]
    public Endereco? EnderecoTomador { get; set; }

    /// <summary>Endereço de e-mail do tomador do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Email? EmailTomador { get; set; }

    /// <summary>CPF ou CNPJ do intermediário do serviço. Opcional.</summary>
    [XmlElement("CPFCNPJIntermediario", Form = XmlSchemaForm.Unqualified)]
    public CpfOrCnpj? CpfCnpjIntermediario { get; set; }

    /// <summary>Inscrição Municipal do intermediário do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? InscricaoMunicipalIntermediario { get; set; }

    /// <summary>
    /// Indica se o ISS foi retido na fonte pelo intermediário do serviço. Opcional.
    /// Quando <see langword="null"/>, o elemento é omitido do XML via <see cref="IssRetidoIntermediarioSpecified"/>.
    /// </summary>
    [XmlElement("ISSRetidoIntermediario", Form = XmlSchemaForm.Unqualified)]
    public bool? IssRetidoIntermediario
    {
        get => _issRetidoIntermediario;
        set
        {
            _issRetidoIntermediario = value;
            IssRetidoIntermediarioSpecified = IssRetidoIntermediario.HasValue;
        }
    }

    /// <summary>
    /// Controla a serialização XML de <see cref="IssRetidoIntermediario"/>.
    /// Implementa o padrão <c>*Specified</c> do <see cref="XmlSerializer"/>:
    /// quando <c>false</c>, o elemento é omitido do XML mesmo que
    /// <see cref="IssRetidoIntermediario"/> tenha valor — necessário para campos
    /// <c>bool?</c> que devem ser condicionalmente serializados.
    /// </summary>
    [XmlIgnore]
    public bool IssRetidoIntermediarioSpecified { get; set; }

    /// <summary>Endereço de e-mail do intermediário do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Email? EmailIntermediario { get; set; }

    /// <summary>Discriminação dos serviços prestados, conforme descrição da nota fiscal.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Discriminacao? Discriminacao { get; set; }

    /// <summary>Valor total da carga tributária incidente sobre o serviço em R$. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorCargaTributaria { get; set; }

    /// <summary>Percentual da carga tributária sobre o valor do serviço. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Percentual? PercentualCargaTributaria { get; set; }

    /// <summary>Fonte das informações da carga tributária (ex.: IBPT). Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public FonteCargaTributaria? FonteCargaTributaria { get; set; }

    /// <summary>Número do Cadastro Específico do INSS (CEI) para serviços de construção civil. Opcional.</summary>
    [XmlElement("CodigoCEI", Form = XmlSchemaForm.Unqualified)]
    public Numero? CodigoCei { get; set; }

    /// <summary>Matrícula da obra junto ao INSS. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Numero? MatriculaObra { get; set; }

    /// <summary>Código IBGE do município onde o serviço foi efetivamente prestado. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CodigoIbge? MunicipioPrestacao { get; set; }

    /// <summary>Número de encapsulamento utilizado em serviços de construção civil com retenção. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroEncapsulamento { get; set; }

    /// <summary>Valor total recebido pelo prestador, incluindo deduções e retenções. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorTotalRecebido { get; set; }
}