using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Configurações do certificado A1 (<c>.pfx</c>/PKCS#12) utilizado para
/// autenticação mútua TLS (mTLS) e assinatura digital dos documentos XML enviados
/// aos Web Services da NF-e Paulistana.
/// Os valores devem ser fornecidos via User Secrets, nunca em arquivos de configuração
/// versionados.
/// </summary>
internal sealed class CertificateOptions
{
    /// <summary>
    /// Caminho absoluto (ou relativo ao diretório de execução) do arquivo <c>.pfx</c>
    /// contendo o certificado A1. Chave User Secrets: <c>Certificado:CaminhoArquivo</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.Certificado)}.{nameof(CaminhoArquivo)} é obrigatório.")]
    public string CaminhoArquivo { get; init; } = string.Empty;

    /// <summary>
    /// Senha do certificado A1 (<c>.pfx</c>). Valor sensível — nunca deve ser logado,
    /// serializado ou comitado no repositório. Chave User Secrets: <c>Certificado:Senha</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.Certificado)}.{nameof(Senha)} é obrigatório.")]
    [DataType(DataType.Password)]
    [JsonIgnore]
    public string Senha { get; init; } = string.Empty;
}