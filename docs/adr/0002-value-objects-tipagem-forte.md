# ADR 0002 — Value Objects e tipagem forte para campos fiscais

**Status:** Aceito
**Data:** 2025-01-01

---

## Contexto

O webservice NFS-e da Prefeitura de São Paulo expõe dezenas de campos com formatos, tamanhos e regras de validação específicos: inscrição municipal (1 a 8 dígitos), CNPJ alfanumérico (Receita Federal 2026), código de serviço (7 dígitos), código NBS (9 dígitos), identificação de obra (`[0-9A-Z]{30}`), entre outros.

A abordagem ingênua — representar todos esses campos como `string` — transfere a responsabilidade de validação para cada consumidor da biblioteca, resultando em:

- Erros descobertos apenas em runtime (resposta de erro do webservice após o envio);
- Validações duplicadas e inconsistentes espalhadas pelo código do consumidor;
- Possibilidade de passar um campo no lugar de outro (`CodigoServico` onde se espera `CodigoNBS`) sem qualquer erro de compilação;
- Dificuldade de testar invariantes de forma isolada.

---

## Decisão

Cada campo fiscal com formato/regra própria é encapsulado em um **Value Object** — um tipo imutável que valida suas invariantes no construtor e falha imediatamente (antes de qualquer chamada de rede) se o valor for inválido.

### Hierarquia base

```
XmlSerializableDataType<T>          ← serialização XML bidirecional + operador explícito de string
    └─ ConstrainedString<T>         ← regex + tamanho mínimo/máximo
    └─ ConstrainedDecimal<T>        ← escala decimal + intervalo numérico
    └─ ModulusElevenValidatedNumber ← algoritmo módulo 11 (CPF / CNPJ numérico)
```

A classe base `XmlSerializableDataType<T>` implementa `IXmlSerializable` de forma que os Value Objects participam nativamente do pipeline de serialização XML do webservice — sem adaptadores extras.

### Invariantes validadas por tipo

| Tipo | Regra |
|---|---|
| `CodigoPaisISO` | `[A-Z]{2}` |
| `CodigoNBS` | `[0-9]{9}` |
| `CodigoNCM` | `[0-9]{8}` |
| `IdentificacaoObra` | `[0-9A-Z]{30}` |
| `CadastroImovel` | `[0-9A-Z]{8,11}` |
| `Cnpj` (V2) | alfanumérico, 14 chars, formato Receita Federal 2026 |
| `Cpf` | 11 dígitos, dígito verificador módulo 11 |
| `InscricaoMunicipal` | inteiro positivo, 1–8 dígitos |
| `Valor`, `Aliquota` | decimal não-negativo com escala máxima configurável |

---

## Alternativas consideradas

| Alternativa | Motivo da rejeição |
|---|---|
| `string` puro | Sem segurança de tipo; validação duplicada nos consumidores |
| `record` simples com propriedade `string` | Não impede instanciação com valor inválido sem construtor validador |
| FluentValidation sobre DTOs | Validação tardia (pós-construção); acoplamento de framework nos consumidores |
| Source Generators para tipos | Complexidade desnecessária para o volume de tipos do projeto |

---

## Consequências

**Positivas:**
- Erro de formato é descoberto na construção do objeto, não na resposta do webservice.
- O compilador impede passar `CodigoNBS` onde se espera `CodigoNCM`.
- Testes de Value Object são simples, isolados e exaustivos (1590 testes na suíte).
- Serialização XML é transparente ao consumidor.

**Negativas:**
- O consumidor precisa conhecer os tipos ao construir pedidos (curva de aprendizado inicial).
- Operadores de conversão explícita (`(CodigoPaisISO)"BR"`) são necessários quando o valor vem de `string` externamente — mitigado pelos construtores e pelo método `FromString`.
