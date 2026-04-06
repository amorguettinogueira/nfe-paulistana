using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para o envio de um RPS em <b>modo de teste</b> via serviço V01
/// (<see cref="Actions.SendRpsTestV1Action"/>).
/// Chave User Secrets raiz: <c>EnvioRpsTesteV1</c>.
/// <para><b>Nota:</b> os campos são passados diretamente ao construtor de RPS e enviados
/// com <c>modoTeste: true</c>; nenhuma NF-e é emitida definitivamente.</para>
/// </summary>
internal sealed class SendRpsTestV1Options
{
    /// <summary>
    /// CNPJ do tomador de serviços. Obrigatório.
    /// Formato: 14 dígitos numéricos sem separadores (ex.: <c>00000000000000</c>).
    /// Chave User Secrets: <c>EnvioRpsTesteV1:CnpjTomador</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(CnpjTomador)} é obrigatório.")]
    public string CnpjTomador { get; init; } = string.Empty;

    /// <summary>
    /// Razão social do tomador de serviços. Obrigatório. Texto livre.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:RazaoSocialTomador</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(RazaoSocialTomador)} é obrigatório.")]
    public string RazaoSocialTomador { get; init; } = string.Empty;

    /// <summary>
    /// Tipo do RPS. Obrigatório.
    /// Deve ser um nome válido do enum <see cref="Nfe.Paulistana.Models.Enums.TipoRps"/>
    /// (ex.: <c>RPS</c>). Validado em runtime por <see cref="Actions.SendRpsTestV1Action"/>.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:TipoRps</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(TipoRps)} é obrigatório.")]
    public string TipoRps { get; init; } = string.Empty;

    /// <summary>
    /// Número sequencial do RPS. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>EnvioRpsTesteV1:NumeroRps</c>.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(NumeroRps)} deve ser maior que zero.")]
    public long NumeroRps { get; init; }

    /// <summary>
    /// Descrição do serviço prestado. Obrigatório. Texto livre.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:Discriminacao</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(Discriminacao)} é obrigatório.")]
    public string Discriminacao { get; init; } = string.Empty;

    /// <summary>
    /// Série do RPS. Obrigatório.
    /// Identificador alfanumérico da série (ex.: <c>T</c> para série de teste).
    /// Chave User Secrets: <c>EnvioRpsTesteV1:SerieRps</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(SerieRps)} é obrigatório.")]
    public string SerieRps { get; init; } = string.Empty;

    /// <summary>
    /// Código de tributação da NF-e. Obrigatório.
    /// Apenas o primeiro caractere é utilizado (cast para <c>TributacaoNfe</c>).
    /// Validado em runtime por <see cref="Actions.SendRpsTestV1Action"/>.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:TributacaoNfe</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(TributacaoNfe)} é obrigatório.")]
    public string TributacaoNfe { get; init; } = string.Empty;

    /// <summary>
    /// Código do serviço prestado conforme tabela ISS da Prefeitura de São Paulo. Obrigatório.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:CodigoServico</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(CodigoServico)} é obrigatório.")]
    public string CodigoServico { get; init; } = string.Empty;

    /// <summary>
    /// Valor total dos serviços prestados em reais. Obrigatório.
    /// Deve ser maior que <c>0,01</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:ValorServicos</c>.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(ValorServicos)} deve ser maior que zero.")]
    public decimal ValorServicos { get; init; }

    /// <summary>
    /// Alíquota do ISS aplicada ao serviço (valor fracionário; ex.: <c>0.05</c> para 5%). Obrigatório.
    /// Deve ser maior que <c>0,0001</c>.
    /// Chave User Secrets: <c>EnvioRpsTesteV1:Aliquota</c>.
    /// </summary>
    [Range(0.0001, double.MaxValue, ErrorMessage = $"{nameof(AppSettings.EnvioRpsTesteV1)}.{nameof(Aliquota)} deve ser zero ou positivo.")]
    public decimal Aliquota { get; init; }

    /// <summary>
    /// Indica se o ISS é retido na fonte pelo tomador de serviços.
    /// Padrão: <c>false</c>. Chave User Secrets: <c>EnvioRpsTesteV1:IssRetido</c>.
    /// </summary>
    public bool IssRetido { get; init; }
}