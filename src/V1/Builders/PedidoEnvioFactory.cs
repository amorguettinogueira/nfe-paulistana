using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V1.Infrastructure;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// <para>
/// Fábrica para construir objetos <see cref="PedidoEnvio"/> com geração automática de assinatura digital.
/// </para>
/// <para>
/// Esta fábrica fornece uma API simples e one-shot para criar objetos <see cref="PedidoEnvio"/> totalmente assinados.
/// Ela gerencia automaticamente certificados, assinatura de <see cref="Rps"/> e assinatura de <see cref="PedidoEnvio"/>,
/// garantindo que objetos retornados estejam sempre corretamente assinados e prontos para envio.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Padrão:</strong> Factory Pattern (não Builder). Métodos retornam objetos PedidoEnvio completos,
/// não builders intermediários.
/// </item>
/// <item>
/// <strong>Gerenciamento de Certificado:</strong> Recebe <see cref="Certificado"/> via dependency injection
/// (construtor primário). Certificados são construídos sob demanda por chamada de método usando padrão "using"
/// para limpeza apropriada de recursos.
/// </item>
/// <item>
/// <strong>Geração de Assinatura:</strong> Usa geradores internos de assinatura (RpsSignatureGenerator e
/// XmlFileSignatureGenerator) que NÃO são expostos ao usuário. Estes são detalhes de implementação ocultos
/// atrás da API pública da fábrica.
/// </item>
/// <item>
/// <strong>Assinaturas Garantidas:</strong> Todo PedidoEnvio retornado por esta fábrica tem
/// tanto o objeto RPS quanto o PedidoEnvio assinados. A fábrica reforça este invariante automaticamente.
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed propositalmente para garantir que os invariantes
/// de assinatura (RPS sempre assinado, PedidoEnvio sempre assinado) não possam ser violados por herança.
/// Usuários devem usar composição se precisarem estender funcionalidades, não herança.
/// </item>
/// <item>
/// <strong>Amigável a DI:</strong> Pode ser registrada em IServiceCollection como um serviço escopo ou singleton.
/// Os usuários dependem desta fábrica, não dos geradores internos de assinatura.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Configuração de dependency injection
/// services.AddSingleton(certificateConfig);
/// services.AddScoped&lt;PedidoEnvioFactory&gt;();
///
/// // Uso
/// var factory = serviceProvider.GetRequiredService&lt;PedidoEnvioFactory&gt;();
/// var rps = RpsBuilder.New(
///         (InscricaoMunicipal)39616924,
///         TipoRps.NotaFiscalConjugada,
///         (Numero)4105,
///         (Discriminacao)"Serviço de consultoria prestado",
///         (SerieRps)"BB")
///     .SetNFe((DataXsd)new DateTime(2024, 1, 20), (TributacaoNfe)'T', StatusNfe.Normal)
///     .SetServico((CodigoServico)"01010", (Valor)1000.0m)
///     .SetIss((Aliquota)5.0m, false)
///     .Build();
///
/// // Cria PedidoEnvio totalmente assinado em uma chamada
/// var pedidoEnvio = factory.NewCpf((Cpf)12345678901L, rps);
/// // ou
/// var pedidoEnvio = factory.NewCnpj((Cnpj)12345678901234L, rps);
///
/// // Resultado: pedidoEnvio.Assinatura está preenchido, pedidoEnvio.Rps.Assinatura está preenchido
/// // Objeto totalmente assinado e pronto para envio à prefeitura
/// </code>
/// </example>
public sealed class PedidoEnvioFactory(Certificado certificate)
{
    /// <summary>
    /// Configuração de certificado fornecida via injeção de dependência.
    /// Usada para construir instâncias de X509Certificate2 para operações de assinatura.
    /// </summary>
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    /// <summary>
    /// Gerador interno de assinatura para objetos RPS.
    /// Detalhe de implementação não exposto ao usuário da biblioteca.
    /// Instanciado localmente (não via DI) pois é uma responsabilidade interna da biblioteca.
    /// </summary>
    private readonly RpsSignatureGenerator _rpsSignatureGenerator = new();

    /// <summary>
    /// Gerador interno de assinatura para objetos PedidoEnvio.
    /// Detalhe de implementação não exposto ao usuário da biblioteca.
    /// Instanciado localmente (não via DI) pois é uma responsabilidade interna da biblioteca.
    /// </summary>
    private readonly XmlFileSignatureGenerator<PedidoEnvio> _pedidoSignatureGenerator = new XmlFileSignatureGenerator<PedidoEnvio>();

    /// <summary>
    /// Cria um <see cref="PedidoEnvio"/> totalmente assinado a partir de um CPF.
    /// </summary>
    /// <param name="cpf">CPF do prestador de serviços.</param>
    /// <param name="rps">O <see cref="Rps"/> a incluir no pedido.</param>
    /// <returns>Objeto <see cref="PedidoEnvio"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> ou <paramref name="rps"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoEnvio NewCpf(Cpf cpf, Rps rps)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(rps);
        return ConstructWith((Cabecalho)cpf, rps);
    }

    /// <summary>
    /// Cria um <see cref="PedidoEnvio"/> totalmente assinado a partir de um CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do prestador de serviços.</param>
    /// <param name="rps">O <see cref="Rps"/> a incluir no pedido.</param>
    /// <returns>Objeto <see cref="PedidoEnvio"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> ou <paramref name="rps"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoEnvio NewCnpj(Cnpj cnpj, Rps rps)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(rps);
        return ConstructWith((Cabecalho)cnpj, rps);
    }

    /// <summary>
    /// Método interno que orquestra
    /// </summary>
    /// <param name="cabecalho">O cabeçalho (Cabecalho) para o PedidoEnvio.</param>
    /// <param name="rps">O RPS a incluir no PedidoEnvio.</param>
    /// <returns>Um objeto PedidoEnvio totalmente assinado.</returns>
    /// <remarks>
    /// <para>
    /// <strong>Processo de Assinatura (Implementação Interna):</strong>
    /// </para>
    /// <list type="number">
    /// <item>Valida que RPS não é nulo</item>
    /// <item>Constrói X509Certificate2 a partir da configuração <see cref="Certificado"/> injetada</item>
    /// <item>Assina o RPS usando RpsSignatureGenerator (popula RPS.Assinatura)</item>
    /// <item>Cria o PedidoEnvio com o RPS assinado</item>
    /// <item>Assina o PedidoEnvio completo usando XmlFileSignatureGenerator (popula PedidoEnvio.Assinatura)</item>
    /// <item>Descarta apropriadamente o certificado usando statement "using"</item>
    /// <item>Retorna o PedidoEnvio totalmente assinado</item>
    /// </list>
    /// </remarks>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se assinatura falhar.</exception>
    private PedidoEnvio ConstructWith(Cabecalho cabecalho, Rps rps)
    {
        using X509Certificate2 certificate = _certificate.Build();

        // RPS deve ser assinado antes de ser adicionado ao PedidoEnvio
        _rpsSignatureGenerator.Sign(rps, certificate);

        var pedidoEnvio = new PedidoEnvio()
        {
            Cabecalho = cabecalho,
            Rps = rps,
        };

        _pedidoSignatureGenerator.Sign(pedidoEnvio, certificate);

        return pedidoEnvio;
    }
}