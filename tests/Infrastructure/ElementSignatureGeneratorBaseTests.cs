using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.Infrastructure;

/// <summary>
/// Testes unitários para os métodos estáticos protegidos de <see cref="ElementSignatureGeneratorBase{T}"/>,
/// cobrindo todos os branches dos formatadores e do digest SHA1.
/// </summary>
public class ElementSignatureGeneratorBaseTests
{
    // Stub mínimo para expor os métodos protegidos sem implementar lógica real.
    private sealed class Stub : ElementSignatureGeneratorBase<object>
    {
        public static string ExposedFormatInscricaoPrestador(string? value, int padding) =>
            FormatInscricaoPrestador(value, padding);

        public static string ExposedFormatSerieRps(SerieRps? value) =>
            FormatSerieRps(value);

        public static string ExposedFormatNumero(Numero? value) =>
            FormatNumero(value);

        public static string ExposedFormatTributacao(TributacaoNfe? value) =>
            FormatTributacao(value);

        public static string ExposedFormatBool(bool? value) =>
            FormatBool(value);

        public static string ExposedFormatValor(Valor? value) =>
            FormatValor(value);

        public static string ExposedFormatCodigoServico(CodigoServico? value) =>
            FormatCodigoServico(value);

        public static string ExposedFormatDataEmissao(DataXsd? value) =>
            FormatDataEmissao(value);

        public static string ExposedFormatStatusRps(StatusNfe? value) =>
            FormatStatusRps(value);

        public static string ExposedFormatCpfOrCnpj(ICpfOrCnpj? value) =>
            FormatCpfOrCnpj(value);

        public static byte[] ExposedSha1Digest(string inputText, X509Certificate2 certificate) =>
            Sha1Digest(inputText, certificate);

        protected override string Generate(object rps) => string.Empty;

        public override void Sign(object rps, X509Certificate2 certificate)
        {
        }
    }

    // ============================================
    // FormatInscricaoPrestador
    // ============================================

    [Fact]
    public void FormatInscricaoPrestador_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatInscricaoPrestador(null, 8);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatInscricaoPrestador_ValueComPoucosDígitos_PadLeftComZeros()
    {
        var result = Stub.ExposedFormatInscricaoPrestador("1234", 8);

        Assert.Equal("00001234", result);
    }

    // ============================================
    // FormatSerieRps
    // ============================================

