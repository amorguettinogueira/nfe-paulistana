using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

public class EnderecoBuilderTests
{
    // ============================================
    // New() Factory Method Tests
    // ============================================

    [Fact]
    public void New_WithoutParameters_ReturnsBuilder()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New();

        Assert.NotNull(builder);
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void New_WithDomainObjects_ReturnsBuilder()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New(uf: new Uf("SP"), cidade: new CodigoIbge(3550308));

        Assert.NotNull(builder);
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void New_WithAllDomainObjects_PrePopulatesFields()
    {
        var uf = new Uf("RJ");
        var cidade = new CodigoIbge(3304557);
        var bairro = new Bairro("Copacabana");
        var cep = new Cep("20040020");
        var tipo = new TipoLogradouro("Av");
        var logradouro = new Logradouro("Atlântica");
        var numero = new NumeroEndereco("1578");
        var complemento = new Complemento("Apt 1001");

        Endereco endereco = EnderecoBuilder
            .New(uf, cidade, bairro, cep, tipo, logradouro, numero, complemento)
            .Build();

        Assert.NotNull(endereco);
        Assert.Equal("RJ", endereco.Uf?.ToString());
        Assert.NotNull(endereco.Cidade);
        Assert.NotNull(endereco.Bairro);
        Assert.NotNull(endereco.Cep);
        Assert.NotNull(endereco.Tipo);
        Assert.NotNull(endereco.Logradouro);
        Assert.NotNull(endereco.Numero);
        Assert.NotNull(endereco.Complemento);
    }

    // ============================================
    // SetUf Tests
    // ============================================

    [Fact]
    public void SetUf_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetUf(new Uf("RJ"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetUf_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Uf? uf = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetUf(uf!));
    }

    // ============================================
    // SetCodigoIbge Tests
    // ============================================

