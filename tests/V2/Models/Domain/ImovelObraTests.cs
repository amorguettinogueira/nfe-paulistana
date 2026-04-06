using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class ImovelObraTests
{
    [Fact]
    public void DefaultConstructor_PropertiesNull()
    {
        var t = new ImovelObra();

        Assert.Null(t.CadastroImovel);
        Assert.Null(t.IdentificacaoObra);
        Assert.Null(t.Endereco);
        Assert.Null(t.InscricaoImobiliaria);
    }

    [Fact]
    public void Constructor_WithCadastro_SetsProperty()
    {
        var cadastro = new CadastroImovel("ABCDEFGH");

        var t = new ImovelObra(cadastro);

        Assert.Equal(cadastro, t.CadastroImovel);
        Assert.Null(t.IdentificacaoObra);
        Assert.Null(t.Endereco);
    }

    [Fact]
    public void Constructor_WithIdentificacao_SetsProperty()
    {
        var id = new IdentificacaoObra(new string('A', 30));

        var t = new ImovelObra(null, id);

        Assert.Equal(id, t.IdentificacaoObra);
        Assert.Null(t.CadastroImovel);
        Assert.Null(t.Endereco);
    }

    [Fact]
    public void Constructor_WithEndereco_SetsProperty()
    {
        var end = new EnderecoSimplesIBSCBS(
            new Logradouro("RUA"),
            new NumeroEndereco("1"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );

        var t = new ImovelObra(null, null, end);

        Assert.Equal(end, t.Endereco);
        Assert.Null(t.CadastroImovel);
        Assert.Null(t.IdentificacaoObra);
    }

    [Fact]
    public void Constructor_NoKeyFields_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new ImovelObra(null, null, null));
    }

    [Fact]
    public void Setters_AssignValues()
    {
        var t = new ImovelObra(new CadastroImovel("ABCDEFGH"));

        t.IdentificacaoObra = new IdentificacaoObra(new string('B', 30));
        t.Endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("RUA"),
            new NumeroEndereco("1"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );
        t.InscricaoImobiliaria = new InscricaoImobiliaria("INSCR");

        Assert.NotNull(t.IdentificacaoObra);
        Assert.NotNull(t.Endereco);
        Assert.Equal("INSCR", t.InscricaoImobiliaria?.ToString());
    }

    [Fact]
    public void Xml_SerializeDeserialize_RoundTrip()
    {
        var cadastro = new CadastroImovel("ABCDEFGH");
        var id = new IdentificacaoObra(new string('A', 30));
        var end = new EnderecoSimplesIBSCBS(
            new Logradouro("RUA"),
            new NumeroEndereco("1"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );

        var original = new ImovelObra(cadastro, id, end, new InscricaoImobiliaria("INSCR"));

        var serializer = new XmlSerializer(typeof(ImovelObra));
        var namespaces = new XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, namespaces);
            xml = sw.ToString();
        }

        ImovelObra? round;
        using (var sr = new StringReader(xml))
        {
            round = (ImovelObra?)serializer.Deserialize(sr);
        }

        Assert.NotNull(round);
        Assert.Equal(original.CadastroImovel?.ToString(), round?.CadastroImovel?.ToString());
        Assert.Equal(original.IdentificacaoObra?.ToString(), round?.IdentificacaoObra?.ToString());
        Assert.Equal(original.Endereco?.Logradouro?.ToString(), round?.Endereco?.Logradouro?.ToString());
    }
}