namespace Nfe.Paulistana.Abstractions;

/// <summary>
/// Contrato genérico dos serviços responsáveis por enviar pedidos ao webservice da NF-e Paulistana
/// e retornar o resultado deserializado.
/// </summary>
/// <typeparam name="TPedido">Tipo do pedido a ser enviado ao webservice.</typeparam>
/// <typeparam name="TRetorno">Tipo do retorno deserializado do webservice.</typeparam>
public interface INfeService<TPedido, TRetorno>
    where TPedido : class
    where TRetorno : class
{
    /// <summary>
    /// Valida e envia um <typeparamref name="TPedido"/> ao webservice, retornando o retorno deserializado.
    /// </summary>
    /// <param name="pedido">Pedido a ser enviado ao webservice, já assinado digitalmente quando necessário.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>Retorno deserializado do webservice.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="pedido"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando a validação XSD falha ou a resposta é inválida.</exception>
    Task<TRetorno> SendAsync(TPedido pedido, CancellationToken cancellationToken = default);
}