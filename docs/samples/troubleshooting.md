# Troubleshooting

Erros comuns ao executar o sample e como resolvê-los.

---

## A aplicação encerra imediatamente com "validação de configuração falhou"

**Sintoma:**

```
Erro: validação de configuração falhou:
  - MeuCnpj é obrigatório.
  - Certificado.CaminhoArquivo é obrigatório.
```

**Causa:** um ou mais campos obrigatórios estão ausentes ou vazios no User Secrets.

**Solução:**

1. Execute `dotnet user-secrets list` para ver o que está configurado
2. Compare com o `secrets.json.example` e adicione os campos faltantes
3. Todos os campos precisam existir — mesmo os de ações que você não vai usar
4. Veja [configuracao.md](configuracao.md) para a lista completa de campos obrigatórios

---

## Certificado não encontrado

**Sintoma:**

```
Erro: validação de configuração falhou:
  - Certificado.CaminhoArquivo: arquivo não encontrado em 'C:\...\certificado.pfx'
```

**Causa:** o caminho do arquivo `.pfx` está incorreto ou o arquivo não existe nesse local.

**Solução:**

- Verifique se o arquivo existe no caminho informado (PowerShell):
  ```powershell
  Test-Path "C:\caminho\certificado.pfx"
  ```
- Use o caminho absoluto completo
- Se estiver usando `dotnet user-secrets set`, barras invertidas simples funcionam normalmente:
  ```bash
  dotnet user-secrets set "Certificado:CaminhoArquivo" "C:\certificados\empresa.pfx"
  ```

---

## Senha do certificado incorreta ou arquivo corrompido

**Sintoma:**

```
Erro: validação de configuração falhou:
  - Certificado: senha incorreta ou arquivo .pfx inválido/corrompido.
```

**Causa:** a senha informada não corresponde ao certificado, ou o arquivo `.pfx` está danificado.

**Solução:**

- Confirme a senha abrindo o `.pfx` manualmente: duplo clique no arquivo no Windows Explorer e tente importar com a senha
- Certifique-se de que a senha não contém espaços extras ao ser configurada via `dotnet user-secrets set`
- Se o arquivo veio compactado (`.zip`), extraia-o antes de informar o caminho

---

## Erro de CNPJ ou Inscrição Municipal inválidos no retorno SOAP

**Sintoma:** a ação executa, mas o serviço da Prefeitura retorna um código de erro no XML de resposta (ex.: código `E1`, `E4` etc.).

**Causa mais comum:** o CNPJ informado em `MeuCnpj` não corresponde ao CNPJ do certificado digital, ou a Inscrição Municipal não pertence a esse CNPJ.

**Solução:**

