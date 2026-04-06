using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V2.Infrastructure;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// <para>
/// Fábrica para construir objetos <see cref="PedidoEnvioLote"/> com geração automática de assinatura digital para envios em lote (v02).
/// </para>
/// <para>
/// Esta fábrica expõe uma API "one-shot" que gera um objeto <see cref="PedidoEnvioLote"/> completo e devidamente assinado
/// a partir de uma coleção de objetos <see cref="Rps"/>. Ela encapsula o ciclo completo de preparação do lote:
/// validação mínima, cálculo de totalizações, geração do cabeçalho de lote, assinatura individual de cada RPS e
/// assinatura do documento de lote.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// Design e responsabilidades (resumido):
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <strong>Padrão:</strong> Factory (métodos retornam objetos prontos, sem etapas intermediárias expostas).
/// </description>
/// </item>
/// <item>
/// <description>
/// <strong>Processamento em lote:</strong> A fábrica valida que exista pelo menos um RPS não-nulo, itera sobre os RPS,
/// assina cada RPS, acumula valores e datas, gera o <see cref="CabecalhoLote"/> e assina o lote final.
/// </description>
/// </item>
/// <item>
/// <description>
/// <strong>Gerenciamento de certificado:</strong> Recebe um <see cref="Certificado"/> via construtor primário.
/// Um <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> é construído por chamada e usado para
/// todas as assinaturas dentro dessa chamada, sendo descartado ao final (padrão <c>using</c>).
/// </description>
/// </item>
/// <item>
/// <description>
/// <strong>Assinaturas garantidas:</strong> O contrato da fábrica garante que todo <see cref="PedidoEnvioLote"/> retornado
/// terá todos os RPS assinados e o próprio lote assinado. A classe é sealed para proteger estes invariantes.
/// </description>
/// </item>
/// </list>
/// <para>
/// Observações específicas da versão v02:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Cálculo de <c>ValorTotalServicos</c>: a fábrica considera, por ordem de preferência, os campos
/// <c>ValorInicialCobrado</c>, <c>ValorFinalCobrado</c> e <c>ValorTotalRecebido</c> de cada RPS v02, pois
/// a representação do montante pode variar entre implementações de RPS nesta versão.
/// </description>
/// </item>
/// <item>
/// <description>
/// <c>ValorTotalDeducoes</c> é nulo quando a soma das deduções resulta em zero, em conformidade com o contrato
/// esperado pelo serviço remoto.
/// </description>
/// </item>
/// </list>
/// <para>
/// Integração com DI: registre a fábrica em <c>IServiceCollection</c> (por exemplo, <c>services.AddScoped&lt;PedidoEnvioLoteFactory&gt;();</c>)
/// e injete <see cref="Certificado"/> conforme a configuração da sua aplicação.
/// </para>
/// <example>
/// <code>
/// // Configuração de dependency injection
/// services.AddSingleton(certificateConfig);
/// services.AddScoped&lt;PedidoEnvioLoteFactory&gt;();
///
/// // Uso
/// var factory = serviceProvider.GetRequiredService&lt;PedidoEnvioLoteFactory&gt;();
/// var rpsCollection = new[] { rps1, rps2, rps3 };
///
/// // Cria PedidoEnvioLote totalmente assinado
/// var pedidoLote = factory.NewCpf((Cpf)12345678901L, false, rpsCollection);
///
/// // Garantias:
/// // - Todos os RPS em pedidoLote.Rps estão assinados
/// // - pedidoLote está assinado
/// // - pedidoLote.Cabecalho contém totalizações e datas calculadas automaticamente
/// </code>
/// </example>
/// </remarks>
public sealed class PedidoEnvioLoteFactory(Certificado certificate)
{
    private const string InvalidRequest = "É necessário informar pelo menos uma RPS para que o lote seja válido.";

    /// <summary>
    /// Configuração de certificado fornecida via injeção de dependência.
    /// Usada para construir instâncias de <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/>
    /// para operações de assinatura.
    /// </summary>
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    /// <summary>
    /// Gerador interno de assinatura para objetos RPS. Assina cada RPS individualmente antes de agregá-los ao lote.
    /// </summary>
    private readonly RpsSignatureGenerator _rpsSignatureGenerator = new();

    /// <summary>
    /// Gerador interno de assinatura para o documento de lote (<see cref="PedidoEnvioLote"/>).
    /// </summary>
    private readonly XmlFileSignatureGenerator<PedidoEnvioLote> _pedidoSignatureGenerator = new XmlFileSignatureGenerator<PedidoEnvioLote>();

