namespace Nfe.Paulistana.Constants;

/// <summary>
/// Centraliza os endpoints do webservice da NF-e Paulistana.
/// </summary>
public static class Endpoints
{
    /// <summary>Endpoint do webservice da NF-e Paulistana em ambiente de produção.</summary>
    /// <remarks>Modificado em 9 de janeiro de 2026, substituindo o endpoint antigo: https://nfe.prefeitura.sp.gov.br/ws/lotenfe.asmx</remarks>
    public const string Producao = "https://nfews.prefeitura.sp.gov.br/lotenfe.asmx";
}