    [Fact]
    public void FormatSerieRps_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatSerieRps(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatSerieRps_ValueComMenosDe5Chars_PadRightComEspacos()
    {
        var result = Stub.ExposedFormatSerieRps(new SerieRps("BB"));

        Assert.Equal("BB   ", result);
    }

    // ============================================
    // FormatNumero
    // ============================================

    [Fact]
    public void FormatNumero_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatNumero(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatNumero_ValueValido_PadLeftComZeros()
    {
        var result = Stub.ExposedFormatNumero(new Numero(42));

        Assert.Equal("000000000042", result);
    }

    // ============================================
    // FormatTributacao
    // ============================================

    [Fact]
    public void FormatTributacao_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatTributacao(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatTributacao_ValueValido_RetornaCaractere()
    {
        var result = Stub.ExposedFormatTributacao((TributacaoNfe)'T');

        Assert.Equal("T", result);
    }

    // ============================================
    // FormatBool
    // ============================================

    [Fact]
    public void FormatBool_ValueNull_RetornaN()
    {
        var result = Stub.ExposedFormatBool(null);

        Assert.Equal("N", result);
    }

    [Fact]
    public void FormatBool_ValueFalse_RetornaN()
    {
        var result = Stub.ExposedFormatBool(false);

        Assert.Equal("N", result);
    }

    [Fact]
    public void FormatBool_ValueTrue_RetornaS()
    {
        var result = Stub.ExposedFormatBool(true);

        Assert.Equal("S", result);
    }

    // ============================================
    // FormatValor
    // ============================================

    [Fact]
    public void FormatValor_ValueNull_RetornaZerosPadded()
    {
        var result = Stub.ExposedFormatValor(null);

        Assert.Equal("000000000000000", result);
    }

    [Fact]
    public void FormatValor_ValueInteiro_AppendDoisZerosEPadLeft()
    {
        var result = Stub.ExposedFormatValor((Valor)1000m);

        Assert.Equal("000000000100000", result);
    }

    [Fact]
    public void FormatValor_ValueFracionario_RemovePontoEPadLeft()
    {
        var result = Stub.ExposedFormatValor((Valor)1000.50m);

        Assert.Equal("000000000100050", result);
    }

    // ============================================
    // FormatCodigoServico
    // ============================================

    [Fact]
    public void FormatCodigoServico_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatCodigoServico(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatCodigoServico_ValueValido_PadLeftComZeros()
    {
        var result = Stub.ExposedFormatCodigoServico(new CodigoServico(7617));

        Assert.Equal("07617", result);
    }

    // ============================================
    // FormatDataEmissao
    // ============================================

    [Fact]
    public void FormatDataEmissao_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatDataEmissao(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatDataEmissao_ValueValido_RetornaFormatoAAAMMDD()
    {
        var result = Stub.ExposedFormatDataEmissao(new DataXsd(new DateTime(2024, 1, 20)));

        Assert.Equal("20240120", result);
    }

    // ============================================
    // FormatStatusRps
    // ============================================

    [Fact]
    public void FormatStatusRps_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatStatusRps(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatStatusRps_StatusComXmlEnum_RetornaValorDoAtributo()
    {
        var result = Stub.ExposedFormatStatusRps(StatusNfe.Normal);

        Assert.Equal("N", result);
    }

    [Fact]
    public void FormatStatusRps_StatusComXmlEnum_RetornaValorDoAtributoCancelada()
    {
        var result = Stub.ExposedFormatStatusRps(StatusNfe.Cancelada);

        Assert.Equal("C", result);
    }

    [Fact]
    public void FormatStatusRps_StatusComXmlEnum_RetornaValorDoAtributoExtraviada()
    {
        var result = Stub.ExposedFormatStatusRps(StatusNfe.Extraviada);

        Assert.Equal("E", result);
    }

    [Fact]
    public void FormatStatusRps_StatusSemMembro_RetornaEnumToString()
    {
        // Valor inteiro que não corresponde a nenhum membro do enum
        var statusInvalido = (StatusNfe)999;

        var result = Stub.ExposedFormatStatusRps(statusInvalido);

        Assert.Equal(string.Empty, result);
    }

    // ============================================
    // FormatCpfOrCnpj
    // ============================================

    [Fact]
    public void FormatCpfOrCnpj_ValueNull_ReturnsEmpty()
    {
        var result = Stub.ExposedFormatCpfOrCnpj(null);

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void FormatCpfOrCnpj_ComCpf_RetornaTipo1EIdPadded(long cpfNumber)
    {
        var doc = new CpfOrCnpj((Cpf)cpfNumber);

        var result = Stub.ExposedFormatCpfOrCnpj(doc);

        var expectedId = cpfNumber.ToString().PadLeft(14, '0');
        Assert.Equal($"1{expectedId}", result);
    }

    [Fact]
    public void FormatCpfOrCnpj_ComCnpj_RetornaTipo2EIdPadded()
    {
        var cnpjNumber = new ValidCnpjNumber().Min();
        var doc = new CpfOrCnpj(new Cnpj(cnpjNumber));

        var result = Stub.ExposedFormatCpfOrCnpj(doc);

        var expectedId = cnpjNumber.ToString().PadLeft(14, '0');
        Assert.Equal($"2{expectedId}", result);
    }

    [Fact]
    public void FormatCpfOrCnpj_SemCpfNemCnpj_RetornaTipo3EZeros()
    {
        // CpfOrCnpj() padrão tem ambos nulos → DefaultTypeIndicator = "3"
        var doc = new CpfOrCnpj();

        var result = Stub.ExposedFormatCpfOrCnpj(doc);

        Assert.Equal($"3{"".PadLeft(14, '0')}", result);
    }

    // ============================================
    // Sha1Digest
    // ============================================

    [Fact]
    public void Sha1Digest_CertificadoValido_RetornaAssinaturaComBytes()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var req = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=Teste", rsa,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        using X509Certificate2 certificate = req.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));

        var result = Stub.ExposedSha1Digest("texto de teste", certificate);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Sha1Digest_CertificadoSemChavePrivada_ThrowsCryptographicException()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var req = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=Teste", rsa,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        using X509Certificate2 certComChave = req.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
        // Exporta apenas a parte pública para remover a chave privada
        using X509Certificate2 certSemChave = X509Certificate2.CreateFromPem(
            certComChave.ExportCertificatePem());

        _ = Assert.Throws<System.Security.Cryptography.CryptographicException>(
            () => Stub.ExposedSha1Digest("texto", certSemChave));
    }
}