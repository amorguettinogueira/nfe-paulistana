namespace Nfe.Paulistana.IntegrationTests.Configuration;

/// <summary>
/// Configurações lidas dos User Secrets para os testes de integração locais.
/// <para>Configure com: <c>dotnet user-secrets set "Chave" "Valor" --project integration-tests</c></para>
/// <para>Chaves obrigatórias:
/// <list type="bullet">
///   <item><c>Certificado:CaminhoArquivo</c> — caminho absoluto para o arquivo .pfx</item>
///   <item><c>Certificado:Senha</c> — senha do arquivo .pfx</item>
///   <item><c>CnpjPrestador</c> — CNPJ do prestador (sem formatação)</item>
///   <item><c>InscricaoMunicipalPrestador</c> — Inscrição Municipal do prestador</item>
///   <item><c>CnpjTomador</c> — CNPJ do tomador de teste</item>
///   <item><c>RazaoSocialTomador</c> — Razão Social do tomador de teste</item>
/// </list>
/// Chaves opcionais com padrão (podem ser sobrescritas via User Secrets):
/// <list type="bullet">
///   <item><c>V1:NumeroRps</c> (padrão 9001), <c>V1:SerieRps</c> (BB), <c>V1:Discriminacao</c>,
///         <c>V1:CodigoServico</c> (7617), <c>V1:ValorServicos</c> (100), <c>V1:Aliquota</c> (0.05)</item>
///   <item><c>V2:NumeroRps</c> (padrão 9002), <c>V2:SerieRps</c> (BB), <c>V2:Discriminacao</c>,
///         <c>V2:CodigoServico</c> (7617), <c>V2:ValorFinalCobrado</c> (100), <c>V2:Aliquota</c> (0.05),
///         <c>V2:CodigoNbs</c> (123456789), <c>V2:CodigoMunicipioIbge</c> (3550308),
///         <c>V2:CodigoOperacao</c> (010101), <c>V2:ClassificacaoTributaria</c> (010101)</item>
/// </list>
/// </para>
/// </summary>
internal sealed record IntegrationTestSettings
{
    public string CnpjPrestador { get; init; } = string.Empty;
    public string InscricaoMunicipalPrestador { get; init; } = string.Empty;
    public string CnpjTomador { get; init; } = string.Empty;
    public string RazaoSocialTomador { get; init; } = string.Empty;
    public CertificadoSettings Certificado { get; init; } = new();
    public RpsV1Settings V1 { get; init; } = new();
    public RpsV2Settings V2 { get; init; } = new();

    /// <summary>
    /// Indica se os segredos mínimos estão presentes e o certificado é acessível.
    /// Quando falso, os testes são pulados automaticamente.
    /// </summary>
    public bool EstaoConfigurados =>
        !string.IsNullOrWhiteSpace(Certificado.CaminhoArquivo) &&
        !string.IsNullOrWhiteSpace(CnpjPrestador) &&
        !string.IsNullOrWhiteSpace(InscricaoMunicipalPrestador) &&
        File.Exists(Certificado.CaminhoArquivo);
}