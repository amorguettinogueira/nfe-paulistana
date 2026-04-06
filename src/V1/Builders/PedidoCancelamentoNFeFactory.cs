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
/// Fábrica para construir objetos <see cref="PedidoCancelamentoNFe"/> com geração automática
/// de assinatura digital de cancelamento e assinatura do envelope XML.
/// </summary>
/// <remarks>
/// <para>
/// Fornece uma API one-shot para criar objetos <see cref="PedidoCancelamentoNFe"/> totalmente assinados,
/// gerenciando automaticamente certificados, assinatura individual de cada detalhe de cancelamento e
/// assinatura do pedido completo.
/// </para>
/// <para>
/// <strong>Design Arquitetural:</strong> Factory Pattern — métodos retornam objetos completos,
/// não builders intermediários. Assinaturas de cancelamento e de envelope são garantidas como
/// invariantes de todos os objetos retornados.
/// </para>
/// </remarks>
/// <param name="certificate">Configuração do certificado digital utilizado para assinar os pedidos.</param>
public sealed class PedidoCancelamentoNFeFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly CancelamentoSignatureGenerator _cancelamentoSignatureGenerator = new();
    private readonly XmlFileSignatureGenerator<PedidoCancelamentoNFe> _signatureGenerator = new();

    /// <summary>
    /// Cria um <see cref="PedidoCancelamentoNFe"/> totalmente assinado a partir de um CPF.
    /// </summary>
    /// <param name="cpf">CPF do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="transacao">Indica se os cancelamentos devem ser tratados como transação atômica.</param>
    /// <param name="detalhes">
    /// Lista de detalhes do pedido (chaves de NFS-e a cancelar). Mínimo 1, máximo 50.
    /// </param>
    /// <returns>Objeto <see cref="PedidoCancelamentoNFe"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cpf"/> ou <paramref name="detalhes"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoCancelamentoNFe NewCpf(Cpf cpf, bool transacao, DetalheCancelamentoNFe[] detalhes)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(detalhes);
        return ConstructWith(new CabecalhoCancelamento((CpfOrCnpj)cpf) { Transacao = transacao }, detalhes);
    }

    /// <summary>
    /// Cria um <see cref="PedidoCancelamentoNFe"/> totalmente assinado a partir de um CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="transacao">Indica se os cancelamentos devem ser tratados como transação atômica.</param>
    /// <param name="detalhes">
    /// Lista de detalhes do pedido (chaves de NFS-e a cancelar). Mínimo 1, máximo 50.
    /// </param>
    /// <returns>Objeto <see cref="PedidoCancelamentoNFe"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cnpj"/> ou <paramref name="detalhes"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoCancelamentoNFe NewCnpj(Cnpj cnpj, bool transacao, DetalheCancelamentoNFe[] detalhes)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(detalhes);
        return ConstructWith(new CabecalhoCancelamento((CpfOrCnpj)cnpj) { Transacao = transacao }, detalhes);
    }

    private PedidoCancelamentoNFe ConstructWith(CabecalhoCancelamento cabecalho, DetalheCancelamentoNFe[] detalhes)
    {
        using X509Certificate2 cert = _certificate.Build();

        foreach (DetalheCancelamentoNFe detalhe in detalhes)
        {
            _cancelamentoSignatureGenerator.Sign(detalhe, cert);
        }

        var pedido = new PedidoCancelamentoNFe
        {
            Cabecalho = cabecalho,
            Detalhe = detalhes,
        };

        _signatureGenerator.Sign(pedido, cert);

        return pedido;
    }
}