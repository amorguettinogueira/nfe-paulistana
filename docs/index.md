# Nfe.Paulistana

Biblioteca .NET para integração com o webservice SOAP da **Nota Fiscal de Serviços Eletrônica Paulistana (NFS-e)** da Prefeitura de São Paulo.

Suporte a **.NET 8, 9 e 10**.

## Instalação

```bash
dotnet add package Nfe.Paulistana
```

## Pré-requisitos

- .NET 8 ou superior
- Certificado digital A1 (`.pfx` / `.p12`) habilitado pela Prefeitura de São Paulo

## Início rápido

```csharp
// Program.cs
builder.Services.AddNfePaulistanaV1(options =>
{
    options.Certificado.FilePath = "/run/secrets/certificado.pfx";
    options.Certificado.Password  = "senha-do-certificado";
});
```

Injete o serviço e a factory correspondente e chame `SendAsync`:

```csharp
public class ConsultaService(IConsultaNFeService service, PedidoConsultaNFeFactory factory)
{
    public async Task<RetornoConsultaNFe> ConsultarAsync(string chaveNfe, CancellationToken ct = default)
    {
        var pedido = factory.NewCnpj(/* ... */);
        return await service.SendAsync(pedido, ct);
    }
}
```

## Próximos passos

- [Registro de serviços](registro-de-servicos.md)
- [Resiliência e retry](resiliencia.md)
- [Diagnóstico e tracing](diagnostico.md)
- [Referência de API](../api/index.html)
