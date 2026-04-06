# ADR 0003 — Suporte dual às versões V1 e V2 da API NFS-e

**Status:** Aceito
**Data:** 2025-01-01

---

## Contexto

A Prefeitura de São Paulo disponibiliza duas versões distintas do webservice NFS-e:

- **Schema v01** — versão original, amplamente adotada; suporta apenas CNPJ numérico de 14 dígitos.
- **Schema v02** — versão mais recente; introduz CNPJ alfanumérico (Receita Federal 2026), campos adicionais de IBS/CBS (Reforma Tributária), inscrição municipal de 1 a 12 dígitos e novos tipos fiscais.

A migração do mercado de V1 para V2 é gradual — muitas empresas ainda operam integralmente em V1. Outras já precisam dos recursos de V2 (CNPJ alfanumérico, IBS/CBS) sem abandonar V1 para operações legadas.

Entregar apenas V2 forçaria adoção prematura em clientes estáveis em V1. Entregar apenas V1 inviabilizaria a biblioteca para a Reforma Tributária.

---

## Decisão

Manter **V1 e V2 como cidadãos de primeira classe** no mesmo pacote NuGet, com:

1. **Namespaces completamente isolados** — `Nfe.Paulistana.V1.*` e `Nfe.Paulistana.V2.*` não compartilham tipos mutáveis nem serviços. Value Objects comuns ficam em `Nfe.Paulistana.Models.*`.

2. **Registro independente via DI** — três métodos de extensão permitem ao consumidor optar pelo mínimo necessário:

   ```csharp
   services.AddNfePaulistanaV1(options => { });   // registra apenas V1
   services.AddNfePaulistanaV2(options => { });   // registra apenas V2
   services.AddNfePaulistanaAll(options => { });  // V1 + V2 coexistindo
   ```

3. **Schemas XSD embarcados por versão** — `PedidoEnvioRPS_v01.xsd` e `PedidoEnvioRPS_v02.xsd` são recursos separados; o `SchemaProvider` carrega o schema correto de acordo com a versão do serviço em execução.

4. **Geradores de assinatura por versão** — `V1.Infrastructure.RpsSignatureGenerator` e `V2.Infrastructure.RpsSignatureGenerator` implementam a mesma interface `IElementSignatureGenerator` mas com lógicas de assinatura adequadas a cada schema.

---

## Alternativas consideradas

| Alternativa | Motivo da rejeição |
|---|---|
| Pacotes NuGet separados (`Nfe.Paulistana.V1` + `Nfe.Paulistana.V2`) | Duplicação de código base; consumidor precisa gerenciar dois pacotes para migração gradual |
| Apenas V2 | Exclui base instalada em V1; breaking change para adotantes existentes |
| Branches Git separadas | Impossibilita coexistência no mesmo processo; histórico divergente |
| Abstração unificada sobre V1/V2 | Vazamento de complexidade; V2 tem conceitos sem equivalente em V1 (IBS/CBS, CNPJ alfanumérico) |

---

## Consequências

**Positivas:**
- Consumidores em V1 não sofrem impacto com a evolução de V2.
- Migração gradual: o mesmo container de DI suporta V1 e V2 simultaneamente durante a transição.
- Cada versão tem seus próprios testes, garantindo não-regressão independente.

**Negativas:**
- Maior superfície de código a manter — cada novo recurso de infraestrutura precisa ser aplicado às duas versões.
- O consumidor precisa qualificar o namespace quando usa serviços de ambas as versões no mesmo arquivo.
