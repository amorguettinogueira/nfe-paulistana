using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CabecalhoInformacoesLote"/> (V2).
/// </summary>
public sealed class CabecalhoInformacoesLoteTests
{
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
    public void DefaultConstructor_VersaoEhDois()
    {
        var cabecalho = new CabecalhoInformacoesLote();

        Assert.Equal(2, cabecalho.Versao);
    }

    // ============================================
    // Construtor com CpfOrCnpj
    // ============================================

    [Fact]
    public void Constructor_ComCpf_DefineCpfOrCnpj()
    {
        var cpfOrCnpj = (CpfOrCnpj)(Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var cabecalho = new CabecalhoInformacoesLote(cpfOrCnpj);

        Assert.Same(cpfOrCnpj, cabecalho.CpfOrCnpj);
    }

    [Fact]
    public void Constructor_ComCnpj_DefineCpfOrCnpj()
    {
        var cpfOrCnpj = (CpfOrCnpj)(Cnpj)TestConstants.ValidFormattedCnpj;

        var cabecalho = new CabecalhoInformacoesLote(cpfOrCnpj);

        Assert.Same(cpfOrCnpj, cabecalho.CpfOrCnpj);
    }

    [Fact]
    public void Constructor_ComCpfOrCnpj_OutrasPropriedadesNulas()
    {
        var cabecalho = new CabecalhoInformacoesLote((CpfOrCnpj)(Cpf)Tests.Helpers.TestConstants.ValidCpf);

        Assert.Null(cabecalho.NumeroLote);
        Assert.Null(cabecalho.InscricaoPrestador);
    }

    // ============================================
    // Propriedades opcionais
    // ============================================

    [Fact]
    public void NumeroLote_QuandoDefinido_RetornaValorCorreto()
    {
        var numero = new Numero(42);
        var cabecalho = new CabecalhoInformacoesLote { NumeroLote = numero };

        Assert.Same(numero, cabecalho.NumeroLote);
    }

    [Fact]
    public void InscricaoPrestador_QuandoDefinida_RetornaValorCorreto()
    {
        var im = new InscricaoMunicipal(39_616_924L);
        var cabecalho = new CabecalhoInformacoesLote { InscricaoPrestador = im };

        Assert.Same(im, cabecalho.InscricaoPrestador);
    }

    // ============================================
    // Versão e tipo
    // ============================================

    [Fact]
    public void Versao_EhSempre2() =>
        Assert.Equal(2, new CabecalhoInformacoesLote((CpfOrCnpj)(Cpf)Tests.Helpers.TestConstants.ValidCpf).Versao);

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CabecalhoInformacoesLote).IsSealed);
}