    [Fact]
    public void SetCodigoIbge_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetCodigoIbge(new CodigoIbge(3550308));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetCodigoIbge_WithNullDomainObject_ThrowsArgumentNullException()
    {
        CodigoIbge? cidade = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetCodigoIbge(cidade!));
    }

    // ============================================
    // SetBairro Tests
    // ============================================

    [Fact]
    public void SetBairro_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetBairro(new Bairro("Vila Mariana"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetBairro_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Bairro? bairro = null;
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetBairro(bairro!));
    }

    // ============================================
    // SetCep Tests
    // ============================================

    [Fact]
    public void SetCep_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetCep(new Cep("01310100"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetCep_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Cep? cep = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetCep(cep!));
    }

    // ============================================
    // SetTipo Tests
    // ============================================

    [Fact]
    public void SetTipo_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetTipo(new TipoLogradouro("Rua"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetTipo_WithNullDomainObject_ThrowsArgumentNullException()
    {
        TipoLogradouro? tipo = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetTipo(tipo!));
    }

    // ============================================
    // SetLogradouro Tests
    // ============================================

    [Fact]
    public void SetLogradouro_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetLogradouro(new Logradouro("Avenida Paulista"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetLogradouro_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Logradouro? logradouro = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetLogradouro(logradouro!));
    }

    // ============================================
    // SetNumero Tests
    // ============================================

    [Fact]
    public void SetNumero_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetNumero(new NumeroEndereco("1000"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetNumero_WithNullDomainObject_ThrowsArgumentNullException()
    {
        NumeroEndereco? numero = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetNumero(numero!));
    }

    // ============================================
    // SetComplemento Tests
    // ============================================

    [Fact]
    public void SetComplemento_WithValidDomainObject_ReturnsBuilder()
    {
        IEnderecoBuilder result = EnderecoBuilder.New().SetComplemento(new Complemento("Sala 100"));

        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(result);
    }

    [Fact]
    public void SetComplemento_WithNullDomainObject_ThrowsArgumentNullException()
    {
        Complemento? complemento = null;

        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetComplemento(complemento!));
    }

    // ============================================
    // IsValid Tests
    // ============================================

    [Fact]
    public void IsValid_WithNoFields_ReturnsFalse() =>
        Assert.False(EnderecoBuilder.New().IsValid());

    [Fact]
    public void IsValid_WithOneField_ReturnsTrue() =>
        Assert.True(EnderecoBuilder.New().SetUf(new Uf("SP")).IsValid());

    [Fact]
    public void IsValid_WithMultipleFields_ReturnsTrue()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New()
            .SetUf(new Uf("SP"))
            .SetCodigoIbge(new CodigoIbge(3550308))
            .SetBairro(new Bairro("Centro"));

        Assert.True(builder.IsValid());
    }

    [Fact]
    public void IsValid_WithTipoButNoLogradouro_ReturnsFalse() =>
        Assert.False(EnderecoBuilder.New().SetTipo(new TipoLogradouro("Av")).IsValid());

    [Fact]
    public void IsValid_WithNumeroButNoLogradouro_ReturnsFalse() =>
        Assert.False(EnderecoBuilder.New().SetNumero(new NumeroEndereco("100")).IsValid());

    [Fact]
    public void IsValid_WithComplementoButNoLogradouro_ReturnsFalse() =>
        Assert.False(EnderecoBuilder.New().SetComplemento(new Complemento("Ap 1")).IsValid());

    // ============================================
    // GetValidationErrors Tests
    // ============================================

    [Fact]
    public void GetValidationErrors_WithNoFields_ReturnsSingleError()
    {
        var errors = EnderecoBuilder.New().GetValidationErrors().ToList();

        _ = Assert.Single(errors);
    }

    [Fact]
    public void GetValidationErrors_WithTipoButNoLogradouro_ReturnsError()
    {
        var errors = EnderecoBuilder.New()
            .SetTipo(new TipoLogradouro("Av"))
            .GetValidationErrors()
            .ToList();

        Assert.NotEmpty(errors);
        Assert.Contains("Logradouro é obrigatório quando Tipo de Logradouro", errors[0]);
    }

    [Fact]
    public void GetValidationErrors_WithNumeroButNoLogradouro_ReturnsError()
    {
        var errors = EnderecoBuilder.New()
            .SetNumero(new NumeroEndereco("100"))
            .GetValidationErrors()
            .ToList();

        Assert.NotEmpty(errors);
        Assert.Contains("Logradouro é obrigatório quando Número ou Complemento", errors[0]);
    }

    [Fact]
    public void GetValidationErrors_WithValidField_ReturnsEmpty()
    {
        var errors = EnderecoBuilder.New()
            .SetUf(new Uf("SP"))
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    [Fact]
    public void GetValidationErrors_WithTipoAndLogradouro_ReturnsEmpty()
    {
        var errors = EnderecoBuilder.New()
            .SetTipo(new TipoLogradouro("Av"))
            .SetLogradouro(new Logradouro("Paulista"))
            .GetValidationErrors()
            .ToList();

        Assert.Empty(errors);
    }

    // ============================================
    // Build Tests
    // ============================================

    [Fact]
    public void Build_WithNoFields_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => EnderecoBuilder.New().Build());

    [Fact]
    public void Build_WithTipoButNoLogradouro_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => EnderecoBuilder.New().SetTipo(new TipoLogradouro("Av")).Build());

    [Fact]
    public void Build_WithNumeroButNoLogradouro_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => EnderecoBuilder.New().SetNumero(new NumeroEndereco("100")).Build());

    [Fact]
    public void Build_WithSingleField_ReturnsEndereco()
    {
        Endereco endereco = EnderecoBuilder.New().SetUf(new Uf("SP")).Build();

        Assert.NotNull(endereco);
        Assert.NotNull(endereco.Uf);
        Assert.Null(endereco.Cidade);
    }

    [Fact]
    public void Build_WithAllFields_ReturnsEnderecoWithAllProperties()
    {
        Endereco endereco = EnderecoBuilder.New()
            .SetUf(new Uf("SP"))
            .SetCodigoIbge(new CodigoIbge(3550308))
            .SetBairro(new Bairro("Bela Vista"))
            .SetCep(new Cep("01310100"))
            .SetTipo(new TipoLogradouro("Av"))
            .SetLogradouro(new Logradouro("Paulista"))
            .SetNumero(new NumeroEndereco("1578"))
            .SetComplemento(new Complemento("Cj 35"))
            .Build();

        Assert.NotNull(endereco);
        Assert.NotNull(endereco.Uf);
        Assert.NotNull(endereco.Cidade);
        Assert.NotNull(endereco.Bairro);
        Assert.NotNull(endereco.Cep);
        Assert.NotNull(endereco.Tipo);
        Assert.NotNull(endereco.Logradouro);
        Assert.NotNull(endereco.Numero);
        Assert.NotNull(endereco.Complemento);
    }

    // ============================================
    // Fluent Chaining Tests
    // ============================================

    [Fact]
    public void FluentChaining_AllSetters_ReturnsConsistentBuilder()
    {
        Endereco endereco = EnderecoBuilder.New()
            .SetUf(new Uf("MG"))
            .SetCodigoIbge(new CodigoIbge(3106200))
            .SetBairro(new Bairro("Savassi"))
            .SetCep(new Cep("30140081"))
            .SetTipo(new TipoLogradouro("Rua"))
            .SetLogradouro(new Logradouro("Getúlio Vargas"))
            .SetNumero(new NumeroEndereco("1200"))
            .SetComplemento(new Complemento("Loja"))
            .Build();

        Assert.NotNull(endereco);
        Assert.NotNull(endereco.Uf);
        Assert.NotNull(endereco.Cidade);
        Assert.NotNull(endereco.Bairro);
        Assert.NotNull(endereco.Cep);
        Assert.NotNull(endereco.Tipo);
        Assert.NotNull(endereco.Logradouro);
        Assert.NotNull(endereco.Numero);
        Assert.NotNull(endereco.Complemento);
    }

    [Fact]
    public void FluentChaining_PartialFields_ReturnsValidEndereco()
    {
        Endereco endereco = EnderecoBuilder.New()
            .SetUf(new Uf("BA"))
            .SetCodigoIbge(new CodigoIbge(2904400))
            .SetBairro(new Bairro("Barra"))
            .Build();

        Assert.NotNull(endereco);
        Assert.NotNull(endereco.Uf);
        Assert.NotNull(endereco.Cidade);
        Assert.NotNull(endereco.Bairro);
        Assert.Null(endereco.Cep);
    }
}