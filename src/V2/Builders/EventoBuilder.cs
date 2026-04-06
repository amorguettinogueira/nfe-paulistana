using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Construtor fluente de objetos <see cref="AtividadeEvento"/>, com validação imediata por setter
/// e validação cruzada de campos (cross-field) no momento do <see cref="Build"/>.
/// </summary>
/// <example>
/// <code>
/// var evento = EventoBuilder.New((NomeEvento)"Evento Exemplo", (Data)"2024-01-01", (Data)"2024-01-02")
///     .SetEnderecoExterior(
///         (CodigoPaisISO)"BR",
///         (CodigoEndPostal)"12345",
///         (NomeCidade)"São Paulo",
///         (EstadoProvinciaRegiao)"SP",
///         (Bairro)"Bela Vista",
///         (Logradouro)"Paulista",
///         (NumeroEndereco)"100",
///         (Complemento)"Apto 101")
///     .Build();
/// </code>
/// </example>
public sealed class EventoBuilder : IEventoBuilder
{
    private NomeEvento? NomeEvento;
    private DataXsd? DataInicioEvento;
    private DataXsd? DataFimEvento;
    private Cep? Cep;
    private EnderecoExterior? EnderecoExterior;
    private Bairro? Bairro;
    private Logradouro? Logradouro;
    private NumeroEndereco? Numero;
    private Complemento? Complemento;

    private EventoBuilder()
    { }

    /// <summary>
    /// Cria uma nova instância do construtor de eventos, pré-configurada com os campos obrigatórios para construção de um evento.
    /// Útil para editar um evento existente sem recriar cada campo individualmente.
    /// </summary>
    /// <param name="nomeEvento">Nome do evento.</param>
    /// <param name="dataInicioEvento">Data de início do evento.</param>
    /// <param name="dataFimEvento">Data de término do evento.</param>
    /// <returns>Uma nova instância de <see cref="IEventoBuilder"/> com os campos informados.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="nomeEvento"/>, <paramref name="dataInicioEvento"/> ou <paramref name="dataFimEvento"/> forem nulos.</exception>
    public static IEventoBuilder New(NomeEvento nomeEvento, DataXsd dataInicioEvento, DataXsd dataFimEvento)
    {
        ArgumentNullException.ThrowIfNull(nomeEvento, nameof(nomeEvento));
        ArgumentNullException.ThrowIfNull(dataInicioEvento, nameof(dataInicioEvento));
        ArgumentNullException.ThrowIfNull(dataFimEvento, nameof(dataFimEvento));

        return new EventoBuilder
        {
            NomeEvento = nomeEvento,
            DataInicioEvento = dataInicioEvento,
            DataFimEvento = dataFimEvento
        };
    }

    /// <summary>
    /// Define os dados do endereço nacional.
    /// </summary>
    /// <param name="cep">Código postal (CEP) do endereço.</param>
    /// <param name="bairro">Bairro do endereço.</param>
    /// <param name="logradouro">Logradouro do endereço.</param>
    /// <param name="numero">Número do endereço.</param>
    /// <param name="complemento">Complemento do endereço (opcional).</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cep"/>, <paramref name="bairro"/>, <paramref name="logradouro"/> ou <paramref name="numero"/> forem nulos.</exception>
    public IEventoBuilder SetEndereco(Cep cep,
                                      Bairro bairro,
                                      Logradouro logradouro,
                                      NumeroEndereco numero,
                                      Complemento? complemento = null)
    {
        ArgumentNullException.ThrowIfNull(cep, nameof(cep));
        ArgumentNullException.ThrowIfNull(bairro, nameof(bairro));
        ArgumentNullException.ThrowIfNull(logradouro, nameof(logradouro));
        ArgumentNullException.ThrowIfNull(numero, nameof(numero));

        Cep = cep;
        Bairro = bairro;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;

        EnderecoExterior = null;

        return this;
    }

