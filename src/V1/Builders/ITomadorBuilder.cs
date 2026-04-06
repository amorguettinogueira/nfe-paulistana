using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Define o contrato para construir objetos <see cref="Tomador"/>
/// com padrão fluente.
/// </summary>
public interface ITomadorBuilder
{
    /// <summary>
    /// Define o endereço de e-mail do tomador de serviços.
    /// </summary>
    /// <param name="email">E-mail do tomador.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="email"/> for nulo.</exception>
    ITomadorBuilder SetEmail(Email email);

    /// <summary>
    /// Define o endereço do tomador de serviços.
    /// </summary>
    /// <param name="endereco">Objeto <see cref="Endereco"/> construído via <c>EnderecoBuilder</c>.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="endereco"/> for nulo.</exception>
    ITomadorBuilder SetEndereco(Endereco endereco);

    /// <summary>
    /// Verifica se o estado atual do construtor é válido para construção, incluindo
    /// presença de ao menos um campo e ausência de violações de dependência cruzada entre campos.
    /// Equivalente a <c>!<see cref="GetValidationErrors"/>().Any()</c>.
    /// </summary>
    /// <returns><c>true</c> se não houver nenhum erro de validação; caso contrário, <c>false</c>.</returns>
    bool IsValid();

    /// <summary>
    /// Retorna todos os erros de validação do estado atual, incluindo regras cruzadas entre campos.
    /// </summary>
    /// <returns>Sequência de mensagens de erro; vazia quando o estado é válido.</returns>
    IEnumerable<string> GetValidationErrors();

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Tomador"/> a partir dos atributos fornecidos.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Tomador"/>.</returns>
    Tomador Build();
}