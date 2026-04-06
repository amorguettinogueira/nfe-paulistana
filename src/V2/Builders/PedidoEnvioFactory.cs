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
/// Fábrica para construir objetos PedidoEnvio v02 com geração automática de assinatura digital.
/// </summary>
public sealed class PedidoEnvioFactory(Certificado certificate)
{
    private readonly Certificado _certificate = certificate
        ?? throw new ArgumentNullException(nameof(certificate), "Configuração de certificado inválida.");

    private readonly RpsSignatureGenerator _rpsSignatureGenerator = new();
    private readonly XmlFileSignatureGenerator<PedidoEnvio> _pedidoSignatureGenerator = new();

    public PedidoEnvio NewCpf(Cpf cpf, Rps rps)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentNullException.ThrowIfNull(rps);
        return ConstructWith((Cabecalho)cpf, rps);
    }

    public PedidoEnvio NewCnpj(Cnpj cnpj, Rps rps)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        ArgumentNullException.ThrowIfNull(rps);
        return ConstructWith((Cabecalho)cnpj, rps);
    }

    private PedidoEnvio ConstructWith(Cabecalho cabecalho, Rps rps)
    {
        using X509Certificate2 certificate = _certificate.Build();

        _rpsSignatureGenerator.Sign(rps, certificate);

        var pedidoEnvio = new PedidoEnvio
        {
            Cabecalho = cabecalho,
            Rps = rps,
        };

        _pedidoSignatureGenerator.Sign(pedidoEnvio, certificate);

        return pedidoEnvio;
    }
}