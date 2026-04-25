using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CpfOrCnpj"/> (V2).
/// </summary>
public sealed class CpfOrCnpjTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var doc = new CpfOrCnpj();

        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    // ============================================
    // Construtor com CPF
    // ============================================

    [Fact]
    public void Constructor_ComCpf_DefiniCpfEDeixaCnpjNulo()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Fact]
    public void Constructor_ComCpfNulo_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => new CpfOrCnpj(cpf!));
    }

    // ============================================
    // Construtor com CNPJ
    // ============================================

    [Fact]
    public void Constructor_ComCnpj_DefiniCnpjEDeixaCpfNulo()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;

        var doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    [Fact]
    public void Constructor_ComCnpjNulo_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => new CpfOrCnpj(cnpj!));
    }

    // ============================================
    // ToString
    // ============================================

    [Fact]
    public void ToString_ComCpf_RetornaRepresentacaoCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf.ToString(), doc.ToString());
    }

    [Fact]
    public void ToString_ComCnpj_RetornaRepresentacaoCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj.ToString(), doc.ToString());
    }

    [Fact]
    public void ToString_SemDocumento_RetornaNull() =>
        Assert.Null(new CpfOrCnpj().ToString());

    // ============================================
    // Fábricas estáticas
    // ============================================

    [Fact]
    public void FromCpf_CriaCpfOrCnpjComCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var doc = CpfOrCnpj.FromCpf(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Fact]
    public void FromCnpj_CriaCpfOrCnpjComCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;

        var doc = CpfOrCnpj.FromCnpj(cnpj);

        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    // ============================================
    // Operadores explícitos
    // ============================================

    [Fact]
    public void ExplicitCast_Cpf_EhEquivalenteAFromCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var viaFactory = CpfOrCnpj.FromCpf(cpf);
        var viaCast = (CpfOrCnpj)cpf;

        Assert.Equal(viaFactory.Cpf, viaCast.Cpf);
    }

    [Fact]
    public void ExplicitCast_Cnpj_EhEquivalenteAFromCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;

        var viaFactory = CpfOrCnpj.FromCnpj(cnpj);
        var viaCast = (CpfOrCnpj)cnpj;

        Assert.Equal(viaFactory.Cnpj, viaCast.Cnpj);
    }

    // ============================================
    // ICpfOrCnpj — implementação explícita
    // ============================================

    [Fact]
    public void ICpfOrCnpj_Cpf_RetornaStringCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        ICpfOrCnpj doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf.ToString(), doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Fact]
    public void ICpfOrCnpj_Cnpj_RetornaStringCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        ICpfOrCnpj doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj.ToString(), doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    [Fact]
    public void ICpfOrCnpj_SemDocumento_AmbosSaoNull()
    {
        ICpfOrCnpj doc = new CpfOrCnpj();

        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    // ============================================
    // Tipo
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CpfOrCnpj).IsSealed);
}