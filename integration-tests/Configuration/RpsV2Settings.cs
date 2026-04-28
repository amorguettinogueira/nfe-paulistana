namespace Nfe.Paulistana.IntegrationTests.Configuration;

internal sealed record RpsV2Settings
{
    public long NumeroRps { get; init; } = 100022;
    public string SerieRps { get; init; } = "RPS";
    public string Discriminacao { get; init; } = "Teste de integracao V2 - Desenvolvimento de software.";
    public string CodigoServico { get; init; } = "03158";
    public decimal ValorFinalCobrado { get; init; } = 100m;
    public decimal Aliquota { get; init; } = 0.05m;
    public string CodigoNbs { get; init; } = "115900000";
    public string CodigoMunicipioIbge { get; init; } = "3550308";
    public string CodigoOperacao { get; init; } = "050101";
    public string ClassificacaoTributaria { get; init; } = "000001";
}