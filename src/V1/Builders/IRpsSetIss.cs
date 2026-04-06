using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Terceiro e último passo obrigatório da cadeia de construção do <see cref="Models.Domain.Rps"/>.
/// Exposto após <see cref="IRpsSetServico.SetServico"/> e requer a chamada de <see cref="SetIss"/>
/// antes de avançar para <see cref="IRpsSetOptionals"/>, que expõe campos opcionais e <see cref="IRpsSetOptionals.Build"/>.
/// </summary>
public interface IRpsSetIss
{
    /// <summary>
    /// Informa ao RPS os dados do Imposto Sobre Serviços (ISS).
    /// </summary>
    /// <param name="aliquota">Alíquota do ISS aplicada ao serviço prestado.</param>
    /// <param name="issRetido"><c>true</c> indica que o ISS já foi retido na fonte pelo tomador.</param>
    /// <returns>Próximo passo da cadeia: <see cref="IRpsSetOptionals"/>, que expõe campos opcionais e <see cref="IRpsSetOptionals.Build"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="aliquota"/> for nulo.</exception>
    IRpsSetOptionals SetIss(Aliquota aliquota, bool issRetido);
}