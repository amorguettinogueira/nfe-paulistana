using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class AtividadeEventoTests
{
    [Fact]
    public void AtividadeEvento_DefaultConstructor_AllPropertiesNull()
    {
        // Arrange & Act
        var atividade = new AtividadeEvento();

        // Assert
        Assert.Null(atividade.NomeEvento);
        Assert.Null(atividade.DataInicioEvento);
        Assert.Null(atividade.DataFimEvento);
        Assert.Null(atividade.EnderecoEvento);
    }

    [Fact]
    public void AtividadeEvento_Constructor_ValidArguments_PropertiesSetCorrectly()
    {
        // Arrange
        var nomeEvento = new NomeEvento("Evento Teste");
        var dataInicio = new DataXsd(new DateTime(2024, 6, 1));
        var dataFim = new DataXsd(new DateTime(2024, 6, 2));
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Teste"),
            new NumeroEndereco("123"),
            new Bairro("Centro"),
            new Cep(12345678),
            null,
            new Complemento("Apto 1")
        );

        // Act
        var atividade = new AtividadeEvento(nomeEvento, dataInicio, dataFim, endereco);

        // Assert
        Assert.Equal(nomeEvento, atividade.NomeEvento);
        Assert.Equal(dataInicio, atividade.DataInicioEvento);
        Assert.Equal(dataFim, atividade.DataFimEvento);
        Assert.Equal(endereco, atividade.EnderecoEvento);
    }

    [Fact]
    public void AtividadeEvento_Constructor_NullNomeEvento_ThrowsArgumentNullException()
    {
        // Arrange
        var dataInicio = new DataXsd(new DateTime(2024, 6, 1));
        var dataFim = new DataXsd(new DateTime(2024, 6, 2));
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Teste"),
            new NumeroEndereco("123"),
            new Bairro("Centro"),
            new Cep(12345678),
            null,
            new Complemento("Apto 1")
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AtividadeEvento(null, dataInicio, dataFim, endereco));
    }

    [Fact]
    public void AtividadeEvento_Constructor_NullDataInicioEvento_ThrowsArgumentNullException()
    {
        // Arrange
        var nomeEvento = new NomeEvento("Evento Teste");
        var dataFim = new DataXsd(new DateTime(2024, 6, 2));
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Teste"),
            new NumeroEndereco("123"),
            new Bairro("Centro"),
            new Cep(12345678),
            null,
            new Complemento("Apto 1")
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AtividadeEvento(nomeEvento, null, dataFim, endereco));
    }

    [Fact]
    public void AtividadeEvento_Constructor_NullDataFimEvento_ThrowsArgumentNullException()
    {
        // Arrange
        var nomeEvento = new NomeEvento("Evento Teste");
        var dataInicio = new DataXsd(new DateTime(2024, 6, 1));
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Teste"),
            new NumeroEndereco("123"),
            new Bairro("Centro"),
            new Cep(12345678),
            null,
            new Complemento("Apto 1")
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AtividadeEvento(nomeEvento, dataInicio, null, endereco));
    }

    [Fact]
    public void AtividadeEvento_Constructor_NullEnderecoEvento_ThrowsArgumentNullException()
    {
        // Arrange
        var nomeEvento = new NomeEvento("Evento Teste");
        var dataInicio = new DataXsd(new DateTime(2024, 6, 1));
        var dataFim = new DataXsd(new DateTime(2024, 6, 2));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AtividadeEvento(nomeEvento, dataInicio, dataFim, null));
    }

    [Fact]
    public void AtividadeEvento_Setters_PropertiesSetCorrectly()
    {
        // Arrange
        var atividade = new AtividadeEvento();
        var nomeEvento = new NomeEvento("Evento Atualizado");
        var dataInicio = new DataXsd(new DateTime(2024, 7, 1));
        var dataFim = new DataXsd(new DateTime(2024, 7, 2));
        var endereco = new EnderecoSimplesIBSCBS(
            new Logradouro("Rua Nova"),
            new NumeroEndereco("456"),
            new Bairro("Bairro Novo"),
            new Cep(87654321),
            null,
            new Complemento("Casa")
        );

        // Act
        atividade.NomeEvento = nomeEvento;
        atividade.DataInicioEvento = dataInicio;
        atividade.DataFimEvento = dataFim;
        atividade.EnderecoEvento = endereco;

        // Assert
        Assert.Equal(nomeEvento, atividade.NomeEvento);
        Assert.Equal(dataInicio, atividade.DataInicioEvento);
        Assert.Equal(dataFim, atividade.DataFimEvento);
        Assert.Equal(endereco, atividade.EnderecoEvento);
    }
}