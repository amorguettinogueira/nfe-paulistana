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
/// Fábrica para construir objetos <see cref="PedidoEnvioLote"/> com geração automática de assinatura digital para envios em lote.
/// </para>
/// <para>
/// Esta fábrica fornece uma API simples one-shot para criar objetos <see cref="PedidoEnvioLote"/> totalmente assinados contendo
/// múltiplas submissões de <see cref="Rps"/>. Ela gerencia certificados, geração automática de cabeçalho de lote (com totalizações
/// e contagem de RPS), assinatura individual de <see cref="Rps"/> e assinatura de nível de lote automaticamente, garantindo que objetos
/// retornados estejam sempre corretamente assinados e prontos para envio à prefeitura de São Paulo.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Padrão:</strong> Factory Pattern (não Builder). Métodos retornam objetos PedidoEnvioLote completos,
/// não builders intermediários. Esta é uma variante em lote de PedidoEnvioFactory.
/// </item>
/// <item>
/// <strong>Processamento em Lote:</strong> Automaticamente:
/// - Valida que pelo menos um RPS é fornecido
/// - Itera através de todos os RPS e assina cada um individualmente
/// - Calcula totalizações de lote (quantidade, valores de serviço, deduções)
/// - Gera CabecalhoLote com valores computados
/// - Assina o lote completo
/// </item>
/// <item>
/// <strong>Gerenciamento de Certificado:</strong> Recebe <see cref="Certificado"/> via dependency injection
/// (construtor primário). Um único certificado é construído uma vez por chamada da fábrica e usado para assinar todos
/// os RPS e o lote. Certificados são apropriadamente descartados usando padrão "using".
/// </item>
/// <item>
/// <strong>Geração de Assinatura:</strong> Usa geradores internos de assinatura (RpsSignatureGenerator e
/// XmlFileSignatureGenerator&lt;PedidoEnvioLote&gt;) que NÃO são expostos ao usuário. Estes são
/// detalhes de implementação ocultos atrás da API pública de fábrica.
/// </item>
/// <item>
/// <strong>Assinaturas Garantidas:</strong> Todo PedidoEnvioLote retornado por esta fábrica tem:
/// - Todos os objetos RPS contidos assinados (RPS[i].Assinatura != null)
/// - O objeto de lote em si assinado (PedidoEnvioLote.Signature != null)
/// A fábrica reforça este invariante automaticamente.
/// </item>
/// <item>
/// <strong>Cálculo de Cabeçalho de Lote:</strong> Computa automaticamente:
/// - QtdRps: Contagem total de RPS no lote
/// - ValorTotalServicos: Soma de todos os ValorServicos de RPS
/// - ValorTotalDeducoes: Soma de todos os ValorDeducoes de RPS (nulo se total for zero)
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed propositalmente para garantir que os invariantes
/// de assinatura (todos os RPS assinados, PedidoEnvioLote assinado, totalizações corretas) não possam ser
/// violados por herança. Usuários devem usar composição se precisarem estender funcionalidades, não herança.
/// </item>
/// <item>
/// <strong>Amigável a DI:</strong> Pode ser registrada em IServiceCollection como um serviço escopo ou singleton.
/// Os usuários dependem desta fábrica, não dos geradores internos de assinatura ou lógica de lote.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Configuração de dependency injection
/// services.AddSingleton(certificateConfig);
/// services.AddScoped&lt;PedidoEnvioLoteFactory&gt;();
///
/// // Uso
/// var factory = serviceProvider.GetRequiredService&lt;PedidoEnvioLoteFactory&gt;();
/// var rpsCollection = new[] { rps1, rps2, rps3 };  // Múltiplos RPS
///
/// // Cria PedidoEnvioLote totalmente assinado em uma chamada
/// // DtInicio e DtFim são inferidas automaticamente das datas de emissão dos RPS
/// var pedidoLote = factory.NewCpf((Cpf)12345678901L, false, rpsCollection);
/// // ou
/// var pedidoLote = factory.NewCnpj((Cnpj)12345678901234L, false, rpsCollection);
///
/// // Resultado:
/// // - Todos os RPS em pedidoLote.Rps têm Assinatura preenchido
/// // - pedidoLote.Assinatura está preenchido
/// // - pedidoLote.Cabecalho tem totalizações e datas computadas automaticamente
/// </code>
/// </example>
public sealed class PedidoEnvioLoteFactory(Certificado certificate)
{
    private const string InvalidRequest = "É necessário informar pelo menos uma RPS para que o lote seja válido.";

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
    /// Gerador interno de assinatura para objetos PedidoEnvioLote.
    /// Detalhe de implementação não exposto ao usuário da biblioteca.
    /// Instanciado localmente (não via DI) pois é uma responsabilidade interna da biblioteca.
    /// </summary>
    private readonly XmlFileSignatureGenerator<PedidoEnvioLote> _pedidoSignatureGenerator = new XmlFileSignatureGenerator<PedidoEnvioLote>();

