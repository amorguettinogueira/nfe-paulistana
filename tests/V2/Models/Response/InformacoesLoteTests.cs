using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Models.Response;

/// <summary>
/// Testes unitários para <see cref="InformacoesLote"/> (V2).
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
        var lote = new InformacoesLote { NumeroLote = "99887766" };

        Assert.Equal("99887766", lote.NumeroLote);
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
        var cpfCnpj = new CpfCnpj { Cnpj = "XA412263000170" };
        var lote = new InformacoesLote { CpfCnpjRemetente = cpfCnpj };

        Assert.Equal("XA412263000170", lote.CpfCnpjRemetente?.Cnpj);
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
        var lote = new InformacoesLote { QtdNotasProcessadas = 10L };

        Assert.Equal(10L, lote.QtdNotasProcessadas);
    }

    [Fact]
    public void TempoProcessamento_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { TempoProcessamento = 850L };

        Assert.Equal(850L, lote.TempoProcessamento);
    }

    [Fact]
    public void ValorTotalServicos_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { ValorTotalServicos = 25000.00m };

        Assert.Equal(25000.00m, lote.ValorTotalServicos);
    }

    [Fact]
    public void ValorTotalDeducoes_SetGet_PropagaValor()
    {
        var lote = new InformacoesLote { ValorTotalDeducoes = 1200.75m };

        Assert.Equal(1200.75m, lote.ValorTotalDeducoes);
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
