using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Models.Validators;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// <para>
/// Construtor fluente de objetos <see cref="Rps"/> que implementa o padrão Step Builder,
/// garantindo em tempo de compilação que todos os campos obrigatórios sejam preenchidos
/// antes de <see cref="IRpsSetOptionals.Build"/> ser chamado.
/// </para>
/// <para>
/// A cadeia obrigatória de chamadas é:
/// <see cref="New"/> → <see cref="IRpsSetNfe.SetNFe"/> →
/// <see cref="IRpsSetServico.SetServico"/> → <see cref="IRpsSetIss.SetIss"/> →
/// [opcionais] → <see cref="IRpsSetOptionals.Build"/>.
/// </para>
/// <para>
/// Após <see cref="IRpsSetIss.SetIss"/>, o chamador acessa <see cref="IRpsSetOptionals"/>,
/// que expõe todos os campos opcionais e o método <see cref="IRpsSetOptionals.Build"/>.
/// O tomador de serviços, apesar de estar em <see cref="IRpsSetOptionals"/>, é validado
/// como obrigatório em <see cref="IRpsSetOptionals.Build"/>.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Step Builder:</strong> Cada interface da cadeia expõe apenas o próximo passo
/// obrigatório, eliminando a possibilidade de construção parcial ou inconsistente.
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed para proteger os invariantes
/// de validação e construção. Estensão via herança abriria brechas para estado inválido.
/// </item>
/// <item>
/// <strong>Value Objects:</strong> Todos os parâmetros são Value Objects type-safe validados
/// no momento da construção, garantindo que o <see cref="Rps"/> produzido seja sempre
/// semanticamente correto conforme o XSD da NF-e Paulistana.
/// </item>
/// <item>
/// <strong>Deduções:</strong> O campo <c>valorDeducoes</c> é opcional em
/// <see cref="IRpsSetServico.SetServico"/>; quando não informado, <see cref="IRpsSetOptionals.Build"/>
/// o preenche com zero — valor exigido pelo XSD quando não há deduções.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var rps = RpsBuilder.New(
///         (InscricaoMunicipal)39616924,
///         TipoRps.NotaFiscalConjugada,
///         (Numero)4105L,
///         (Discriminacao)"Desenvolvimento de software.",
///         (SerieRps)"BB")
///     .SetNFe((Data)new DateTime(2024, 1, 20), (TributacaoNfe)'T', StatusNfe.Normal)
///     .SetServico((CodigoServico)7617, (Valor)20500m)
///     .SetIss((Aliquota)0.05m, false)
///     .SetTributos(valorPis: (Valor)10m, valorCofins: (Valor)10m)
///     .SetTomador(tomador)
///     .Build();
/// </code>
/// </example>
public sealed class RpsBuilder :
    IRpsSetNfe,
    IRpsSetServico,
    IRpsSetIss,
    IRpsSetOptionals
{
    private const string InvalidTaxes = "Ao menos um dos tributos deve ser informado.";
    private const string InvalidTaxation = "Ao menos um dos atributos da carga tributária deve ser informado.";
    private const string InvalidArgument = "Atributo obrigatório não informado: {0}";

    private readonly TipoRps tipoRps;
    private readonly Numero numeroRps;
    private readonly Discriminacao discriminacao;
    private readonly SerieRps? serieRps;
    private DataXsd? dataEmissao;
    private TributacaoNfe? tributacaoNfe;
    private StatusNfe? statusNfe;
    private Valor? valorServicos;
    private Valor? valorDeducoes;
    private CodigoServico? codigoServico;
    private Aliquota? aliquotaServicos;
    private bool? issRetido;
    private Valor? valorPis;
    private Valor? valorCofins;
    private Valor? valorInss;
    private Valor? valorIr;
    private Valor? valorCsll;
    private Valor? valorCargaTributaria;
    private Percentual? percentualCargaTributaria;
    private FonteCargaTributaria? fonteCargaTributaria;
    private Tomador? tomador;
    private Intermediario? intermediario;
    private Numero? codigoCei;
    private Numero? matriculaObra;
    private CodigoIbge? municipioPrestacao;
    private Numero? numeroEncapsulamento;
    private Valor? valorTotalRecebido;
    private InscricaoMunicipal? inscricaoPrestador;

    private RpsBuilder(InscricaoMunicipal inscricaoPrestador,
                       TipoRps tipoRps,
                       Numero numeroRps,
                       Discriminacao discriminacao,
                       SerieRps? serieRps = null)
    {
        this.inscricaoPrestador = inscricaoPrestador;
        this.tipoRps = tipoRps;
        this.numeroRps = numeroRps;
        this.discriminacao = discriminacao;
        this.serieRps = serieRps;
    }

    /// <summary>
    /// Cria uma instância do construtor de Recibo Provisório de Serviços (RPS).
    /// </summary>
    /// <param name="inscricaoPrestador">Inscrição Municipal do prestador de serviços.</param>
    /// <param name="tipoRps">Tipo de RPS.</param>
    /// <param name="numeroRps">Número do RPS.</param>
    /// <param name="discriminacao">Discriminação dos serviços prestados.</param>
    /// <param name="serieRps">Série do RPS. Obrigatória para construção — se omitida, <see cref="IRpsSetOptionals.Build"/> lançará exceção.</param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetNfe"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inscricaoPrestador"/>, <paramref name="numeroRps"/> ou <paramref name="discriminacao"/> for nulo.</exception>
    public static IRpsSetNfe New(InscricaoMunicipal inscricaoPrestador,
                                 TipoRps tipoRps,
                                 Numero numeroRps,
                                 Discriminacao discriminacao,
                                 SerieRps? serieRps = null)
    {
        ArgumentNullException.ThrowIfNull(inscricaoPrestador, nameof(inscricaoPrestador));
        ArgumentNullException.ThrowIfNull(numeroRps, nameof(numeroRps));
        ArgumentNullException.ThrowIfNull(discriminacao, nameof(discriminacao));

        return new RpsBuilder(inscricaoPrestador, tipoRps, numeroRps, discriminacao, serieRps);
    }

    /// <summary>
    /// Adiciona ao RPS informações relativas à Nota Fiscal Eletrônica (NF-e).
    /// </summary>
    /// <param name="dataEmissao">Data de emissão do RPS.</param>
    /// <param name="tributacaoNFe">Tipo de tributação da NF-e.</param>
    /// <param name="statusNFe">Status da emissão da NF-e. Padrão: <see cref="StatusNfe.Normal"/>.</param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetServico"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="dataEmissao"/> ou <paramref name="tributacaoNFe"/> for nulo.</exception>
    public IRpsSetServico SetNFe(DataXsd dataEmissao,
                                 TributacaoNfe tributacaoNFe,
                                 StatusNfe statusNFe = StatusNfe.Normal)
    {
        ArgumentNullException.ThrowIfNull(dataEmissao, nameof(dataEmissao));
        ArgumentNullException.ThrowIfNull(tributacaoNFe, nameof(tributacaoNFe));

        this.dataEmissao = dataEmissao;
        statusNfe = statusNFe;
        tributacaoNfe = tributacaoNFe;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS informações sobre os serviços prestados.
    /// </summary>
    /// <param name="codigoServico">Código do serviço prestado.</param>
    /// <param name="valorServicos">Valor dos serviços em R$.</param>
    /// <param name="valorDeducoes">
    /// Valor das deduções em R$. Opcional: quando não informado,
    /// <see cref="IRpsSetOptionals.Build"/> utiliza zero como valor padrão.
    /// </param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetIss"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoServico"/> ou <paramref name="valorServicos"/> for nulo.</exception>
    public IRpsSetIss SetServico(CodigoServico codigoServico,
                                 Valor valorServicos,
                                 Valor? valorDeducoes = null)
    {
        ArgumentNullException.ThrowIfNull(codigoServico, nameof(codigoServico));
        ArgumentNullException.ThrowIfNull(valorServicos, nameof(valorServicos));

        this.codigoServico = codigoServico;
        this.valorServicos = valorServicos;
        this.valorDeducoes = valorDeducoes;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS detalhes do Imposto Sobre Serviços (ISS).
    /// </summary>
    /// <param name="aliquota">Alíquota do serviço prestado.</param>
    /// <param name="issRetido"><c>true</c> indica que o ISS já foi retido na fonte.</param>
    /// <returns>Próximo passo da cadeia: <see cref="IRpsSetOptionals"/>, que expõe campos opcionais e <see cref="IRpsSetOptionals.Build"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="aliquota"/> for nulo.</exception>
    public IRpsSetOptionals SetIss(Aliquota aliquota, bool issRetido)
    {
        ArgumentNullException.ThrowIfNull(aliquota, nameof(aliquota));

        aliquotaServicos = aliquota;
        this.issRetido = issRetido;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS valores de outros impostos retidos na fonte.
    /// </summary>
    /// <param name="valorPis">Valor da retenção do PIS em R$ (Programa de Integração Social).</param>
    /// <param name="valorCofins">Valor da retenção do COFINS em R$ (Contribuição para Financiamento da Seguridade Social).</param>
    /// <param name="valorInss">Valor da retenção do INSS em R$ (Instituto Nacional do Seguro Social).</param>
    /// <param name="valorIr">Valor da retenção do IR em R$ (Imposto de Renda).</param>
    /// <param name="valorCsll">Valor da retenção do CSLL em R$ (Contribuição Social Sobre o Lucro Líquido).</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentException">Se todos os parâmetros forem nulos. Ao menos um tributo deve ser informado.</exception>
    public IRpsSetOptionals SetTributos(Valor? valorPis = null,
                                        Valor? valorCofins = null,
                                        Valor? valorInss = null,
                                        Valor? valorIr = null,
                                        Valor? valorCsll = null)
    {
        if (valorPis == null &&
            valorCofins == null &&
            valorInss == null &&
            valorIr == null &&
            valorCsll == null)
        {
            throw new ArgumentException(InvalidTaxes);
        }

        this.valorPis = valorPis;
        this.valorCofins = valorCofins;
        this.valorInss = valorInss;
        this.valorIr = valorIr;
        this.valorCsll = valorCsll;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS informações sobre a carga tributária total.
    /// </summary>
    /// <param name="fonteCargaTributaria">Fonte de informação da carga tributária.</param>
    /// <param name="valorCargaTributaria">Valor da carga tributária total em R$.</param>
    /// <param name="percentualCargaTributaria">Percentual da carga tributária.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentException">Se todos os parâmetros forem nulos. Ao menos um atributo da carga tributária deve ser informado.</exception>
    public IRpsSetOptionals SetCargaTributaria(FonteCargaTributaria? fonteCargaTributaria = null,
                                               Valor? valorCargaTributaria = null,
                                               Percentual? percentualCargaTributaria = null)
    {
        if (valorCargaTributaria == null &&
            percentualCargaTributaria == null &&
            fonteCargaTributaria == null)
        {
            throw new ArgumentException(InvalidTaxation);
        }

        this.fonteCargaTributaria = fonteCargaTributaria;
        this.valorCargaTributaria = valorCargaTributaria;
        this.percentualCargaTributaria = percentualCargaTributaria;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS os dados do tomador de serviços.
    /// </summary>
    /// <param name="tomador">Objeto <see cref="Tomador"/> construído via <see cref="TomadorBuilder"/>.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="tomador"/> for nulo.</exception>
    public IRpsSetOptionals SetTomador(Tomador tomador)
    {
        ArgumentNullException.ThrowIfNull(tomador, nameof(tomador));

        this.tomador = tomador;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS os dados do intermediário do serviço.
    /// </summary>
    /// <param name="intermediario">Objeto <see cref="Intermediario"/> construído via <see cref="IntermediarioBuilder"/>.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="intermediario"/> for nulo.</exception>
    public IRpsSetOptionals SetIntermediario(Intermediario intermediario)
    {
        ArgumentNullException.ThrowIfNull(intermediario, nameof(intermediario));

        this.intermediario = intermediario;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS o Código de Cadastro Específico do INSS (CEI).
    /// </summary>
    /// <param name="codigoCei">Código de Cadastro Específico do INSS (CEI).</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoCei"/> for nulo.</exception>
    public IRpsSetOptionals SetCodigoCei(Numero codigoCei)
    {
        ArgumentNullException.ThrowIfNull(codigoCei, nameof(codigoCei));

        this.codigoCei = codigoCei;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS o número da matrícula de obra.
    /// </summary>
    /// <param name="matriculaObra">Número da matrícula de obra.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="matriculaObra"/> for nulo.</exception>
    public IRpsSetOptionals SetObra(Numero matriculaObra)
    {
        ArgumentNullException.ThrowIfNull(matriculaObra, nameof(matriculaObra));

        this.matriculaObra = matriculaObra;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS o código do município de prestação do serviço, conforme tabela IBGE.
    /// </summary>
    /// <param name="municipioPrestacao">Código IBGE do município onde ocorreu a prestação do serviço.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="municipioPrestacao"/> for nulo.</exception>
    public IRpsSetOptionals SetMunicipio(CodigoIbge municipioPrestacao)
    {
        ArgumentNullException.ThrowIfNull(municipioPrestacao, nameof(municipioPrestacao));

        this.municipioPrestacao = municipioPrestacao;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS o código do encapsulamento de notas dedutoras.
    /// </summary>
    /// <param name="numeroEncapsulamento">Código do encapsulamento de notas dedutoras.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="numeroEncapsulamento"/> for nulo.</exception>
    public IRpsSetOptionals SetEncapsulamento(Numero numeroEncapsulamento)
    {
        ArgumentNullException.ThrowIfNull(numeroEncapsulamento, nameof(numeroEncapsulamento));

        this.numeroEncapsulamento = numeroEncapsulamento;

        return this;
    }

    /// <summary>
    /// Adiciona ao RPS o valor total recebido em R$, inclusive valores repassados a terceiros.
    /// </summary>
    /// <param name="valorTotalRecebido">Valor total recebido em R$.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="valorTotalRecebido"/> for nulo.</exception>
    public IRpsSetOptionals SetValorRecebido(Valor valorTotalRecebido)
    {
        ArgumentNullException.ThrowIfNull(valorTotalRecebido, nameof(valorTotalRecebido));

        this.valorTotalRecebido = valorTotalRecebido;

        return this;
    }

    /// <summary>
    /// Substitui a Inscrição Municipal do prestador de serviços informada em <see cref="New"/>.
    /// </summary>
    /// <param name="inscricaoMunicipal">Nova Inscrição Municipal do prestador de serviços.</param>
    /// <returns>Esta mesma instância de <see cref="IRpsSetOptionals"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inscricaoMunicipal"/> for nulo.</exception>
    public IRpsSetOptionals SetInscricaoMunicipalPrestador(InscricaoMunicipal inscricaoMunicipal)
    {
        ArgumentNullException.ThrowIfNull(inscricaoMunicipal, nameof(inscricaoMunicipal));

        inscricaoPrestador = inscricaoMunicipal;

        return this;
    }

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Rps"/> a partir dos atributos fornecidos na cadeia de chamadas.
    /// </summary>
    /// <returns>Um objeto <see cref="Rps"/> consistente e pronto para uso.</returns>
    /// <exception cref="ArgumentException">
    /// Se algum campo obrigatório não foi informado durante a cadeia:
    /// Inscrição Municipal do prestador, Série do RPS, Data de emissão, Status da NF-e,
    /// Tipo de tributação, Valor dos serviços, Código do serviço, Alíquota do serviço,
    /// Indicador de retenção do ISS ou Tomador de serviços.
    /// </exception>
    public Rps Build()
    {
        // Validação das propriedades obrigatórias
        InscricaoMunicipal inscricaoPrestador = this.inscricaoPrestador ??
                throw new ArgumentException(InvalidArgument.Format("Inscrição municipal do prestador de serviços"));

        SerieRps serieRps = this.serieRps ??
                throw new ArgumentException(InvalidArgument.Format("Série do RPS"));

        ChaveRps chaveRps = new(inscricaoPrestador, serieRps, numeroRps);

        DataXsd dataEmissao = this.dataEmissao ??
                throw new ArgumentException(InvalidArgument.Format("Data da emissão do RPS"));

        StatusNfe statusNfe = this.statusNfe ??
                throw new ArgumentException(InvalidArgument.Format("Status da NF-e"));

        TributacaoNfe tributacaoNfe = this.tributacaoNfe ??
                throw new ArgumentException(InvalidArgument.Format("Tipo de tributação da NF-e"));

        Valor valorServicos = this.valorServicos ??
                throw new ArgumentException(InvalidArgument.Format("Valor dos serviços em R$"));

        // Deduções são opcionais: quando não informadas, o XSD exige o valor zero
        Valor valorDeducoes = this.valorDeducoes ?? (Valor)0;

        CodigoServico codigoServico = this.codigoServico ??
                throw new ArgumentException(InvalidArgument.Format("Código do serviço prestado"));

        Aliquota aliquotaServicos = this.aliquotaServicos ??
                throw new ArgumentException(InvalidArgument.Format("Alíquota do serviço prestado"));

        bool issRetido = this.issRetido ??
                throw new ArgumentException(InvalidArgument.Format("Indicador de retenção do ISS"));

        Tomador tomador = this.tomador ??
                throw new ArgumentException(InvalidArgument.Format("Tomador de serviços"));

        TributacaoDataEmissaoValidator.ThrowIfInvalid(tributacaoNfe, dataEmissao);

        Rps rps = new(chaveRps: chaveRps, tipoRps: tipoRps, dataEmissao: dataEmissao, statusNfe: statusNfe,
                      tributacaoNfe: tributacaoNfe, valorServicos: valorServicos, valorDeducoes: valorDeducoes,
                      codigoServico: codigoServico, aliquotaServicos: aliquotaServicos, issRetido: issRetido,
                      discriminacao: discriminacao, valorPis: valorPis, valorCofins: valorCofins, valorInss: valorInss,
                      valorIr: valorIr, valorCsll: valorCsll, cpfOrCnpjTomador: tomador.CpfOrCnpjTomador,
                      inscricaoMunicipalTomador: tomador.InscricaoMunicipalTomador,
                      inscricaoEstadualTomador: tomador.InscricaoEstadualTomador,
                      razaoSocialTomador: tomador.RazaoSocialTomador, enderecoTomador: tomador.EnderecoTomador,
                      emailTomador: tomador.EmailTomador, cpfCnpjIntermediario: intermediario?.CpfCnpjIntermediario,
                      inscricaoMunicipalIntermediario: intermediario?.InscricaoMunicipalIntermediario,
                      issRetidoIntermediario: intermediario?.IssRetidoIntermediario,
                      emailIntermediario: intermediario?.EmailIntermediario, valorCargaTributaria: valorCargaTributaria,
                      percentualCargaTributaria: percentualCargaTributaria, fonteCargaTributaria: fonteCargaTributaria,
                      codigoCei: codigoCei, matriculaObra: matriculaObra, municipioPrestacao: municipioPrestacao,
                      numeroEncapsulamento: numeroEncapsulamento, valorTotalRecebido: valorTotalRecebido);

        return rps;
    }
}