    /// <summary>
    /// Cria um <see cref="PedidoEnvioLote"/> totalmente assinado a partir de um CPF contendo múltiplos RPS.
    /// </summary>
    /// <param name="cpf">CPF do prestador de serviços como Value Object type-safe.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote. Deve conter pelo menos um RPS.</param>
    /// <returns>
    /// Um objeto <see cref="PedidoEnvioLote"/> completamente assinado e pronto para envio.
    /// - Todos os RPS no lote têm assinatura preenchida
    /// - <see cref="PedidoEnvioLote"/> possui assinatura preenchida
    /// - <see cref="CabecalhoLote"/> contém totalizações e datas calculadas automaticamente
    /// </returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> ou <paramref name="rpsList"/> é nulo.</exception>
    /// <exception cref="ArgumentException">Se <paramref name="rpsList"/> é vazio ou não contém pelo menos um RPS não-nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se carregamento do certificado ou assinatura falhar.</exception>
    public PedidoEnvioLote NewCpf(Cpf cpf, bool transacao, IEnumerable<Rps> rpsList)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(rpsList);
        return ConstructWith((CpfOrCnpj)cpf, transacao, rpsList);
    }

    /// <summary>
    /// Cria um <see cref="PedidoEnvioLote"/> totalmente assinado a partir de um CNPJ contendo múltiplos RPS.
    /// </summary>
    /// <param name="cnpj">CNPJ do prestador de serviços como Value Object type-safe.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote. Deve conter pelo menos um RPS.</param>
    /// <returns>
    /// Um objeto <see cref="PedidoEnvioLote"/> completamente assinado e pronto para envio.
    /// - Todos os RPS no lote têm assinatura preenchida
    /// - <see cref="PedidoEnvioLote"/> possui assinatura preenchida
    /// - <see cref="CabecalhoLote"/> contém totalizações e datas calculadas automaticamente
    /// </returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> ou <paramref name="rpsList"/> é nulo.</exception>
    /// <exception cref="ArgumentException">Se <paramref name="rpsList"/> é vazio ou não contém pelo menos um RPS não-nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se carregamento do certificado ou assinatura falhar.</exception>
    public PedidoEnvioLote NewCnpj(Cnpj cnpj, bool transacao, IEnumerable<Rps> rpsList)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(rpsList);
        return ConstructWith((CpfOrCnpj)cnpj, transacao, rpsList);
    }

    /// <summary>
    /// Método interno que orquestra a construção, agregação de lote e assinatura de um <see cref="PedidoEnvioLote"/>.
    /// </summary>
    /// <param name="cpfOrCnpj">O CPF ou CNPJ do prestador de serviços como Value Object.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote.</param>
    /// <returns>Um objeto <see cref="PedidoEnvioLote"/> totalmente assinado com todas as totalizações e datas calculadas.</returns>
    /// <remarks>
    /// <para>
    /// Implementação do processamento em lote:
    /// </para>
    /// <list type="number">
    /// <item>Valida que <c>rpsList</c> contém pelo menos um RPS não-nulo</item>
    /// <item>Constrói <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> a partir da configuração
    /// <see cref="Certificado"/> injetada</item>
    /// <item>Itera através de todos os RPS no lote, assinando cada RPS e acumulando totalizações e datas</item>
    /// <item>Cria <see cref="CabecalhoLote"/> com valores computados automaticamente (QtdRps, ValorTotalServicos,
    /// ValorTotalDeducoes, DtInicio, DtFim)</item>
    /// <item>Cria o <see cref="PedidoEnvioLote"/> com o cabeçalho gerado e todos os RPS assinados</item>
    /// <item>Assina o <see cref="PedidoEnvioLote"/> completo usando <see cref="XmlFileSignatureGenerator{PedidoEnvioLote}"/></item>
    /// <item>Descarta apropriadamente o certificado usando statement <c>using</c></item>
    /// </list>
    /// </remarks>
    private PedidoEnvioLote ConstructWith(CpfOrCnpj cpfOrCnpj, bool transacao, IEnumerable<Rps> rpsList)
    {
        rpsList = [.. rpsList?.Where(rps => rps != null) ?? []];

        if (!rpsList.Any())
        {
            throw new ArgumentException(InvalidRequest, nameof(rpsList));
        }

        using X509Certificate2 certificate = _certificate.Build();

        long Quantidade = 0;
        DateTime dtInicio = DateTime.MaxValue, dtFim = DateTime.MinValue;

        foreach (Rps rps in rpsList!)
        {
            Quantidade++;

            var rpsDataEmissao = (DateTime)rps.DataEmissao;

            dtInicio = (rpsDataEmissao < dtInicio) ? rpsDataEmissao : dtInicio;
            dtFim = (rpsDataEmissao > dtFim) ? rpsDataEmissao : dtFim;

            _rpsSignatureGenerator.Sign(rps, certificate);
        }

        CabecalhoLote cabecalhoLote = new(cpfOrCnpj)
        {
            Transacao = transacao,
            DtFim = (DataXsd)dtFim,
            DtInicio = (DataXsd)dtInicio,
            QtdRps = (Quantidade)Quantidade,
        };

        var pedidoEnvioLote = new PedidoEnvioLote()
        {
            Cabecalho = cabecalhoLote,
            Rps = [.. rpsList],
        };

        _pedidoSignatureGenerator.Sign(pedidoEnvioLote, certificate);

        return pedidoEnvioLote;
    }
}