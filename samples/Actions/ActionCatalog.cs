namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Catálogo centralizado de constantes de menu para todas as implementações de
/// <see cref="IIntegrationAction"/>. Cada classe aninhada expõe <c>Order</c>
/// (posição única no menu interativo, validada em startup pelo <c>ConsoleMenuHost</c>)
/// e <c>Description</c> (rótulo exibido ao usuário).
/// </summary>
internal static class ActionCatalog
{
    /// <summary>Constantes para a ação de download do WSDL da Prefeitura de São Paulo.</summary>
    internal static class DownloadWsdl
    {
        public const int Order = 0;
        public const string Description = "Download do WSDL da Prefeitura";
    }

    /// <summary>Constantes para a ação de consulta de CNPJ via serviço V01.</summary>
    internal static class QueryCnpjV1
    {
        public const int Order = 1;
        public const string Description = "Consulta CNPJ V01";
    }

    /// <summary>Constantes para a ação de consulta de informações de lote via serviço V01.</summary>
    internal static class QueryBatchInfoV1
    {
        public const int Order = 2;
        public const string Description = "Consulta Informações Lote V01";
    }

    /// <summary>Constantes para a ação de consulta de lote de NFS-e via serviço V01.</summary>
    internal static class QueryBatchV1
    {
        public const int Order = 3;
        public const string Description = "Consulta Lote V01";
    }

    /// <summary>Constantes para a ação de consulta de NFS-e recebidas via serviço V01.</summary>
    internal static class QueryReceivedNfeV1
    {
        public const int Order = 4;
        public const string Description = "Consulta NFS-e Recebidas V01";
    }

    /// <summary>Constantes para a ação de consulta de NFS-e emitidas via serviço V01.</summary>
    internal static class QueryIssuedNfeV1
    {
        public const int Order = 5;
        public const string Description = "Consulta NFS-e Emitidas V01";
    }

    /// <summary>Constantes para a ação de consulta individual de NFS-e via serviço V01.</summary>
    internal static class QueryNfeV1
    {
        public const int Order = 6;
        public const string Description = "Consulta NFS-e V01";
    }

    /// <summary>Constantes para a ação de cancelamento de NFS-e via serviço V01.</summary>
    internal static class CancelNfeV1
    {
        public const int Order = 7;
        public const string Description = "Cancelamento NFS-e V01";
    }

    /// <summary>Constantes para a ação de envio de RPS em modo de teste via serviço V01.</summary>
    internal static class SendRpsTestV1
    {
        public const int Order = 8;
        public const string Description = "Envio de RPS de teste V01";
    }

    /// <summary>Constantes para a ação de consulta de CNPJ via serviço V02.</summary>
    internal static class QueryCnpjV2
    {
        public const int Order = 9;
        public const string Description = "Consulta CNPJ V02";
    }

    /// <summary>Constantes para a ação de consulta de informações de lote via serviço V02.</summary>
    internal static class QueryBatchInfoV2
    {
        public const int Order = 10;
        public const string Description = "Consulta Informações Lote V02";
    }

    /// <summary>Constantes para a ação de consulta de lote de NFS-e via serviço V02.</summary>
    internal static class QueryBatchV2
    {
        public const int Order = 11;
        public const string Description = "Consulta Lote V02";
    }

    /// <summary>Constantes para a ação de consulta de NFS-e recebidas via serviço V02.</summary>
    internal static class QueryReceivedNfeV2
    {
        public const int Order = 12;
        public const string Description = "Consulta NFS-e Recebidas V02";
    }

    /// <summary>Constantes para a ação de consulta de NFS-e emitidas via serviço V02.</summary>
    internal static class QueryIssuedNfeV2
    {
        public const int Order = 13;
        public const string Description = "Consulta NFS-e Emitidas V02";
    }

    /// <summary>Constantes para a ação de consulta individual de NFS-e via serviço V02.</summary>
    internal static class QueryNfeV2
    {
        public const int Order = 14;
        public const string Description = "Consulta NFS-e V02";
    }

    /// <summary>Constantes para a ação de cancelamento de NFS-e via serviço V02.</summary>
    internal static class CancelNfeV2
    {
        public const int Order = 15;
        public const string Description = "Cancelamento NFS-e V02";
    }

    /// <summary>Constantes para a ação de envio de RPS em modo de teste via serviço V02.</summary>
    internal static class SendRpsTestV2
    {
        public const int Order = 16;
        public const string Description = "Envio de RPS de teste V02";
    }
}