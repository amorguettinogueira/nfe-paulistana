# Configuração dos User Secrets

Este guia explica como configurar todos os campos necessários para executar o sample. As configurações são fornecidas exclusivamente via [User Secrets do .NET](https://learn.microsoft.com/aspnet/core/security/app-secrets), garantindo que dados sensíveis nunca sejam comitados no repositório.

> **Importante:** todos os campos precisam ser preenchidos, mesmo que você não pretenda executar todas as ações. A aplicação valida toda a configuração na inicialização e encerra com mensagem de erro se algum campo obrigatório estiver ausente.

---

## Como configurar

### Opção 1 — Pelo Visual Studio (recomendada)

1. Clique com o botão direito no projeto `Nfe.Paulistana.Integration.Sample` no Solution Explorer
2. Selecione **Manage User Secrets**
3. O arquivo `secrets.json` abrirá para edição
4. Cole o conteúdo de `secrets.json.example` (na raiz do projeto) e ajuste os valores

### Opção 2 — Pelo terminal

```bash
# Inicialize os User Secrets (necessário apenas uma vez)
dotnet user-secrets init

# Configure os campos obrigatórios individualmente
dotnet user-secrets set "MeuCnpj"                    "00.000.000/0001-00"
dotnet user-secrets set "CnpjDaMinhaFilial"          "00.000.000/0001-00"
dotnet user-secrets set "MinhaInscricaoMunicipal"    "0.000.000-0"
dotnet user-secrets set "Certificado:CaminhoArquivo" "C:\caminho\certificado.pfx"
dotnet user-secrets set "Certificado:Senha"          "sua-senha"
# ... continue com os campos de cada ação (veja seções abaixo)
```

Para verificar o que está configurado:

```bash
dotnet user-secrets list
```

> O `secrets.json.example` na raiz do projeto contém todos os campos com valores de exemplo e comentários inline. É a referência mais rápida para preencher o `secrets.json` de uma vez.

---

## Campos obrigatórios compartilhados

Esses campos são usados por praticamente todas as ações. Devem refletir os dados reais do seu certificado e do seu cadastro na Prefeitura de SP.

| Chave | Tipo | Descrição |
|---|---|---|
| `MeuCnpj` | string | CNPJ da empresa que realizará as operações. Deve ser válido, cadastrado na Prefeitura de SP e **corresponder exatamente ao certificado digital fornecido**. |
| `CnpjDaMinhaFilial` | string | CNPJ da filial. Se não houver filial, repita o valor de `MeuCnpj`. Usado nas consultas de NFS-e emitidas e recebidas. |
| `MinhaInscricaoMunicipal` | string | Inscrição Municipal da empresa junto à Prefeitura de SP. Deve pertencer ao CNPJ informado em `MeuCnpj`. |
| `Certificado:CaminhoArquivo` | string | Caminho absoluto para o arquivo `.pfx` (e-CNPJ A1). Exemplo: `C:\certificados\empresa.pfx` |
| `Certificado:Senha` | string | Senha do arquivo `.pfx`. Valor sensível — nunca commite este valor no repositório. |

---

## Campos por ação

### Consulta NFS-e

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaNfeV1:NumeroNotaFiscal` | número | Número da NFS-e a consultar via serviço V1. |
| `ConsultaNfeV2:NumeroNotaFiscal` | número | Número da NFS-e a consultar via serviço V2. |

---

### Consulta de Lote

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaLoteV1:NumeroLote` | número | Número do lote a consultar via serviço V1. |
| `ConsultaLoteV2:NumeroLote` | número | Número do lote a consultar via serviço V2. |

---

### Consulta de Informações de Lote

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaInformacoesLoteV1:NumeroLote` | número | Informe `0` para obter dados do último lote enviado, ou um número específico (V1). |
| `ConsultaInformacoesLoteV2:NumeroLote` | número | Informe `0` para obter dados do último lote enviado, ou um número específico (V2). |

---

### Consulta NFS-e Recebidas

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaNfeRecebidasV1:NumeroPagina` | número | Número da página de resultados (inicia em 1). |
| `ConsultaNfeRecebidasV1:ConsultaNDiasAtras` | número | Janela de dias retroativos a partir de hoje (máx. 31). |
| `ConsultaNfeRecebidasV2:NumeroPagina` | número | Número da página de resultados (inicia em 1). |
| `ConsultaNfeRecebidasV2:ConsultaNDiasAtras` | número | Janela de dias retroativos a partir de hoje (máx. 31). |

---

### Consulta NFS-e Emitidas

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaNfeEmitidasV1:NumeroPagina` | número | Número da página de resultados (inicia em 1). |
| `ConsultaNfeEmitidasV1:ConsultaNDiasAtras` | número | Janela de dias retroativos a partir de hoje (máx. 31). |
| `ConsultaNfeEmitidasV2:NumeroPagina` | número | Número da página de resultados (inicia em 1). |
| `ConsultaNfeEmitidasV2:ConsultaNDiasAtras` | número | Janela de dias retroativos a partir de hoje (máx. 31). |

---

### Consulta CNPJ

| Chave | Tipo | Descrição |
|---|---|---|
| `ConsultaCnpjV1:CnpjAConsultar` | string | CNPJ do contribuinte a consultar via serviço V1. |
| `ConsultaCnpjV2:CnpjAConsultar` | string | CNPJ do contribuinte a consultar via serviço V2. |

---

### Cancelamento de NFS-e

> ⚠️ **Atenção:** o cancelamento é uma operação irreversível no servidor da Prefeitura. Use com cautela. Infelizmente a prefeitura não disponibiliza modo de teste para essa operação, então certifique-se de usar um número de NFS-e que seja seguro cancelar (ex.: uma nota de teste ou uma nota real que você tenha certeza que pode cancelar).

| Chave | Tipo | Descrição |
|---|---|---|
| `CancelamentoNfeV1:NumeroNfeParaCancelar` | número | Número da NFS-e a cancelar via serviço V1. |
| `CancelamentoNfeV2:NumeroNfeParaCancelar` | número | Número da NFS-e a cancelar via serviço V2. |

---

### Envio de RPS de Teste (V1 e V2)

Os campos com prefixo `*` existem tanto para `EnvioRpsTesteV1` quanto para `EnvioRpsTesteV2`, exceto onde indicado como exclusivo de uma versão.

#### Campos comuns a V1 e V2

| Chave | Tipo | Descrição |
|---|---|---|
| `*:CnpjTomador` | string | CNPJ do tomador do serviço. Deve ser um CNPJ válido. |
| `*:RazaoSocialTomador` | string | Razão social do tomador do serviço. |
| `*:TributacaoNfe` | string | Tipo de tributação (`tpTributacaoNFe`). Consulte a [documentação da API](https://notadomilhao.sf.prefeitura.sp.gov.br/wp-content/uploads/2026/01/NFe_Web_Service.pdf). |
| `*:TipoRps` | string | Tipo do RPS (`tpTipoRPS`). Consulte a [documentação da API](https://notadomilhao.sf.prefeitura.sp.gov.br/wp-content/uploads/2026/01/NFe_Web_Service.pdf). |
| `*:CodigoServico` | string | Código do serviço conforme [tabela da Prefeitura de SP](https://drive.prefeitura.sp.gov.br/cidade/secretarias/upload/arquivos/secretarias/financas/legislacao/IN-SF-Surem-08-2011-Anexo-1.pdf). |
| `*:Discriminacao` | string | Descrição do serviço prestado (texto livre). |
| `*:SerieRps` | string | Série do RPS (ex.: `"RPS"`). |
| `*:NumeroRps` | número | Número sequencial do RPS. |
| `*:IssRetido` | boolean | `true` se o ISS foi retido na fonte pelo tomador. |
| `*:Aliquota` | decimal | Alíquota do ISS (ex.: `0.0796` para 7,96%). |
| `*:ValorServicos` | decimal | Valor total dos serviços em reais (ex.: `350.00`). |

#### Campos exclusivos do V2

| Chave | Tipo | Descrição |
|---|---|---|
| `EnvioRpsTesteV2:ExigibilidadeSuspensa` | boolean | `true` se a emissão é com exigibilidade suspensa. |
| `EnvioRpsTesteV2:PagamentoParceladoAntecipado` | boolean | `true` se houve pagamento parcelado antecipado. |
| `EnvioRpsTesteV2:CodigoNBS` | string | Código NBS do serviço. Consulte a [tabela NBS](https://www.gov.br/produtividade-e-comercio-exterior/pt-br/images/REPOSITORIO/scs/decos/NBS/Anexoa_Ia_NBSa_2.0a_coma_alteraa_esa_6.12.18.pdf) e sua [conversão tributária](https://www.gov.br/nfse/pt-br/biblioteca/documentacao-tecnica/rtc/anexoviii-correlacaoitemnbsindopcclasstrib_ibscbs_v1-00-00.xlsx/view). |
| `EnvioRpsTesteV2:CodigoMunicipioIbge` | número | Código IBGE do município de prestação do serviço. Consulte a [tabela do IBGE](https://www.ibge.gov.br/explica/codigos-dos-municipios.php). São Paulo = `3550308`. |
| `EnvioRpsTesteV2:CodigoOperacao` | string | Código da operação. Consulte a [tabela do gov.br](https://www.gov.br/nfse/pt-br/biblioteca/documentacao-tecnica/anexovii-indop_ibscbs_v1-00-00.xlsx/view). |
| `EnvioRpsTesteV2:ClassificacaoTributaria` | string | Classificação tributária do serviço. Consulte a [tabela do portal NF-e](https://www.nfe.fazenda.gov.br/portal/exibirArquivo.aspx?conteudo=AVRVVz1Jgl4=). |

> Informações adicionais sobre campos relacionados à reforma tributária estão disponíveis no [site da NF-e](https://www.nfe.fazenda.gov.br/portal/listaConteudo.aspx?tipoConteudo=/NJarYc9nus=).
