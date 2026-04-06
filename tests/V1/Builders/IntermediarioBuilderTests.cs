using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
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

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_WithNullEmail_ThrowsArgumentNullException(long cpfNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cpf(cpfNumber), true);
        Email? email = null;

        _ = Assert.Throws<ArgumentNullException>(() => builder.SetEmail(email!));
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_WithInvalidEmailFormat_ThrowsArgumentException(long cpfNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cpf(cpfNumber), true);

        _ = Assert.Throws<ArgumentException>(() => builder.SetEmail(new Email("formato-invalido")));
    }

    // ============================================
    // New() Factory Method Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void New_WithCpfDomainObject_ReturnsBuilder(long cpfNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cpf(cpfNumber), true);

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpjDomainObject_ReturnsBuilder(long cnpjNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null);

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpjAndInscricao_ReturnsBuilder(long cnpjNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, new InscricaoMunicipal(98765432));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(builder);
    }

    // ============================================
    // Build Output Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void New_WithCpfOnly_BuildIncludesCpfAndNullInscricao(long cpfNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), true).Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpjOnly_BuildIncludesCnpjAndNullInscricao(long cnpjNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null).Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    // ============================================
    // IssRetidoIntermediario Flag Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void New_WithCpf_IssRetidoTrue_CorrectlySet(long cpfNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), true).Build();

        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void New_WithCpf_IssRetidoFalse_CorrectlySet(long cpfNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), false).Build();

        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpj_IssRetidoTrue_CorrectlySet(long cnpjNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null).Build();

        Assert.True(intermediario.IssRetidoIntermediario);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpj_IssRetidoFalse_CorrectlySet(long cnpjNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New(new Cnpj(cnpjNumber), false, null).Build();

        Assert.False(intermediario.IssRetidoIntermediario);
    }

    // ============================================
    // SetEmail Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_OnCpfBuilder_ReturnsBuilder(long cpfNumber)
    {
        IIntermediarioBuilder result = IntermediarioBuilder.New(new Cpf(cpfNumber), true)
            .SetEmail(new Email("intermediario@teste.com.br"));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(result);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_OnCpfBuilder_BuildIncludesEmail(long cpfNumber)
    {
        var email = new Email("intermediario@teste.com.br");
        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), true)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void SetEmail_OnCnpjBuilder_ReturnsBuilder(long cnpjNumber)
    {
        IIntermediarioBuilder result = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null)
            .SetEmail(new Email("intermediario@teste.com.br"));

        _ = Assert.IsAssignableFrom<IIntermediarioBuilder>(result);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void SetEmail_MultipleCalls_LastEmailWins(long cnpjNumber)
    {
        var email2 = new Email("segundo@teste.com.br");
        Intermediario intermediario = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null)
            .SetEmail(new Email("primeiro@teste.com.br"))
            .SetEmail(email2)
            .Build();

        Assert.Equal(email2.ToString(), intermediario.EmailIntermediario?.ToString());
    }

    // ============================================
    // Complete Chain Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithCpfAndEmail_AllPropertiesSet(long cpfNumber)
    {
        var email = new Email("intermediario@teste.com.br");

        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), true)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.True(intermediario.IssRetidoIntermediario);
        Assert.Equal(email.ToString(), intermediario.EmailIntermediario.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithCpfAndEmailIssRetidoFalse_AllPropertiesSet(long cpfNumber)
    {
        var email = new Email("intermediario@teste.com.br");

        Intermediario intermediario = IntermediarioBuilder.New(new Cpf(cpfNumber), false)
            .SetEmail(email)
            .Build();

        Assert.NotNull(intermediario.CpfCnpjIntermediario);
        Assert.Null(intermediario.InscricaoMunicipalIntermediario);
        Assert.NotNull(intermediario.EmailIntermediario);
        Assert.False(intermediario.IssRetidoIntermediario);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void New_WithCpfAndEmailInDomainObjectsMode_AllPropertiesSet(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
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

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void New_WithCnpjAndEmailInDomainObjectsMode_AllPropertiesSet(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
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

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void IsValid_WithCpfBuilder_ReturnsTrue(long cpfNumber) =>
        Assert.True(IntermediarioBuilder.New(new Cpf(cpfNumber), true).IsValid());

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void IsValid_WithCnpjBuilder_ReturnsTrue(long cnpjNumber) =>
        Assert.True(IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null).IsValid());

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void GetValidationErrors_WithCpfBuilder_ReturnsEmpty(long cpfNumber)
    {
        var errors = IntermediarioBuilder.New(new Cpf(cpfNumber), true)
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void GetValidationErrors_WithCnpjBuilder_ReturnsEmpty(long cnpjNumber)
    {
        var errors = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null)
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void PreBuildValidation_WithValidIdentifier_AllowsBuild(long cnpjNumber)
    {
        IIntermediarioBuilder builder = IntermediarioBuilder.New(new Cnpj(cnpjNumber), true, null);

        bool isValid = builder.IsValid();
        var errors = builder.GetValidationErrors().ToList();
        Intermediario? intermediario = isValid ? builder.Build() : null;

        Assert.True(isValid);
        Assert.Empty(errors);
        Assert.NotNull(intermediario);
    }
}