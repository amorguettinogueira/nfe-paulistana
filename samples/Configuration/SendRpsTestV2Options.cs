using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para o envio de um RPS em <b>modo de teste</b> via serviço V02
/// (<see cref="Actions.SendRpsTestV2Action"/>).
/// Chave User Secrets raiz: <c>EnvioRpsTesteV2</c>.
/// <para>Além dos campos básicos compartilhados com V01, inclui campos adicionais
/// introduzidos na versão V02: informações de IBS/CBS (<c>CodigoOperacao</c>,
/// <c>ClassificacaoTributaria</c>), <c>ExigibilidadeSuspensa</c>,
/// <c>PagamentoParceladoAntecipado</c>, <c>CodigoNBS</c> e <c>CodigoMunicipioIbge</c>.</para>
/// <para><b>Nota:</b> os campos são passados diretamente ao construtor de RPS e enviados
/// com <c>modoTeste: true</c>; nenhuma NF-e é emitida definitivamente.</para>
/// </summary>
internal sealed class SendRpsTestV2Options
{
    /// <summary>
    /// CNPJ do tomador de serviços. Obrigatório.
    /// Formato: 14 dígitos numéricos sem separadores (ex.: <c>00000000000000</c>).
    /// Chave User Secrets: <c>EnvioRpsTesteV2:CnpjTomador</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CnpjTomador)} é obrigatório.")]
    public string CnpjTomador { get; init; } = string.Empty;

    /// <summary>
    /// Razão social do tomador de serviços. Obrigatório. Texto livre.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:RazaoSocialTomador</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(RazaoSocialTomador)} é obrigatório.")]
    public string RazaoSocialTomador { get; init; } = string.Empty;

    /// <summary>
    /// Tipo do RPS. Obrigatório.
    /// Deve ser um nome válido do enum <see cref="Nfe.Paulistana.Models.Enums.TipoRps"/>
    /// (ex.: <c>RPS</c>). Validado em runtime por <see cref="Actions.SendRpsTestV2Action"/>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:TipoRps</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(TipoRps)} é obrigatório.")]
    public string TipoRps { get; init; } = string.Empty;

    /// <summary>
    /// Número sequencial do RPS. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>EnvioRpsTesteV2:NumeroRps</c>.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(NumeroRps)} deve ser maior que zero.")]
    public long NumeroRps { get; init; }

    /// <summary>
    /// Descrição do serviço prestado. Obrigatório. Texto livre.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:Discriminacao</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(Discriminacao)} é obrigatório.")]
    public string Discriminacao { get; init; } = string.Empty;

    /// <summary>
    /// Série do RPS. Obrigatório.
    /// Identificador alfanumérico da série (ex.: <c>T</c> para série de teste).
    /// Chave User Secrets: <c>EnvioRpsTesteV2:SerieRps</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(SerieRps)} é obrigatório.")]
    public string SerieRps { get; init; } = string.Empty;

    /// <summary>
    /// Código de tributação da NF-e. Obrigatório.
    /// Apenas o primeiro caractere é utilizado (cast para <c>TributacaoNfe</c>).
    /// Validado em runtime por <see cref="Actions.SendRpsTestV2Action"/>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:TributacaoNfe</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(TributacaoNfe)} é obrigatório.")]
    public string TributacaoNfe { get; init; } = string.Empty;

    /// <summary>
    /// Código do serviço prestado conforme tabela ISS da Prefeitura de São Paulo. Obrigatório.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:CodigoServico</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoServico)} é obrigatório.")]
    public string CodigoServico { get; init; } = string.Empty;

    /// <summary>
    /// Valor total dos serviços prestados em reais. Obrigatório.
    /// Deve ser maior que <c>0,01</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:ValorServicos</c>.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(ValorServicos)} deve ser maior que zero.")]
    public decimal ValorServicos { get; init; }

    /// <summary>
    /// Alíquota do ISS aplicada ao serviço (valor fracionário; ex.: <c>0.05</c> para 5%). Obrigatório.
    /// Deve ser maior que <c>0,0001</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:Aliquota</c>.
    /// </summary>
    [Range(0.0001, double.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(Aliquota)} deve ser zero ou positivo.")]
    public decimal Aliquota { get; init; }

    /// <summary>
    /// Indica se o ISS é retido na fonte pelo tomador de serviços.
    /// Padrão: <c>false</c>. Chave User Secrets: <c>EnvioRpsTesteV2:IssRetido</c>.
    /// </summary>
    public bool IssRetido { get; init; }

    /// <summary>
    /// Indica se a exigibilidade do ISS está suspensa por decisão judicial ou administrativa.
    /// Padrão: <c>false</c>. Chave User Secrets: <c>EnvioRpsTesteV2:ExigibilidadeSuspensa</c>.
    /// </summary>
    public bool ExigibilidadeSuspensa { get; init; }

    /// <summary>
    /// Indica se houve pagamento parcelado antecipado do serviço.
    /// Padrão: <c>false</c>. Chave User Secrets: <c>EnvioRpsTesteV2:PagamentoParceladoAntecipado</c>.
    /// </summary>
    public bool PagamentoParceladoAntecipado { get; init; }

    /// <summary>
    /// Código NBS (Nomenclatura Brasileira de Serviços). Obrigatório.
    /// Formato: exatamente 9 dígitos numéricos — regex <c>[0-9]{9}</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:CodigoNBS</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoNBS)} é obrigatório.")]
    [RegularExpression("[0-9]{9}", ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoNBS)} deve ter 9 dígitos.")]
    public string CodigoNBS { get; init; } = string.Empty;

    /// <summary>
    /// Código IBGE do município do tomador de serviços. Obrigatório.
    /// Range válido: 1.000.000 a 9.999.999 (código IBGE de 7 dígitos).
    /// Chave User Secrets: <c>EnvioRpsTesteV2:CodigoMunicipioIbge</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoMunicipioIbge)} é obrigatório.")]
    [Range(1_000_000, 9_999_999, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoMunicipioIbge)} deve estar entre 1.000.000 e 9.999.999.")]
    public int CodigoMunicipioIbge { get; init; } = 0;

    /// <summary>
    /// Código da operação de fornecimento para IBS/CBS. Obrigatório.
    /// Formato: exatamente 6 dígitos numéricos — regex <c>[0-9]{6}</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:CodigoOperacao</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoOperacao)} é obrigatório.")]
    [RegularExpression("[0-9]{6}", ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(CodigoOperacao)} deve ter 6 dígitos.")]
    public string CodigoOperacao { get; init; } = string.Empty;

    /// <summary>
    /// Classificação tributária do serviço para fins de IBS/CBS. Obrigatório.
    /// Formato: exatamente 6 dígitos numéricos — regex <c>[0-9]{6}</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV2:ClassificacaoTributaria</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(ClassificacaoTributaria)} é obrigatório.")]
    [RegularExpression("[0-9]{6}", ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV2)}.{nameof(ClassificacaoTributaria)} deve ter 6 dígitos.")]
    public string ClassificacaoTributaria { get; init; } = string.Empty;
}