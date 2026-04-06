using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Segundo passo obrigatório da cadeia de construção do <see cref="Rps"/>.
/// Exposto após <see cref="IRpsSetNfe.SetNFe"/> e requer a chamada de <see cref="SetServico"/>
/// antes de avançar para <see cref="IRpsSetIss"/>.
/// </summary>
public interface IRpsSetServico
{
    /// <summary>
    /// Informa ao RPS os dados do serviço prestado.
    /// </summary>
    /// <param name="codigoServico">Código do serviço prestado, conforme tabela da Prefeitura de São Paulo.</param>
    /// <param name="nbs">Código da Nomenclatura Brasileira de Serviços (NBS).</param>
    /// <param name="valorDeducoes">
    /// Valor das deduções em R$. Opcional: quando omitido,
    /// <see cref="IRpsSetOptionals.Build"/> utiliza zero como valor padrão exigido pelo XSD.
    /// </param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetIss"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoServico"/> ou <paramref name="nbs"/> for nulo.</exception>
    IRpsSetIss SetServico(CodigoServico codigoServico,
                          CodigoNBS nbs,
                          Valor? valorDeducoes = null);
}