using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Modelo de dados do tomador de serviços (contratante do serviço prestado).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c>.
/// Construa instâncias via <see cref="Builders.TomadorBuilder"/>.
/// </remarks>
/// <param name="cpfOrCnpjTomador">CPF, CNPJ ou NIF do tomador, ou <c>null</c> quando identificado apenas pela Razão Social.</param>
/// <param name="razaoSocialTomador">Razão Social do tomador.</param>
/// <param name="inscricaoMunicipalTomador">Inscrição Municipal do tomador, ou <c>null</c>.</param>
/// <param name="inscricaoEstadualTomador">Inscrição Estadual do tomador, ou <c>null</c>.</param>
/// <param name="emailTomador">E-mail do tomador, ou <c>null</c>.</param>
/// <param name="enderecoTomador">Endereço do tomador, ou <c>null</c>.</param>
public sealed class Tomador(CpfOrCnpjOrNif? cpfOrCnpjTomador,
                            RazaoSocial? razaoSocialTomador,
                            InscricaoMunicipal? inscricaoMunicipalTomador,
                            InscricaoEstadual? inscricaoEstadualTomador,
                            Email? emailTomador,
                            Endereco? enderecoTomador)
{
    /// <summary>CPF, CNPJ ou NIF do tomador.</summary>
    public CpfOrCnpjOrNif? CpfOrCnpjTomador { get; set; } = cpfOrCnpjTomador;

    /// <summary>Razão Social do tomador de serviços.</summary>
    public RazaoSocial? RazaoSocialTomador { get; set; } = razaoSocialTomador;

    /// <summary>Inscrição Municipal do tomador.</summary>
    public InscricaoMunicipal? InscricaoMunicipalTomador { get; set; } = inscricaoMunicipalTomador;

    /// <summary>Inscrição Estadual do tomador.</summary>
    public InscricaoEstadual? InscricaoEstadualTomador { get; set; } = inscricaoEstadualTomador;

    /// <summary>E-mail do tomador.</summary>
    public Email? EmailTomador { get; set; } = emailTomador;

    /// <summary>Endereço do tomador.</summary>
    public Endereco? EnderecoTomador { get; set; } = enderecoTomador;
}