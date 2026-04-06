using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Modelo de dados do intermediário do serviço, utilizado quando um terceiro
/// atua como ponte entre o prestador e o tomador de serviços.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c>.
/// Construa instâncias via <see cref="Nfe.Paulistana.Builders.IntermediarioBuilder"/>.
/// </remarks>
/// <param name="cpfCnpjIntermediario">CPF ou CNPJ do intermediário, ou <c>null</c> se identificado apenas pela Inscrição Municipal.</param>
/// <param name="inscricaoMunicipalIntermediario">Inscrição Municipal do intermediário, ou <c>null</c>.</param>
/// <param name="issRetidoIntermediario"><c>true</c> se o ISS foi retido pelo intermediário; <c>null</c> quando não aplicável.</param>
/// <param name="emailIntermediario">E-mail do intermediário, ou <c>null</c>.</param>
public sealed class Intermediario(CpfOrCnpj? cpfCnpjIntermediario,
                                  InscricaoMunicipal? inscricaoMunicipalIntermediario,
                                  bool? issRetidoIntermediario,
                                  Email? emailIntermediario)
{
    /// <summary>CPF ou CNPJ do intermediário.</summary>
    public CpfOrCnpj? CpfCnpjIntermediario { get; set; } = cpfCnpjIntermediario;

    /// <summary>Inscrição Municipal do intermediário.</summary>
    public InscricaoMunicipal? InscricaoMunicipalIntermediario { get; set; } = inscricaoMunicipalIntermediario;

    /// <summary>Indica se o ISS foi retido pelo intermediário.</summary>
    public bool? IssRetidoIntermediario { get; set; } = issRetidoIntermediario;

    /// <summary>E-mail do intermediário.</summary>
    public Email? EmailIntermediario { get; set; } = emailIntermediario;
}