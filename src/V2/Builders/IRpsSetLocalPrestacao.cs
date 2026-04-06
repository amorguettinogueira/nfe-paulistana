using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Quarto e último passo obrigatório da cadeia de construção do <see cref="Rps"/>.
/// Exposto após <see cref="IRpsSetIss.SetIss"/> e requer a chamada de <see cref="SetIbsCbs"/>
/// antes de avançar para <see cref="IRpsSetOptionals"/>, que expõe campos opcionais e <see cref="IRpsSetOptionals.Build"/>.
/// </summary>
public interface IRpsSetLocalPrestacao
{
    /// <summary>
    /// Adiciona o código IBGE do local de prestação do serviço ao RPS (elemento <c>cLocPrestacao</c> do grupo <c>gpPrestacao</c>).
    /// Mutuamente exclusivo com <see cref="SetPaisPrestacao"/>.
    /// </summary>
    /// <param name="localPrestacao">Código IBGE do município de prestação.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="localPrestacao"/> for nulo.</exception>
    IRpsSetOptionals SetLocalPrestacao(CodigoIbge localPrestacao);

    /// <summary>
    /// Adiciona o código ISO do país de prestação do serviço ao RPS (elemento <c>cPaisPrestacao</c> do grupo <c>gpPrestacao</c>).
    /// Mutuamente exclusivo com <see cref="SetLocalPrestacao"/>.
    /// </summary>
    /// <param name="paisPrestacao">Código ISO do país de prestação.</param>
    /// <returns>Esta mesma instância para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="paisPrestacao"/> for nulo.</exception>
    IRpsSetOptionals SetPaisPrestacao(CodigoPaisISO paisPrestacao);
}