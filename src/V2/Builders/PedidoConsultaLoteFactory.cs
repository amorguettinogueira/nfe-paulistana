using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Fábrica para construir objetos <see cref="PedidoConsultaLote"/> v02 com geração automática
/// de assinatura digital.
/// </summary>
/// <remarks>
/// <para>
/// Fornece uma API one-shot para criar objetos <see cref="PedidoConsultaLote"/> totalmente assinados,
/// gerenciando automaticamente certificados e assinatura do pedido.
/// </para>
/// <para>
/// <strong>Design Arquitetural:</strong> Factory Pattern — métodos retornam objetos completos,
/// não builders intermediários. Assinatura é garantida como invariante de todos os objetos retornados.
/// </para>
/// </remarks>
/// <param name="certificate">Configuração do certificado digital utilizado para assinar os pedidos.</param>
public sealed class PedidoConsultaLoteFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly XmlFileSignatureGenerator<PedidoConsultaLote> _signatureGenerator = new();

    /// <summary>
    /// Cria um <see cref="PedidoConsultaLote"/> totalmente assinado a partir de um CPF remetente.
    /// </summary>
    /// <param name="cpf">CPF do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="numeroLote">Número do lote que se deseja consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaLote"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cpf"/> ou <paramref name="numeroLote"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaLote NewCpf(Cpf cpf, Numero numeroLote)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(numeroLote);
        return ConstructWith((Cabecalho)cpf, numeroLote);
    }

    /// <summary>
    /// Cria um <see cref="PedidoConsultaLote"/> totalmente assinado a partir de um CNPJ remetente.
    /// </summary>
    /// <param name="cnpj">CNPJ alfanumérico do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="numeroLote">Número do lote que se deseja consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaLote"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cnpj"/> ou <paramref name="numeroLote"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaLote NewCnpj(Cnpj cnpj, Numero numeroLote)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(numeroLote);
        return ConstructWith((Cabecalho)cnpj, numeroLote);
    }

    private PedidoConsultaLote ConstructWith(Cabecalho cabecalho, Numero numeroLote)
    {
        var pedido = new PedidoConsultaLote
        {
            Cabecalho = new CabecalhoConsultaLote(cabecalho.CpfOrCnpj!)
            {
                NumeroLote = numeroLote,
            },
        };

        using X509Certificate2 cert = _certificate.Build();
        _signatureGenerator.Sign(pedido, cert);

        return pedido;
    }
}
