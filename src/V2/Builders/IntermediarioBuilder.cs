using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Construtor fluente de objetos <see cref="Intermediario"/>, com entrada obrigatória
/// de um identificador (CPF ou CNPJ) via factory method.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Identificador obrigatório na construção:</strong> Diferentemente de <see cref="EnderecoBuilder"/>,
/// todo caminho de entrada via <see cref="New(Cpf, bool)"/> ou
/// <see cref="New(Cnpj, bool, InscricaoMunicipal?)"/> garante que ao menos um identificador
/// esteja sempre definido. <see cref="IIntermediarioBuilder.IsValid"/> e
/// <see cref="IIntermediarioBuilder.GetValidationErrors"/> existem como contrato defensivo
/// e para consistência com os demais builders da biblioteca.
/// </item>
/// <item>
/// <strong>ISS retido:</strong> O indicador de retenção do ISS é sempre requerido na construção
/// e não pode ser alterado após a criação do builder.
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed para proteger os invariantes
/// de validação. Extensão via herança abriria brechas para estado inválido.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Com CPF
/// var intermediario = IntermediarioBuilder
///     .New((Cpf)12345678909L, intermediarioRetemIss: true)
///     .SetEmail((Email)"intermediario@example.com")
///     .Build();
///
/// // Com CNPJ e Inscrição Municipal opcional
/// var intermediario = IntermediarioBuilder
///     .New((Cnpj)"11222333000181", intermediarioRetemIss: false, inscricaoMunicipal: null)
///     .Build();
/// </code>
/// </example>
public sealed class IntermediarioBuilder : IIntermediarioBuilder
{
    private CpfOrCnpj? cpfCnpjIntermediario;
    private InscricaoMunicipal? inscricaoMunicipalIntermediario;
    private bool issRetidoIntermediario;
    private Email? emailIntermediario;

    private IntermediarioBuilder()
    { }

    /// <summary>
    /// Cria uma instância do construtor a partir de um CPF, indicando se o ISS foi retido.
    /// </summary>
    /// <param name="cpf">Cadastro de Pessoa Física (CPF) do intermediário.</param>
    /// <param name="intermediarioRetemIss"><c>true</c> indica que o ISS foi retido pelo intermediário.</param>
    /// <returns>Uma nova instância de <see cref="IIntermediarioBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> for nulo.</exception>
    public static IIntermediarioBuilder New(
        Cpf cpf,
        bool intermediarioRetemIss)
    {
        ArgumentNullException.ThrowIfNull(cpf, nameof(cpf));

        return new IntermediarioBuilder()
        {
            cpfCnpjIntermediario = (CpfOrCnpj)cpf,
            issRetidoIntermediario = intermediarioRetemIss,
        };
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de um CNPJ, indicando se o ISS foi retido
    /// e, opcionalmente, a Inscrição Municipal.
    /// </summary>
    /// <param name="cnpj">Cadastro Nacional de Pessoa Jurídica (CNPJ) do intermediário.</param>
    /// <param name="intermediarioRetemIss"><c>true</c> indica que o ISS foi retido pelo intermediário.</param>
    /// <param name="inscricaoMunicipal">Inscrição Municipal do intermediário, ou <c>null</c> se não aplicável.</param>
    /// <returns>Uma nova instância de <see cref="IIntermediarioBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> for nulo.</exception>
    public static IIntermediarioBuilder New(
        Cnpj cnpj,
        bool intermediarioRetemIss,
        InscricaoMunicipal? inscricaoMunicipal)
    {
        ArgumentNullException.ThrowIfNull(cnpj, nameof(cnpj));

        return new IntermediarioBuilder()
        {
            cpfCnpjIntermediario = (CpfOrCnpj)cnpj,
            issRetidoIntermediario = intermediarioRetemIss,
            inscricaoMunicipalIntermediario = inscricaoMunicipal
        };
    }

    /// <summary>
    /// Define o endereço de e-mail do intermediário.
    /// </summary>
    /// <param name="email">Endereço de e-mail do intermediário.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="email"/> for nulo.</exception>
    public IIntermediarioBuilder SetEmail(Email email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        emailIntermediario = email;

        return this;
    }

    /// <summary>
    /// Verifica se o estado atual do construtor é válido para construção.
    /// Na prática sempre retorna <c>true</c> após qualquer chamada a <c>New()</c>,
    /// pois todos os overloads garantem ao menos um identificador definido.
    /// Existe como contrato defensivo e para consistência com os demais builders.
    /// Equivalente a <c>!<see cref="GetValidationErrors"/>().Any()</c>.
    /// </summary>
    /// <returns><c>true</c> se não houver nenhum erro de validação; caso contrário, <c>false</c>.</returns>
    public bool IsValid() => !GetValidationErrors().Any();

    /// <summary>
    /// Retorna os erros de validação do estado atual do construtor.
    /// </summary>
    /// <returns>
    /// Sequência de mensagens de erro; vazia quando o estado é válido.
    /// Possível erro: CPF/CNPJ e Inscrição Municipal ambos ausentes.
    /// </returns>
    public IEnumerable<string> GetValidationErrors()
    {
        if (cpfCnpjIntermediario == null && inscricaoMunicipalIntermediario == null)
        {
            yield return "É necessário fornecer pelo menos um CPF/CNPJ ou uma Inscrição Municipal para construir um intermediário.";
        }
    }

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Intermediario"/> a partir das propriedades configuradas.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Intermediario"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Se CPF/CNPJ e Inscrição Municipal forem ambos nulos.
    /// Inalcançável na prática após qualquer chamada válida a <c>New()</c>.
    /// </exception>
    public Intermediario Build()
    {
        string[] errors = [.. GetValidationErrors()];

        return errors.Length > 0
            ? throw new ArgumentException(errors[0])
            : new(cpfCnpjIntermediario, inscricaoMunicipalIntermediario, issRetidoIntermediario, emailIntermediario);
    }
}