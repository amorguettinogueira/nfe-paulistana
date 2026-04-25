using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para a classe <see cref="CpfOrCnpjOrNif"/>.
/// </summary>
public sealed class CpfOrCnpjOrNifTests
{
    [Fact]
    public void DefaultConstructor_AllPropertiesNull()
    {
        var doc = new CpfOrCnpjOrNif();
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void Constructor_WithCpf_SetsCpfAndLeavesOthersNull()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = new CpfOrCnpjOrNif(cpf);
        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void Constructor_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;
        Action act = () => _ = new CpfOrCnpjOrNif(cpf!);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithCnpj_SetsCnpjAndLeavesOthersNull()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var doc = new CpfOrCnpjOrNif(cnpj);
        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void Constructor_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;
        Action act = () => _ = new CpfOrCnpjOrNif(cnpj!);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithNif_SetsNifAndLeavesOthersNull()
    {
        var nif = new Nif("1234567890");
        var doc = new CpfOrCnpjOrNif(nif);
        Assert.Equal(nif, doc.Nif);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void Constructor_WithNullNif_ThrowsArgumentNullException()
    {
        Nif? nif = null;
        Action act = () => _ = new CpfOrCnpjOrNif(nif!);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_WithMotivoNifNaoInformado_SetsMotivoAndLeavesOthersNull()
    {
        const MotivoNifNaoInformado motivo = MotivoNifNaoInformado.Dispensado;
        var doc = new CpfOrCnpjOrNif(motivo);
        Assert.Equal(motivo, doc.MotivoNifNaoInformado);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
    }

    [Fact]
    public void ToString_ReturnsCpfWhenSet()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = new CpfOrCnpjOrNif(cpf);
        Assert.Equal(cpf.ToString(), doc.ToString());
    }

    [Fact]
    public void ToString_ReturnsCnpjWhenSetAndCpfNull()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var doc = new CpfOrCnpjOrNif(cnpj);
        Assert.Equal(TestConstants.ValidUnformattedCnpj, doc.ToString());
    }

    [Fact]
    public void ToString_ReturnsNifWhenSetAndCpfCnpjNull()
    {
        var nif = new Nif("1234567890");
        var doc = new CpfOrCnpjOrNif(nif);
        Assert.Equal(nif.ToString(), doc.ToString());
    }

    [Fact]
    public void ToString_ReturnsMotivoWhenSetAndOthersNull()
    {
        const MotivoNifNaoInformado motivo = MotivoNifNaoInformado.SemExigencia;
        var doc = new CpfOrCnpjOrNif(motivo);
        Assert.Equal(motivo.ToString(), doc.ToString());
    }

    [Fact]
    public void FromCpf_StaticMethod_CreatesInstanceWithCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = CpfOrCnpjOrNif.FromCpf(cpf);
        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void FromCnpj_StaticMethod_CreatesInstanceWithCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var doc = CpfOrCnpjOrNif.FromCnpj(cnpj);
        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void FromNif_StaticMethod_CreatesInstanceWithNif()
    {
        var nif = new Nif("1234567890");
        var doc = CpfOrCnpjOrNif.FromNif(nif);
        Assert.Equal(nif, doc.Nif);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void FromMotivoNifNaoInformado_StaticMethod_CreatesInstanceWithMotivo()
    {
        const MotivoNifNaoInformado motivo = MotivoNifNaoInformado.NaoInformadoNaOrigem;
        var doc = CpfOrCnpjOrNif.FromMotivoNifNaoInformado(motivo);
        Assert.Equal(motivo, doc.MotivoNifNaoInformado);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
    }

    [Fact]
    public void ExplicitOperator_FromCpf_CreatesInstanceWithCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = (CpfOrCnpjOrNif)cpf;
        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void ExplicitOperator_FromCnpj_CreatesInstanceWithCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var doc = (CpfOrCnpjOrNif)cnpj;
        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Nif);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void ExplicitOperator_FromNif_CreatesInstanceWithNif()
    {
        var nif = new Nif("1234567890");
        var doc = (CpfOrCnpjOrNif)nif;
        Assert.Equal(nif, doc.Nif);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.MotivoNifNaoInformado);
    }

    [Fact]
    public void ExplicitOperator_FromMotivoNifNaoInformado_CreatesInstanceWithMotivo()
    {
        const MotivoNifNaoInformado motivo = MotivoNifNaoInformado.Dispensado;
        var doc = (CpfOrCnpjOrNif)motivo;
        Assert.Equal(motivo, doc.MotivoNifNaoInformado);
        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
        Assert.Null(doc.Nif);
    }
}