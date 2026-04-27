using Microsoft.IO;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Fornece uma instância compartilhada de <see cref="RecyclableMemoryStreamManager"/> para
/// reutilização de buffers de <see cref="MemoryStream"/> ao longo do pipeline SOAP.
/// </summary>
internal static class StreamManager
{
    internal static readonly RecyclableMemoryStreamManager Instance = new();
}
