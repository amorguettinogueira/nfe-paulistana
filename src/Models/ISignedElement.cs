namespace Nfe.Paulistana.Models;

/// <summary>
/// <para>
/// Define o contrato para objetos que podem ser assinados digitalmente.
/// </para>
/// <para>
/// Esta interface representa objetos que participam de operações de assinatura digital
/// dentro do processo de submissão de NF-e. Fornece uma propriedade padrão para armazenar
/// a assinatura digital gerada.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <strong>Notas Arquiteturais:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Contrato Mínimo:</strong> Esta interface define apenas o mecanismo de armazenamento
/// para assinaturas digitais (propriedade Assinatura). Não impõe nenhum comportamento de assinatura.
/// </item>
/// <item>
/// <strong>Separação de Responsabilidades:</strong> A lógica real de assinatura é delegada a
/// implementações de IObjectSignatureGenerator&lt;T&gt;, mantendo limites claros de responsabilidade.
/// </item>
/// <item>
/// <strong>Genérico por Design:</strong> Esta interface é intencionalmente mínima para permitir
/// que diferentes objetos de domínio (Rps, Cabecalho, etc.) participem de operações de assinatura
/// via implementações especializadas de IObjectSignatureGenerator.
/// </item>
/// <item>
/// <strong>Armazenamento Base64:</strong> A propriedade Assinatura armazena os bytes
/// da assinatura codificados em base64, adequados para serialização XML.
/// </item>
/// </list>
/// </remarks>
internal interface ISignedElement
{
    /// <summary>
    /// Obtém ou define a assinatura digital deste objeto.
    /// </summary>
    /// <value>
    /// Os bytes da assinatura codificados em base64, ou <c>null</c> se o objeto não foi assinado.
    /// Esta propriedade é utilizada durante a serialização XML de documentos assinados.
    /// </value>
    byte[]? Assinatura { get; set; }
}