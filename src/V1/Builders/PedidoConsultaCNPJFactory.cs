using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Fábrica para construir objetos <see cref="PedidoConsultaCNPJ"/> com geração automática
/// de assinatura digital.
/// </summary>
/// <remarks>
/// <para>
/// Fornece uma API one-shot para criar objetos <see cref="PedidoConsultaCNPJ"/> totalmente assinados,
/// gerenciando automaticamente certificados e assinatura do pedido.
/// </para>
/// <para>
/// <strong>Design Arquitetural:</strong> Factory Pattern — métodos retornam objetos completos,
/// não builders intermediários. Assinatura é garantida como invariante de todos os objetos retornados.
/// </para>
/// </remarks>
/// <param name="certificate">Configuração do certificado digital utilizado para assinar os pedidos.</param>
public sealed class PedidoConsultaCNPJFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly XmlFileSignatureGenerator<PedidoConsultaCNPJ> _signatureGenerator = new();

    /// <summary>
    /// Cria um <see cref="PedidoConsultaCNPJ"/> totalmente assinado a partir de um CPF remetente.
    /// </summary>
    /// <param name="cpf">CPF do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="cnpjContribuinte">CPF ou CNPJ do contribuinte que se deseja consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaCNPJ"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cpf"/> ou <paramref name="cnpjContribuinte"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaCNPJ NewCpf(Cpf cpf, CpfOrCnpj cnpjContribuinte)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(cnpjContribuinte);
        return ConstructWith((Cabecalho)cpf, cnpjContribuinte);
    }

    /// <summary>
    /// Cria um <see cref="PedidoConsultaCNPJ"/> totalmente assinado a partir de um CNPJ remetente.
    /// </summary>
    /// <param name="cnpj">CNPJ do remetente autorizado a transmitir a mensagem XML.</param>
    /// <param name="cnpjContribuinte">CPF ou CNPJ do contribuinte que se deseja consultar.</param>
    /// <returns>Objeto <see cref="PedidoConsultaCNPJ"/> completamente assinado e pronto para envio.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="cnpj"/> ou <paramref name="cnpjContribuinte"/> é nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Lançado quando o carregamento do certificado ou a assinatura falhar.</exception>
    public PedidoConsultaCNPJ NewCnpj(Cnpj cnpj, CpfOrCnpj cnpjContribuinte)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(cnpjContribuinte);
        return ConstructWith((Cabecalho)cnpj, cnpjContribuinte);
    }

    private PedidoConsultaCNPJ ConstructWith(Cabecalho cabecalho, CpfOrCnpj cnpjContribuinte)
    {
        var pedido = new PedidoConsultaCNPJ
        {
            Cabecalho = cabecalho,
            CnpjContribuinte = cnpjContribuinte,
        };

        using X509Certificate2 cert = _certificate.Build();
        _signatureGenerator.Sign(pedido, cert);

        return pedido;
    }
}
