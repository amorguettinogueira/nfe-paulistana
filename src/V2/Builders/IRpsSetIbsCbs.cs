using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Quarto passo obrigatório da cadeia de construção do <see cref="Rps"/>.
/// Exposto após <see cref="IRpsSetIss.SetIss"/> e requer a chamada de <see cref="SetIbsCbs"/>
/// antes de avançar para <see cref="IRpsSetValorCobrado"/>.
/// </summary>
public interface IRpsSetIbsCbs
{
    /// <summary>
    /// Informa ao RPS as informações declaradas pelo emitente referentes ao IBS e à CBS.
    /// </summary>
    /// <param name="ibsCbs">Objeto <see cref="InformacoesIbsCbs"/> com os dados de IBS/CBS.</param>
    /// <returns>Próximo passo da cadeia: <see cref="IRpsSetValorCobrado"/>, que expõe métodos para definir o valor cobrado.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="ibsCbs"/> for nulo.</exception>
    IRpsSetValorCobrado SetIbsCbs(InformacoesIbsCbs ibsCbs);
}