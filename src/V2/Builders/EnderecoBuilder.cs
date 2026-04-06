using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Construtor fluente de objetos <see cref="Endereco"/>, com validação imediata por setter
/// e validação cruzada de campos (cross-field) no momento do <see cref="Build"/>.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Campos opcionais:</strong> Todos os campos do endereço são opcionais individualmente.
/// O XSD da NF-e Paulistana permite endereço parcial, mas exige ao menos um campo definido.
/// </item>
/// <item>
/// <strong>Validação cruzada:</strong> <see cref="Build"/> verifica regras de dependência entre
/// campos — <c>logradouro</c> é obrigatório quando <c>tipo</c>, <c>numero</c> ou <c>complemento</c>
/// estão definidos. Use <see cref="GetValidationErrors"/> para inspecionar erros antes de construir.
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed para proteger os invariantes de validação.
/// Extensão via herança abriria brechas para estado inválido.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Endereço nacional
/// var endereco = EnderecoBuilder.New()
///     .SetTipo((TipoLogradouro)"Av")
///     .SetLogradouro((Logradouro)"Paulista")
///     .SetNumero((NumeroEndereco)"100")
///     .SetBairro((Bairro)"Bela Vista")
///     .SetCodigoIbge((CodigoIbge)3550308)
///     .SetUf((Uf)"SP")
///     .SetCep((Cep)1310100)
///     .Build();
///
/// // Endereço exterior (mutuamente exclusivo com os campos nacionais)
/// var enderecoExt = EnderecoBuilder.New()
///     .SetEnderecoExterior((CodigoPaisISO)"US", (CodigoEndPostal)"10001", (NomeCidade)"New York", (EstadoProvinciaRegiao)"NY")
///     .Build();
/// </code>
/// </example>
public sealed class EnderecoBuilder : IEnderecoBuilder
{
    private Uf? uf;
    private CodigoIbge? cidade;
    private Bairro? bairro;
    private Cep? cep;
    private TipoLogradouro? tipo;
    private Logradouro? logradouro;
    private NumeroEndereco? numero;
    private Complemento? complemento;
    private EnderecoExterior? enderecoExterior;

    private EnderecoBuilder()
    { }

    /// <summary>
    /// Cria uma nova instância do construtor de endereço sem campos pré-preenchidos.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="IEnderecoBuilder"/>.</returns>
    public static IEnderecoBuilder New() => new EnderecoBuilder();

    /// <summary>
    /// Cria uma nova instância do construtor de endereço com campos pré-preenchidos.
    /// Útil para editar um endereço existente sem recriar cada campo individualmente.
    /// </summary>
    /// <param name="uf">UF pré-preenchida ou <c>null</c>.</param>
    /// <param name="cidade">Código IBGE do município pré-preenchido ou <c>null</c>.</param>
    /// <param name="bairro">Bairro pré-preenchido ou <c>null</c>.</param>
    /// <param name="cep">CEP pré-preenchido ou <c>null</c>.</param>
    /// <param name="tipo">Tipo de logradouro pré-preenchido ou <c>null</c>.</param>
    /// <param name="logradouro">Logradouro pré-preenchido ou <c>null</c>.</param>
    /// <param name="numero">Número pré-preenchido ou <c>null</c>.</param>
    /// <param name="complemento">Complemento pré-preenchido ou <c>null</c>.</param>
    /// <param name="codigoPais">Código do país pré-preenchido ou <c>null</c>.</param>
    /// <param name="codigoEndereco">Código do endereço pré-preenchido ou <c>null</c>.</param>
    /// <param name="nomeCidade">Nome da cidade pré-preenchido ou <c>null</c>.</param>
    /// <param name="estadoProvinciaRegiao">Estado, província ou região pré-preenchida ou <c>null</c>.</param>
    /// <returns>Uma nova instância de <see cref="IEnderecoBuilder"/> com os campos informados.</returns>
    public static IEnderecoBuilder New(Uf? uf = null,
                                       CodigoIbge? cidade = null,
                                       Bairro? bairro = null,
                                       Cep? cep = null,
                                       TipoLogradouro? tipo = null,
                                       Logradouro? logradouro = null,
                                       NumeroEndereco? numero = null,
                                       Complemento? complemento = null,
                                       CodigoPaisISO? codigoPais = null,
                                       CodigoEndPostal? codigoEndereco = null,
                                       NomeCidade? nomeCidade = null,
                                       EstadoProvinciaRegiao? estadoProvinciaRegiao = null)
    {
        var builder = new EnderecoBuilder
        {
            uf = uf,
            cidade = cidade,
            bairro = bairro,
            cep = cep,
            tipo = tipo,
            logradouro = logradouro,
            numero = numero,
            complemento = complemento
        };

        if (codigoPais is not null && codigoEndereco is not null && nomeCidade is not null && estadoProvinciaRegiao is not null)
        {
            _ = builder.SetEnderecoExterior(codigoPais, codigoEndereco, nomeCidade, estadoProvinciaRegiao);
        }

        return builder;
    }

