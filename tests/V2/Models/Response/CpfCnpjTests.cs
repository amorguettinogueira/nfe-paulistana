using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Models.Response;

/// <summary>
/// Testes unitários para <see cref="CpfCnpj"/>.
/// </summary>
public sealed class CpfCnpjTests
{
    private static string SerializarParaXml(CpfCnpj obj)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CpfCnpj));
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, obj);
        return sw.ToString();
    }

    private static CpfCnpj? DesserializarDeXml(string xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CpfCnpj));
        using var sr = new System.IO.StringReader(xml);
        return (CpfCnpj?)serializer.Deserialize(sr);
    }

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new CpfCnpj());

    [Fact]
    public void Cpf_QuandoDefinido_RetornaMesmoValor()
    {
        var doc = new CpfCnpj { Cpf = "12345678901" };

        Assert.Equal("12345678901", doc.Cpf);
    }

    [Fact]
    public void Cnpj_QuandoDefinido_RetornaMesmoValor()
    {
        var doc = new CpfCnpj { Cnpj = "12345678000190" };

        Assert.Equal("12345678000190", doc.Cnpj);
    }

    [Fact]
    public void ToString_QuandoApenasCpfDefinido_RetornaCpf() =>
        Assert.Equal("12345678901", new CpfCnpj { Cpf = "12345678901" }.ToString());

    [Fact]
    public void ToString_QuandoApenasOCnpjDefinido_RetornaCnpj() =>
        Assert.Equal("12345678000190", new CpfCnpj { Cnpj = "12345678000190" }.ToString());

    [Fact]
    public void ToString_QuandoCpfECnpjDefinidos_RetornaCpf() =>
        Assert.Equal("12345678901", new CpfCnpj { Cpf = "12345678901", Cnpj = "12345678000190" }.ToString());

    [Fact]
    public void ToString_QuandoNenhumDefinido_RetornaNull() =>
        Assert.Null(new CpfCnpj().ToString());

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaCpf()
    {
        // Arrange
        var doc = new CpfCnpj { Cpf = "12345678901" };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(doc))!;

        // Assert
        Assert.Equal(doc.Cpf, desserializado.Cpf);
        Assert.Null(desserializado.Cnpj);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaCnpj()
    {
        // Arrange
        var doc = new CpfCnpj { Cnpj = "12345678000190" };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(doc))!;

        // Assert
        Assert.Equal(doc.Cnpj, desserializado.Cnpj);
        Assert.Null(desserializado.Cpf);
    }

    [Fact]
    public void XmlSerialization_CpfECnpjUsamElementosEmMaiusculas()
    {
        // Arrange
        var doc = new CpfCnpj { Cpf = "12345678901", Cnpj = "12345678000190" };

        // Act
        var xml = SerializarParaXml(doc);

        // Assert
        Assert.Contains("<CPF>12345678901</CPF>", xml);
        Assert.Contains("<CNPJ>12345678000190</CNPJ>", xml);
    }
}
