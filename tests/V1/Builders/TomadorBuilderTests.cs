using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

public class TomadorBuilderTests
{
    private static readonly RazaoSocial RazaoSocialPadrao = new("Empresa de Teste Ltda");
    private static readonly Email EmailPadrao = new("tomador@teste.com.br");

    // ============================================
    // NewCpf() Factory Method Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_WithValidCpfAndRazaoSocial_ReturnsBuilder(long cpfNumber)
    {
        var builder = TomadorBuilder.NewCpf((Cpf)cpfNumber);

        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact()]
    public void NewCpf_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf(cpf!));
    }

    // ============================================
    // NewCnpj() Factory Method Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_WithValidCnpjAndRazaoSocial_ReturnsBuilder(long cnpjNumber)
    {
        var builder = TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), RazaoSocialPadrao);

        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact()]
    public void NewCnpj_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj(cnpj!, RazaoSocialPadrao));
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewCnpj_WithNullRazaoSocial_ThrowsArgumentNullException(long cnpjNumber)
    {
        RazaoSocial? razaoSocial = null;

        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), razaoSocial!));
    }

    // ============================================
    // NewRazaoSocial() Factory Method Tests
    // ============================================

    [Fact()]
    public void NewRazaoSocial_WithValidRazaoSocial_ReturnsBuilder()
    {
        var builder = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao);

        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact()]
    public void NewRazaoSocial_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        RazaoSocial? razaoSocial = null;

        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewRazaoSocial(razaoSocial!));
    }

    // ============================================
    // SetInscricaoMunicipal Tests
    // ============================================

    // ============================================
    // SetInscricaoEstadual Tests
    // ============================================

    // ============================================
    // SetEmail Tests
    // ============================================

    [Fact()]
    public void SetEmail_WithValidDomainObject_ReturnsBuilder()
    {
        var result = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao)
            .SetEmail(EmailPadrao);

        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Fact()]
    public void SetEmail_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Email? email = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao).SetEmail(email!));
    }

    [Fact()]
    public void SetEmail_WithInvalidEmailFormat_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() =>
            TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao).SetEmail(new Email("formato-invalido")));
    }

    // ============================================
    // SetEndereco Tests
    // ============================================

    [Fact()]
    public void SetEndereco_WithValidDomainObject_ReturnsBuilder()
    {
        var endereco = EnderecoBuilder.New().SetUf(new Uf("SP")).Build();

        var result = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao)
            .SetEndereco(endereco);

        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Fact()]
    public void SetEndereco_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Endereco? endereco = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao).SetEndereco(endereco!));
    }

    // ============================================
    // Build() Output Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithCpf_ReturnsTomadorWithCpfAndRazaoSocial(long cpfNumber)
    {
        var tomador = TomadorBuilder.NewCpf((Cpf)cpfNumber).Build();

        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.Null(tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
        Assert.Null(tomador.EmailTomador);
        Assert.Null(tomador.EnderecoTomador);
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Build_WithCnpj_ReturnsTomadorWithCnpjAndRazaoSocial(long cnpjNumber)
    {
        var tomador = TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), RazaoSocialPadrao).Build();

        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
    }

    [Fact()]
    public void Build_WithRazaoSocialOnly_ReturnsTomadorWithNullCpfCnpj()
    {
        var tomador = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao).Build();

        Assert.NotNull(tomador);
        Assert.Null(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
    }

    // ============================================
    // Fluent Chain Tests
    // ============================================

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithCompleteChain_AllPropertiesSet(long cpfNumber)
    {
        var endereco = EnderecoBuilder.New()
            .SetUf(new Uf("SP"))
            .SetCodigoIbge(new CodigoIbge(3550308))
            .SetBairro(new Bairro("Bela Vista"))
            .SetCep(new Cep("01310100"))
            .SetTipo(new TipoLogradouro("Av"))
            .SetLogradouro(new Logradouro("Paulista"))
            .SetNumero(new NumeroEndereco("1578"))
            .Build();

        var tomador = TomadorBuilder.NewCpf((Cpf)cpfNumber)
            .SetEmail(EmailPadrao)
            .SetEndereco(endereco)
            .Build();

        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.Null(tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
        Assert.NotNull(tomador.EmailTomador);
        Assert.NotNull(tomador.EnderecoTomador);
    }

    [Fact()]
    public void Build_WithRazaoSocialAndEmail_CorrectlySet()
    {
        var tomador = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao)
            .SetEmail(EmailPadrao)
            .Build();

        Assert.Null(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
        Assert.NotNull(tomador.EmailTomador);
        Assert.Equal(EmailPadrao.ToString(), tomador.EmailTomador.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Build_WithCnpjAndEndereco_CorrectlySet(long cnpjNumber)
    {
        var endereco = EnderecoBuilder.New().SetUf(new Uf("RJ")).Build();

        var tomador = TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), RazaoSocialPadrao)
            .SetEndereco(endereco)
            .Build();

        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.EnderecoTomador);
        Assert.Null(tomador.EmailTomador);
    }

    [Fact()]
    public void SetEmail_MultipleCalls_LastEmailWins()
    {
        var email2 = new Email("segundo@teste.com.br");

        var tomador = TomadorBuilder.NewRazaoSocial(RazaoSocialPadrao)
            .SetEmail(new Email("primeiro@teste.com.br"))
            .SetEmail(email2)
            .Build();

        Assert.Equal(email2.ToString(), tomador.EmailTomador?.ToString());
    }

    // ============================================
    // NewInscricaoMunicipal() Factory Method Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewInscricaoMunicipal_WithValidArgs_ReturnsBuilder(long cnpjNumber)
    {
        ITomadorBuilder builder = TomadorBuilder.NewInscricaoMunicipal(new InscricaoMunicipal(12345678), new Cnpj(cnpjNumber));
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void NewInscricaoMunicipal_WithNullInscricaoMunicipal_ThrowsArgumentNullException(long cnpjNumber)
    {
        InscricaoMunicipal? inscricao = null;
        var cnpj = new Cnpj(cnpjNumber);
        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoMunicipal(inscricao!, cnpj));
    }

    [Fact]
    public void NewInscricaoMunicipal_WithNullCnpj_ThrowsArgumentNullException()
    {
        var inscricao = new InscricaoMunicipal(12345678);
        Cnpj? cnpj = null;
        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoMunicipal(inscricao, cnpj!));
    }

    // ============================================
    // NewInscricaoEstadual() Factory Method Tests
    // ============================================

    [Fact]
    public void NewInscricaoEstadual_WithValidArgs_ReturnsBuilder()
    {
        ITomadorBuilder builder = TomadorBuilder.NewInscricaoEstadual(new InscricaoEstadual(123456789), RazaoSocialPadrao);
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewInscricaoEstadual_WithNullInscricaoEstadual_ThrowsArgumentNullException()
    {
        InscricaoEstadual? inscricao = null;
        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoEstadual(inscricao!, RazaoSocialPadrao));
    }

    [Fact]
    public void NewInscricaoEstadual_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        var inscricao = new InscricaoEstadual(123456789);
        RazaoSocial? razao = null;
        _ = Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoEstadual(inscricao, razao!));
    }

    // ============================================
    // SetEmail/SetEndereco on all builder types
    // ============================================

    [Fact]
    public void SetEmail_OnCpfBuilder_ReturnsBuilder()
    {
        ITomadorBuilder builder = TomadorBuilder.NewCpf(new Cpf(12345678909)).SetEmail(EmailPadrao);
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void SetEmail_OnCnpjBuilder_ReturnsBuilder(long cnpjNumber)
    {
        ITomadorBuilder builder = TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), RazaoSocialPadrao).SetEmail(EmailPadrao);
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void SetEndereco_OnCpfBuilder_ReturnsBuilder()
    {
        Endereco endereco = EnderecoBuilder.New().SetUf(new Uf("SP")).Build();
        ITomadorBuilder builder = TomadorBuilder.NewCpf(new Cpf(12345678909)).SetEndereco(endereco);
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void SetEndereco_OnCnpjBuilder_ReturnsBuilder(long cnpjNumber)
    {
        Endereco endereco = EnderecoBuilder.New().SetUf(new Uf("SP")).Build();
        ITomadorBuilder builder = TomadorBuilder.NewCnpj(new Cnpj(cnpjNumber), RazaoSocialPadrao).SetEndereco(endereco);
        _ = Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    // ============================================
    // Build() Output for all static methods
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Build_WithInscricaoMunicipal_SetsPropertiesCorrectly(long cnpjNumber)
    {
        Tomador tomador = TomadorBuilder.NewInscricaoMunicipal(new InscricaoMunicipal(12345678), new Cnpj(cnpjNumber)).Build();
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
    }

    [Fact]
    public void Build_WithInscricaoEstadual_SetsPropertiesCorrectly()
    {
        Tomador tomador = TomadorBuilder.NewInscricaoEstadual(new InscricaoEstadual(123456789), RazaoSocialPadrao).Build();
        Assert.NotNull(tomador.InscricaoEstadualTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
        Assert.Null(tomador.CpfOrCnpjTomador);
    }
}