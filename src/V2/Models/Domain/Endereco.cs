using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Modelo de endereço utilizado nos dados do tomador e intermediário do RPS.
/// Todos os campos são opcionais individualmente; o XSD exige ao menos um definido.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpEndereco</c>, linha 484.
/// Construa instâncias via <see cref="Nfe.Paulistana.Builders.EnderecoBuilder"/>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class Endereco
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Endereco()
    { }

    /// <summary>Inicializa o endereço com todos os campos, todos opcionais.</summary>
    public Endereco(Uf? uf = null,
                    CodigoIbge? cidade = null,
                    Bairro? bairro = null,
                    Cep? cep = null,
                    TipoLogradouro? tipo = null,
                    Logradouro? logradouro = null,
                    NumeroEndereco? numero = null,
                    Complemento? complemento = null,
                    EnderecoExterior? enderecoExterior = null)
    {
        Uf = uf;
        Cidade = cidade;
        Bairro = bairro;
        Cep = cep;
        Tipo = tipo;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        EnderecoExterior = enderecoExterior;
    }

    [XmlElement("TipoLogradouro", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public TipoLogradouro? Tipo { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Logradouro? Logradouro { get; set; }

    [XmlElement("NumeroEndereco", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public NumeroEndereco? Numero { get; set; }

    [XmlElement("ComplementoEndereco", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Complemento? Complemento { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Bairro? Bairro { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CodigoIbge? Cidade { get; set; }

    [XmlElement("UF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Uf? Uf { get; set; }

    [XmlElement("CEP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Cep? Cep { get; set; }

    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EnderecoExterior? EnderecoExterior { get; set; }
}