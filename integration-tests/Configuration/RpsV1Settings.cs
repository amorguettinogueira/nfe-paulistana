namespace Nfe.Paulistana.IntegrationTests.Configuration;

internal sealed record RpsV1Settings
{
    public long NumeroRps { get; init; } = 100022;
    public string SerieRps { get; init; } = "RPS";
    public string Discriminacao { get; init; } = "Teste de integracao V1 - Desenvolvimento de software.";
    public string CodigoServico { get; init; } = "03158";
    public decimal ValorServicos { get; init; } = 100m;
    public decimal Aliquota { get; init; } = 0.05m;
}