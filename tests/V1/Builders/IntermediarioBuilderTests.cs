using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

public partial class IntermediarioBuilderTests
{
    // ============================================
    // New() — Null Value Object Tests
    // ============================================

    [Fact()]
    public void New_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => IntermediarioBuilder.New(cpf!, true));
    }

    [Fact()]
    public void New_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => IntermediarioBuilder.New(cnpj!, true, null));
    }

    // ============================================
    // SetEmail — Null and Invalid Value Object Tests
    // ============================================

    [Fact]
    public void SetEmail_WithNullEmail_ThrowsArgumentNullException()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true);
        Email? email = null;

        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEmail(email!));
    }

    [Fact]
    public void SetEmail_WithInvalidEmailFormat_ThrowsArgumentException()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true);

        _ = Assert.Throws<ArgumentException>(() => builder.SetEmail(new Email("formato-invalido")));
    }

    // ============================================
    // New() Factory Method Tests
    // ============================================

    [Fact]
    public void New_WithCpfDomainObject_ReturnsBuilder()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true);

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void New_WithCnpjDomainObject_ReturnsBuilder()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null);

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Fact]
    public void New_WithCnpjAndInscricao_ReturnsBuilder()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, new InscricaoMunicipal(98765432));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    // ============================================
    // Build Output Tests
    // ============================================

    [Fact]
    public void New_WithCpfOnly_BuildIncludesCpfAndNullInscricao()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true).Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    [Fact]
    public void New_WithCnpjOnly_BuildIncludesCnpjAndNullInscricao()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null).Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    // ============================================
    // IssRetidoIntermediario Flag Tests
    // ============================================

    [Fact]
    public void New_WithCpf_IssRetidoTrue_CorrectlySet()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true).Build();

        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void New_WithCpf_IssRetidoFalse_CorrectlySet()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, false).Build();

        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void New_WithCnpj_IssRetidoTrue_CorrectlySet()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null).Build();

        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void New_WithCnpj_IssRetidoFalse_CorrectlySet()
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, false, null).Build();

        Assert.False(intermediario.IssRetidoIntermediario);
    }

    // ============================================
    // SetEmail Tests
    // ============================================

    [Fact]
    public void SetEmail_OnCpfBuilder_ReturnsBuilder()
    {
        IIntermediarioBuilder result = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true)
            .SetEmail(new Email("intermediario@teste.com.br"));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(result);
    }

    [Fact]
    public void SetEmail_OnCpfBuilder_BuildIncludesEmail()
    {
        var email = new Email("intermediario@teste.com.br");
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Fact]
    public void SetEmail_OnCnpjBuilder_ReturnsBuilder()
    {
        IIntermediarioBuilder result = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null)
            .SetEmail(new Email("intermediario@teste.com.br"));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(result);
    }

    [Fact]
    public void SetEmail_MultipleCalls_LastEmailWins()
    {
        var email2 = new Email("segundo@teste.com.br");
        Intermediario intermediario = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null)
            .SetEmail(new Email("primeiro@teste.com.br"))
            .SetEmail(email2)
            .Build();

        Assert.Equal(email2.ToString(), intermediario.EmailIntermediario?.ToString());
    }

    // ============================================
    // Complete Chain Tests
    // ============================================

    [Fact]
    public void Build_WithCpfAndEmail_AllPropertiesSet()
    {
        var email = new Email("intermediario@teste.com.br");

        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.True(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Fact]
    public void Build_WithCpfAndEmailIssRetidoFalse_AllPropertiesSet()
    {
        var email = new Email("intermediario@teste.com.br");

        Intermediario intermediario = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, false)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Fact]
    public void New_WithCpfAndEmailInDomainObjectsMode_AllPropertiesSet()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var email = new Email("intermediario@teste.com.br");

        Intermediario intermediario = IntermediarioBuilder.New(cpf, true)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.True(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Fact]
    public void New_WithCnpjAndEmailInDomainObjectsMode_AllPropertiesSet()
    {
        var cnpj = (Cnpj)Helpers.TestConstants.ValidCnpj;
        var email = new Email("intermediario@teste.com.br");
        var inscricao = new InscricaoMunicipal(1234123);

        Intermediario intermediario = IntermediarioBuilder.New(cnpj, false, inscricao)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.False(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
        Assert.Equal(inscricao.ToString(), intermediario.InscricaoMunicipalIntermediario.ToString());
    }

    // ============================================
    // IsValid / GetValidationErrors Tests
    // ============================================

    [Fact]
    public void IsValid_WithCpfBuilder_ReturnsTrue() =>
        Assert.True(IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true).IsValid());

    [Fact]
    public void IsValid_WithCnpjBuilder_ReturnsTrue() =>
        Assert.True(IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null).IsValid());

    [Fact]
    public void GetValidationErrors_WithCpfBuilder_ReturnsEmpty()
    {
        var errors = IntermediarioBuilder.New((Cpf)Tests.Helpers.TestConstants.ValidCpf, true)
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    [Fact]
    public void GetValidationErrors_WithCnpjBuilder_ReturnsEmpty()
    {
        var errors = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null)
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    [Fact]
    public void PreBuildValidation_WithValidIdentifier_AllowsBuild()
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New((Cnpj)Helpers.TestConstants.ValidCnpj, true, null);

        bool isValid = builder.IsValid();
        var errors = builder.GetValidationErrors().ToList();
        Intermediario? intermediario = isValid ? builder.Build() : null;

        Assert.True(isValid);
        Assert.Empty(errors);
        Assert.NotNull(intermediario);
    }
}