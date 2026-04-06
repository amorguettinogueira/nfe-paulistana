using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.V1.Builders;

/// <summary>
/// Segundo passo obrigatório da cadeia de construção do <see cref="Models.Domain.Rps"/>.
/// Exposto após <see cref="IRpsSetNfe.SetNFe"/> e requer a chamada de <see cref="SetServico"/>
/// antes de avançar para <see cref="IRpsSetIss"/>.
/// </summary>
public interface IRpsSetServico
{
    /// <summary>
    /// Informa ao RPS os dados do serviço prestado.
    /// </summary>
    /// <param name="codigoServico">Código do serviço prestado, conforme tabela da Prefeitura de São Paulo.</param>
    /// <param name="valorServicos">Valor dos serviços em R$.</param>
    /// <param name="valorDeducoes">
    /// Valor das deduções em R$. Opcional: quando omitido,
    /// <see cref="IRpsSetOptionals.Build"/> utiliza zero como valor padrão exigido pelo XSD.
    /// </param>
    /// <returns>Próximo passo obrigatório da cadeia: <see cref="IRpsSetIss"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="codigoServico"/> ou <paramref name="valorServicos"/> for nulo.</exception>
    IRpsSetIss SetServico(CodigoServico codigoServico,
                          Valor valorServicos,
                          Valor? valorDeducoes = null);
}