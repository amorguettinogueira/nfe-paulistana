using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Define o contrato para construir objetos <see cref="Intermediario"/>
/// com padrão fluente.
/// </summary>
public interface IIntermediarioBuilder
{
    /// <summary>
    /// Define o endereço de e-mail do intermediário.
    /// </summary>
    /// <param name="email">Endereço de e-mail do intermediário.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="email"/> for nulo.</exception>
    IIntermediarioBuilder SetEmail(Email email);

    /// <summary>
    /// Verifica se o estado atual do construtor é válido para construção.
    /// Na prática sempre retorna <c>true</c> após qualquer chamada a <c>New()</c>,
    /// pois todos os overloads garantem ao menos um identificador definido.
    /// Equivalente a <c>!<see cref="GetValidationErrors"/>().Any()</c>.
    /// </summary>
    /// <returns><c>true</c> se não houver nenhum erro de validação; caso contrário, <c>false</c>.</returns>
    bool IsValid();

    /// <summary>
    /// Retorna os erros de validação do estado atual do construtor.
    /// </summary>
    /// <returns>Sequência de mensagens de erro; vazia quando o estado é válido.</returns>
    IEnumerable<string> GetValidationErrors();

    /// <summary>
    /// Constrói e retorna o objeto <see cref=" Intermediario"/> a partir das propriedades configuradas.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Intermediario"/>.</returns>
    /// <exception cref="ArgumentException">Se CPF/CNPJ e Inscrição Municipal forem ambos nulos.</exception>
    Intermediario Build();
}