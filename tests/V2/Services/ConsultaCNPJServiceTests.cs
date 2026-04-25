using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaCNPJService"/> v02:
/// guard clauses do construtor e de <see cref="IConsultaCNPJService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaCNPJServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoConsultaCNPJ CriarConsultaAssinada()
    {
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var cnpjContribuinte = new CpfOrCnpj(cpf);
        return factory.NewCpf(cpf, cnpjContribuinte);
    }

    private PedidoConsultaCNPJ CriarConsultaAssinadaComCnpj()
    {
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cnpj = new Cnpj(new ValidCnpjStrings().FirstOrDefault([string.Empty, string.Empty])?.FirstOrDefault(string.Empty).ToString() ?? string.Empty);
        var cnpjContribuinte = new CpfOrCnpj(cnpj);
        return factory.NewCnpj(cnpj, cnpjContribuinte);
    }

    // Sem payload
    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaCNPJResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload (Versao=2)
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaCNPJResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoConsultaCNPJ xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho><Detalhe xmlns=""><InscricaoMunicipal>39616924</InscricaoMunicipal><EmiteNFe>true</EmiteNFe></Detalhe></RetornoConsultaCNPJ></RetornoXML></ConsultaCNPJResponse></soap:Body></soap:Envelope>""";

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_HttpClientNulo_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaCNPJService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaCNPJService(httpClient);
        var pedido = new PedidoConsultaCNPJ();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    // ============================================
    // Resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinada();

        RetornoConsultaCNPJ resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinada();

        RetornoConsultaCNPJ resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoVersaoIgualADois()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinada();

        RetornoConsultaCNPJ resultado = await service.SendAsync(pedido);

        Assert.Equal(2, resultado.Cabecalho?.Versao);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_DetalheContemInscricaoMunicipal()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinada();

        RetornoConsultaCNPJ resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado.Detalhe);
        Assert.Single(resultado.Detalhe);
        Assert.Equal(39616924, resultado.Detalhe[0].InscricaoMunicipal);
        Assert.True(resultado.Detalhe[0].EmiteNFe);
    }

    // ============================================
    // Factory — CNPJ alfanumérico
    // ============================================

    [Fact]
    public async Task SendAsync_CriadoComCnpjAlfanumerico_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaCNPJService(httpClient);
        PedidoConsultaCNPJ pedido = CriarConsultaAssinadaComCnpj();

        RetornoConsultaCNPJ resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    // ============================================
    // Cnpj V2 — validação
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Cnpj_FormatoAlfanumericoValido_CriaComSucesso(string formatted, string unformatted)
    {
        var cnpj = new Cnpj(formatted);
        Assert.Equal(unformatted, cnpj.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Cnpj_FormatoMinusculo_NormalizaParaMaiusculo(string formatted, string unformatted)
    {
        var cnpj = new Cnpj(formatted.ToLowerInvariant());
        Assert.Equal(unformatted, cnpj.ToString());
    }

    [Fact]
    public void Cnpj_FormatoInvalido_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new Cnpj("123"));
    }

    [Fact]
    public void Cnpj_UltimosDigitosComLetras_ThrowsArgumentException()
    {
        // Os 2 últimos dígitos devem ser numéricos [0-9]{2}
        _ = Assert.Throws<ArgumentException>(() => new Cnpj("ABCD1234EF56AB"));
    }

    [Fact]
    public void Cnpj_StringVazia_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new Cnpj(""));
    }

    [Fact]
    public void Cnpj_StringNula_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new Cnpj(null!));
    }

    // ============================================
    // Cnpj V2 — edge cases
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Cnpj_FormatoComPontuacaoMinusculo_NormalizaERemoveFormatacao(string formatted, string unformatted)
    {
        var cnpj = new Cnpj(formatted.ToLowerInvariant());
        Assert.Equal(unformatted, cnpj.ToString());
    }

    [Fact]
    public void Cnpj_ApenasDigitosNumericos_DigitoVerificadorInvalido_ThrowsArgumentException()
    {
        // 14 dígitos numéricos com dígitos verificadores inválidos
        _ = Assert.Throws<ArgumentException>(() => new Cnpj("12345678000199"));
    }

    [Fact]
    public void Cnpj_TodosDigitosIguais_ThrowsArgumentException()
    {
        // Todos os caracteres iguais (00000000000000) — rejeitado pela regra allSameValues
        _ = Assert.Throws<ArgumentException>(() => new Cnpj("00000000000000"));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Cnpj_ComEspacos_TrimeEValida(string formatted, string unformatted)
    {
        var cnpj = new Cnpj($"  {formatted}  ");
        Assert.Equal(unformatted, cnpj.ToString());
    }

    [Fact]
    public void Cnpj_CaracteresEspeciaisInvalidos_ThrowsArgumentException()
    {
        // Caracteres que não são alfanuméricos nem de formatação padrão
        _ = Assert.Throws<ArgumentException>(() => new Cnpj("BX#5S4!X0C@001"));
    }
}