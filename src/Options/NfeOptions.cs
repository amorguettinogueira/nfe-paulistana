using Nfe.Paulistana.Constants;

namespace Nfe.Paulistana.Options;

/// <summary>
/// Opções de configuração para os serviços da NF-e Paulistana.
/// </summary>
public sealed class NfeOptions
{
    /// <summary>
    /// URI do endpoint SOAP do webservice da NF-e Paulistana.
    /// O valor padrão aponta para o ambiente de produção (<see cref="Endpoints.Producao"/>).
    /// </summary>
    public Uri EndpointUrl { get; set; } = new(Endpoints.Producao);

    /// <summary>
    /// Configuração do certificado digital utilizado para autenticação mútua TLS (mTLS)
    /// e assinatura dos documentos XML. O certificado é obrigatório para o funcionamento do serviço.
    /// </summary>
    public Certificado Certificado { get; set; } = new();
}