    /// <summary>
    /// Define os dados do endereço exterior.
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
    public IEventoBuilder SetEnderecoExterior(CodigoPaisISO codigoPais,
                                              CodigoEndPostal codigoEndereco,
                                              NomeCidade nomeCidade,
                                              EstadoProvinciaRegiao estadoProvinciaRegiao,
                                              Bairro bairro,
                                              Logradouro logradouro,
                                              NumeroEndereco numero,
                                              Complemento? complemento = null)
    {
        ArgumentNullException.ThrowIfNull(codigoPais, nameof(codigoPais));
        ArgumentNullException.ThrowIfNull(codigoEndereco, nameof(codigoEndereco));
        ArgumentNullException.ThrowIfNull(nomeCidade, nameof(nomeCidade));
        ArgumentNullException.ThrowIfNull(estadoProvinciaRegiao, nameof(estadoProvinciaRegiao));
        ArgumentNullException.ThrowIfNull(bairro, nameof(bairro));
        ArgumentNullException.ThrowIfNull(logradouro, nameof(logradouro));
        ArgumentNullException.ThrowIfNull(numero, nameof(numero));

        EnderecoExterior = new EnderecoExterior(codigoPais, codigoEndereco, nomeCidade, estadoProvinciaRegiao);
        Bairro = bairro;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;

        Cep = null;

        return this;
    }

    /// <summary>
    /// Verifica se o estado atual do construtor é válido para construção, incluindo
    /// presença de ao menos um campo e ausência de violações de dependência cruzada entre campos.
    /// Equivalente a <c>!<see cref="GetValidationErrors"/>.Any()</c>.
    /// </summary>
    /// <returns><c>true</c> se não houver nenhum erro de validação; caso contrário, <c>false</c>.</returns>
    public bool IsValid() => !GetValidationErrors().Any();

    /// <summary>
    /// Retorna todos os erros de validação do estado atual, incluindo regras cruzadas entre campos.
    /// </summary>
    /// <returns>
    /// Sequência de mensagens de erro. Vazia quando o estado é válido.
    /// Possíveis erros:
    /// <list type="bullet">
    /// <item>Nome do evento é obrigatório.</item>
    /// <item>Data de início do evento é obrigatória.</item>
    /// <item>Data de término do evento é obrigatória.</item>
    /// <item>É necessário fornecer o CEP ou os dados do Endereço no Exterior.</item>
    /// <item>Logradouro é obrigatório.</item>
    /// <item>Número é obrigatório.</item>
    /// <item>Bairro é obrigatório.</item>
    /// </list>
    /// </returns>
    public IEnumerable<string> GetValidationErrors()
    {
        if (NomeEvento == null)
        {
            yield return "Nome do evento é obrigatório.";
        }

        if (DataInicioEvento == null)
        {
            yield return "Data de início do evento é obrigatória.";
        }

        if (DataFimEvento == null)
        {
            yield return "Data de término do evento é obrigatória.";
        }

        if (Cep == null && EnderecoExterior == null)
        {
            yield return "É necessário fornecer o CEP ou o dos dados do Endereço no Exterior.";
        }

        if (Logradouro == null)
        {
            yield return "Logradouro é obrigatório.";
        }

        if (Numero == null)
        {
            yield return "Número é obrigatório.";
        }

        if (Bairro == null)
        {
            yield return "Bairroé obrigatório.";
        }
    }

    /// <summary>
    /// Constrói e retorna o objeto <see cref="AtividadeEvento"/> a partir dos campos configurados.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="AtividadeEvento"/> com os campos informados.</returns>
    /// <exception cref="ArgumentException">
    /// Se o estado atual do construtor for inválido para construção, incluindo presença de erros de validação ou ausência de campos obrigatórios.
    /// Consulte <see cref="GetValidationErrors"/> para inspecionar todos os erros antes de construir.
    /// </exception>
    public AtividadeEvento Build()
    {
        string[] errors = [.. GetValidationErrors()];

        return errors.Length > 0
            ? throw new ArgumentException(errors[0])
            : new AtividadeEvento(NomeEvento, DataInicioEvento, DataFimEvento, new EnderecoSimplesIBSCBS(Logradouro, Numero, Bairro, Cep, EnderecoExterior, Complemento));
    }
}