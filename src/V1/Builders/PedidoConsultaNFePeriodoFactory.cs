using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Fábrica para construir objetos <see cref="PedidoConsultaNFePeriodo"/> com geração automática
/// de assinatura digital.
/// </summary>
/// <remarks>
/// <para>
/// Fornece uma API one-shot para criar objetos <see cref="PedidoConsultaNFePeriodo"/> totalmente assinados,
/// gerenciando automaticamente certificados e assinatura do pedido.
/// </para>
/// <para>
/// <strong>Design Arquitetural:</strong> Factory Pattern — métodos retornam objetos completos,
/// não builders intermediários. Assinatura é garantida como invariante de todos os objetos retornados.
/// </para>
/// <para>
/// Compartilhada pelas operações <c>ConsultaNFeRecebidas</c> e <c>ConsultaNFeEmitidas</c>,
/// já que ambas utilizam o mesmo XSD (<c>PedidoConsultaNFePeriodo_v01.xsd</c>).
/// </para>
/// </remarks>
/// <param name="certificate">Configuração do certificado digital utilizado para assinar os pedidos.</param>
public sealed class PedidoConsultaNFePeriodoFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly XmlFileSignatureGenerator<PedidoConsultaNFePeriodo> _signatureGenerator = new();

    /// <summary>
    /// Cria um <see cref="PedidoConsultaNFePeriodo"/> totalmente assinado a partir de um CPF remetente.
    /// </summary>
    /// <param name="cpf">CPF do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="cpfCnpj">CPF ou CNPJ do prestador ou tomador a consultar.</param>
    /// <param name="inscricao">Inscrição Municipal do prestador/tomador. Opcional para consulta de NFS-e recebidas.</param>
    /// <param name="dtInicio">Data de início do período a consultar.</param>
    /// <param name="dtFim">Data de fim do período a consultar.</param>
    /// <param name="numeroPagina">Número da página de resultados a consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaNFePeriodo"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando algum parâmetro obrigatório é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaNFePeriodo NewCpf(
        Cpf cpf,
        CpfOrCnpj cpfCnpj,
        InscricaoMunicipal? inscricao,
        DataXsd dtInicio,
        DataXsd dtFim,
        Numero numeroPagina)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(cpfCnpj);
        ArgumentNullException.ThrowIfNull(dtInicio);
        ArgumentNullException.ThrowIfNull(dtFim);
        ArgumentNullException.ThrowIfNull(numeroPagina);
        return ConstructWith((Cabecalho)cpf, cpfCnpj, inscricao, dtInicio, dtFim, numeroPagina);
    }

    /// <summary>
    /// Cria um <see cref="PedidoConsultaNFePeriodo"/> totalmente assinado a partir de um CNPJ remetente.
    /// </summary>
    /// <param name="cnpj">CNPJ do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="cpfCnpj">CPF ou CNPJ do prestador ou tomador a consultar.</param>
    /// <param name="inscricao">Inscrição Municipal do prestador/tomador. Opcional para consulta de NFS-e recebidas.</param>
    /// <param name="dtInicio">Data de início do período a consultar.</param>
    /// <param name="dtFim">Data de fim do período a consultar.</param>
    /// <param name="numeroPagina">Número da página de resultados a consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaNFePeriodo"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando algum parâmetro obrigatório é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaNFePeriodo NewCnpj(
        Cnpj cnpj,
        CpfOrCnpj cpfCnpj,
        InscricaoMunicipal? inscricao,
        DataXsd dtInicio,
        DataXsd dtFim,
        Numero numeroPagina)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(cpfCnpj);
        ArgumentNullException.ThrowIfNull(dtInicio);
        ArgumentNullException.ThrowIfNull(dtFim);
        ArgumentNullException.ThrowIfNull(numeroPagina);
        return ConstructWith((Cabecalho)cnpj, cpfCnpj, inscricao, dtInicio, dtFim, numeroPagina);
    }

    private PedidoConsultaNFePeriodo ConstructWith(
        Cabecalho cabecalho,
        CpfOrCnpj cpfCnpj,
        InscricaoMunicipal? inscricao,
        DataXsd dtInicio,
        DataXsd dtFim,
        Numero numeroPagina)
    {
        var cabecalhoPeriodo = new CabecalhoConsultaPeriodo
        {
            CpfOrCnpj = cabecalho.CpfOrCnpj,
            CpfCnpj = cpfCnpj,
            Inscricao = inscricao,
            DtInicio = dtInicio,
            DtFim = dtFim,
            NumeroPagina = numeroPagina,
        };

        var pedido = new PedidoConsultaNFePeriodo
        {
            Cabecalho = cabecalhoPeriodo,
        };

        using X509Certificate2 cert = _certificate.Build();
        _signatureGenerator.Sign(pedido, cert);

        return pedido;
    }
}