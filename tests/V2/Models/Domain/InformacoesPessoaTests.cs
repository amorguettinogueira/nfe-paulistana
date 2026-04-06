using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class InformacoesPessoaTests
{
    [Fact]
    public void DefaultConstructor_InheritsFornecedor_Defaults()
    {
        var p = new InformacoesPessoa();

        Assert.Null(p.Cpf);
        Assert.Null(p.Cnpj);
        Assert.Null(p.Nif);
        Assert.Null(p.Nome);
        Assert.Null(p.Endereco);
        Assert.Null(p.Email);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Constructor_WithCpf_SetsProperties(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var nome = new RazaoSocial("Pessoa");
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Teste"),
            new NumeroEndereco("1"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );
        var email = new Email("test@example.com");

        var p = new InformacoesPessoa(cpf, nome, endereco, email);

        Assert.Equal(cpf, p.Cpf);
        Assert.Equal(nome, p.Nome);
        Assert.Equal(endereco, p.Endereco);
        Assert.Equal(email, p.Email);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Constructor_WithCnpj_SetsProperties(string cnpjFormatted, string _)
    {
        var cnpj = new Cnpj(cnpjFormatted);
        var nome = new RazaoSocial("Empresa");

        var p = new InformacoesPessoa(cnpj, nome);

        Assert.Equal(cnpj, p.Cnpj);
        Assert.Equal(nome, p.Nome);
        Assert.Null(p.Endereco);
        Assert.Null(p.Email);
    }

    [Fact]
    public void Constructor_WithNif_SetsProperties()
    {
        var nif = new Nif("NIF123");
        var nome = new RazaoSocial("Estrangeiro");

        var p = new InformacoesPessoa(nif, nome);

        Assert.Equal(nif, p.Nif);
        Assert.Equal(nome, p.Nome);
    }

    [Fact]
    public void Constructor_WithMotivoNif_SetsProperties()
    {
        var motivo = MotivoNifNaoInformado.Dispensado;
        var nome = new RazaoSocial("SemNif");

        var p = new InformacoesPessoa(motivo, nome);

        Assert.Equal(motivo, p.MotivoNifNaoInformado);
        Assert.Equal(nome, p.Nome);
        Assert.True(p.MotivoNifNaoInformadoSpecified);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Constructor_NullNome_ThrowsArgumentNullException_ForCpf(long cpfNumber)
    {
        RazaoSocial? nome = null;
        var cpf = new Cpf(cpfNumber);

        _ = Assert.Throws<ArgumentNullException>(() => new InformacoesPessoa(cpf, nome!));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Constructor_NullNome_ThrowsArgumentNullException_ForCnpj(string cnpjFormatted, string _)
    {
        RazaoSocial? nome = null;
        var cnpj = new Cnpj(cnpjFormatted);

        Assert.Throws<ArgumentNullException>(() => new InformacoesPessoa(cnpj, nome!));
    }

    [Fact]
    public void Constructor_NullNome_ThrowsArgumentNullException_ForNif()
    {
        RazaoSocial? nome = null;
        var nif = new Nif("123");

        _ = Assert.Throws<ArgumentNullException>(() => new InformacoesPessoa(nif, nome!));
    }

    [Fact]
    public void Setters_AssignValues()
    {
        var p = new InformacoesPessoa();

        p.Endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("L"),
            new NumeroEndereco("2"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );

        p.Email = new Email("a@b.com");

        Assert.NotNull(p.Endereco);
        Assert.Equal("a@b.com", p.Email?.ToString());
    }

    // ==========================
    // XML serialization tests
    // ==========================

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Xml_SerializeAndDeserialize_RoundTripPreservesValues(long cpfNumber)
    {
        // Arrange
        var cpf = new Cpf(cpfNumber);
        var nome = new RazaoSocial("Pessoa XML");
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua XML"),
            new NumeroEndereco("10"),
            new Bairro("Centro"),
            new Cep(12345678),
            null,
            new Complemento("Apto")
        );
        var email = new Email("xml@test.com");

        var original = new InformacoesPessoa(cpf, nome, endereco, email);

        var serializer = new XmlSerializer(typeof(InformacoesPessoa));
        var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, namespaces);
            xml = sw.ToString();
        }

        // Act: deserialize
        InformacoesPessoa? roundtripped;
        using (var sr = new StringReader(xml))
        {
            roundtripped = (InformacoesPessoa?)serializer.Deserialize(sr);
        }

        // Assert
        Assert.NotNull(roundtripped);
        Assert.Equal(original.Cpf?.ToString(), roundtripped?.Cpf?.ToString());
        Assert.Equal(original.Nome?.ToString(), roundtripped?.Nome?.ToString());
        Assert.Equal(original.Email?.ToString(), roundtripped?.Email?.ToString());
        Assert.Equal(original.Endereco?.Logradouro, roundtripped?.Endereco?.Logradouro);
        Assert.Equal(original.Endereco?.Numero, roundtripped?.Endereco?.Numero);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Xml_Serialize_WithoutDefaultNamespaces_WhenEmptyNamespacesProvided(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var p = new InformacoesPessoa(cpf, new RazaoSocial("X"));

        var serializer = new XmlSerializer(typeof(InformacoesPessoa));
        var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, p, namespaces);
            xml = sw.ToString();
        }

        Assert.DoesNotContain("xmlns:xsd", xml);
        Assert.DoesNotContain("xmlns:xsi", xml);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Xml_Deserialize_InvalidCnpj_ThrowsSerializationExceptionInner(string cnpjFormatted, string _)
    {
        // Arrange: build a valid object and XML then corrupt the CNPJ value
        var cnpj = new Cnpj(cnpjFormatted);
        var original = new InformacoesPessoa(cnpj, new RazaoSocial("Empresa XML"));

        var serializer = new XmlSerializer(typeof(InformacoesPessoa));
        var namespaces = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, namespaces);
            xml = sw.ToString();
        }

        // replace the valid CNPJ with an invalid value
        xml = xml.Replace(cnpj.ToString() ?? string.Empty, "INVALIDCNPJ");

        using var sr = new StringReader(xml);

        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}