using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Options;

/// <summary>
/// Configuração do certificado digital utilizado nas operações de assinatura da NF-e Paulistana.
/// Suporta três fontes mutuamente alternativas: caminho de arquivo, dados brutos ou handle nativo.
/// </summary>
/// <remarks>
/// <para>
/// Configure exatamente uma fonte de certificado via <c>AddNfePaulistanaV1</c>, <c>AddNfePaulistanaV2</c>
/// ou <c>AddNfePaulistanaAll</c>. O método <see cref="Build"/> avalia as fontes na ordem:
/// <see cref="FilePath"/>, <see cref="PointerHandle"/>, <see cref="RawData"/>.
/// </para>
/// <para>
/// O <see cref="X509Certificate2"/> retornado por <see cref="Build"/> é um recurso gerenciado;
/// o chamador é responsável por descartá-lo com <c>using</c>.
/// </para>
/// <para>
/// Consumidores que já possuem uma instância <see cref="X509Certificate2"/> podem obter os bytes
/// via <c>cert.Export(X509ContentType.Pfx, senha)</c> para uso em <see cref="RawData"/>,
/// ou o handle nativo via <c>cert.Handle</c> para uso em <see cref="PointerHandle"/>.
/// </para>
/// </remarks>
public sealed class Certificado
{
#pragma warning disable IDE0032
    private string? filePath;
#pragma warning restore IDE0032

    private const string InvalidConfiguration =
        "Nenhuma fonte de certificado válida configurada. Defina FilePath, RawData ou PointerHandle.";

    /// <summary>Caminho do arquivo do certificado no sistema de arquivos (.pfx / .p12).</summary>
    public string? FilePath
    {
        get => filePath;
        set => filePath = !string.IsNullOrWhiteSpace(value) ? value : null;
    }

#pragma warning disable IDE0032
    private ReadOnlyCollection<byte>? rawData;
#pragma warning restore IDE0032

    /// <summary>Bytes brutos do certificado (.pfx / .p12) encapsulados em coleção somente leitura.</summary>
    public ReadOnlyCollection<byte>? RawData
    {
        get => rawData;
        set => rawData = (value?.Count ?? 0) > 0 ? value : null;
    }

#pragma warning disable IDE0032
    private nint? pointerHandle;
#pragma warning restore IDE0032

    /// <summary>Handle nativo do certificado carregado na memória do sistema operacional.</summary>
    public nint? PointerHandle
    {
        get => pointerHandle;
        set => pointerHandle = value != (nint)0 ? value : null;
    }

    /// <summary>Senha do certificado, quando exigida pelo arquivo ou dados brutos.</summary>
    public string? Password { get; set; }

    /// <summary>
    /// Opções de armazenamento de chave aplicadas ao carregar o certificado via <see cref="FilePath"/> ou <see cref="RawData"/>.
    /// O padrão é <see cref="X509KeyStorageFlags.EphemeralKeySet"/>, que mantém a chave privada apenas na memória sem
    /// persistência em disco — o comportamento mais seguro para a maioria dos cenários de servidor.
    /// Atribua explicitamente para sobrescrever, por exemplo <c>X509KeyStorageFlags.MachineKeySet</c>.
    /// </summary>
    public X509KeyStorageFlags? KeyStorageFlags { get; set; }

    /// <summary>
    /// Constrói uma instância de <see cref="X509Certificate2"/> a partir da fonte de certificado configurada.
    /// </summary>
    /// <returns>Instância do certificado configurado. O chamador é responsável pelo descarte com <c>using</c>.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando nenhuma fonte de certificado válida está configurada.</exception>
    public X509Certificate2 Build()
    {
        X509KeyStorageFlags flags = KeyStorageFlags ?? X509KeyStorageFlags.EphemeralKeySet;

        if (!string.IsNullOrEmpty(FilePath))
        {
            return X509CertificateLoader.LoadPkcs12FromFile(FilePath, Password, flags);
        }

        if (PointerHandle != null)
        {
            return new X509Certificate2(PointerHandle.Value);
        }

        if (RawData?.Count > 0)
        {
            return X509CertificateLoader.LoadPkcs12(RawData.ToArray(), Password, flags);
        }

        throw new InvalidOperationException(InvalidConfiguration);
    }
}