    /// <summary>
    /// Define a UF (Unidade Federativa) do endereço.
    /// </summary>
    /// <param name="uf">Abreviação do estado (ex: "SP", "RJ").</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="uf"/> for nulo.</exception>
    public IEnderecoBuilder SetUf(Uf uf)
    {
        ArgumentNullException.ThrowIfNull(uf, nameof(uf));
        this.uf = uf;
        return this;
    }

    /// <summary>
    /// Define o código IBGE do município do endereço.
    /// </summary>
    /// <param name="cidade">Código IBGE do município.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cidade"/> for nulo.</exception>
    public IEnderecoBuilder SetCodigoIbge(CodigoIbge cidade)
    {
        ArgumentNullException.ThrowIfNull(cidade, nameof(cidade));
        this.cidade = cidade;
        return this;
    }

    /// <summary>
    /// Define o bairro do endereço.
    /// </summary>
    /// <param name="bairro">Nome do bairro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="bairro"/> for nulo.</exception>
    public IEnderecoBuilder SetBairro(Bairro bairro)
    {
        ArgumentNullException.ThrowIfNull(bairro, nameof(bairro));
        this.bairro = bairro;
        return this;
    }

    /// <summary>
    /// Define o CEP (Código de Endereçamento Postal) do endereço.
    /// </summary>
    /// <param name="cep">Código postal.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cep"/> for nulo.</exception>
    public IEnderecoBuilder SetCep(Cep cep)
    {
        ArgumentNullException.ThrowIfNull(cep, nameof(cep));
        this.cep = cep;
        return this;
    }

    /// <summary>
    /// Define o tipo de logradouro do endereço (ex: "Av", "R", "Praça").
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="tipo">Tipo de logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="tipo"/> for nulo.</exception>
    public IEnderecoBuilder SetTipo(TipoLogradouro tipo)
    {
        ArgumentNullException.ThrowIfNull(tipo, nameof(tipo));
        this.tipo = tipo;
        return this;
    }

    /// <summary>
    /// Define o logradouro do endereço.
    /// </summary>
    /// <param name="logradouro">Nome do logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="logradouro"/> for nulo.</exception>
    public IEnderecoBuilder SetLogradouro(Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(logradouro, nameof(logradouro));
        this.logradouro = logradouro;
        return this;
    }

    /// <summary>
    /// Define o número do logradouro do endereço.
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="numero">Número do logradouro.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="numero"/> for nulo.</exception>
    public IEnderecoBuilder SetNumero(NumeroEndereco numero)
    {
        ArgumentNullException.ThrowIfNull(numero, nameof(numero));
        this.numero = numero;
        return this;
    }

