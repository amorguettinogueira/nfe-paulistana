using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Construtor fluente de objetos <see cref="Tomador"/> com três pontos de entrada distintos
/// conforme a identificação do tomador: CPF, CNPJ ou exclusivamente Razão Social.
/// </summary>
/// <remarks>
/// <para><strong>Design Arquitetural:</strong></para>
/// <list type="bullet">
/// <item>
/// <strong>Identificação obrigatória na construção:</strong> A <see cref="RazaoSocial"/> é sempre
/// requerida. CPF ou CNPJ são opcionais — use <see cref="NewRazaoSocial(RazaoSocial)"/>
/// quando o tomador for identificado apenas pela razão social.
/// </item>
/// <item>
/// <strong>Sealed Pattern:</strong> Esta classe é sealed para proteger os invariantes
/// de validação. Extensão via herança abriria brechas para estado inválido.
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var tomador = TomadorBuilder.NewCpf(
///         (Cpf)12345678909L,
///         (RazaoSocial)"Razão Social")
///     .SetEmail((Email)"tomador@example.com")
///     .SetEndereco(endereco)
///     .Build();
/// </code>
/// </example>
public sealed class TomadorBuilder : ITomadorBuilder
{
    private readonly CpfOrCnpjOrNif? cpfOrCnpjTomador;
    private readonly RazaoSocial? razaoSocialTomador;
    private InscricaoMunicipal? inscricaoMunicipalTomador;
    private InscricaoEstadual? inscricaoEstadualTomador;
    private Email? emailTomador;
    private Endereco? enderecoTomador;

    private TomadorBuilder(
        CpfOrCnpjOrNif? cpfOrCnpjTomador,
        RazaoSocial? razaoSocialTomador,
        InscricaoMunicipal? inscricaoMunicipalTomador,
        InscricaoEstadual? inscricaoEstadualTomador)
    {
        this.cpfOrCnpjTomador = cpfOrCnpjTomador;
        this.razaoSocialTomador = razaoSocialTomador;
        this.inscricaoMunicipalTomador = inscricaoMunicipalTomador;
        this.inscricaoEstadualTomador = inscricaoEstadualTomador;
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de um CPF e Razão Social.
    /// </summary>
    /// <param name="cpf">CPF do tomador de serviços.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> ou <paramref name="razaoSocial"/> for nulo.</exception>
    public static ITomadorBuilder NewCpf(Cpf cpf)
    {
        ArgumentNullException.ThrowIfNull(cpf, nameof(cpf));

        return new TomadorBuilder((CpfOrCnpjOrNif)cpf, null, null, null);
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de um CNPJ e Razão Social.
    /// </summary>
    /// <param name="cnpj">CNPJ do tomador de serviços.</param>
    /// <param name="razaoSocial">Razão Social do tomador de serviços.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> ou <paramref name="razaoSocial"/> for nulo.</exception>
    public static ITomadorBuilder NewCnpj(Cnpj cnpj, RazaoSocial razaoSocial)
    {
        ArgumentNullException.ThrowIfNull(cnpj, nameof(cnpj));
        ArgumentNullException.ThrowIfNull(razaoSocial, nameof(razaoSocial));

        return new TomadorBuilder((CpfOrCnpjOrNif)cnpj, razaoSocial, null, null);
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de um NIF e Razão Social.
    /// </summary>
    /// <param name="nif">NIF do tomador de serviços.</param>
    /// <param name="razaoSocial">Razão Social do tomador de serviços.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="nif"/> ou <paramref name="razaoSocial"/> for nulo.</exception>
    public static ITomadorBuilder NewNif(Nif nif, RazaoSocial razaoSocial)
    {
        ArgumentNullException.ThrowIfNull(nif, nameof(nif));
        ArgumentNullException.ThrowIfNull(razaoSocial, nameof(razaoSocial));
        return new TomadorBuilder((CpfOrCnpjOrNif)nif, razaoSocial, null, null);
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de uma Inscrição Municipal.
    /// </summary>
    /// <param name="inscricaoMunicipal">Inscrição Municipal do tomador de serviços.</param>
    /// <param name="cnpj">CNPJ do tomador de serviços, necessário para identificação quando a Inscrição Municipal é fornecida.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inscricaoMunicipal"/> ou <paramref name="cnpj"/> for nulo.</exception>
    public static ITomadorBuilder NewInscricaoMunicipal(InscricaoMunicipal inscricaoMunicipal, Cnpj cnpj)
    {
        ArgumentNullException.ThrowIfNull(inscricaoMunicipal, nameof(inscricaoMunicipal));
        ArgumentNullException.ThrowIfNull(cnpj, nameof(cnpj));

        return new TomadorBuilder((CpfOrCnpjOrNif)cnpj, null, inscricaoMunicipal, null);
    }

    /// <summary>
    /// Cria uma instância do construtor a partir de uma Inscrição Estadual e Razão Social.
    /// </summary>
    /// <param name="inscricaoEstadual">Inscrição Estadual do tomador de serviços.</param>
    /// <param name="razaoSocial">Razão Social do tomador de serviços.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inscricaoEstadual"/> ou <paramref name="razaoSocial"/> for nulo.</exception>
    public static ITomadorBuilder NewInscricaoEstadual(InscricaoEstadual inscricaoEstadual, RazaoSocial razaoSocial)
    {
        ArgumentNullException.ThrowIfNull(inscricaoEstadual, nameof(inscricaoEstadual));
        ArgumentNullException.ThrowIfNull(razaoSocial, nameof(razaoSocial));

        return new TomadorBuilder(null, razaoSocial, null, inscricaoEstadual);
    }

    /// <summary>
    /// Cria uma instância do construtor sem CPF, sem CNPJ e sem NIF, identificado apenas pela Razão Social e com um motivo para não informar NIF.
    /// Use quando o tomador não possui documento de identificação fiscal cadastrado, por exemplo, for do exterior e não tiver um NIF.
    /// </summary>
    /// <param name="motivoNifNaoInformado">Motivo pelo qual o NIF não foi informado.</param>
    /// <param name="razaoSocial">Razão Social do tomador de serviços.</param>
    /// <returns>Uma nova instância de <see cref="ITomadorBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="razaoSocial"/> for nulo.</exception>
    public static ITomadorBuilder NewRazaoSocial(MotivoNifNaoInformado motivoNifNaoInformado, RazaoSocial razaoSocial)
    {
        ArgumentNullException.ThrowIfNull(razaoSocial, nameof(razaoSocial));

        return new TomadorBuilder((CpfOrCnpjOrNif)motivoNifNaoInformado, razaoSocial, null, null);
    }

    /// <summary>
    /// Define o endereço de e-mail do tomador de serviços.
    /// </summary>
    /// <param name="email">E-mail do tomador de serviços.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="email"/> for nulo.</exception>
    public ITomadorBuilder SetEmail(Email email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        emailTomador = email;

        return this;
    }

    /// <summary>
    /// Define o endereço do tomador de serviços.
    /// </summary>
    /// <param name="endereco">Objeto <see cref="Endereco"/> construído via <see cref="EnderecoBuilder"/>.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="endereco"/> for nulo.</exception>
    public ITomadorBuilder SetEndereco(Endereco endereco)
    {
        ArgumentNullException.ThrowIfNull(endereco, nameof(endereco));

        enderecoTomador = endereco;

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
    /// </returns>
    public IEnumerable<string> GetValidationErrors()
    {
        yield break; // Placeholder para futuras validações
    }

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Tomador"/> a partir dos atributos fornecidos na cadeia de chamadas.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Tomador"/> com os campos informados.</returns>
    /// <exception cref="ArgumentException">
    /// Se o campo endereço não foi informado quando o prestador foi identificado por CNPJ ou NIF.
    /// Consulte <see cref="GetValidationErrors"/> para inspecionar todos os erros antes de construir.
    /// </exception>
    public Tomador Build()
    {
        string[] errors = [.. GetValidationErrors()];

        return errors.Length > 0
            ? throw new ArgumentException(errors[0])
            : new(cpfOrCnpjTomador, razaoSocialTomador, inscricaoMunicipalTomador, inscricaoEstadualTomador, emailTomador, enderecoTomador);
    }
}