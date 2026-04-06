using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Builders;

public class EventoBuilderTests
{
    [Fact]
    public void New_WithValidArgs_ReturnsBuilder()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        Assert.NotNull(builder);
        _ = Assert.IsAssignableFrom<IEventoBuilder>(builder);
    }

    [Fact]
    public void New_WithNullArgs_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => EventoBuilder.New(null!, new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2))));
        _ = Assert.Throws<ArgumentNullException>(() => EventoBuilder.New(new NomeEvento("Evento Teste"), null!, new DataXsd(new DateTime(2024, 1, 2))));
        _ = Assert.Throws<ArgumentNullException>(() => EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), null!));
    }

    [Fact]
    public void SetEndereco_WithValidArgs_ReturnsBuilder()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)))
            .SetEndereco(new Cep("01310100"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100"), new Complemento("Apto 1"));
        _ = Assert.IsAssignableFrom<IEventoBuilder>(builder);
    }

    [Fact]
    public void SetEndereco_WithNullArgs_ThrowsArgumentNullException()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEndereco(null!, new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEndereco(new Cep("01310100"), null!, new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEndereco(new Cep("01310100"), new Bairro("Centro"), null!, new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEndereco(new Cep("01310100"), new Bairro("Centro"), new Logradouro("Paulista"), null!));
    }

    [Fact]
    public void SetEnderecoExterior_WithValidArgs_ReturnsBuilder()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)))
            .SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100"), new Complemento("Apto 1"));
        _ = Assert.IsAssignableFrom<IEventoBuilder>(builder);
    }

    [Fact]
    public void SetEnderecoExterior_WithNullArgs_ThrowsArgumentNullException()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(null!, new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), null!, new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), null!, new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), null!, new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), null!, new Logradouro("Paulista"), new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), null!, new NumeroEndereco("100")));
        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), null!));
    }

    [Fact]
    public void IsValid_WithNoFields_ReturnsFalse()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        Assert.False(builder.IsValid());
    }

    [Fact]
    public void GetValidationErrors_WithNoFields_ReturnsErrors()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        var errors = builder.GetValidationErrors().ToList();
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Build_WithValidEndereco_Succeeds()
    {
        AtividadeEvento evento = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)))
            .SetEndereco(new Cep("01310100"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100"), new Complemento("Apto 1"))
            .Build();
        Assert.NotNull(evento);
        Assert.NotNull(evento.EnderecoEvento);
    }

    [Fact]
    public void Build_WithValidEnderecoExterior_Succeeds()
    {
        AtividadeEvento evento = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)))
            .SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"), new Bairro("Centro"), new Logradouro("Paulista"), new NumeroEndereco("100"), new Complemento("Apto 1"))
            .Build();
        Assert.NotNull(evento);
        Assert.NotNull(evento.EnderecoEvento);
        Assert.NotNull(evento.EnderecoEvento.EnderecoExterior);
    }

    [Fact]
    public void Build_WithMissingRequiredFields_ThrowsArgumentException()
    {
        IEventoBuilder builder = EventoBuilder.New(new NomeEvento("Evento Teste"), new DataXsd(new DateTime(2024, 1, 1)), new DataXsd(new DateTime(2024, 1, 2)));
        _ = Assert.Throws<ArgumentException>(() => builder.Build());
    }
}