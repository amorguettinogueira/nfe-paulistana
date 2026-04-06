using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Fábrica para construir objetos <see cref="PedidoConsultaNFe"/> com geração automática
/// de assinatura digital.
/// </summary>
/// <remarks>
/// <para>
/// Fornece uma API one-shot para criar objetos <see cref="PedidoConsultaNFe"/> totalmente assinados,
/// gerenciando automaticamente certificados e assinatura do pedido.
/// </para>
/// <para>
/// <strong>Design Arquitetural:</strong> Factory Pattern — métodos retornam objetos completos,
/// não builders intermediários. Assinatura é garantida como invariante de todos os objetos retornados.
/// </para>
/// </remarks>
/// <param name="certificate">Configuração do certificado digital utilizado para assinar os pedidos.</param>
public sealed class PedidoConsultaNFeFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly XmlFileSignatureGenerator<PedidoConsultaNFe> _signatureGenerator = new();

    /// <summary>
    /// Cria um <see cref="PedidoConsultaNFe"/> totalmente assinado a partir de um CPF.
    /// </summary>
    /// <param name="cpf">CPF do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="detalhes">
    /// Lista de detalhes do pedido (chaves de NFS-e ou RPS a consultar). Mínimo 1, máximo 50.
    /// </param>
    /// <returns>Objeto <see cref="PedidoConsultaNFe"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cpf"/> ou <paramref name="detalhes"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaNFe NewCpf(Cpf cpf, DetalheConsultaNFe[] detalhes)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(detalhes);
        return ConstructWith((Cabecalho)cpf, detalhes);
    }

    /// <summary>
    /// Cria um <see cref="PedidoConsultaNFe"/> totalmente assinado a partir de um CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="detalhes">
    /// Lista de detalhes do pedido (chaves de NFS-e ou RPS a consultar). Mínimo 1, máximo 50.
    /// </param>
    /// <returns>Objeto <see cref="PedidoConsultaNFe"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cnpj"/> ou <paramref name="detalhes"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaNFe NewCnpj(Cnpj cnpj, DetalheConsultaNFe[] detalhes)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(detalhes);
        return ConstructWith((Cabecalho)cnpj, detalhes);
    }

    private PedidoConsultaNFe ConstructWith(Cabecalho cabecalho, DetalheConsultaNFe[] detalhes)
    {
        var pedido = new PedidoConsultaNFe
        {
            Cabecalho = cabecalho,
            Detalhe = detalhes,
        };

        using X509Certificate2 cert = _certificate.Build();
        _signatureGenerator.Sign(pedido, cert);

        return pedido;
    }
}
