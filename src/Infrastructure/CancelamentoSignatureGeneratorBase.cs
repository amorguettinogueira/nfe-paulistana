using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Classe base abstrata para geradores de assinatura de cancelamento de NFS-e,
/// parametrizável pelo tipo do detalhe de cancelamento e pelas regras de formatação do esquema.
/// </summary>
/// <typeparam name="TDetalhe">
/// Tipo do detalhe de cancelamento. Deve implementar <see cref="ISignedElement"/>
/// para que a assinatura gerada possa ser armazenada no objeto.
/// </typeparam>
/// <remarks>
/// <para>
/// O texto de assinatura é composto por InscricaoPrestador + NumeroNFe, concatenados
/// com padding de zeros à esquerda cujos comprimentos variam por versão do esquema.
/// </para>
/// <para>
/// A lógica de formatação (<see cref="Generate"/>) e de assinatura (<see cref="Sign"/>)
/// é centralizada aqui; as subclasses fornecem apenas os parâmetros do esquema específico
/// e os acessores dos campos via construtor.
/// </para>
/// </remarks>
public abstract class CancelamentoSignatureGeneratorBase<TDetalhe>
    : ElementSignatureGeneratorBase<TDetalhe>
    where TDetalhe : ISignedElement
{
    private readonly int _inscricaoMunicipalPaddingLength;
    private readonly int _expectedSigningTextLength;
    private readonly Func<TDetalhe, string?> _getInscricaoPrestador;
    private readonly Func<TDetalhe, Numero?> _getNumeroNFe;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CancelamentoSignatureGeneratorBase{TDetalhe}"/>
    /// com os parâmetros de formatação e acessores de campos do esquema correspondente.
    /// </summary>
    /// <param name="inscricaoMunicipalPaddingLength">
    /// Comprimento do padding para InscricaoMunicipal (zeros à esquerda).
    /// </param>
    /// <param name="expectedSigningTextLength">
    /// Comprimento total esperado do texto de assinatura após concatenação.
    /// </param>
    /// <param name="getInscricaoPrestador">
    /// Função que extrai o valor de InscricaoPrestador como <see cref="string"/> do detalhe.
    /// </param>
    /// <param name="getNumeroNFe">
    /// Função que extrai o <see cref="Numero"/> da NFS-e do detalhe.
    /// </param>
    protected CancelamentoSignatureGeneratorBase(
        int inscricaoMunicipalPaddingLength,
        int expectedSigningTextLength,
        Func<TDetalhe, string?> getInscricaoPrestador,
        Func<TDetalhe, Numero?> getNumeroNFe)
    {
        _inscricaoMunicipalPaddingLength = inscricaoMunicipalPaddingLength;
        _expectedSigningTextLength = expectedSigningTextLength;
        _getInscricaoPrestador = getInscricaoPrestador;
        _getNumeroNFe = getNumeroNFe;
    }

    /// <summary>
    /// Gera o texto de assinatura de cancelamento a partir dos campos da chave da NFS-e.
    /// </summary>
    /// <param name="detalhe">O detalhe de cancelamento contendo a chave da NFS-e.</param>
    /// <returns>
    /// Texto de assinatura formado por InscricaoPrestador + NumeroNFe,
    /// com comprimento total esperado de caracteres definido no construtor.
    /// </returns>
    protected override string Generate(TDetalhe detalhe) =>
        new StringBuilder(_expectedSigningTextLength)
            .Append(FormatInscricaoPrestador(_getInscricaoPrestador(detalhe), _inscricaoMunicipalPaddingLength))
            .Append(FormatNumero(_getNumeroNFe(detalhe)))
            .ToString();

    /// <summary>
    /// Assina o detalhe de cancelamento com o certificado fornecido, populando a propriedade
    /// de assinatura via <see cref="ISignedElement.Assinatura"/>.
    /// </summary>
    /// <param name="detalhe">O detalhe de cancelamento a ser assinado.</param>
    /// <param name="certificate">Certificado X509 com chave privada para assinatura RSA-SHA1.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="detalhe"/> ou <paramref name="certificate"/> for nulo.</exception>
    /// <exception cref="InvalidOperationException">Se o texto de assinatura gerado tiver comprimento inválido.</exception>
    public override void Sign(TDetalhe detalhe, X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(detalhe);
        ArgumentNullException.ThrowIfNull(certificate);

        detalhe.Assinatura = null;

        string signingText = Generate(detalhe);

        if (signingText.Length != _expectedSigningTextLength)
        {
            throw new InvalidOperationException(ComprimentoInvalido.Format(signingText.Length));
        }

        detalhe.Assinatura = Sha1Digest(signingText, certificate);
    }
}