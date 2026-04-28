namespace Nfe.Paulistana.IntegrationTests.Configuration;

internal sealed record CertificadoSettings
{
    public string CaminhoArquivo { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}