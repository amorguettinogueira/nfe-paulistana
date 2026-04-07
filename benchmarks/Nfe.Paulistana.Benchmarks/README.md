# Nfe.Paulistana.Benchmarks

Benchmarks de performance para os hot paths de CPU da library `Nfe.Paulistana`,
medindo o custo de serialização SOAP, deserialização SOAP, validação XSD e assinatura
de lote de RPS — as operações mais pesadas executadas a cada chamada à library.

> **I/O de rede excluído por design.**
> Medir o `SendAsync` completo incluiria latência de rede, tornando os números
> não-determinísticos e sem valor comparativo. Os benchmarks isolam exclusivamente
> o custo de CPU pago pelo consumidor da library a cada requisição.

## Pré-requisitos

- .NET 10 SDK

## Como executar

```powershell
dotnet run --project benchmarks/Nfe.Paulistana.Benchmarks -c Release
```

> ⚠️ **Sempre use `-c Release`.**
> Em Debug o JIT não otimiza o código e os números chegam a ser 10× piores.
> O BenchmarkDotNet avisa sobre isso, mas nunca execute benchmarks em Debug.

Ao executar, o `BenchmarkSwitcher` exibe um menu para escolher qual classe rodar.
Para rodar todas de uma vez:

```powershell
dotnet run --project benchmarks/Nfe.Paulistana.Benchmarks -c Release -- --all
```

## Benchmarks

### `SoapSerializationBenchmarks`

Mede `SoapClient.SerializeEnvelope<T>` — serialização do envelope SOAP para string
UTF-8 sem BOM, executada antes de cada envio HTTP.

**Compara:** schema V1 vs V2.

### `SoapDeserializationBenchmarks`

Mede `SoapClient.DeserializeEnvelope<T>` — deserialização da resposta XML retornada
pelo webservice em um `SoapEnvelope<TResponse>` tipado.

**Compara:** schema V1 vs V2.

### `XsdValidationBenchmarks`

Mede `IsValidXsd` — serialização para `XmlDocument` + validação contra o `XmlSchemaSet`
embutido no assembly como `EmbeddedResource`.

**Compara:** schema V1 vs V2.

### `LoteSigningBenchmarks`

Mede `PedidoEnvioLoteFactory.NewCpf` — o caminho crítico de cada envio de lote:
construção de um `X509Certificate2`, **N assinaturas RSA-2048** (uma por RPS) e
**1 assinatura do documento de lote** via XmlDsig, mais a acumulação de totalizações
do cabeçalho. É a operação mais cara da library.

O parâmetro `TamanhoLote` varia em `1`, `10` e `50` RPS, permitindo observar a
escalabilidade linear do custo com N+1 assinaturas.

**Compara:** schema V1 vs V2 para cada tamanho de lote.

## Como interpretar os resultados

| Coluna         | Significado                                                              |
|----------------|--------------------------------------------------------------------------|
| `Mean`         | Tempo médio de execução por iteração                                     |
| `Ratio`        | Quantas vezes mais lento/rápido que o baseline (V1)                      |
| `TamanhoLote`  | (apenas `LoteSigningBenchmarks`) número de RPS incluídos no lote         |
| `Gen0`         | Coletas de GC de geração 0 por 1.000 operações (zero = sem pressão)      |
| `Allocated`    | Bytes alocados no heap por iteração                                      |

Um `Ratio` de `1.20` em V2 significa que V2 é 20% mais lento que V1 para aquela operação.

## Resultados de referência

> Execute localmente para obter números precisos para a sua máquina.
> Os resultados variam com hardware, sistema operacional e carga do sistema.

```
// * Summary *

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.526.15411), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.5 (10.0.526.15411), X64 RyuJIT AVX2

| Method            | Mean     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------ |---------:|------:|-------:|-------:|----------:|------------:|
| 'Deserialize V1'  | 9.863 us |  1.00 | 1.8005 | 0.0305 |  18.45 KB |        1.00 |
| 'Deserialize V2'  | 9.458 us |  0.96 | 1.8005 | 0.0305 |  18.45 KB |        1.00 |
|                   |          |       |        |        |           |             |
| 'Serialize V1'    | 9.014 us |  1.00 | 2.2888 | 0.0916 |  23.44 KB |        1.00 |
| 'Serialize V2'    | 8.871 us |  0.98 | 2.2888 | 0.0916 |  23.44 KB |        1.00 |
|                   |          |       |        |        |           |             |
| 'XSD Validate V1' | 11.17 us |  1.00 | 3.4180 | 0.1221 |  34.98 KB |        1.00 |
| 'XSD Validate V2' | 10.76 us |  0.96 | 3.4180 | 0.0610 |  34.98 KB |        1.00 |
```

> Execute `LoteSigningBenchmarks` separadamente — por envolver criptografia RSA-2048,
> os tempos são na ordem de milissegundos e dominam a tabela combinada.

```
| Method         | TamanhoLote | Mean      | Ratio | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|--------------- |------------ |----------:|------:|---------:|---------:|---------:|-----------:|------------:|
| 'Sign Lote V1' | 1           |  1.364 ms |  1.00 |  21.4844 |   3.9063 |        - |  232.69 KB |        1.00 |
| 'Sign Lote V2' | 1           |  1.453 ms |  1.06 |  27.3438 |   3.9063 |        - |  285.91 KB |        1.23 |
|                |             |           |       |          |          |          |            |             |
| 'Sign Lote V1' | 10          |  7.021 ms |  1.00 |  62.5000 |  15.6250 |        - |  761.38 KB |        1.00 |
| 'Sign Lote V2' | 10          |  7.326 ms |  1.04 | 109.3750 |  46.8750 |        - | 1219.01 KB |        1.60 |
|                |             |           |       |          |          |          |            |             |
| 'Sign Lote V1' | 50          | 31.995 ms |  1.00 | 285.7143 | 142.8571 |  71.4286 |  2942.7 KB |        1.00 |
| 'Sign Lote V2' | 50          | 34.259 ms |  1.07 | 500.0000 | 416.6667 | 166.6667 | 5249.05 KB |        1.78 |
```
