namespace Nfe.Paulistana.Models;

/// <summary>
/// Define o contrato para objetos XML que podem ser assinados digitalmente via XMLDSig.
/// </summary>
/// <remarks>
/// <para>
/// O conteúdo XML assinado é armazenado em <see cref="SignedXmlContent"/> e utilizado
/// diretamente por <c>MensagemXml</c> para transmissão, garantindo que o XML enviado
/// seja idêntico ao que foi assinado digitalmente.
/// </para>
/// <para>
/// A lógica de assinatura é delegada a implementações de
/// <c>IXmlFileSignatureGenerator&lt;T&gt;</c>, mantendo separação de responsabilidades.
/// </para>
/// </remarks>
internal interface ISignedXmlFile
{
    /// <summary>
    /// Obtém ou define o conteúdo XML assinado para transmissão direta ao servidor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Quando definido, este conteúdo é utilizado diretamente por <c>MensagemXml</c>
    /// ao invés de re-serializar o objeto, garantindo que o XML enviado seja
    /// idêntico ao que foi assinado digitalmente.
    /// </para>
    /// </remarks>
    string? SignedXmlContent { get; set; }
}