using Nfe.Paulistana.Models.DataTypes;
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

    [Fact]
    public void NewCpf_WithValidCpf_ReturnsBuilder()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var builder = TomadorBuilder.NewCpf(cpf);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewCpf_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf(cpf!));
    }

    [Fact]
    public void NewCnpj_WithValidCnpjAndRazaoSocial_ReturnsBuilder()
    {
        ITomadorBuilder builder = TomadorBuilder.NewCnpj((Cnpj)TestConstants.ValidFormattedCnpj, RazaoSocialPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewCnpj_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj(cnpj!, RazaoSocialPadrao));
    }

    [Fact]
    public void NewCnpj_WithNullRazaoSocial_ThrowsArgumentNullException()
    {
        RazaoSocial? razaoSocial = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCnpj((Cnpj)TestConstants.ValidFormattedCnpj, razaoSocial!));
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

    [Fact]
    public void NewInscricaoMunicipal_WithValidArgs_ReturnsBuilder()
    {
        var builder = TomadorBuilder.NewInscricaoMunicipal(InscricaoMunicipalPadrao, (Cnpj)TestConstants.ValidFormattedCnpj);
        Assert.IsAssignableFrom<ITomadorBuilder>(builder);
    }

    [Fact]
    public void NewInscricaoMunicipal_WithNullInscricaoMunicipal_ThrowsArgumentNullException()
    {
        InscricaoMunicipal? inscricao = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewInscricaoMunicipal(inscricao!, (Cnpj)TestConstants.ValidFormattedCnpj));
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

    [Fact]
    public void SetEmail_WithValidDomainObject_ReturnsBuilder()
    {
        var result = TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).SetEmail(EmailPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Fact]
    public void SetEmail_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Email? email = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).SetEmail(email!));
    }

    [Fact]
    public void SetEndereco_WithValidDomainObject_ReturnsBuilder()
    {
        var result = TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).SetEndereco(EnderecoPadrao);
        Assert.IsAssignableFrom<ITomadorBuilder>(result);
    }

    [Fact]
    public void SetEndereco_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Endereco? endereco = null;
        Assert.Throws<ArgumentNullException>(() => TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).SetEndereco(endereco!));
    }

    [Fact]
    public void Build_WithCpf_ReturnsTomadorWithCpf()
    {
        var tomador = TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).Build();
        Assert.NotNull(tomador);
        Assert.NotNull(tomador.CpfOrCnpjTomador);
        Assert.Null(tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
        Assert.Null(tomador.EmailTomador);
        Assert.Null(tomador.EnderecoTomador);
    }

    [Fact]
    public void Build_WithCnpj_ReturnsTomadorWithCnpjAndRazaoSocial()
    {
        var tomador = TomadorBuilder.NewCnpj((Cnpj)TestConstants.ValidFormattedCnpj, RazaoSocialPadrao).Build();
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

    [Fact]
    public void Build_WithInscricaoMunicipal_SetsPropertiesCorrectly()
    {
        var tomador = TomadorBuilder.NewInscricaoMunicipal(InscricaoMunicipalPadrao, (Cnpj)TestConstants.ValidFormattedCnpj).Build();
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

    [Fact]
    public void SetEmail_MultipleCalls_LastEmailWins()
    {
        var email2 = new Email("segundo@teste.com.br");
        var tomador = TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf)
            .SetEmail(new Email("primeiro@teste.com.br"))
            .SetEmail(email2)
            .Build();
        Assert.Equal(email2.ToString(), tomador.EmailTomador?.ToString());
    }
}