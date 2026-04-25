using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Helpers;

/// <summary>
/// Fábrica de objetos canônicos para testes da API V1.
/// Fornece um <see cref="Rps"/> pré-configurado para cenários que não testam os campos do RPS em si.
/// </summary>
internal static class RpsTestFactory
{
    /// <summary>Tomador padrão reutilizado nos RPS de teste.</summary>
    internal static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf).Build();

    /// <summary>
    /// Cria um <see cref="Rps"/> canônico para uso em testes que dependem de um RPS válido
    /// mas não exercitam seus campos individualmente.
    /// </summary>
    /// <param name="valorServicos">Valor dos serviços em reais. Padrão: 1000m.</param>
    /// <param name="dataEmissao">Data de emissão do RPS. Padrão: hoje.</param>
    /// <param name="tomador">Tomador a utilizar. Quando nulo, usa <see cref="TomadorPadrao"/>.</param>
    /// <param name="intermediario">Intermediário a utilizar. Padrão: null (sem intermediário).</param>
    internal static Rps Padrao(
        decimal valorServicos = 1000m,
        DateTime dataEmissao = default,
        Tomador? tomador = null,
        Intermediario? intermediario = null)
    {
        DateTime dataEmissaoFinal = dataEmissao == default
            ? DateTime.Today
            : dataEmissao;

        IRpsSetOptionals builder = RpsBuilder.New(
                (InscricaoMunicipal)39616924,
                TipoRps.Rps,
                (Numero)4105,
                (Discriminacao)"Desenvolvimento de software.",
                (SerieRps)"BB")
            .SetNFe((DataXsd)dataEmissaoFinal, (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico((CodigoServico)7617, (Valor)valorServicos)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(tomador ?? TomadorPadrao);

        if (intermediario is not null)
        {
            _ = builder.SetIntermediario(intermediario);
        }

        return builder.Build();
    }
}