using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Builders;

public class IntermediarioBuilderTests
{
    [Fact]
    public void New_WithValidCpf_ReturnsBuilder()
    {
        var cpf = new Cpf(12345678909L);
        var builder = IntermediarioBuilder.New(cpf, true);
        Assert.NotNull(builder);
        Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void New_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;
        Assert.Throws<ArgumentNullException>(() => IntermediarioBuilder.New(cpf!, true));
    }

    [Fact]
    public void New_WithValidCnpjAndInscricaoMunicipal_ReturnsBuilder()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var im = new InscricaoMunicipal("12345678");
        var builder = IntermediarioBuilder.New(cnpj, false, im);
        Assert.NotNull(builder);
        Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void New_WithValidCnpjAndNullInscricaoMunicipal_ReturnsBuilder()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var builder = IntermediarioBuilder.New(cnpj, true, null);
        Assert.NotNull(builder);
        Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void New_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;
        Assert.Throws<ArgumentNullException>(() => IntermediarioBuilder.New(cnpj!, false, null));
    }

    [Fact]
    public void SetEmail_WithValidEmail_ReturnsBuilder()
    {
        var builder = IntermediarioBuilder.New(new Cpf(12345678909L), true)
            .SetEmail(new Email("intermediario@teste.com.br"));
        Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void SetEmail_WithNullEmail_ThrowsArgumentNullException()
    {
        var builder = IntermediarioBuilder.New(new Cpf(12345678909L), true);
        Email? email = null;
        Assert.Throws<ArgumentNullException>(() => builder.SetEmail(email!));
    }

    [Fact]
    public void SetEmail_MultipleCalls_LastEmailWins()
    {
        var email1 = new Email("primeiro@teste.com.br");
        var email2 = new Email("segundo@teste.com.br");
        var intermediario = IntermediarioBuilder.New(new Cpf(12345678909L), true)
            .SetEmail(email1)
            .SetEmail(email2)
            .Build();
        Assert.Equal(email2.ToString(), intermediario.EmailIntermediario?.ToString());
    }

    [Fact]
    public void Build_WithCpfOnly_BuildIncludesCpfAndNullInscricaoAndEmail()
    {
        var intermediario = IntermediarioBuilder.New(new Cpf(12345678909L), true).Build();
        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    [Fact]
    public void Build_WithCnpjOnly_BuildIncludesCnpjAndNullInscricaoAndEmail()
    {
        var intermediario = IntermediarioBuilder.New((Cnpj)TestConstants.ValidFormattedCnpj, true, null).Build();
        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    [Fact]
    public void Build_WithCpfAndEmail_AllPropertiesSet()
    {
        var email = new Email("intermediario@teste.com.br");
        var intermediario = IntermediarioBuilder.New(new Cpf(12345678909L), true)
            .SetEmail(email)
            .Build();
        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.True(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Fact]
    public void Build_WithCnpjAndEmail_AllPropertiesSet()
    {
        var cnpj = (Cnpj)TestConstants.ValidFormattedCnpj;
        var email = new Email("intermediario@teste.com.br");
        var im = new InscricaoMunicipal("12345678");
        var intermediario = IntermediarioBuilder.New(cnpj, false, im)
            .SetEmail(email)
            .Build();
        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.NotNull(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.False(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
        Assert.Equal(im.ToString(), intermediario.InscricaoMunicipalIntermediario.ToString());
    }

    [Fact]
    public void Build_WithCpf_IssRetidoTrue_CorrectlySet()
    {
        var intermediario = IntermediarioBuilder.New(new Cpf(12345678909L), true).Build();
        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void Build_WithCpf_IssRetidoFalse_CorrectlySet()
    {
        var intermediario = IntermediarioBuilder.New(new Cpf(12345678909L), false).Build();
        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void Build_WithCnpj_IssRetidoTrue_CorrectlySet()
    {
        var intermediario = IntermediarioBuilder.New((Cnpj)TestConstants.ValidFormattedCnpj, true, null).Build();
        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void Build_WithCnpj_IssRetidoFalse_CorrectlySet()
    {
        var intermediario = IntermediarioBuilder.New((Cnpj)TestConstants.ValidFormattedCnpj, false, null).Build();
        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void IsValid_WithCpfBuilder_ReturnsTrue()
    {
        Assert.True(IntermediarioBuilder.New(new Cpf(12345678909L), true).IsValid());
    }

    [Fact]
    public void IsValid_WithCnpjBuilder_ReturnsTrue()
    {
        Assert.True(IntermediarioBuilder.New((Cnpj)TestConstants.ValidFormattedCnpj, true, null).IsValid());
    }

    [Fact]
    public void GetValidationErrors_WithCpfBuilder_ReturnsEmpty()
    {
        var errors = IntermediarioBuilder.New(new Cpf(12345678909L), true)
            .GetValidationErrors()
            .ToList();
        Assert.Empty(errors);
    }

    [Fact]
    public void GetValidationErrors_WithCnpjBuilder_ReturnsEmpty()
    {
        var errors = IntermediarioBuilder.New((Cnpj)TestConstants.ValidFormattedCnpj, true, null)
            .GetValidationErrors()
            .ToList();
        Assert.Empty(errors);
    }
}