using Nfe.Paulistana.Xml;
using System.Xml.Schema;

namespace Nfe.Paulistana.Tests.Xml;

/// <summary>
/// Testes unitários para <see cref="SchemaProvider"/>:
/// guard clauses, retorno de <see cref="XmlSchemaSet"/> compilado e cache por recurso.
/// </summary>
public class SchemaProviderTests
{
    private const string NfeNamespace = "http://www.prefeitura.sp.gov.br/nfe";
    private const string RecursoValido = "PedidoEnvioRPS_v01.xsd";
    private const string RecursoValidoV2 = "PedidoEnvioRPS_v02.xsd";

    // ============================================
    // Guard clauses
    // ============================================

    [Fact]
    public void GetSchemaSet_NamespaceNulo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SchemaProvider.GetSchemaSetV1(null!, RecursoValido));
    }

    [Fact]
    public void GetSchemaSet_RecursoNulo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SchemaProvider.GetSchemaSetV1(NfeNamespace, null!));
    }

    // ============================================
    // Retorno correto
    // ============================================

    [Fact]
    public void GetSchemaSet_RecursoValido_RetornaXmlSchemaSetNaoNulo()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public void GetSchemaSet_RecursoValido_RetornaSchemaSetCompilado()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);

        Assert.True(resultado.IsCompiled);
    }

    [Fact]
    public void GetSchemaSet_RecursoValido_ContemSchemasCarregados()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);

        Assert.True(resultado.Count > 0);
    }

    // ============================================
    // Cache
    // ============================================

    [Fact]
    public void GetSchemaSet_MesmoRecurso_RetornaAMesmaInstancia()
    {
        XmlSchemaSet primeira = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);
        XmlSchemaSet segunda = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);

        Assert.Same(primeira, segunda);
    }

    [Fact]
    public void GetSchemaSet_RecursosDiferentes_RetornaInstanciasDiferentes()
    {
        const string outroRecurso = "PedidoEnvioLoteRPS_v01.xsd";
        const string nfeNamespace = "http://www.prefeitura.sp.gov.br/nfe";

        XmlSchemaSet primeira = SchemaProvider.GetSchemaSetV1(NfeNamespace, RecursoValido);
        XmlSchemaSet segunda = SchemaProvider.GetSchemaSetV1(nfeNamespace, outroRecurso);

        Assert.NotSame(primeira, segunda);
    }

    // ============================================
    // GetSchemaSetV2 — Guard clauses
    // ============================================

    [Fact]
    public void GetSchemaSetV2_NamespaceNulo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SchemaProvider.GetSchemaSetV2(null!, RecursoValidoV2));
    }

    [Fact]
    public void GetSchemaSetV2_RecursoNulo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SchemaProvider.GetSchemaSetV2(NfeNamespace, null!));
    }

    // ============================================
    // GetSchemaSetV2 — Retorno correto
    // ============================================

    [Fact]
    public void GetSchemaSetV2_RecursoValido_RetornaXmlSchemaSetNaoNulo()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);

        Assert.NotNull(resultado);
    }

    [Fact]
    public void GetSchemaSetV2_RecursoValido_RetornaSchemaSetCompilado()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);

        Assert.True(resultado.IsCompiled);
    }

    [Fact]
    public void GetSchemaSetV2_RecursoValido_ContemSchemasCarregados()
    {
        XmlSchemaSet resultado = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);

        Assert.True(resultado.Count > 0);
    }

    // ============================================
    // GetSchemaSetV2 — Cache
    // ============================================

    [Fact]
    public void GetSchemaSetV2_MesmoRecurso_RetornaAMesmaInstancia()
    {
        XmlSchemaSet primeira = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);
        XmlSchemaSet segunda = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);

        Assert.Same(primeira, segunda);
    }

    [Fact]
    public void GetSchemaSetV2_RecursosDiferentes_RetornaInstanciasDiferentes()
    {
        const string outroRecurso = "PedidoEnvioLoteRPS_v02.xsd";

        XmlSchemaSet primeira = SchemaProvider.GetSchemaSetV2(NfeNamespace, RecursoValidoV2);
        XmlSchemaSet segunda = SchemaProvider.GetSchemaSetV2(NfeNamespace, outroRecurso);

        Assert.NotSame(primeira, segunda);
    }
}
