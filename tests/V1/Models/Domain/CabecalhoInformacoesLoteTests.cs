using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CabecalhoInformacoesLote"/> (V1).
/// </summary>
public sealed class CabecalhoInformacoesLoteTests
{
    private static CpfOrCnpj CriarCpfOrCnpj() =>
        new(new Cpf(new ValidCpfNumber().Min()));

    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var cabecalho = new CabecalhoInformacoesLote();

        Assert.Null(cabecalho.CpfOrCnpj);
        Assert.Null(cabecalho.NumeroLote);
        Assert.Null(cabecalho.InscricaoPrestador);
    }

    [Fact]
    public void DefaultConstructor_VersaoEhUm()
    {
        var cabecalho = new CabecalhoInformacoesLote();

        Assert.Equal(1, cabecalho.Versao);
    }

    // ============================================
    // Construtor com CpfOrCnpj
    // ============================================

    [Fact]
    public void Constructor_ComCpfOrCnpj_DefineCpfOrCnpj()
    {
        var cpfOrCnpj = CriarCpfOrCnpj();

        var cabecalho = new CabecalhoInformacoesLote(cpfOrCnpj);

        Assert.Same(cpfOrCnpj, cabecalho.CpfOrCnpj);
    }

    [Fact]
    public void Constructor_ComCpfOrCnpj_OutrasPropriedadesNulas()
    {
        var cabecalho = new CabecalhoInformacoesLote(CriarCpfOrCnpj());

        Assert.Null(cabecalho.NumeroLote);
        Assert.Null(cabecalho.InscricaoPrestador);
    }

    // ============================================
    // Propriedades opcionais
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NumeroLote_QuandoDefinido_RetornaValorCorreto(long cnpjNumber)
    {
        var numero = new Numero((int)cnpjNumber % 100_000 + 1);
        var cabecalho = new CabecalhoInformacoesLote { NumeroLote = numero };

        Assert.Same(numero, cabecalho.NumeroLote);
    }

    [Fact]
    public void InscricaoPrestador_QuandoDefinida_RetornaValorCorreto()
    {
        var im = new InscricaoMunicipal(39_616_924);
        var cabecalho = new CabecalhoInformacoesLote { InscricaoPrestador = im };

        Assert.Same(im, cabecalho.InscricaoPrestador);
    }

    // ============================================
    // Versão e tipo
    // ============================================

    [Fact]
    public void Versao_EhSempre1() =>
        Assert.Equal(1, new CabecalhoInformacoesLote(CriarCpfOrCnpj()).Versao);

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CabecalhoInformacoesLote).IsSealed);
}
