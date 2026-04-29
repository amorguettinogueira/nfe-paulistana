# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
e este projeto adere ao [Versionamento Semântico](https://semver.org/spec/v2.0.0.html).

## [2.0.2] - 2026-04-29

### Observação
- Lançamento de validação para teste do fluxo de release (GitHub Release Skill).
  Nenhuma mudança funcional no código.

## [2.0.1] - 2026-04-28

### Corrigido
- `Certificado.Build()` agora usa `X509KeyStorageFlags.DefaultKeySet` como padrão quando
  `KeyStorageFlags` não é definido explicitamente, delegando ao sistema operacional a decisão
  de armazenamento e garantindo compatibilidade com todas as plataformas suportadas.
  O padrão anterior (`EphemeralKeySet`) causava falhas em macOS e algumas distribuições Linux.

## [2.0.0] - 2026-04-27

### Adicionado
- `NfeOptions.TimeoutPorTentativa` (padrão: 10 s) — configura o tempo limite por tentativa
  aplicado pelo novo handler de resiliência embutido.
- `ISignedElement` agora é `public`, permitindo que consumidores implementem a interface em
  seus próprios tipos de domínio.

### Alterado
- **Quebra de compatibilidade:** `AddNfePaulistanaV1`, `AddNfePaulistanaV2` e
  `AddNfePaulistanaAll` agora registram automaticamente um handler de resiliência (até 3
  tentativas, backoff exponencial e timeout por tentativa configurável) quando o delegate
  `configureClient` **não** é fornecido. Ao fornecer `configureClient`, o handler embutido é
  desabilitado por completo — retry, backoff e timeout passam a ser responsabilidade do
  consumidor, independentemente de `TimeoutPorTentativa`.
- `Certificado.KeyStorageFlags` agora tem como padrão `X509KeyStorageFlags.EphemeralKeySet`
  quando não definido explicitamente, mantendo a chave privada apenas em memória, sem
  persistência em disco.
- Atualizados `Microsoft.Extensions.Http`, `Microsoft.Extensions.Options` e
  `System.Security.Cryptography.Xml` de 10.0.5 para 10.0.7.

### Removido
- **Quebra de compatibilidade:** Propriedade `Certificado.Certificate` (`X509Certificate2`)
  foi removida. Migre para `RawData` via `cert.Export(X509ContentType.Pfx, senha)`, ou para
  `PointerHandle` via `cert.Handle`.

## [1.0.0] - 2026-04-09

### Adicionado
- Lançamento inicial do `Nfe.Paulistana` — biblioteca .NET para emissão, cancelamento e
  consulta de NF-e pela plataforma Nota do Milhão da Prefeitura de São Paulo, com suporte
  aos webservices SOAP V1 e V2.

[2.0.2]: https://github.com/amorguettinogueira/nfe-paulistana/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/amorguettinogueira/nfe-paulistana/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/amorguettinogueira/nfe-paulistana/compare/v1.0.0...v2.0.0
[1.0.0]: https://github.com/amorguettinogueira/nfe-paulistana/releases/tag/v1.0.0
