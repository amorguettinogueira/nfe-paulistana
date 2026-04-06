using Nfe.Paulistana.Models;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// <para>
/// Define o contrato para geração de assinaturas de objetos de domínio que implementam <see cref="ISignedElement"/>.
/// </para>
/// <para>
/// Esta interface de serviço genérica encapsula o processo completo de assinatura: converter um objeto de domínio
/// em um formato padronizado de texto de assinatura, computar a assinatura digital e armazená-la no objeto.
/// </para>
/// </summary>
/// <typeparam name="T">
/// O tipo de objeto a ser assinado. Deve implementar <see cref="ISignedElement"/> para participar de operações de assinatura.
/// </typeparam>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Parâmetro de Tipo Genérico:</strong> A interface é genérica para permitir que diferentes objetos de domínio
/// (Rps, Cabecalho, PedidoEnvio, etc.) tenham implementações de assinatura especializadas sem duplicação de código.
/// </item>
/// <item>
/// <strong>Lógica Centralizada:</strong> Todo o conhecimento sobre o formato do texto de assinatura para um tipo T específico
/// está encapsulado nas implementações desta interface. Alterações de formato afetam apenas o serviço, não o modelo de domínio.
/// </item>
/// <item>
/// <strong>Suporte a Versionamento:</strong> Diferentes versões de texto de assinatura (V1, V2, etc.) podem coexistir como
/// implementações separadas de IObjectSignatureGenerator&lt;T&gt;. Por exemplo:
/// <list type="bullet">
/// <item>RpsSigningTextGeneratorV1 — Usa indicadores de tipo numéricos ("1", "2", "3")</item>
/// <item>RpsSigningTextGeneratorV2 — Usa indicadores de tipo por letras ("A", "B", "C")</item>
/// </list>
/// </item>
/// <item>
/// <strong>Responsabilidade Única:</strong> Esta interface foca exclusivamente na geração e aplicação
/// de assinaturas digitais. Modelos de domínio (implementando ISignedElement) permanecem livres de lógica de assinatura.
/// </item>
/// <item>
/// <strong>Dirigido por Especificação:</strong> Implementações são guiadas pelas especificações oficiais da
/// Prefeitura de São Paulo (Nota do Milhão) para requisitos de assinatura de NF-e.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Exemplo de uso para assinar um RPS
/// var rps = new RpsBuilder().Build();
/// var generator = new RpsSigningTextGenerator();
/// var certificate = LoadCertificate();
///
/// generator.Sign(rps, certificate);
/// // Neste ponto, rps.Assinatura contém a assinatura digital
/// </code>
/// </example>
internal interface IElementSignatureGenerator<T> where T : ISignedElement
{
    /// <summary>
    /// Assina o objeto fornecido utilizando o certificado especificado.
    /// </summary>
    /// <param name="objectToSign">
    /// O objeto de domínio a ser assinado. Não deve ser nulo. Após a conclusão, sua propriedade Assinatura
    /// conterá a assinatura digital gerada (ou <c>null</c> se a assinatura falhar).
    /// </param>
    /// <param name="certificate">
    /// O certificado X509 contendo a chave privada para geração da assinatura.
    /// O certificado deve possuir uma chave privada válida acessível ao contexto atual.
    /// </param>
    /// <remarks>
    /// <para>
    /// <strong>Processo de Assinatura:</strong>
    /// </para>
    /// <list type="number">
    /// <item>Gera o texto de assinatura formatando todas as propriedades relevantes do objeto</item>
    /// <item>Codifica o texto de assinatura como bytes UTF-8</item>
    /// <item>Computa o digest SHA1 do texto de assinatura</item>
    /// <item>Assina o digest usando a chave privada do certificado com padding RSA/PKCS#1</item>
    /// <item>Armazena os bytes da assinatura na propriedade Assinatura do objeto</item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Se <paramref name="objectToSign"/> ou <paramref name="certificate"/> for nulo.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// Se o certificado não possuir chave privada ou a operação de assinatura falhar.
    /// </exception>
    void Sign(T objectToSign, X509Certificate2 certificate);
}