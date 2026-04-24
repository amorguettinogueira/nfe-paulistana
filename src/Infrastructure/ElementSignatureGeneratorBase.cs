using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Base abstrata para geradores de assinatura de RPS, contendo lógica comum de formatação e assinatura.
/// </summary>
/// <typeparam name="T">Tipo do modelo RPS.</typeparam>
public abstract class ElementSignatureGeneratorBase<T>
{
    /// <summary>Mensagem de erro para casos em que a chave privada necessária para a assinatura digital não é encontrada no certificado.</summary>
    protected const string ChavePrivadaNaoEncontrada = "Chave privada não encontrada no certificado.";

    /// <summary>Comprimento do padding para SerieRps (5 caracteres, espaço à direita).</summary>
    protected const int SerieRpsPaddingLength = 5;

    /// <summary>Comprimento do padding para campos Numero (12 dígitos, zeros à esquerda).</summary>
    protected const int NumeroPaddingLength = 12;

    /// <summary>Comprimento do padding para campos Valor (15 dígitos sem ponto decimal).</summary>
    protected const int ValorPaddingLength = 15;

    /// <summary>Comprimento do padding para CodigoServico (5 dígitos, zeros à esquerda).</summary>
    protected const int CodigoServicoPaddingLength = 5;

    /// <summary>Comprimento do padding para campos CpfOrCnpj (14 dígitos, zeros à esquerda).</summary>
    protected const int CpfOrCnpjPaddingLength = 14;

    /// <summary>Comprimento do padding para TributacaoNfe (1 caractere).</summary>
    protected const int TributacaoNfePaddingLength = 1;

    /// <summary>Indicador de tipo CPF no formato de texto de assinatura.</summary>
    protected const string CpfTypeIndicator = "1";

    /// <summary>Indicador de tipo CNPJ no formato de texto de assinatura.</summary>
    protected const string CnpjTypeIndicator = "2";

    /// <summary>Indicador de tipo padrão (não especificado) no formato de texto de assinatura.</summary>
    protected const string DefaultTypeIndicator = "3";

    /// <summary>Mensagem de erro para texto de assinatura com comprimento inválido.</summary>
    protected const string ComprimentoInvalido = "O texto de assinatura gerado tem comprimento inválido: {0} caracteres.";

    /// <summary>Formata o campo InscricaoPrestador com zeros à esquerda até 8 dígitos.</summary>
    protected static string FormatInscricaoPrestador(string? value, int paddingLength) =>
        value == null ? string.Empty : value.PadLeft(paddingLength, '0');

    /// <summary>Formata o campo SerieRps com espaços à direita até 5 caracteres.</summary>
    protected static string FormatSerieRps(SerieRps? value) =>
        value == null ? string.Empty : (value.ToString() ?? string.Empty).PadRight(SerieRpsPaddingLength, ' ');

    /// <summary>Formata o campo NumeroRps com zeros à esquerda até 12 dígitos.</summary>
    protected static string FormatNumero(Numero? value) =>
        value == null ? string.Empty : (value.ToString() ?? string.Empty).PadLeft(NumeroPaddingLength, '0');

    /// <summary>Formata o campo TributacaoRps com espaço à direita até 1 caractere.</summary>
    protected static string FormatTributacao(TributacaoNfe? value) =>
        value == null ? string.Empty : (value.ToString() ?? string.Empty).PadRight(TributacaoNfePaddingLength, ' ');

    /// <summary>Formata valores booleanos como "S" (verdadeiro) ou "N" (falso/nulo).</summary>
    protected static string FormatBool(bool? value) =>
        (value ?? false) ? "S" : "N";

    /// <summary>Formata campos Valor como inteiro de 15 dígitos sem ponto decimal.</summary>
    protected static string FormatValor(Valor? value)
    {
        string currency = value?.ToString() ?? string.Empty;
        currency = currency.Contains('.', StringComparison.InvariantCultureIgnoreCase)
            ? currency.Replace(".", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            : $"{currency}00";
        return currency.PadLeft(ValorPaddingLength, '0');
    }

    /// <summary>Formata o campo CodigoServico com zeros à esquerda até 5 dígitos.</summary>
    protected static string FormatCodigoServico(CodigoServico? value) =>
        value == null ? string.Empty : (value.ToString() ?? string.Empty).PadLeft(CodigoServicoPaddingLength, '0');

    /// <summary>
    /// Assina com a chave privada do certificado o texto de assinatura gerado, utilizando o algoritmo RSA-SHA1 conforme especificação da Prefeitura de São Paulo.
    /// </summary>
    /// <param name="inputText">Texto de entrada para a assinatura.</param>
    /// <param name="certificate">Certificado com chave privada para assinatura RSA-SHA1.</param>
    /// <returns>Assinatura RSA-SHA1 dos bytes de entrada.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inputText"/> for nulo.</exception>
    /// <exception cref="CryptographicException">Se o certificado não contiver chave privada RSA.</exception>
    [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Requisito do modelo NF-e Paulistana que utiliza XMLDSig com SHA1")]
    protected static byte[] Sha1Digest(string inputText, X509Certificate2 certificate)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);

        using RSA privateKey = certificate.GetRSAPrivateKey()
            ?? throw new CryptographicException(ChavePrivadaNaoEncontrada);

        return privateKey.SignData(inputBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
    }

    /// <summary>Formata o campo DataEmissao no formato AAAAMMDD (sem hífens).</summary>
    protected static string FormatDataEmissao(DataXsd? value) =>
        value == null
            ? string.Empty
            : (value.ToString() ?? string.Empty).Replace("-", string.Empty, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>Formata o campo StatusRps</summary>
    protected static string FormatStatusRps(StatusNfe? value) =>
        // convertido de Reflection para switch para reduzir complexidade e melhorar legibilidade e performance (zero alocação de objetos e chamadas de método)
        value switch
        {
            StatusNfe.Normal => "N",
            StatusNfe.Cancelada => "C",
            StatusNfe.Extraviada => "E",
            _ => string.Empty
        };

    /// <summary>Formata o campo CpfOrCnpj com indicador de tipo e identificador com 14 dígitos.</summary>
    /// <remarks>Formato: [Tipo][ID com 14 dígitos] — Tipo 1: CPF, Tipo 2: CNPJ, Tipo 3: Padrão.</remarks>
    protected static string FormatCpfOrCnpj(ICpfOrCnpj? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string type = !string.IsNullOrWhiteSpace(value.Cpf) ? CpfTypeIndicator : (!string.IsNullOrWhiteSpace(value.Cnpj) ? CnpjTypeIndicator : DefaultTypeIndicator);
        string id = (!string.IsNullOrWhiteSpace(value.Cpf) ? value.Cpf : value.Cnpj) ?? string.Empty;

        return $"{type}{id.PadLeft(CpfOrCnpjPaddingLength, '0')}";
    }

    /// <summary>
    /// Implementação concreta deve gerar o texto de assinatura para o RPS.
    /// </summary>
    protected abstract string Generate(T rps);

    /// <summary>
    /// Assina o RPS com o certificado fornecido, populando a propriedade de assinatura.
    /// </summary>
    public abstract void Sign(T rps, X509Certificate2 certificate);
}