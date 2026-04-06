using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Agrega todas as configurações da aplicação, carregadas exclusivamente de
/// <see href="https://learn.microsoft.com/aspnet/core/security/app-secrets">User Secrets</see>
/// (ID: <c>Nfe.Paulistana.Integration.Sample</c>).
/// <para>
/// Cada propriedade corresponde a uma seção de chave hierárquica nos User Secrets
/// (ex.: <c>Certificado:CaminhoArquivo</c>). As propriedades decoradas com
/// <see cref="RequiredAttribute"/> são validadas automaticamente pelo pipeline de
/// validação em <see cref="Host.AppSettingsLoader"/>; a validação customizada do
/// certificado digital ocorre em <see cref="Validate"/>.
/// </para>
/// </summary>
internal sealed class AppSettings : IValidatableObject
{
    /// <summary>
    /// Inscrição Municipal do prestador de serviços junto à Prefeitura de São Paulo.
    /// Chave User Secrets: <c>MinhaInscricaoMunicipal</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(MinhaInscricaoMunicipal)} é obrigatória.")]
    public string MinhaInscricaoMunicipal { get; init; } = string.Empty;

    /// <summary>
    /// CNPJ da empresa prestadora de serviços (estabelecimento principal/matriz).
    /// Chave User Secrets: <c>MeuCnpj</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(MeuCnpj)} é obrigatório.")]
    public string MeuCnpj { get; init; } = string.Empty;

    /// <summary>
    /// CNPJ da filial utilizado nas operações de emissão e cancelamento de NF-e.
    /// Chave User Secrets: <c>CnpjDaMinhaFilial</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(CnpjDaMinhaFilial)} é obrigatório.")]
    public string CnpjDaMinhaFilial { get; init; } = string.Empty;

