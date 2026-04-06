using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Options;

/// <summary>
/// Configuração do certificado digital utilizado nas operações de assinatura da NF-e Paulistana.
/// Suporta quatro fontes mutuamente alternativas: caminho de arquivo, dados brutos, handle nativo ou instância direta.
/// </summary>
/// <remarks>
/// <para>
/// Configure exatamente uma fonte de certificado via <c>AddNfePaulistanaV1</c>, <c>AddNfePaulistanaV2</c>
/// ou <c>AddNfePaulistanaAll</c>. O método <see cref="Build"/> avalia as fontes na ordem:
/// <see cref="FilePath"/>, <see cref="PointerHandle"/>, <see cref="RawData"/>, <see cref="Certificate"/>.
/// </para>
/// <para>
/// O <see cref="X509Certificate2"/> retornado por <see cref="Build"/> é um recurso gerenciado;
/// o chamador é responsável por descartá-lo com <c>using</c>.
/// </para>
/// </remarks>
public sealed class Certificado
{
    private string? filePath;

    private const string InvalidConfiguration =
        "Nenhuma fonte de certificado válida configurada. Defina FilePath, RawData, PointerHandle ou Certificate.";

    /// <summary>Caminho do arquivo do certificado no sistema de arquivos (.pfx / .p12).</summary>
    public string? FilePath
    {
        get => filePath;
        set => filePath = !string.IsNullOrWhiteSpace(value) ? value : null;
    }

    private ReadOnlyCollection<byte>? rawData;

    /// <summary>Bytes brutos do certificado (.pfx / .p12) encapsulados em coleção somente leitura.</summary>
    public ReadOnlyCollection<byte>? RawData
    {
        get => rawData;
        set => rawData = value?.Count > 0 ? value : null;
    }

    private nint? pointerHandle;

    /// <summary>Handle nativo do certificado carregado na memória do sistema operacional.</summary>
    public nint? PointerHandle
    {
        get => pointerHandle;
        set => pointerHandle = value != (nint)0 ? value : null;
    }

    private string? password;

    /// <summary>Senha do certificado, quando exigida pelo arquivo ou dados brutos.</summary>
    public string? Password
    {
        get => password;
        set => password = !string.IsNullOrWhiteSpace(value) ? value : null;
    }

    /// <summary>Opções de armazenamento de chave aplicadas ao carregar o certificado via <see cref="FilePath"/> ou <see cref="RawData"/>.</summary>
    public X509KeyStorageFlags? KeyStorageFlags { get; set; }

    /// <summary>Instância de certificado já carregada, utilizada diretamente sem reprocessamento. O chamador é responsável por descartar essa instância com <c>using</c>.</summary>
    public X509Certificate2? Certificate { get; set; }

    /// <summary>
    /// Constrói uma instância de <see cref="X509Certificate2"/> a partir da fonte de certificado configurada.
    /// </summary>
    /// <returns>Instância do certificado configurado. O chamador é responsável pelo descarte com <c>using</c>.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando nenhuma fonte de certificado válida está configurada.</exception>
    public X509Certificate2 Build()
    {
        if (!string.IsNullOrEmpty(FilePath))
        {
            return KeyStorageFlags.HasValue
                ? new X509Certificate2(FilePath, Password, KeyStorageFlags.Value)
                : new X509Certificate2(FilePath, Password);
        }

        if (PointerHandle != null)
        {
            return new X509Certificate2(PointerHandle.Value);
        }

        if (RawData?.Count > 0)
        {
            return KeyStorageFlags.HasValue
                 ? new X509Certificate2(RawData.ToArray(), Password, KeyStorageFlags.Value)
                 : new X509Certificate2([.. RawData], Password);
        }

        if (Certificate != null)
        {
            return new X509Certificate2(Certificate);
        }

        throw new InvalidOperationException(InvalidConfiguration);
    }
}