    /// <summary>
    /// Define o complemento do endereço.
    /// Requer que <see cref="SetLogradouro"/> também seja chamado; caso contrário, <see cref="Build"/> lançará exceção.
    /// </summary>
    /// <param name="complemento">Complemento do endereço (ex: "Apto 35", "Bloco B").</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="complemento"/> for nulo.</exception>
    public IEnderecoBuilder SetComplemento(Complemento complemento)
    {
        ArgumentNullException.ThrowIfNull(complemento, nameof(complemento));
        this.complemento = complemento;
        return this;
    }

    /// <summary>
    /// Define os dados do endereço exterior.
    /// </summary>
    /// <param name="codigoPais">Código do país.</param>
    /// <param name="codigoEndereco">Código do endereço postal.</param>
    /// <param name="nomeCidade">Nome da cidade.</param>
    /// <param name="estadoProvinciaRegiao">Estado, província ou região.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoPais"/>, <paramref name="codigoEndereco"/>, <paramref name="nomeCidade"/> ou <paramref name="estadoProvinciaRegiao"/> for nulo.</exception>
    public IEnderecoBuilder SetEnderecoExterior(CodigoPaisISO codigoPais,
                                                CodigoEndPostal codigoEndereco,
                                                NomeCidade nomeCidade,
                                                EstadoProvinciaRegiao estadoProvinciaRegiao)
    {
        ArgumentNullException.ThrowIfNull(codigoPais, nameof(codigoPais));
        ArgumentNullException.ThrowIfNull(codigoEndereco, nameof(codigoEndereco));
        ArgumentNullException.ThrowIfNull(nomeCidade, nameof(nomeCidade));
        ArgumentNullException.ThrowIfNull(estadoProvinciaRegiao, nameof(estadoProvinciaRegiao));
        enderecoExterior = new EnderecoExterior(codigoPais, codigoEndereco, nomeCidade, estadoProvinciaRegiao);
        return this;
    }

    /// <summary>
    /// Verifica se o estado atual do construtor é válido para construção, incluindo
    /// presença de ao menos um campo e ausência de violações de dependência cruzada entre campos.
    /// Equivalente a <c>!<see cref="GetValidationErrors"/>().Any()</c>.
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
    /// <item>Nenhum campo definido.</item>
    /// <item><c>logradouro</c> obrigatório quando <c>tipo</c> está definido.</item>
    /// <item><c>logradouro</c> obrigatório quando <c>numero</c> ou <c>complemento</c> estão definidos.</item>
    /// </list>
    /// </returns>
    public IEnumerable<string> GetValidationErrors()
    {
        if (uf is null
           && cidade is null
           && bairro is null
           && cep is null
           && tipo is null
           && logradouro is null
           && numero is null
           && complemento is null
           && enderecoExterior is null)
        {
            yield return "É necessário fornecer ao menos um atributo do endereço para que ele seja considerado válido.";
        }

        if (tipo is not null && logradouro is null)
        {
            yield return "Logradouro é obrigatório quando Tipo de Logradouro é fornecido.";
        }

        if ((numero is not null || complemento is not null) && logradouro is null)
        {
            yield return "Logradouro é obrigatório quando Número ou Complemento é fornecido.";
        }
    }

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Endereco"/> a partir dos campos configurados.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Endereco"/> com os campos informados.</returns>
    /// <exception cref="ArgumentException">
    /// Se nenhum campo foi definido, ou se <c>logradouro</c> é nulo quando <c>tipo</c>,
    /// <c>numero</c> ou <c>complemento</c> estão definidos.
    /// Consulte <see cref="GetValidationErrors"/> para inspecionar todos os erros antes de construir.
    /// </exception>
    public Endereco Build()
    {
        string[] errors = [.. GetValidationErrors()];

        return errors.Length > 0
            ? throw new ArgumentException(errors[0])
            : new Endereco(uf, cidade, bairro, cep, tipo, logradouro, numero, complemento, enderecoExterior);
    }
}