using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Primeiro passo obrigatório da cadeia de construção do <see cref="Models.Domain.Rps"/>.
/// Exposto após <see cref="RpsBuilder.New"/> e requer
/// a chamada de <see cref="SetNFe"/> antes de avançar para <see cref="IRpsSetServico"/>.
/// </summary>
public interface IRpsSetNfe
{
    /// <summary>
    /// Informa ao RPS os dados da Nota Fiscal Eletrônica (NF-e).
    /// </summary>
    /// <param name="dataEmissao">Data de emissão do RPS.</param>
    /// <param name="tributacaoNFe">Tipo de tributação da NF-e.</param>
    /// <param name="exigibilidadeSuspensa">Indica se é uma emissão com exigibilidade suspensa.</param>
    /// <param name="pagamentoParceladoAntecipado">Indica se é nota fiscal de pagamento parcelado antecipado (realizado antes do fornecimento).</param>
    /// <param name="statusNFe">Status da emissão da NF-e. Padrão: <see cref="StatusNfe.Normal"/>.</param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetServico"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="dataEmissao"/> ou <paramref name="tributacaoNFe"/> for nulo.</exception>
    IRpsSetServico SetNFe(DataXsd dataEmissao,
                          TributacaoNfe tributacaoNFe,
                          NaoSim exigibilidadeSuspensa,
                          NaoSim pagamentoParceladoAntecipado,
                          StatusNfe statusNFe = StatusNfe.Normal);
}