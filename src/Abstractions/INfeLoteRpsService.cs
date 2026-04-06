namespace Nfe.Paulistana.Abstractions;

/// <summary>
/// Contrato genérico dos serviços responsáveis por enviar pedidos de envio em lote de RPS
/// ao webservice da NF-e Paulistana, em modo de produção ou de teste.
/// </summary>
/// <typeparam name="TPedido">Tipo do pedido de envio em lote a ser enviado ao webservice.</typeparam>
/// <typeparam name="TRetorno">Tipo do retorno deserializado do webservice.</typeparam>
public interface INfeLoteRpsService<TPedido, TRetorno>
    where TPedido : class
    where TRetorno : class
{
    /// <summary>
    /// Valida e envia um <typeparamref name="TPedido"/> ao webservice, retornando o retorno deserializado.
    /// </summary>
    /// <param name="pedido">Pedido de envio em lote de RPS, já assinado digitalmente.</param>
    /// <param name="modoTeste">
    /// Quando <see langword="true"/>, utiliza o envelope de teste para validação sem impacto nos registros fiscais.
    /// Padrão: <see langword="false"/>.
    /// </param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>Retorno deserializado do webservice.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="pedido"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando a validação XSD falha ou a resposta é inválida.</exception>
    Task<TRetorno> SendAsync(TPedido pedido, bool modoTeste = false, CancellationToken cancellationToken = default);
}
