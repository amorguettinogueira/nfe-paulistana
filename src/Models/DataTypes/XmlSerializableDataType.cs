using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// <para>
/// Classe base abstrata para tipos de dados serializáveis em XML que encapsulam um valor string.
/// </para>
/// <para>
/// Implementa <see cref="IXmlSerializable"/> para suportar desserialização e serialização
/// de dados em formato XML, com validação pós-desserialização através do padrão Template Method.
/// </para>
/// <para>
/// <strong>Nota de Design:</strong> Esta classe fornece a infraestrutura de serialização XML,
/// enquanto subclasses são responsáveis pela validação específica do domínio através do
/// override do método <see cref="OnXmlDeserialized"/>.
/// </para>
/// </summary>
public abstract class XmlSerializableDataType : IXmlSerializable
{
    /// <summary>
    /// Obtém ou define o valor armazenado como string.
    /// </summary>
    /// <remarks>
    /// Definido pelas subclasses durante a construção ou pela infraestrutura de desserialização
    /// via <see cref="ReadXml"/>. O setter protegido preserva o encapsulamento sem
    /// impedir que subclasses atualizem o valor internamente quando necessário.
    /// </remarks>
    protected string? Value { get; set; }

    /// <summary>
    /// Obtém o esquema XML. Retorna null, indicando que não há esquema específico.
    /// </summary>
    /// <returns>Sempre retorna <c>null</c>.</returns>
    public XmlSchema? GetSchema() => null;

    /// <summary>
    /// Lê e desserializa o objeto a partir de um leitor XML.
    /// </summary>
    /// <param name="reader">O <see cref="XmlReader"/> de onde ler o conteúdo XML.</param>
    /// <remarks>
    /// <para>
    /// Este método implementa a leitura do valor XML e chama <see cref="OnXmlDeserialized"/>
    /// para permitir que subclasses validem o valor após a desserialização.
    /// </para>
    /// <para>
    /// Se o elemento XML está vazio, o valor é definido como <c>null</c>.
    /// Caso contrário, o conteúdo é lido como string.
    /// </para>
    /// </remarks>
    public void ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsEmptyElement)
        {
            _ = reader.Read();
            Value = null;
            return;
        }

        Value = reader.ReadElementContentAsString();

        // Hook para subclasses validarem após leitura de XML
        OnXmlDeserialized();
    }

    /// <summary>
    /// Escreve e serializa o objeto para um escritor XML.
    /// </summary>
    /// <param name="writer">O <see cref="XmlWriter"/> para o qual escrever o conteúdo XML.</param>
    /// <remarks>
    /// Escreve o valor (se não estiver vazio) como conteúdo do elemento XML.
    /// </remarks>
    public void WriteXml(XmlWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (!string.IsNullOrEmpty(Value))
        {
            writer.WriteValue(Value);
        }
    }

    /// <summary>
    /// Retorna a representação em string do objeto.
    /// </summary>
    /// <returns>O valor armazenado, ou <c>null</c> se não estiver definido.</returns>
    public override string? ToString() =>
        Value;

    /// <summary>
    /// Determina igualdade por valor: dois Value Objects do mesmo tipo concreto
    /// com o mesmo valor interno são considerados iguais.
    /// </summary>
    /// <param name="obj">Objeto a comparar.</param>
    /// <returns>
    /// <c>true</c> se <paramref name="obj"/> for do mesmo tipo concreto e tiver
    /// o mesmo valor interno; caso contrário, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is XmlSerializableDataType other &&
        GetType() == other.GetType() &&
        Value == other.Value;

    /// <summary>
    /// Retorna o hash code baseado no tipo concreto e no valor interno,
    /// consistente com a implementação de <see cref="Equals"/>.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(GetType(), Value);

    /// <summary>
    /// Hook chamado após a desserialização XML completa.
    /// Subclasses devem fazer override para adicionar validação específica do domínio.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este método é chamado automaticamente por <see cref="ReadXml"/> após o valor
    /// ter sido lido do XML. É o local apropriado para:
    /// <list type="bullet">
    /// <item>Validar que o valor desserializado é consistente com as regras de domínio</item>
    /// <item>Lançar <see cref="System.Runtime.Serialization.SerializationException"/> se a validação falhar</item>
    /// <item>Processar o valor se necessário</item>
    /// </list>
    /// </para>
    /// <para>
    /// Exemplo de implementação em subclasse:
    /// <code>
    /// protected override void OnXmlDeserialized()
    /// {
    ///     try
    ///     {
    ///         ValidateAfterDeserialization();
    ///     }
    ///     catch (ArgumentException ex)
    ///     {
    ///         throw new SerializationException("Erro ao desserializar", ex);
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    protected virtual void OnXmlDeserialized()
    {
        // Override em subclasses se necessário
    }
}