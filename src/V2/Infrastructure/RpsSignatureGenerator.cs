using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Domain;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nfe.Paulistana.V2.Infrastructure;

/// <summary>
/// Gera o texto de assinatura para objetos RPS conforme especificação da Prefeitura de São Paulo e realiza a assinatura digital.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Fonte:</strong> https://notadomilhao.prefeitura.sp.gov.br/empresas/informacoes-gerais/manuais-arquivos/nfe_web_service.pdf<br/>
/// <strong>Documento:</strong> NFe_Web_Service v2.8.1.pdf — Páginas 31-32, Seção 4.3.2. Envio de RPS
/// </para>
/// <para>
/// Regras de formatação dos campos:
/// - CpfOrCnpj: indicador de tipo (1=CPF, 2=CNPJ, 3=Não informado) + identificador com 14 dígitos
/// - Valor: valor com 15 dígitos sem ponto decimal
/// - Data: 8 dígitos no formato AAAAMMDD
/// - bool: "S" para verdadeiro, "N" para falso
/// - StatusNfe: valor do atributo XmlEnum ou nome do enum como fallback
/// </para>
/// </remarks>
public sealed class RpsSignatureGenerator : ElementSignatureGeneratorBase<Rps>, IElementSignatureGenerator<Rps>
{
    /// <summary>Comprimento do padding para InscricaoMunicipal (12 dígitos, zeros à esquerda).</summary>
    private const int InscricaoMunicipalPaddingLength = 12;

    /// <summary>Indicador de tipo NIF no formato de texto de assinatura para casos sem CPF ou CNPJ.</summary>
    private const string NifTypeIndicator = "4";

    /// <summary>
    /// Formata o campo CpfOrCnpjOrNif para o texto de assinatura, aplicando as regras de formatação específicas.
    /// </summary>
    /// <param name="value">O objeto CpfOrCnpjOrNif a ser formatado.</param>
    /// <returns>Texto de assinatura formatado.</returns>
    private static string FormatCpfOrCnpjOrNif(CpfOrCnpjOrNif? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string type = value.Cpf != null ? CpfTypeIndicator : (value.Cnpj != null ? CnpjTypeIndicator : (value.Nif != null ? NifTypeIndicator : DefaultTypeIndicator));
        string id = value.Cpf?.ToString() ?? value.Cnpj?.ToString() ?? string.Empty;
        return $"{type}{id.PadLeft(CpfOrCnpjPaddingLength, '0')}";
    }

    /// <summary>
    /// Gera o texto de assinatura completo para um RPS com todos os campos formatados e concatenados.
    /// </summary>
    /// <param name="rps">O RPS para geração do texto de assinatura.</param>
    /// <returns>Texto de assinatura com todos os campos do RPS formatados e concatenados.</returns>
    protected override string Generate(Rps rps)
    {
        StringBuilder builder = new StringBuilder(146)
            .Append(FormatInscricaoPrestador(rps.ChaveRps?.InscricaoPrestador?.ToString(), InscricaoMunicipalPaddingLength))
            .Append(FormatSerieRps(rps.ChaveRps?.SerieRps))
            .Append(FormatNumero(rps.ChaveRps?.NumeroRps))
            .Append(FormatDataEmissao(rps.DataEmissao))
            .Append(FormatTributacao(rps.TributacaoRps))
            .Append(FormatStatusRps(rps.StatusRps))
            .Append(FormatBool(rps.IssRetido))
            .Append(FormatValor(rps.ValorInicialCobrado ?? rps.ValorFinalCobrado))
            .Append(FormatValor(rps.ValorDeducoes))
            .Append(FormatCodigoServico(rps.CodigoServico))
            .Append(FormatCpfOrCnpjOrNif(rps.CpfCnpjNifTomador));

        if (rps.CpfCnpjIntermediario != null)
        {
            _ = builder
                .Append(FormatCpfOrCnpj(rps.CpfCnpjIntermediario))
                .Append(FormatBool(rps.IssRetidoIntermediario));
        }

        if (rps.CpfCnpjNifTomador?.MotivoNifNaoInformado != null)
        {
            _ = builder.Append(rps.CpfCnpjNifTomador.MotivoNifNaoInformado.ToString());
        }
        else if (rps.CpfCnpjNifTomador?.Nif != null)
        {
            _ = builder.Append(rps.CpfCnpjNifTomador.Nif);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Assina o RPS com o certificado fornecido, populando a propriedade <see cref="Rps.Assinatura"/>.
    /// </summary>
    /// <param name="rps">O RPS a ser assinado.</param>
    /// <param name="certificate">Certificado X509 com chave privada para assinatura RSA-SHA1.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="rps"/> ou <paramref name="certificate"/> for nulo.</exception>
    /// <exception cref="InvalidOperationException">Se o texto de assinatura gerado tiver comprimento inválido.</exception>
    public override void Sign(Rps rps, X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(rps);
        ArgumentNullException.ThrowIfNull(certificate);

        rps.Assinatura = null;

        string signingText = Generate(rps);
        string nif = rps.CpfCnpjNifTomador?.Nif?.ToString() ?? string.Empty;

        // Comprimentos esperados:
        // * CPF ou CNPJ sem intermediário = 90
        // * CPF ou CNPJ COM intermediário = 106
        // * Não NIF sem intermediário = 91
        // * Não NIF COM intermediário = 107
        // * NIF sem intermediário = 90 + comprimento do NIF
        // * NIF COM intermediário = 106 + comprimento do NIF
        if (signingText.Length is not (90 or 91 or 106 or 107) && signingText.Length != (90 + nif.Length) && signingText.Length != (106 + nif.Length))
        {
            throw new InvalidOperationException(ComprimentoInvalido.Format(signingText.Length));
        }

        rps.Assinatura = Sha1Digest(signingText, certificate);
    }
}