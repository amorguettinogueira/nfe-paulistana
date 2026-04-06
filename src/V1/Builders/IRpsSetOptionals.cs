using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Último passo da cadeia de construção do <see cref="Rps"/>.
/// Exposto após <see cref="IRpsSetIss.SetIss"/> e permite o preenchimento de todos os
/// campos opcionais antes da chamada final de <see cref="Build"/>.
/// </summary>
/// <remarks>
/// <para>
/// Apesar de <see cref="SetTomador"/> estar nesta interface, o tomador de serviços é
/// validado como obrigatório em <see cref="Build"/>. Seu posicionamento aqui é
/// intencional para manter a flexibilidade do XSD, que não o exige em todos os
/// tipos de prestação. Quando omitido, <see cref="Build"/> lançará <see cref="ArgumentException"/>.
/// </para>
/// </remarks>
public interface IRpsSetOptionals
{
    /// <summary>
    /// Adiciona informações sobre a carga tributária total ao RPS.
    /// Ao menos um dos parâmetros deve ser informado.
    /// </summary>
    /// <param name="fonteCargaTributaria">Fonte de informação da carga tributária.</param>
    /// <param name="valorCargaTributaria">Valor da carga tributária total em R$.</param>
    /// <param name="percentualCargaTributaria">Percentual da carga tributária.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentException">Se todos os parâmetros forem nulos.</exception>
    IRpsSetOptionals SetCargaTributaria(FonteCargaTributaria? fonteCargaTributaria = null,
                                        Valor? valorCargaTributaria = null,
                                        Percentual? percentualCargaTributaria = null);

    /// <summary>Adiciona o Código de Cadastro Específico do INSS (CEI) ao RPS.</summary>
    /// <param name="codigoCei">Código CEI.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoCei"/> for nulo.</exception>
    IRpsSetOptionals SetCodigoCei(Numero codigoCei);

    /// <summary>Adiciona o código do encapsulamento de notas dedutoras ao RPS.</summary>
    /// <param name="numeroEncapsulamento">Código do encapsulamento.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="numeroEncapsulamento"/> for nulo.</exception>
    IRpsSetOptionals SetEncapsulamento(Numero numeroEncapsulamento);

    /// <summary>
    /// Substitui a Inscrição Municipal do prestador informada em <see cref="RpsBuilder.New"/>.
    /// </summary>
    /// <param name="inscricaoMunicipal">Nova Inscrição Municipal.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="inscricaoMunicipal"/> for nulo.</exception>
    IRpsSetOptionals SetInscricaoMunicipalPrestador(InscricaoMunicipal inscricaoMunicipal);

    /// <summary>
    /// Adiciona os dados do intermediário do serviço ao RPS.
    /// </summary>
    /// <param name="intermediario">Objeto <see cref="Intermediario"/> construído via <c>IntermediarioBuilder</c>.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="intermediario"/> for nulo.</exception>
    IRpsSetOptionals SetIntermediario(Intermediario intermediario);

    /// <summary>
    /// Adiciona o código IBGE do município de prestação do serviço ao RPS.
    /// </summary>
    /// <param name="municipioPrestacao">Código IBGE do município.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="municipioPrestacao"/> for nulo.</exception>
    IRpsSetOptionals SetMunicipio(CodigoIbge municipioPrestacao);

    /// <summary>Adiciona o número da matrícula de obra ao RPS.</summary>
    /// <param name="matriculaObra">Número da matrícula de obra.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="matriculaObra"/> for nulo.</exception>
    IRpsSetOptionals SetObra(Numero matriculaObra);

    /// <summary>
    /// Adiciona os dados do tomador de serviços ao RPS.
    /// Obrigatório para <see cref="Build"/>: se omitido, <see cref="Build"/> lançará <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="tomador">Objeto <see cref="Tomador"/> construído via <c>TomadorBuilder</c>.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="tomador"/> for nulo.</exception>
    IRpsSetOptionals SetTomador(Tomador tomador);

    /// <summary>
    /// Adiciona valores de impostos retidos na fonte ao RPS.
    /// Ao menos um dos parâmetros deve ser informado.
    /// </summary>
    /// <param name="valorPis">PIS — Programa de Integração Social.</param>
    /// <param name="valorCofins">COFINS — Contribuição para Financiamento da Seguridade Social.</param>
    /// <param name="valorInss">INSS — Instituto Nacional do Seguro Social.</param>
    /// <param name="valorIr">IR — Imposto de Renda.</param>
    /// <param name="valorCsll">CSLL — Contribuição Social Sobre o Lucro Líquido.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentException">Se todos os parâmetros forem nulos.</exception>
    IRpsSetOptionals SetTributos(Valor? valorPis = null,
                                 Valor? valorCofins = null,
                                 Valor? valorInss = null,
                                 Valor? valorIr = null,
                                 Valor? valorCsll = null);

    /// <summary>Adiciona o valor total recebido em R$ ao RPS, inclusive valores repassados a terceiros.</summary>
    /// <param name="valorTotalRecebido">Valor total recebido em R$.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="valorTotalRecebido"/> for nulo.</exception>
    IRpsSetOptionals SetValorRecebido(Valor valorTotalRecebido);

    /// <summary>
    /// Constrói e retorna o objeto <see cref="Rps"/> a partir de todos os atributos fornecidos na cadeia.
    /// </summary>
    /// <returns>Um objeto <see cref="Rps"/> consistente, com todos os campos obrigatórios preenchidos.</returns>
    /// <exception cref="ArgumentException">
    /// Se algum campo obrigatório não foi informado: Inscrição Municipal do prestador,
    /// Série do RPS, Data de emissão, Status da NF-e, Tipo de tributação, Valor dos serviços,
    /// Código do serviço, Alíquota do ISS, Indicador de retenção do ISS ou Tomador de serviços.
    /// </exception>
    Rps Build();
}