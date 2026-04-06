namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Contrato de uma ação de integração exibida no menu interativo do console.
/// <para>
/// Cada implementação encapsula uma operação contra os Web Services SOAP da NF-e Paulistana
/// (Prefeitura de São Paulo). As implementações são registradas no contêiner de DI como
/// <c>IEnumerable&lt;IIntegrationAction&gt;</c> e resolvidas por
/// <see cref="Presentation.Console.ConsoleMenuHost"/>, que as ordena por <see cref="MenuOrder"/>
/// e valida unicidade antes de exibir o menu.
/// </para>
/// <para>
/// <b>Registro:</b> implementações concretas são descobertas por reflexão em
/// <see cref="Host.ServiceConfigurator"/> e registradas como <c>Singleton</c>. Portanto,
/// nenhuma implementação deve manter estado mutável entre execuções.
/// </para>
/// </summary>
internal interface IIntegrationAction
{
    /// <summary>
    /// Posição da ação no menu interativo do console.
    /// <para>
    /// Deve ser único entre todas as implementações registradas — valores duplicados são
    /// detectados por <see cref="Presentation.Console.ConsoleMenuHost"/> na inicialização
    /// e impedem a execução do menu. Os valores canônicos estão em <see cref="ActionCatalog"/>.
    /// </para>
    /// </summary>
    int MenuOrder { get; }

    /// <summary>
    /// Rótulo legível exibido na linha do menu correspondente a esta ação.
    /// Deve ser conciso o suficiente para caber em uma linha de terminal.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executa a operação de integração de forma assíncrona.
    ///
    /// <para><b>Semântica de execução:</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <b>Assíncrono end-to-end:</b> toda E/S (chamadas SOAP via <c>HttpClient</c>,
    ///       leitura/gravação de arquivos) deve ser aguardada com <c>await</c>.
    ///       O método nunca deve bloquear a thread chamadora (<c>.Result</c>, <c>.Wait()</c>
    ///       e <c>Thread.Sleep</c> são proibidos).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <b>Cancelamento:</b> o <paramref name="cancellationToken"/> deve ser propagado
    ///       a todas as chamadas assíncronas internas (serviços SOAP, <c>HttpClient</c>,
    ///       operações de arquivo). O token é originado em
    ///       <see cref="Presentation.Console.ConsoleMenuHost"/> e ativado via
    ///       <c>Console.CancelKeyPress</c> (Ctrl+C), garantindo encerramento limpo.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <b>Modelo de threading:</b> as chamadas ao menu são estritamente seriais —
    ///       apenas uma ação é executada por vez. Implementações <b>não precisam</b> ser
    ///       thread-safe, mas <b>não devem</b> manter estado mutável entre invocações
    ///       (o ciclo de vida do singleton impede reinicialização).
    ///     </description>
    ///   </item>
    /// </list>
    ///
    /// <para><b>Idempotência e efeitos colaterais:</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <b>Ações de consulta</b> (ex.: <see cref="QueryCnpjV1Action"/>,
    ///       <see cref="QueryNfeV1Action"/>): somente leitura no servidor da Prefeitura —
    ///       idempotentes e sem efeitos colaterais externos além da chamada de rede.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <b>Ações de mutação</b> (ex.: <see cref="CancelNfeV1Action"/>,
    ///       <see cref="SendRpsTestV1Action"/>): produzem efeito permanente no servidor
    ///       da Prefeitura de São Paulo — <b>não são idempotentes</b>. Repetir a chamada
    ///       pode resultar em erro de negócio (NF-e já cancelada, RPS duplicado etc.).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <b>Efeitos locais</b>: impressão no console via
    ///       <see cref="Presentation.Console.ConsolePresenter"/> e, em alguns casos,
    ///       gravação de arquivo no diretório temporário do sistema (<c>Path.GetTempPath()</c>).
    ///     </description>
    ///   </item>
    /// </list>
    ///
    /// <para><b>Tratamento de erros:</b> implementações devem capturar exceções internamente
    /// e reportar falhas ao usuário via <see cref="Presentation.Console.ConsolePresenter"/>,
    /// sem propagar exceções não tratadas ao chamador.</para>
    /// </summary>
    /// <param name="cancellationToken">
    /// Token para solicitar o cancelamento cooperativo da operação.
    /// O padrão é <see cref="CancellationToken.None"/>.
    /// </param>
    Task RunAsync(CancellationToken cancellationToken = default);
}
