using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.V1.Infrastructure;

/// <summary>
/// Gera a assinatura de cancelamento para objetos <see cref="DetalheCancelamentoNFe"/>
/// conforme especificação da Prefeitura de São Paulo.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Fonte:</strong> <c>TiposNFe_v01.xsd</c> — Tipo <c>tpAssinaturaCancelamento</c>.<br/>
/// O texto de assinatura é composto por:
/// </para>
/// <list type="bullet">
/// <item><strong>InscricaoPrestador:</strong> 8 dígitos, zeros à esquerda.</item>
/// <item><strong>NumeroNFe:</strong> 12 dígitos, zeros à esquerda.</item>
/// </list>
/// <para>Total: 20 caracteres, assinados com RSA-SHA1 e armazenados como <c>base64Binary</c>.</para>
/// </remarks>
public sealed class CancelamentoSignatureGenerator()
    : CancelamentoSignatureGeneratorBase<DetalheCancelamentoNFe>(
          inscricaoMunicipalPaddingLength: 8,
          expectedSigningTextLength: 20,
          getInscricaoPrestador: d => d.ChaveNfe?.InscricaoPrestador?.ToString(),
          getNumeroNFe: d => d.ChaveNfe?.NumeroNFe),
      IElementSignatureGenerator<DetalheCancelamentoNFe>
{ }