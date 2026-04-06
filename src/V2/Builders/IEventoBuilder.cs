using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Define o contrato para construir objetos <see cref="AtividadeEvento"/>
/// com padrão fluente e validação cruzada de campos.
/// </summary>
public interface IEventoBuilder
{
    /// <summary>
    /// Define os campos de endereço para um endereço nacional, incluindo validação cruzada entre campos.
    /// </summary>
    /// <param name="cep">Código postal (CEP) do endereço.</param>
    /// <param name="bairro">Bairro do endereço.</param>
    /// <param name="logradouro">Logradouro do endereço.</param>
    /// <param name="numero">Número do endereço.</param>
    /// <param name="complemento">Complemento do endereço (opcional).</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cep"/>, <paramref name="bairro"/>, <paramref name="logradouro"/> ou <paramref name="numero"/> forem nulos.</exception>
    IEventoBuilder SetEndereco(
        Cep cep,
        Bairro bairro,
        Logradouro logradouro,
        NumeroEndereco numero,
        Complemento? complemento = null);

    /// <summary>
    /// Define os campos de endereço para um endereço exterior, incluindo validação cruzada entre campos.
    /// </summary>
    /// <param name="codigoPais">Código do país.</param>
    /// <param name="codigoEndereco">Código do endereço postal.</param>
    /// <param name="nomeCidade">Nome da cidade.</param>
    /// <param name="estadoProvinciaRegiao">Estado, província ou região.</param>
    /// <param name="bairro">Bairro do endereço.</param>
    /// <param name="logradouro">Logradouro do endereço.</param>
    /// <param name="numero">Número do endereço.</param>
    /// <param name="complemento">Complemento do endereço (opcional).</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoPais"/>, <paramref name="codigoEndereco"/>, <paramref name="nomeCidade"/>, <paramref name="estadoProvinciaRegiao"/>, <paramref name="bairro"/>, <paramref name="logradouro"/> ou <paramref name="numero"/> forem nulos.</exception>
    IEventoBuilder SetEnderecoExterior(
        CodigoPaisISO codigoPais,
        CodigoEndPostal codigoEndereco,
        NomeCidade nomeCidade,
        EstadoProvinciaRegiao estadoProvinciaRegiao,
        Bairro bairro,
        Logradouro logradouro,
        NumeroEndereco numero,
        Complemento? complemento = null);

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
    /// Constrói e retorna o objeto <see cref="AtividadeEvento"/> a partir dos campos configurados.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="AtividadeEvento"/> com os campos informados.</returns>
    /// <exception cref="ArgumentException">
    /// Se o estado atual do construtor for inválido para construção, incluindo presença de erros de validação ou ausência de campos obrigatórios.
    /// </exception>
    AtividadeEvento Build();
}