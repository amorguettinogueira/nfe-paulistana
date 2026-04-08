using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.Tests.V1.Models.Response;

/// <summary>
/// Testes unitários para <see cref="EnderecoNfe"/>.
/// </summary>
public sealed class EnderecoNfeTests
{
    private static string SerializarParaXml(EnderecoNfe obj)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EnderecoNfe));
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, obj);
        return sw.ToString();
    }

    private static EnderecoNfe? DesserializarDeXml(string xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EnderecoNfe));
        using var sr = new System.IO.StringReader(xml);
        return (EnderecoNfe?)serializer.Deserialize(sr);
    }

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new EnderecoNfe());

    [Fact]
    public void Propriedades_QuandoDefinidas_RetornamMesmosValores()
    {
        var endereco = new EnderecoNfe
        {
            TipoLogradouro = "Rua",
            Logradouro = "Paulista",
            NumeroEndereco = "123",
            ComplementoEndereco = "Apto 1",
            Bairro = "Bela Vista",
            Cidade = "3550308",
            Uf = "SP",
            Cep = "01310100"
        };

        Assert.Equal("Rua", endereco.TipoLogradouro);
        Assert.Equal("Paulista", endereco.Logradouro);
        Assert.Equal("123", endereco.NumeroEndereco);
        Assert.Equal("Apto 1", endereco.ComplementoEndereco);
        Assert.Equal("Bela Vista", endereco.Bairro);
        Assert.Equal("3550308", endereco.Cidade);
        Assert.Equal("SP", endereco.Uf);
        Assert.Equal("01310100", endereco.Cep);
    }

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamNull()
    {
        var endereco = new EnderecoNfe();

        Assert.Null(endereco.TipoLogradouro);
        Assert.Null(endereco.Logradouro);
        Assert.Null(endereco.NumeroEndereco);
        Assert.Null(endereco.ComplementoEndereco);
        Assert.Null(endereco.Bairro);
        Assert.Null(endereco.Cidade);
        Assert.Null(endereco.Uf);
        Assert.Null(endereco.Cep);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaTodasAsPropriedades()
    {
        // Arrange
        var endereco = new EnderecoNfe
        {
            TipoLogradouro = "Rua",
            Logradouro = "Paulista",
            NumeroEndereco = "123",
            ComplementoEndereco = "Apto 1",
            Bairro = "Bela Vista",
            Cidade = "3550308",
            Uf = "SP",
            Cep = "01310100"
        };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(endereco))!;

        // Assert
        Assert.Equal(endereco.TipoLogradouro, desserializado.TipoLogradouro);
        Assert.Equal(endereco.Logradouro, desserializado.Logradouro);
        Assert.Equal(endereco.NumeroEndereco, desserializado.NumeroEndereco);
        Assert.Equal(endereco.ComplementoEndereco, desserializado.ComplementoEndereco);
        Assert.Equal(endereco.Bairro, desserializado.Bairro);
        Assert.Equal(endereco.Cidade, desserializado.Cidade);
        Assert.Equal(endereco.Uf, desserializado.Uf);
        Assert.Equal(endereco.Cep, desserializado.Cep);
    }

    [Fact]
    public void XmlSerialization_UfECepUsamElementosEmMaiusculas()
    {
        // Arrange
        var endereco = new EnderecoNfe { Uf = "SP", Cep = "01310100" };

        // Act
        var xml = SerializarParaXml(endereco);

        // Assert
        Assert.Contains("<UF>SP</UF>", xml);
        Assert.Contains("<CEP>01310100</CEP>", xml);
    }
}