    /// <summary>
    /// Configurações do certificado A1 (<c>.pfx</c>/PKCS#12) utilizado para
    /// autenticação mútua TLS (mTLS) e assinatura digital dos documentos XML.
    /// Chaves User Secrets: <c>Certificado:CaminhoArquivo</c> e <c>Certificado:Senha</c>.
    /// A integridade do arquivo e a validade da senha são verificadas em <see cref="Validate"/>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(Certificado)} é obrigatório.")]
    public CertificateOptions Certificado { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de cancelamento de NF-e via serviço V1.
    /// Chave User Secrets raiz: <c>CancelamentoNfeV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(CancelamentoNfeV1)} é obrigatório.")]
    public CancelNfeV1Options CancelamentoNfeV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de cancelamento de NF-e via serviço V2.
    /// Chave User Secrets raiz: <c>CancelamentoNfeV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(CancelamentoNfeV2)} é obrigatório.")]
    public CancelNfeV2Options CancelamentoNfeV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta por CNPJ via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaCnpjV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaCnpjV1)} é obrigatório.")]
    public QueryCnpjV1Options ConsultaCnpjV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta por CNPJ via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaCnpjV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaCnpjV2)} é obrigatório.")]
    public QueryCnpjV2Options ConsultaCnpjV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de informações de lote via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaInformacoesLoteV1</c>.
    /// Não é obrigatório — ausência de valor impede apenas a execução da ação correspondente.
    /// </summary>
    public QueryBatchInfoV1Options ConsultaInformacoesLoteV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de informações de lote via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaInformacoesLoteV2</c>.
    /// Não é obrigatório — ausência de valor impede apenas a execução da ação correspondente.
    /// </summary>
    public QueryBatchInfoV2Options ConsultaInformacoesLoteV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de lote RPS via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaLoteV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaLoteV1)} é obrigatório.")]
    public QueryBatchV1Options ConsultaLoteV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de lote RPS via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaLoteV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaLoteV2)} é obrigatório.")]
    public QueryBatchV2Options ConsultaLoteV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e específica via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaNfeV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeV1)} é obrigatório.")]
    public QueryNfeV1Options ConsultaNfeV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e específica via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaNfeV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeV2)} é obrigatório.")]
    public QueryNfeV2Options ConsultaNfeV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e recebidas via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaNfeRecebidasV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeRecebidasV1)} é obrigatório.")]
    public QueryReceivedNfeV1Options ConsultaNfeRecebidasV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e recebidas via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaNfeRecebidasV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeRecebidasV2)} é obrigatório.")]
    public QueryReceivedNfeV2Options ConsultaNfeRecebidasV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e emitidas via serviço V1.
    /// Chave User Secrets raiz: <c>ConsultaNfeEmitidasV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeEmitidasV1)} é obrigatório.")]
    public QueryIssuedNfeV1Options ConsultaNfeEmitidasV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de consulta de NF-e emitidas via serviço V2.
    /// Chave User Secrets raiz: <c>ConsultaNfeEmitidasV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(ConsultaNfeEmitidasV2)} é obrigatório.")]
    public QueryIssuedNfeV2Options ConsultaNfeEmitidasV2 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de envio de RPS de teste via serviço V1.
    /// Chave User Secrets raiz: <c>EnvioRpsTesteV1</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(EnvioRpsTesteV1)} é obrigatório.")]
    public SendRpsTestV1Options EnvioRpsTesteV1 { get; init; } = new();

    /// <summary>
    /// Parâmetros para a ação de envio de RPS de teste via serviço V2.
    /// Chave User Secrets raiz: <c>EnvioRpsTesteV2</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(EnvioRpsTesteV2)} é obrigatório.")]
    public SendRpsTestV2Options EnvioRpsTesteV2 { get; init; } = new();

    /// <summary>
    /// Executa validações customizadas que não podem ser expressas apenas por atributos declarativos.
    /// <para><b>Efeitos colaterais e comportamento:</b></para>
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///       <b>Existência do arquivo:</b> se <see cref="CertificateOptions.CaminhoArquivo"/> não
    ///       estiver em branco e o arquivo não existir no sistema de arquivos, retorna um
    ///       <see cref="ValidationResult"/> com a mensagem
    ///       <c>"Certificado.CaminhoArquivo: Arquivo não foi encontrado."</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <b>Abertura do PFX:</b> se o arquivo existir, tenta carregá-lo via
    ///       <see cref="X509CertificateLoader.LoadPkcs12FromFile"/> usando a senha configurada.
    ///       Em caso de falha (senha incorreta, arquivo corrompido ou formato inválido), captura
    ///       a exceção e retorna um <see cref="ValidationResult"/> com a mensagem
    ///       <c>"Certificado: Falha ao carregar o certificado. Verifique o caminho e a senha. Detalhes: {ex.Message}"</c>.
    ///       <b>Efeito colateral:</b> o arquivo PFX é lido do disco e o objeto
    ///       <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> é
    ///       descartado imediatamente (<c>using var cert</c>); nenhuma chave privada permanece em memória.
    ///     </description>
    ///   </item>
    /// </list>
    /// <para>
    /// Invocado automaticamente por
    /// <see cref="System.ComponentModel.DataAnnotations.Validator.TryValidateObject"/> quando
    /// <c>validateAllProperties: true</c> — o que ocorre em <see cref="Host.AppSettingsLoader"/>.
    /// </para>
    /// </summary>
    /// <param name="validationContext">Contexto de validação fornecido pelo framework.</param>
    /// <returns>
    /// Sequência (possivelmente vazia) de <see cref="ValidationResult"/> descrevendo
    /// cada falha de validação customizada encontrada.
    /// </returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Custom validation: certificate file exists
        if (!string.IsNullOrWhiteSpace(Certificado.CaminhoArquivo) && !File.Exists(Certificado.CaminhoArquivo))
        {
            yield return new ValidationResult($"{nameof(AppSettings.Certificado)}.{nameof(CertificateOptions.CaminhoArquivo)}: Arquivo não foi encontrado.", [nameof(Certificado.CaminhoArquivo)]);
        }

        // Tries to open the certificate file to validate the password
        if (!string.IsNullOrWhiteSpace(Certificado.CaminhoArquivo) && File.Exists(Certificado.CaminhoArquivo))
        {
            ValidationResult? validationResult = null;
            try
            {
                using var cert = X509CertificateLoader.LoadPkcs12FromFile(Certificado.CaminhoArquivo, Certificado.Senha);
            }
            catch (Exception ex)
            {
                validationResult = new ValidationResult($"{nameof(AppSettings.Certificado)}: Falha ao carregar o certificado. Verifique o caminho e a senha. Detalhes: {ex.Message}", [nameof(Certificado)]);
            }

            if (validationResult is not null)
            {
                yield return validationResult;
            }
        }
    }
}