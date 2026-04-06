using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Modelo de endereço no exterior utilizado nos dados do tomador e intermediário do RPS.
/// Todos os campos são obrigatórios se o objeto for instanciado.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpEnderecoExterior</c>, linha 943.
/// Construa instâncias via o método <c>SetEnderecoExterior</c> de <see cref="Nfe.Paulistana.V2.Builders.EnderecoBuilder"/>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class EnderecoExterior
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnderecoExterior()
    { }

    /// <summary>Inicializa o endereço com todos os campos, todos obrigatórios.</summary>
    public EnderecoExterior(CodigoPaisISO codigoPais,
                            CodigoEndPostal codigoEndereco,
                            NomeCidade nomeCidade,
                            EstadoProvinciaRegiao estadoProvinciaRegiao)
    {
        ArgumentNullException.ThrowIfNull(codigoPais, nameof(codigoPais));
        ArgumentNullException.ThrowIfNull(codigoEndereco, nameof(codigoEndereco));
        ArgumentNullException.ThrowIfNull(nomeCidade, nameof(nomeCidade));
        ArgumentNullException.ThrowIfNull(estadoProvinciaRegiao, nameof(estadoProvinciaRegiao));

        CodigoPais = codigoPais;
        CodigoEndereco = codigoEndereco;
        NomeCidade = nomeCidade;
        EstadoProvinciaRegiao = estadoProvinciaRegiao;
    }

    [XmlElement("cPais", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CodigoPaisISO? CodigoPais { get; set; }

    [XmlElement("cEndPost", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CodigoEndPostal? CodigoEndereco { get; set; }

    [XmlElement("xCidade", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public NomeCidade? NomeCidade { get; set; }

    [XmlElement("xEstProvReg", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EstadoProvinciaRegiao? EstadoProvinciaRegiao { get; set; }

    /// <summary>
    /// Determina igualdade por valor: dois endereços são considerados iguais
    /// se todos os campos (<see cref="CodigoPais"/>, <see cref="CodigoEndereco"/>,
    /// <see cref="NomeCidade"/> e <see cref="EstadoProvinciaRegiao"/>) forem iguais.
    /// </summary>
    /// <param name="obj">Objeto a comparar.</param>
    /// <returns>
    /// <c>true</c> se <paramref name="obj"/> for um <see cref="EnderecoExterior"/> com
    /// os mesmos valores em todos os campos; caso contrário, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is EnderecoExterior other &&
        GetType() == other.GetType() &&
        (CodigoPais?.Equals(other.CodigoPais) ?? false) &&
        (CodigoEndereco?.Equals(other.CodigoEndereco) ?? false) &&
        (NomeCidade?.Equals(other.NomeCidade) ?? false) &&
        (EstadoProvinciaRegiao?.Equals(other.EstadoProvinciaRegiao) ?? false);

    /// <summary>
    /// Retorna o hash code baseado em todos os campos,
    /// consistente com a implementação de <see cref="Equals"/>.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(GetType(), CodigoPais, CodigoEndereco, NomeCidade, EstadoProvinciaRegiao);
}