    /// <summary>
    /// Cria um <see cref="PedidoEnvioLote"/> totalmente assinado a partir de um CPF contendo múltiplos <see cref="Rps"/>.
    /// </summary>
    /// <param name="cpf">CPF do prestador de serviços.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote. Deve conter pelo menos um RPS.</param>
    /// <returns>
    /// Um objeto PedidoEnvioLote completamente assinado e pronto para envio.
    /// - Todos os RPS no lote têm Assinatura preenchido
    /// - PedidoEnvioLote.Assinatura está preenchido
    /// - CabecalhoLote contém totalizações computadas, contagem de RPS e datas calculadas automaticamente
    /// </returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> ou <paramref name="rpsList"/> é nulo.</exception>
    /// <exception cref="ArgumentException">Se <paramref name="rpsList"/> é vazio ou não contém pelo menos um RPS não-nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se carregamento do certificado ou assinatura falhar.</exception>
    /// <remarks>
    /// <para>
    /// <strong>Comportamento de Processamento em Lote:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <strong>CPF:</strong> Recebido como Value Object (Cpf) já validado conforme módulo 11.
    /// </item>
    /// <item>
    /// <strong>Datas Automáticas:</strong> DtInicio e DtFim são calculadas automaticamente a partir da menor e maior
    /// data de emissão dos RPS fornecidos, respectivamente. Não precisam ser passadas como parâmetros.
    /// </item>
    /// <item>
    /// <strong>Totalizações:</strong> QtdRps, ValorTotalServicos e ValorTotalDeducoes são calculadas
    /// automaticamente a partir dos RPS fornecidos.
    /// </item>
    /// <item>
    /// <strong>Assinatura Individual:</strong> Cada RPS é assinado individualmente antes de ser adicionado ao lote.
    /// </item>
    /// </list>
    /// </remarks>
    public PedidoEnvioLote NewCpf(Cpf cpf, bool transacao, IEnumerable<Rps> rpsList)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(rpsList);
        return ConstructWith((CpfOrCnpj)cpf, transacao, rpsList);
    }

    /// <summary>
    /// Cria um <see cref="PedidoEnvioLote"/> totalmente assinado a partir de um CNPJ contendo múltiplos <see cref="Rps"/>.
    /// </summary>
    /// <param name="cnpj">CNPJ do prestador de serviços.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote. Deve conter pelo menos um RPS.</param>
    /// <returns>
    /// Um objeto PedidoEnvioLote completamente assinado e pronto para envio.
    /// - Todos os RPS no lote têm Assinatura preenchido
    /// - PedidoEnvioLote.Assinatura está preenchido
    /// - CabecalhoLote contém totalizações computadas, contagem de RPS e datas calculadas automaticamente
    /// </returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> ou <paramref name="rpsList"/> é nulo.</exception>
    /// <exception cref="ArgumentException">Se <paramref name="rpsList"/> é vazio ou não contém pelo menos um RPS não-nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se carregamento do certificado ou assinatura falhar.</exception>
    /// <remarks>
    /// <para>
    /// <strong>Comportamento de Processamento em Lote:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <strong>CNPJ:</strong> Recebido como Value Object (Cnpj) já validado conforme módulo 11.
    /// </item>
    /// <item>
    /// <strong>Datas Automáticas:</strong> DtInicio e DtFim são calculadas automaticamente a partir da menor e maior
    /// data de emissão dos RPS fornecidos, respectivamente. Não precisam ser passadas como parâmetros.
    /// </item>
    /// <item>
    /// <strong>Totalizações:</strong> QtdRps, ValorTotalServicos e ValorTotalDeducoes são calculadas
    /// automaticamente a partir dos RPS fornecidos.
    /// </item>
    /// <item>
    /// <strong>Assinatura Individual:</strong> Cada RPS é assinado individualmente antes de ser adicionado ao lote.
    /// </item>
    /// </list>
    /// </remarks>
    public PedidoEnvioLote NewCnpj(Cnpj cnpj, bool transacao, IEnumerable<Rps> rpsList)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(rpsList);
        return ConstructWith((CpfOrCnpj)cnpj, transacao, rpsList);
    }

    /// <summary>
    /// Método interno que orquestra a construção, agregação de lote e assinatura de um PedidoEnvioLote.
    /// </summary>
    /// <param name="cpfOrCnpj">O CPF ou CNPJ do prestador de serviços como Value Object.</param>
    /// <param name="transacao">Indica se esta é uma submissão de transação.</param>
    /// <param name="rpsList">Coleção de objetos RPS a incluir no lote.</param>
    /// <returns>Um objeto PedidoEnvioLote totalmente assinado com todas as totalizações e datas calculadas.</returns>
    /// <remarks>
    /// <para>
    /// <strong>Implementação do Processamento em Lote (7 Passos):</strong>
    /// </para>
    /// <list type="number">
    /// <item>Valida que rpsList contém pelo menos um RPS não-nulo</item>
    /// <item>Constrói X509Certificate2 a partir da configuração <see cref="Certificado"/> injetada</item>
    /// <item>Itera através de todos os RPS no lote:
    ///     <list type="bullet">
    ///     <item>Incrementa contador de quantidade (QtdRps)</item>
    ///     <item>Acumula valores totais de serviço (ValorServicos)</item>
    ///     <item>Acumula deduções totais (ValorDeducoes)</item>
    ///     <item>Rastreia data mínima e máxima de emissão para cálculo automático de DtInicio e DtFim</item>
    ///     <item>Assina cada RPS individualmente usando RpsSignatureGenerator (popula RPS.Assinatura)</item>
    ///     </list>
    /// </item>
    /// <item>Cria CabecalhoLote com valores computados automaticamente:
    ///     <list type="bullet">
    ///     <item>QtdRps: Contagem total de RPS processados</item>
    ///     <item>ValorTotalServicos: Soma de todos os ValorServicos de RPS</item>
    ///     <item>ValorTotalDeducoes: Soma de todos os ValorDeducoes de RPS (nulo se zero)</item>
    ///     <item>DtInicio: Data de emissão MAIS ANTIGA entre todos os RPS</item>
    ///     <item>DtFim: Data de emissão MAIS RECENTE entre todos os RPS</item>
    ///     <item>Transacao: Flag de transação fornecido pelo chamador</item>
    ///     </list>
    /// </item>
    /// <item>Cria o PedidoEnvioLote com o cabeçalho gerado e todos os RPS assinados</item>
    /// <item>Assina o PedidoEnvioLote completo usando XmlFileSignatureGenerator (popula PedidoEnvioLote.Assinatura)</item>
    /// <item>Descarta apropriadamente o certificado usando statement "using"</item>
    /// <item>Retorna o PedidoEnvioLote totalmente assinado com datas e totalizações garantidas</item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentException">Se <paramref name="rpsList"/> é vazio ou não contém pelo menos um RPS não-nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Se assinatura falhar.</exception>
    private PedidoEnvioLote ConstructWith(CpfOrCnpj cpfOrCnpj, bool transacao, IEnumerable<Rps> rpsList)
    {
        rpsList = [.. rpsList?.Where(rps => rps != null) ?? []];

        if (!rpsList.Any())
        {
            throw new ArgumentException(InvalidRequest, nameof(rpsList));
        }

        // Build certificate from configuration (using ensures proper disposal)
        using X509Certificate2 certificate = _certificate.Build();

        long Quantidade = 0;
        decimal valorTotalServicos = 0, valorDeducoes = 0;
        DateTime dtInicio = DateTime.MaxValue, dtFim = DateTime.MinValue;

        foreach (Rps rps in rpsList!)
        {
            Quantidade++;
            valorTotalServicos += (decimal)rps.ValorServicos;
            valorDeducoes += (decimal)rps.ValorDeducoes;

            var rpsDataEmissao = (DateTime)rps.DataEmissao;

            dtInicio = (rpsDataEmissao < dtInicio) ? rpsDataEmissao : dtInicio;
            dtFim = (rpsDataEmissao > dtFim) ? rpsDataEmissao : dtFim;

            // RPS deve ser assinado antes de ser adicionado ao PedidoEnvioLote
            _rpsSignatureGenerator.Sign(rps, certificate);
        }

        CabecalhoLote cabecalhoLote = new(cpfOrCnpj)
        {
            Transacao = transacao,
            DtFim = (DataXsd)dtFim,
            DtInicio = (DataXsd)dtInicio,
            QtdRps = (Quantidade)Quantidade,
            ValorTotalServicos = (Valor)valorTotalServicos,
            ValorTotalDeducoes = valorDeducoes == 0 ? null : (Valor)valorDeducoes,
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