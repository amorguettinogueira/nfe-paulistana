using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Host;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;

// Ensure console uses UTF-8 so accented characters render correctly
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

// Load and validate configuration (secret configuration should be stored in User Secrets, see README.md)
AppSettings? settings = AppSettingsLoader.LoadAndValidate();
if (settings is null)
{
    return;
}

// Build DI container
var services = new ServiceCollection();
ServiceConfigurator.RegisterServices(services, settings);
await using ServiceProvider provider = services.BuildServiceProvider();

// Run interactive menu
var runner = new ConsoleMenuHost(provider);
await runner.RunAsync();

/// <summary>
/// Ponto de entrada da aplicação de integração com os Web Services SOAP da NF-e Paulistana
/// (Nota Fiscal Eletrônica de Serviços — Prefeitura de São Paulo).
///
/// <para><b>Objetivo:</b> Console interativo para testar e demonstrar as operações disponíveis
/// nos serviços da NF-e Paulistana, como cancelamento de NF-e, consulta por CNPJ,
/// consulta de lotes, envio de RPS de teste, download de WSDL, entre outros.
/// Suporta as versões V1 e V2 de cada serviço.</para>
///
/// <para><b>Como iniciar:</b></para>
/// <list type="bullet">
///   <item><description>
///     Terminal: <c>dotnet run --project Nfe.Paulistana.Integration.Sample</c>
///   </description></item>
///   <item><description>
///     Visual Studio: definir como projeto de inicialização e pressionar <c>F5</c> (com depuração)
///     ou <c>Ctrl+F5</c> (sem depuração).
///   </description></item>
/// </list>
///
/// <para><b>Perfil de execução:</b> O menu interativo é exibido no terminal. O usuário escolhe
/// a operação desejada e o resultado é impresso diretamente no console.
/// Não há servidor HTTP nem processo em background — o ciclo de vida é totalmente síncrono
/// ao menu.</para>
///
/// <para><b>Dependência — User Secrets:</b> Todas as configurações (inclusive as sensíveis) são
/// carregadas <em>exclusivamente</em> via
/// <see cref="Microsoft.Extensions.Configuration.UserSecretsConfigurationExtensions"/>.
/// O <c>UserSecretsId</c> definido no <c>.csproj</c> é <c>Nfe.Paulistana.Integration.Sample</c>.
/// Configure antes de executar:</para>
/// <code>
/// dotnet user-secrets set "MinhaInscricaoMunicipal"    "0000000"
/// dotnet user-secrets set "MeuCnpj"                    "00000000000000"
/// dotnet user-secrets set "CnpjDaMinhaFilial"          "00000000000000"
/// dotnet user-secrets set "Certificado:CaminhoArquivo" "C:\caminho\certificado.pfx"
/// dotnet user-secrets set "Certificado:Senha"          "sua-senha"
/// </code>
/// <para>Os demais campos obrigatórios (parâmetros de cada operação) devem ser adicionados
/// seguindo o mesmo padrão de chave hierárquica, conforme a classe
/// <see cref="Configuration.AppSettings"/>.</para>
///
/// <para><b>Dependência — Certificado Digital:</b> É obrigatório um certificado A1 no formato
/// <c>.pfx</c> (PKCS#12). O certificado é utilizado para:</para>
/// <list type="number">
///   <item><description>Autenticação mútua TLS (mTLS) nas chamadas SOAP ao endpoint da Prefeitura.</description></item>
///   <item><description>Assinatura digital dos documentos XML (RPS, cancelamentos etc.).</description></item>
/// </list>
/// <para>O caminho e a senha do arquivo são lidos dos User Secrets via
/// <see cref="Configuration.CertificateOptions"/> (<c>Certificado:CaminhoArquivo</c> e
/// <c>Certificado:Senha</c>). A senha <b>nunca</b> deve ser comitada no repositório.</para>
/// </summary>
internal sealed partial class Program;