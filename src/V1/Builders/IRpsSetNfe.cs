using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Primeiro passo obrigatório da cadeia de construção do <see cref="Nfe.Paulistana.Models.Domain.Rps"/>.
/// Exposto após <see cref="Nfe.Paulistana.Builders.RpsBuilder.New"/> e requer
/// a chamada de <see cref="SetNFe"/> antes de avançar para <see cref="IRpsSetServico"/>.
/// </summary>
public interface IRpsSetNfe
{
    /// <summary>
    /// Informa ao RPS os dados da Nota Fiscal Eletrônica (NF-e).
    /// </summary>
    /// <param name="dataEmissao">Data de emissão do RPS.</param>
    /// <param name="tributacaoNFe">Tipo de tributação da NF-e.</param>
    /// <param name="statusNFe">Status da emissão da NF-e. Padrão: <see cref="StatusNfe.Normal"/>.</param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetServico"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="dataEmissao"/> ou <paramref name="tributacaoNFe"/> for nulo.</exception>
    IRpsSetServico SetNFe(DataXsd dataEmissao,
                          TributacaoNfe tributacaoNFe,
                          StatusNfe statusNFe = StatusNfe.Normal);
}