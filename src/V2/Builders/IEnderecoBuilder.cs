using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Define o contrato para construir objetos <see cref="Models.Domain.Endereco"/>
/// com padrão fluente e validação cruzada de campos.
/// </summary>
public interface IEnderecoBuilder
{
    /// <summary>Define a UF (Unidade Federativa) do endereço.</summary>
    /// <param name="uf">Abreviação do estado (ex: "SP", "RJ").</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="uf"/> for nulo.</exception>
    IEnderecoBuilder SetUf(Uf uf);

    /// <summary>Define o código IBGE do município do endereço.</summary>
    /// <param name="cidade">Código IBGE do município.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cidade"/> for nulo.</exception>
    IEnderecoBuilder SetCodigoIbge(CodigoIbge cidade);

    /// <summary>Define o bairro do endereço.</summary>
    /// <param name="bairro">Nome do bairro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="bairro"/> for nulo.</exception>
    IEnderecoBuilder SetBairro(Bairro bairro);

    /// <summary>Define o CEP (Código de Endereçamento Postal) do endereço.</summary>
    /// <param name="cep">Código postal.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cep"/> for nulo.</exception>
    IEnderecoBuilder SetCep(Cep cep);

    /// <summary>
    /// Define o tipo de logradouro do endereço (ex: "Av", "R", "Praça").
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="tipo">Tipo de logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="tipo"/> for nulo.</exception>
    IEnderecoBuilder SetTipo(TipoLogradouro tipo);

    /// <summary>Define o logradouro do endereço.</summary>
    /// <param name="logradouro">Nome do logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="logradouro"/> for nulo.</exception>
    IEnderecoBuilder SetLogradouro(Logradouro logradouro);

    /// <summary>
    /// Define o número do logradouro do endereço.
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="numero">Número do logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="numero"/> for nulo.</exception>
    IEnderecoBuilder SetNumero(NumeroEndereco numero);

    /// <summary>
    /// Define o complemento do endereço (ex: "Apto 35", "Bloco B").
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="complemento">Complemento do endereço.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="complemento"/> for nulo.</exception>
    IEnderecoBuilder SetComplemento(Complemento complemento);

    /// <summary>
    /// Define os dados de um endereço no exterior, que é mutuamente exclusivo com os campos de endereço nacional (UF, código IBGE, bairro, CEP, tipo, logradouro, número e complemento).
    /// </summary>
    /// <param name="codigoPais">Código do País segundo tabela ISO.</param>
    /// <param name="codigoEndereco">Código do endereço.</param>
    /// <param name="nomeCidade">Nome da cidade.</param>
    /// <param name="estadoProvinciaRegiao">Estado, província ou região.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoPais"/>, <paramref name="codigoEndereco"/>, <paramref name="nomeCidade"/> ou <paramref name="estadoProvinciaRegiao"/> for nulo.</exception>
    IEnderecoBuilder SetEnderecoExterior(
        CodigoPaisISO codigoPais,
        CodigoEndPostal codigoEndereco,
        NomeCidade nomeCidade,
        EstadoProvinciaRegiao estadoProvinciaRegiao);

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
    /// Constrói e retorna o objeto <see cref="Models.Domain.Endereco"/> a partir dos campos configurados.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Models.Domain.Endereco"/> com os campos informados.</returns>
    /// <exception cref="ArgumentException">
    /// Se nenhum campo foi definido, ou se <c>logradouro</c> é nulo quando <c>tipo</c>,
    /// <c>numero</c> ou <c>complemento</c> estão definidos.
    /// </exception>
    Endereco Build();
}