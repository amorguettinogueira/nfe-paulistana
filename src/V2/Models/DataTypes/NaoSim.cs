using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o tipo Tipo de Não ou Sim, armazenado como um único caractere.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpNaoSim</c>, Linha: 310.
/// </para>
/// </remarks>
[Serializable]
public sealed class NaoSim : XmlSerializableDataType
{
    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NaoSim()
    { }

    /// <summary>
    /// Inicializa o Value Object com booleano informado.
    /// </summary>
    /// <param name="value">Booleano que representa o tipo de Não ou Sim.</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> não for uma letra.
    /// </exception>
    public NaoSim(bool value) => Value = value ? "1" : "0";

    /// <summary>
    /// Cria uma instância de <see cref="NaoSim"/> a partir de um booleano.
    /// </summary>
    /// <param name="value">Booleano que representa o tipo de Não ou Sim.</param>
    /// <returns>Nova instância de <see cref="NaoSim"/>.</returns>
    public static NaoSim FromBoolean(bool value) => new(value);

    /// <summary>
    /// Converte explicitamente um booleano em <see cref="NaoSim"/>.
    /// </summary>
    /// <param name="value">Booleano que representa o tipo de Não ou Sim.</param>
    public static explicit operator NaoSim(bool value) => FromBoolean(value);
}