using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Quarto e último passo obrigatório da cadeia de construção do <see cref="Rps"/>.
/// Exposto após <see cref="IRpsSetIss.SetIss"/> e requer a chamada de <see cref="SetIbsCbs"/>
/// antes de avançar para <see cref="IRpsSetOptionals"/>, que expõe campos opcionais e <see cref="IRpsSetOptionals.Build"/>.
/// </summary>
public interface IRpsSetValorCobrado
{
    /// <summary>
    /// Adiciona o valor inicial cobrado pela prestação do serviço, antes de tributos, multa e juros.
    /// Mutuamente exclusivo com <see cref="SetValorFinalCobrado"/>.
    /// </summary>
    /// <param name="valorInicialCobrado">Valor inicial cobrado em R$.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="valorInicialCobrado"/> for nulo.</exception>
    IRpsSetLocalPrestacao SetValorInicialCobrado(Valor valorInicialCobrado);

    /// <summary>
    /// Adiciona o valor final cobrado pela prestação do serviço, incluindo todos os tributos.
    /// Mutuamente exclusivo com <see cref="SetValorInicialCobrado"/>.
    /// </summary>
    /// <param name="valorFinalCobrado">Valor final cobrado em R$.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="valorFinalCobrado"/> for nulo.</exception>
    IRpsSetLocalPrestacao SetValorFinalCobrado(Valor valorFinalCobrado);
}