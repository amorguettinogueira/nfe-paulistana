using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Domain;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nfe.Paulistana.V2.Infrastructure;

/// <summary>
/// Gera a assinatura de cancelamento para objetos <see cref="DetalheCancelamentoNFe"/>
/// conforme especificação da Prefeitura de São Paulo para o schema v02.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Fonte:</strong> <c>TiposNFe_v02.xsd</c> — Tipo <c>tpAssinaturaCancelamento</c>.<br/>
/// O texto de assinatura é composto por:
/// </para>
/// <list type="bullet">
/// <item><strong>InscricaoPrestador:</strong> 8 dígitos, zeros à esquerda.</item>
/// <item><strong>NumeroNFe:</strong> 12 dígitos, zeros à esquerda.</item>
/// </list>
/// <para>Total: 20 caracteres, assinados com RSA-SHA1 e armazenados como <c>base64Binary</c>.</para>
/// </remarks>
public sealed class CancelamentoSignatureGenerator : ElementSignatureGeneratorBase<DetalheCancelamentoNFe>, IElementSignatureGenerator<DetalheCancelamentoNFe>
{
    /// <summary>Comprimento do padding para InscricaoMunicipal (12 dígitos, zeros à esquerda).</summary>
    private const int InscricaoMunicipalPaddingLength = 12;

    /// <summary>Comprimento esperado do texto de assinatura de cancelamento.</summary>
    private const int ExpectedSigningTextLength = 24;

    protected override string Generate(DetalheCancelamentoNFe detalhe) =>
        new StringBuilder(ExpectedSigningTextLength)
            .Append(FormatInscricaoPrestador(detalhe.ChaveNfe?.InscricaoPrestador?.ToString(), InscricaoMunicipalPaddingLength))
            .Append(FormatNumero(detalhe.ChaveNfe?.NumeroNFe))
            .ToString();

    /// <summary>
    /// Assina o detalhe de cancelamento com o certificado fornecido, populando a propriedade
    /// <see cref="DetalheCancelamentoNFe.AssinaturaCancelamento"/>.
    /// </summary>
    /// <param name="detalhe">O detalhe de cancelamento a ser assinado.</param>
    /// <param name="certificate">Certificado X509 com chave privada para assinatura RSA-SHA1.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="detalhe"/> ou <paramref name="certificate"/> for nulo.</exception>
    /// <exception cref="InvalidOperationException">Se o texto de assinatura gerado tiver comprimento inválido.</exception>
    public override void Sign(DetalheCancelamentoNFe detalhe, X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(detalhe);
        ArgumentNullException.ThrowIfNull(certificate);

        detalhe.AssinaturaCancelamento = null;

        string signingText = Generate(detalhe);

        if (signingText.Length != ExpectedSigningTextLength)
        {
            throw new InvalidOperationException(ComprimentoInvalido.Format(signingText.Length));
        }

        detalhe.AssinaturaCancelamento = Sha1Digest(signingText, certificate);
    }
}