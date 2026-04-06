# ADR 0004 — Assinatura digital XML e validação XSD embarcada

**Status:** Aceito
**Data:** 2025-01-01

---

## Contexto

O webservice NFS-e da Prefeitura de São Paulo exige que determinados elementos XML (RPS, pedidos de cancelamento) sejam assinados digitalmente com o certificado A1/A3 da empresa emissora, seguindo o padrão **XML Digital Signature (xmldsig)** do W3C. Adicionalmente, todos os documentos XML devem ser válidos contra os schemas **XSD** publicados pela Prefeitura antes do envio — documentos inválidos resultam em rejeição imediata com código de erro.

Duas decisões de design são necessárias:

1. **Onde executar a assinatura** — na biblioteca ou no consumidor?
2. **Como distribuir os schemas XSD** — arquivo externo ou recurso embarcado?

---

## Decisão

### 1. Assinatura digital transparente ao consumidor

A biblioteca assina automaticamente os elementos necessários como parte do pipeline `SendAsync`. O consumidor não tem contato com `X509Certificate2`, `SignedXml` ou qualquer API de criptografia diretamente.

**Fluxo interno:**

```
IXxxService.SendAsync(pedido)
    └─ ValidationHelper.Validate(pedido)         ← XSD embarcado
    └─ IElementSignatureGenerator.Sign(pedido)   ← xmldsig com cert do NfeOptions
    └─ SoapClient.SendAsync(envelope)            ← HttpClient com mTLS
```

A interface `IElementSignatureGenerator` é injetada nos serviços, tornando o comportamento de assinatura substituível em testes (NSubstitute) sem instanciar certificados reais.

**Fonte do certificado** — configurável via `NfeOptions.Certificado` em quatro formas mutuamente exclusivas:

| Fonte | Caso de uso |
|---|---|
| `FilePath` + `Password` | Arquivo `.pfx` em disco (desenvolvimento, on-prem) |
| `RawData` | Bytes do certificado (Azure Key Vault, AWS Secrets Manager) |
| `PointerHandle` | Handle nativo (HSM, Windows Certificate Store) |
| `Certificate` | Instância `X509Certificate2` gerenciada pelo chamador |

### 2. Schemas XSD como `EmbeddedResource`

Todos os schemas XSD (V1 e V2) são compilados dentro do assembly como `EmbeddedResource`. O `SchemaProvider` carrega e faz cache de cada schema por nome na primeira utilização via `XmlSchemaSet`.

```csharp
// Carregamento transparente — sem IO externo
var schema = SchemaProvider.Get("PedidoEnvioRPS_v02.xsd");
ValidationHelper.Validate(document, schema);
```

---

## Alternativas consideradas

### Assinatura

| Alternativa | Motivo da rejeição |
|---|---|
| Consumidor assina antes de chamar o serviço | Vazamento de complexidade criptográfica para fora da biblioteca; cada consumidor precisa conhecer xmldsig |
| Middleware HttpClient para assinatura | Assinatura xmldsig ocorre no nível do payload XML, não HTTP — incompatível com pipeline de DelegatingHandler |
| Wrapper sobre `System.Security.Cryptography.Xml` exposto publicamente | Expõe API de baixo nível incompatível com o objetivo de alto nível da biblioteca |

### XSD

| Alternativa | Motivo da rejeição |
|---|---|
| Arquivo `.xsd` externo distribuído junto ao pacote | Dependência de caminho no sistema de arquivos; falhas silenciosas se o arquivo for movido ou deletado |
| Download do XSD em runtime a partir do endpoint da Prefeitura | Dependência de rede na inicialização; indisponibilidade temporária do endpoint bloquearia o startup |
| Validação apenas na Prefeitura (confia na resposta de erro) | Erros descobertos após o envio de rede; latência desnecessária e experiência de debugging ruim |

---

## Consequências

**Positivas:**
- Zero configuração de criptografia para o consumidor — apenas aponte para o `.pfx`.
- Erros de schema são detectados localmente, antes de qualquer chamada de rede.
- A biblioteca funciona offline (validação XSD e assinatura não precisam de internet).
- `IElementSignatureGenerator` injetável garante testabilidade sem certificados reais.

**Negativas:**
- Os schemas XSD embarcados precisam ser atualizados manualmente a cada nova versão publicada pela Prefeitura (processo controlado e explícito no versionamento do pacote).
- O assembly aumenta em tamanho pelo embedding dos schemas — impacto negligenciável (~50 KB).