- Confirme que o CNPJ no certificado `.pfx` é exatamente o mesmo valor em `MeuCnpj`
- A `MinhaInscricaoMunicipal` deve ser da mesma empresa do `MeuCnpj`
- Para consultas de emitidas/recebidas, `CnpjDaMinhaFilial` também precisa ser válido e pertencer à mesma empresa
- Para outros erros de validação da prefeitura, revise a [documentação da API](https://notadomilhao.sf.prefeitura.sp.gov.br/wp-content/uploads/2026/01/NFe_Web_Service.pdf)

---

## Erro 641 — "Deverá ser utilizado o leiaute 1"

**Sintoma:**

```
[641] Contribuinte cadastrado como Simples Nacional na data informada. Deverá ser utilizado o leiaute 1.
```

**Causa:** o servidor da Prefeitura de SP rejeita chamadas às ações `*V2Action` de empresas optantes pelo Simples Nacional.

**Solução:** utilize as ações `*V1Action` correspondentes em vez das `*V2Action`.

---

### Contexto regulatório — restrição não documentada oficialmente

O código de erro `641` **não consta** na tabela de erros do manual [`NFe_Web_Service.pdf`](https://notadomilhao.sf.prefeitura.sp.gov.br/wp-content/uploads/2026/01/NFe_Web_Service.pdf) da Prefeitura de SP: a tabela salta do código `631` diretamente para `1100`. Nenhuma das fontes oficiais consultadas documenta essa restrição:

- A [página de orientações da Secretaria Municipal da Fazenda de SP](https://prefeitura.sp.gov.br/web/fazenda/w/nfs-e_orientacoes) (15/12/2025) descreve ambos os leiautes como válidos para **todos** os contribuintes, sem distinção de regime:
  > *"O layout 1 continua válido para as emissões pelo meio Online, Web Service e arquivo de TXT."*
  > *"O layout 2 será válido a partir de 01/01/2026, e estará disponível para as emissões Online e Web Service."*

- O [Comunicado Conjunto RFB + CGIBS (02/12/2025)](https://www.gov.br/receitafederal/pt-br/assuntos/noticias/2025/dezembro/comunicado-conjunto) determina a obrigatoriedade da NFS-e com destaque de CBS/IBS para **todos os contribuintes**, sem distinção por regime tributário.

- As [Orientações da Reforma Tributária para 2026](https://www.gov.br/receitafederal/pt-br/acesso-a-informacao/acoes-e-programas/programas-e-atividades/reforma-consumo/orientacoes-2026) (RFB, 12/12/2025) também não fazem qualquer distinção entre regimes quanto ao leiaute a utilizar.

A restrição existe na implementação do servidor da Prefeitura, mas não está amparada por nenhum documento oficial publicado. Se você tiver informações adicionais ou o comportamento mudar, abra uma issue ou PR.

---

## Timeout ou falha de conexão ao serviço SOAP

**Sintoma:**

```
System.Net.Http.HttpRequestException: Connection refused
```

ou a chamada demora mais de 30 segundos e encerra com erro de timeout.

**Causa:** o endpoint SOAP da Prefeitura de SP está indisponível ou inacessível.

**Solução:**

- Verifique sua conexão com a internet
- O pipeline de resiliência configurado executa automaticamente 3 tentativas com backoff exponencial antes de falhar — aguarde o ciclo completo
- Acesse o [portal da Nota do Milhão](https://notadomilhao.sf.prefeitura.sp.gov.br) para verificar se há manutenção programada

---

## Caracteres acentuados aparecem como símbolos estranhos no terminal

**Sintoma:** caracteres como `ã`, `ç`, `é` aparecem como `?` ou sequências de símbolos no console.

**Causa:** o terminal não está configurado para UTF-8.

**Solução:**

- No PowerShell, execute antes de rodar o projeto:
  ```powershell
  [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
  ```
- Ou use o **Windows Terminal**, que suporta UTF-8 por padrão
- O `Program.cs` já configura `Console.OutputEncoding = UTF8` na inicialização; se o problema persistir, o ajuste precisa ser feito no próprio terminal

---

## "Erro: valores duplicados de MenuOrder detectados"

**Sintoma:**

```
Erro: valores duplicados de MenuOrder detectados nas ações:
  Ordem 3: QueryNfeV1Action, QueryCnpjV1Action
```

**Causa:** duas ações foram registradas com o mesmo valor de `MenuOrder` em `ActionCatalog`.

**Solução:** este erro só deve ocorrer ao adicionar novas ações. Verifique `Actions/ActionCatalog.cs` e atribua ordens únicas a cada ação.

---

## Como inspecionar o XML SOAP trocado com a Prefeitura

A biblioteca expõe um `SoapDiagnosticsHandler` para capturar os XMLs de cada chamada diretamente no pipeline do `HttpClient`, sem necessidade de proxies externos ou debug de código-fonte.

### Uso básico — callback direto

```csharp
services.AddNfePaulistanaAll(
    options => options.Certificado = ...,
    configureClient: b => b
        .AddNfePaulistanaDiagnostics(exchange =>
        {
            Console.WriteLine($"[{exchange.SoapAction}] {exchange.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine("REQUEST:");
            Console.WriteLine(exchange.RequestXml);
            Console.WriteLine("RESPONSE:");
            Console.WriteLine(exchange.ResponseXml);
        })
        .AddStandardResilienceHandler());
```

### Uso com ILogger via DI (como está configurado neste sample)

```csharp
configureClient: b => b
    .AddNfePaulistanaDiagnostics(sp =>
    {
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("SoapDiagnostics");
        return exchange => logger.LogDebug(
            "[{SoapAction}] {ElapsedMs}ms | Sucesso: {IsSuccess}\n--- REQUEST ---\n{RequestXml}\n--- RESPONSE ---\n{ResponseXml}",
            exchange.SoapAction,
            (long)exchange.Elapsed.TotalMilliseconds,
            exchange.IsSuccess,
            exchange.RequestXml,
            exchange.ResponseXml);
    })
    .AddStandardResilienceHandler()
```

Para ver a saída do logger `SoapDiagnostics`, o nível de log precisa estar em `Debug`.

**No Visual Studio** — selecione o perfil de lançamento na barra de execução (ao lado do botão ▶):

> `Nfe.Paulistana.Integration.Sample (Diagnósticos SOAP)`

O perfil já tem `Logging__LogLevel__SoapDiagnostics=Debug` configurado em `Properties/launchSettings.json`. Nenhuma outra configuração é necessária.

**No terminal** — defina a variável de ambiente antes de executar:

```powershell
$env:Logging__LogLevel__SoapDiagnostics = "Debug"
dotnet run
```

### Sobre o posicionamento no pipeline

O `AddNfePaulistanaDiagnostics` adiciona o handler como o mais externo da cadeia. Com `AddStandardResilienceHandler` configurado, cada tentativa de retry gera uma notificação separada — útil para diagnóstico de retentativas. Para capturar apenas o resultado final (ignorando retries):

```csharp
b.AddStandardResilienceHandler().AddNfePaulistanaDiagnostics(onExchange)
```

### O que SoapExchange expõe

| Propriedade | Tipo | Descrição |
|---|---|---|
| `SoapAction` | string | Operação invocada (ex.: `consultaNFe`, `cancelamentoNFe`) |
| `RequestXml` | string | Envelope SOAP completo enviado |
| `ResponseXml` | string | Envelope SOAP completo recebido |
| `Elapsed` | TimeSpan | Tempo de resposta da chamada HTTP |
| `IsSuccess` | bool | `false` quando o status HTTP é 4xx/5xx (a `NfeRequestException` é lançada após o handler) |
