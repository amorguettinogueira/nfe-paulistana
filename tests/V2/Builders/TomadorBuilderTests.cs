using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Builders;

public class TomadorBuilderTests
{
    private static readonly RazaoSocial RazaoSocialPadrao = new("Empresa de Teste Ltda");
    private static readonly Email EmailPadrao = new("tomador@teste.com.br");
    private static readonly Nif NifPadrao = new("NIF123456");
    private static readonly MotivoNifNaoInformado MotivoNif = MotivoNifNaoInformado.NaoInformadoNaOrigem;
    private static readonly InscricaoMunicipal InscricaoMunicipalPadrao = new(12345678);
    private static readonly InscricaoEstadual InscricaoEstadualPadrao = new(123456789);
    private static readonly Endereco EnderecoPadrao = new((Uf)"SP", (CodigoIbge)1234567, (Bairro)"Bairro", (Cep)"12345678", (TipoLogradouro)"Rua", (Logradouro)"Teste", (NumeroEndereco)"123");

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void NewCpf_WithValidCpf_ReturnsBuilder(long cpfNumber)
    {
        var cpf = (Cpf)cpfNumber;
        var builder = TomadorBuilder.NewCpf(cpf);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewCpf_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf(cpf!));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void NewCnpj_WithValidCnpjAndRazaoSocial_ReturnsBuilder(string cnpjFormatted, string _)
    {
        ITomadorBuilder builder = TomadorBuilder.NewCnpj((Cnpj)cnpjFormatted, RazaoSocialPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewCnpj_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj(cnpj!, RazaoSocialPadrao));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void NewCnpj_WithNullRazaoSocial_ThrowsArgumentNullException(string cnpjFormatted, string _)
    {
        RazaoSocial? razaoSocial = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj((Cnpj)cnpjFormatted, razaoSocial!));
    }

    [Fact]
    public void NewNif_WithValidNifAndRazaoSocial_ReturnsBuilder()
    {
        var builder = TomadorBuilder.NewNif(NifPadrao, RazaoSocialPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewNif_WithNullNif_ThrowsArgumentNullException()
    {
        Nif? nif = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewNif(nif!, RazaoSocialPadrao));
    }

    [Fact]
    public void NewNif_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        RazaoSocial? razaoSocial = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewNif(NifPadrao, razaoSocial!));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void NewInscricaoMunicipal_WithValidArgs_ReturnsBuilder(string cnpjFormatted, string _)
    {
        var builder = TomadorBuilder.NewInscricaoMunicipal(InscricaoMunicipalPadrao, (Cnpj)cnpjFormatted);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void NewInscricaoMunicipal_WithNullInscricaoMunicipal_ThrowsArgumentNullException(string cnpjFormatted, string _)
    {
        InscricaoMunicipal? inscricao = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoMunicipal(inscricao!, (Cnpj)cnpjFormatted));
    }

    [Fact]
    public void NewInscricaoMunicipal_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoMunicipal(InscricaoMunicipalPadrao, cnpj!));
    }

    [Fact]
    public void NewInscricaoEstadual_WithValidArgs_ReturnsBuilder()
    {
        var builder = TomadorBuilder.NewInscricaoEstadual(InscricaoEstadualPadrao, RazaoSocialPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewInscricaoEstadual_WithNullInscricaoEstadual_ThrowsArgumentNullException()
    {
        InscricaoEstadual? inscricao = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoEstadual(inscricao!, RazaoSocialPadrao));
    }

    [Fact]
    public void NewInscricaoEstadual_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        RazaoSocial? razao = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoEstadual(InscricaoEstadualPadrao, razao!));
    }

    [Fact]
    public void NewRazaoSocial_WithValidArgs_ReturnsBuilder()
    {
        var builder = TomadorBuilder.NewRazaoSocial(MotivoNif, RazaoSocialPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewRazaoSocial_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        RazaoSocial? razao = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewRazaoSocial(MotivoNif, razao!));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_WithValidDomainObject_ReturnsBuilder(long cpfNumber)
    {
        var result = TomadorBuilder.NewCpf((Cpf)cpfNumber).SetEmail(EmailPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_WithNullDomainObject_ThrowsArgumentNullException(long cpfNumber)
    {
        Email? email = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf((Cpf)cpfNumber).SetEmail(email!));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEndereco_WithValidDomainObject_ReturnsBuilder(long cpfNumber)
    {
        var result = TomadorBuilder.NewCpf((Cpf)cpfNumber).SetEndereco(EnderecoPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEndereco_WithNullDomainObject_ThrowsArgumentNullException(long cpfNumber)
    {
        Endereco? endereco = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf((Cpf)cpfNumber).SetEndereco(endereco!));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithCpf_ReturnsTomadorWithCpf(long cpfNumber)
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

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Build_WithCnpj_ReturnsTomadorWithCnpjAndRazaoSocial(string cnpjFormatted, string _)
    {
        var tomador = TomadorBuilder.NewCnpj((Cnpj)cnpjFormatted, RazaoSocialPadrao).Build();
        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
    }

    [Fact]
    public void Build_WithNif_ReturnsTomadorWithNifAndRazaoSocial()
    {
        var tomador = TomadorBuilder.NewNif(NifPadrao, RazaoSocialPadrao).Build();
        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
    }

    [Fact]
    public void Build_WithRazaoSocialOnly_ReturnsTomadorWithMotivoNif()
    {
        var tomador = TomadorBuilder.NewRazaoSocial(MotivoNif, RazaoSocialPadrao).Build();
        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Build_WithInscricaoMunicipal_SetsPropertiesCorrectly(string cnpjFormatted, string _)
    {
        var tomador = TomadorBuilder.NewInscricaoMunicipal(InscricaoMunicipalPadrao, (Cnpj)cnpjFormatted).Build();
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.NotNull(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
    }

    [Fact]
    public void Build_WithInscricaoEstadual_SetsPropertiesCorrectly()
    {
        var tomador = TomadorBuilder.NewInscricaoEstadual(InscricaoEstadualPadrao, RazaoSocialPadrao).Build();
        Assert.NotNull(tomador.InscricaoEstadualTomador);
        Assert.NotNull(tomador.RazaoSocialTomador);
        Assert.Null(tomador.CpfOrCnpjTomador);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetEmail_MultipleCalls_LastEmailWins(long cpfNumber)
    {
        var email2 = new Email("segundo@teste.com.br");
        var tomador = TomadorBuilder.NewCpf((Cpf)cpfNumber)
            .SetEmail(new Email("primeiro@teste.com.br"))
            .SetEmail(email2)
            .Build();
        Assert.Equal(email2.ToString(), tomador.EmailTomador?.ToString());
    }
}