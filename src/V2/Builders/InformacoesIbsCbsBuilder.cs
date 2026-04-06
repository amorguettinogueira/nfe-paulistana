using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.V2.Builders;

/// <summary>
/// Construtor fluente para <see cref="InformacoesIbsCbs"/>, garantindo preenchimento correto dos campos obrigatórios e validação cruzada.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Design Arquitetural:</strong>
/// </para>
/// <list type="bullet">
/// <item>
/// <strong>Step Builder:</strong> Permite construção segura e legível de <see cref="InformacoesIbsCbs"/>, evitando estados inválidos.
/// </item>
/// <item>
/// <strong>Validação:</strong> Valida campos obrigatórios e regras de negócio no <see cref="Build"/>.
/// </item>
/// <item>
/// <strong>Extensibilidade:</strong> Facilita manutenção e evolução do modelo.
/// </item>
/// </list>
/// </remarks>
public sealed class InformacoesIbsCbsBuilder
{
    private FinalidadeEmissaoNfe _finalidadeEmissaoNfe = FinalidadeEmissaoNfe.NfseRegular;
    private NaoSim? _usoOuConsumoPessoal;
    private DestinatarioServicos _destinatarioServicos = DestinatarioServicos.ProprioTomador;
    private Valores? _valores;
    private CodigoOperacaoFornecimento? _codigoOperacaoFornecimento;
    private TipoOperacao? _tipoOperacao;
    private GrupoNfeReferenciada? _grupoNFSe;
    private TipoEnteGovernamental? _enteGovernamental;
    private InformacoesPessoa? _destinatario;
    private ImovelObra? _imovelObra;

    public static InformacoesIbsCbsBuilder New() => new();

    public InformacoesIbsCbsBuilder SetFinalidadeEmissao(FinalidadeEmissaoNfe finalidade)
    {
        _finalidadeEmissaoNfe = finalidade;
        return this;
    }

    public InformacoesIbsCbsBuilder SetUsoOuConsumoPessoal(NaoSim usoOuConsumoPessoal)
    {
        ArgumentNullException.ThrowIfNull(usoOuConsumoPessoal, nameof(usoOuConsumoPessoal));

        _usoOuConsumoPessoal = usoOuConsumoPessoal;

        return this;
    }

    public InformacoesIbsCbsBuilder SetDestinatarioServicos(DestinatarioServicos destinatarioServicos)
    {
        _destinatarioServicos = destinatarioServicos;
        return this;
    }

    public InformacoesIbsCbsBuilder SetClassificacaoTributaria(ClassificacaoTributaria classificacaoTributaria, ClassificacaoTributaria? classificacaoTributariaRegular = null)
    {
        ArgumentNullException.ThrowIfNull(classificacaoTributaria, nameof(classificacaoTributaria));

        TributoRegular? tributoRegular = classificacaoTributariaRegular != null
            ? new TributoRegular(classificacaoTributariaRegular)
            : null;

        var tributoIbsCbs = new TributoIbsCbs(classificacaoTributaria, tributoRegular);

        _valores ??= new Valores();
        _valores.TributosIbsCbs = new TributosIbsCbs(tributoIbsCbs);

        return this;
    }

    public InformacoesIbsCbsBuilder AddDocumentosReembolsoRepasseOuRessarcimento(params Documento[] documentos)
    {
        if (_valores == null)
        {
            throw new InvalidOperationException("Adicione documentos após definir a classificação tributária (SetClassificacaoTributaria).");
        }

        _valores.GrupoValoresInclusos = new GrupoValorIncluso(documentos);

        return this;
    }

    public InformacoesIbsCbsBuilder SetCodigoOperacaoFornecimento(CodigoOperacaoFornecimento codigo)
    {
        _codigoOperacaoFornecimento = codigo ?? throw new ArgumentNullException(nameof(codigo));
        return this;
    }

    public InformacoesIbsCbsBuilder SetTipoOperacao(TipoOperacao tipoOperacao)
    {
        _tipoOperacao = tipoOperacao;
        return this;
    }

    public InformacoesIbsCbsBuilder SetGrupoNfeReferenciada(GrupoNfeReferenciada grupo)
    {
        ArgumentNullException.ThrowIfNull(grupo, nameof(grupo));

        _grupoNFSe = grupo;

        return this;
    }

    public InformacoesIbsCbsBuilder SetEnteGovernamental(TipoEnteGovernamental ente)
    {
        _enteGovernamental = ente;
        return this;
    }

    public InformacoesIbsCbsBuilder SetDestinatario(InformacoesPessoa destinatario)
    {
        ArgumentNullException.ThrowIfNull(destinatario, nameof(destinatario));

        _destinatario = destinatario;

        return this;
    }

    public InformacoesIbsCbsBuilder SetImovelObra(ImovelObra imovelObra)
    {
        ArgumentNullException.ThrowIfNull(imovelObra, nameof(imovelObra));

        _imovelObra = imovelObra;

        return this;
    }

    /// <summary>
    /// Constrói a instância de <see cref="InformacoesIbsCbs"/> validando obrigatoriedades.
    /// </summary>
    /// <returns>Instância de <see cref="InformacoesIbsCbs"/> pronta para uso.</returns>
    /// <exception cref="InvalidOperationException">Se algum campo obrigatório não for preenchido.</exception>
    public InformacoesIbsCbs Build()
    {
        if (_usoOuConsumoPessoal is null)
        {
            throw new InvalidOperationException("O campo 'UsoOuConsumoPessoal' é obrigatório.");
        }

        if (_codigoOperacaoFornecimento is null)
        {
            throw new InvalidOperationException("O campo 'CodigoOperacaoFornecimento' é obrigatório.");
        }

        if (_valores is null)
        {
            throw new InvalidOperationException("O campo 'Valores' é obrigatório.");
        }

        return new InformacoesIbsCbs(
            _finalidadeEmissaoNfe,
            _usoOuConsumoPessoal,
            _destinatarioServicos,
            _valores,
            _codigoOperacaoFornecimento,
            _tipoOperacao,
            _grupoNFSe,
            _enteGovernamental,
            _destinatario,
            _imovelObra
        );
    }
}