using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Modelo de endereço simplificado para o IBSCBS usado no evento do RPS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpEnderecoSimplesIBSCBS</c>, linha 1007.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class EnderecoSimplesIBSCBS
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnderecoSimplesIBSCBS()
    { }

    /// <summary>Inicializa o endereço com todos os campos, todos opcionais.</summary>
    /// <param name="logradouro">Logradouro do endereço.</param>
    /// <param name="numero">Número do endereço.</param>
    /// <param name="bairro">Bairro do endereço.</param>
    /// <param name="cep">CEP do endereço.</param>
    /// <param name="enderecoExterior">Endereço exterior.</param>
    /// <param name="complemento">Complemento do endereço.</param>
    /// <exception cref="ArgumentNullException">Lançada quando os campos Logradouro, Número ou Bairro são nulos.</exception>
    /// <exception cref="ArgumentException">Lançada quando nenhum dos campos CEP ou Endereço Exterior é fornecido.</exception>
    public EnderecoSimplesIBSCBS(
        Logradouro? logradouro = null,
        NumeroEndereco? numero = null,
        Bairro? bairro = null,
        Cep? cep = null,
        EnderecoExterior? enderecoExterior = null,
        Complemento? complemento = null)
    {
        ArgumentNullException.ThrowIfNull(logradouro, nameof(logradouro));
        ArgumentNullException.ThrowIfNull(numero, nameof(numero));
        ArgumentNullException.ThrowIfNull(bairro, nameof(bairro));

        if (cep == null && enderecoExterior == null)
        {
            throw new ArgumentException("É necessário fornecer pelo menos um dos campos: CEP ou Endereço Exterior.");
        }

        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cep = cep;
        EnderecoExterior = enderecoExterior;
        Complemento = complemento;
    }

    [XmlElement("CEP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Cep? Cep { get; set; }

    [XmlElement("endExt", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EnderecoExterior? EnderecoExterior { get; set; }

    [XmlElement("xLgr", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Logradouro? Logradouro { get; set; }

    [XmlElement("nro", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public NumeroEndereco? Numero { get; set; }

    [XmlElement("xCpl", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Complemento? Complemento { get; set; }

    [XmlElement("xBairro", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Bairro? Bairro { get; set; }

    /// <summary>
    /// Determina igualdade por valor: dois Value Objects do mesmo tipo concreto
    /// com o mesmo valor interno são considerados iguais.
    /// </summary>
    /// <param name="obj">Objeto a comparar.</param>
    /// <returns>
    /// <c>true</c> se <paramref name="obj"/> for do mesmo tipo concreto e tiver
    /// o mesmo valor interno; caso contrário, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is EnderecoSimplesIBSCBS other &&
        GetType() == other.GetType() &&
        Cep == other.Cep &&
        EnderecoExterior == other.EnderecoExterior &&
        Logradouro == other.Logradouro &&
        Numero == other.Numero &&
        Complemento == other.Complemento &&
        Bairro == other.Bairro;

    /// <summary>
    /// Retorna o hash code baseado no tipo concreto e no valor interno,
    /// consistente com a implementação de <see cref="Equals"/>.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(GetType(), Cep, EnderecoExterior, Logradouro, Numero, Complemento, Bairro);
}