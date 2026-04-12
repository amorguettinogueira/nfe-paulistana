using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.Tests.V1.Models.Response;

/// <summary>
/// Testes unitários para <see cref="InformacoesLote"/> (V1).
/// </summary>
public sealed class InformacoesLoteTests
{
    [Fact]
    public void DefaultConstructor_PropriedadesComValoresPadrao()
    {
        var lote = new InformacoesLote();

        Assert.Null(lote.NumeroLote);
        Assert.Null(lote.InscricaoPrestador);
        Assert.Null(lote.CpfCnpjRemetente);
        Assert.Equal(default, lote.DataEnvioLote);
        Assert.Equal(0L, lote.QtdNotasProcessadas);
        Assert.Equal(0L, lote.TempoProcessamento);
        Assert.Equal(0m, lote.ValorTotalServicos);
        Assert.Null(lote.ValorTotalDeducoes);
    }

    [Fact]
    public void NumeroLote_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { NumeroLote = "12345" };

        Assert.Equal("12345", lote.NumeroLote);
    }

    [Fact]
    public void InscricaoPrestador_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { InscricaoPrestador = "39616924" };

        Assert.Equal("39616924", lote.InscricaoPrestador);
    }

    [Fact]
    public void CpfCnpjRemetente_SetGet_PropagaValor()
    {
        var cpfCnpj = new CpfCnpj { Cpf = "46381819618" };
        var lote = new InformacoesLote { CpfCnpjRemetente = cpfCnpj };

        Assert.Equal("46381819618", lote.CpfCnpjRemetente?.Cpf);
    }

    [Fact]
    public void DataEnvioLote_SetGet_PropagaValor()
    {
        var data = new DateTime(2024, 6, 15, 10, 30, 0);
        var lote = new InformacoesLote { DataEnvioLote = data };

        Assert.Equal(data, lote.DataEnvioLote);
    }

    [Fact]
    public void QtdNotasProcessadas_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { QtdNotasProcessadas = 42L };

        Assert.Equal(42L, lote.QtdNotasProcessadas);
    }

    [Fact]
    public void TempoProcessamento_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { TempoProcessamento = 1500L };

        Assert.Equal(1500L, lote.TempoProcessamento);
    }

    [Fact]
    public void ValorTotalServicos_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { ValorTotalServicos = 9999.99m };

        Assert.Equal(9999.99m, lote.ValorTotalServicos);
    }

    [Fact]
    public void ValorTotalDeducoes_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { ValorTotalDeducoes = 500.50m };

        Assert.Equal(500.50m, lote.ValorTotalDeducoes);
    }

    [Fact]
    public void ValorTotalDeducoes_NaoDefinido_EhNull()
    {
        var lote = new InformacoesLote();

        Assert.Null(lote.ValorTotalDeducoes);
    }

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(InformacoesLote).IsSealed);
}
