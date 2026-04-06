using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Terceiro passo obrigatório da cadeia de construção do <see cref="Models.Domain.Rps"/>.
/// Exposto após <see cref="IRpsSetServico.SetServico"/> e requer a chamada de <see cref="SetIss"/>
/// antes de avançar para <see cref="IRpsSetIbsCbs"/>, que requer as informações do IBS/CBS
/// antes de expor <see cref="IRpsSetOptionals"/> com campos opcionais e <see cref="IRpsSetOptionals.Build"/>.
/// </summary>
public interface IRpsSetIss
{
    /// <summary>
    /// Informa ao RPS os dados do Imposto Sobre Serviços (ISS).
    /// </summary>
    /// <param name="aliquota">Alíquota do ISS aplicada ao serviço prestado.</param>
    /// <param name="issRetido"><c>true</c> indica que o ISS já foi retido na fonte pelo tomador.</param>
    /// <returns>Próximo passo da cadeia: <see cref="IRpsSetIbsCbs"/>, que requer as informações de IBS/CBS.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="aliquota"/> for nulo.</exception>
    IRpsSetIbsCbs SetIss(Aliquota aliquota, bool issRetido);
}