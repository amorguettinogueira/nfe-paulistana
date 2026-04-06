using Nfe.Paulistana.Exceptions;
using System.Net;

namespace Nfe.Paulistana.Tests.Exceptions;

/// <summary>
/// Testes unitários para <see cref="NfeRequestException"/>:
/// construtores públicos, construtor interno e estrutura de herança.
/// </summary>
public class NfeRequestExceptionTests
{
    // ============================================
    // Herança e estrutura
    // ============================================

    [Fact]
    public void NfeRequestException_HerdaDeException() =>
        Assert.True(typeof(NfeRequestException).IsSubclassOf(typeof(Exception)));

    [Fact]
    public void NfeRequestException_EhSealed() =>
        Assert.True(typeof(NfeRequestException).IsSealed);

    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void ConstrutorPadrao_PropriedadesComValoresPadrao()
    {
        var ex = new NfeRequestException();

        Assert.Equal(default, ex.StatusCode);
        Assert.Null(ex.RequestPayload);
        Assert.Null(ex.ResponseBody);
        Assert.Null(ex.InnerException);
    }

    // ============================================
    // NfeRequestException(string message)
    // ============================================

    [Fact]
    public void ConstrutorComMensagem_MensagemPreenchida()
    {
        const string mensagem = "Erro de teste na requisição NF-e.";

        var ex = new NfeRequestException(mensagem);

        Assert.Equal(mensagem, ex.Message);
        Assert.Equal(default, ex.StatusCode);
        Assert.Null(ex.RequestPayload);
        Assert.Null(ex.ResponseBody);
        Assert.Null(ex.InnerException);
    }

    // ============================================
    // NfeRequestException(string message, Exception innerException)
    // ============================================

    [Fact]
    public void ConstrutorComMensagemEInnerException_PropriedadesPreenchidas()
    {
        const string mensagem = "Erro de teste na requisição NF-e.";
        var inner = new InvalidOperationException("Falha interna.");

        var ex = new NfeRequestException(mensagem, inner);

        Assert.Equal(mensagem, ex.Message);
        Assert.Same(inner, ex.InnerException);
        Assert.Equal(default, ex.StatusCode);
        Assert.Null(ex.RequestPayload);
        Assert.Null(ex.ResponseBody);
    }

    // ============================================
    // Construtor interno — StatusCode, payloads e mensagem
    // ============================================

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    [InlineData(HttpStatusCode.Forbidden, 403)]
    [InlineData(HttpStatusCode.NotFound, 404)]
    [InlineData(HttpStatusCode.InternalServerError, 500)]
    [InlineData(HttpStatusCode.ServiceUnavailable, 503)]
    public void ConstrutorInterno_StatusCodePreenchidoCorretamente(HttpStatusCode statusCode, int codigoEsperado)
    {
        var ex = new NfeRequestException(statusCode, null, null);

        Assert.Equal(statusCode, ex.StatusCode);
        Assert.Equal(codigoEsperado, (int)ex.StatusCode);
    }

    [Fact]
    public void ConstrutorInterno_MensagemContemCodigoHttp()
    {
        var ex = new NfeRequestException(HttpStatusCode.InternalServerError, null, null);

        Assert.Contains("500", ex.Message);
    }

    [Fact]
    public void ConstrutorInterno_RequestPayloadPreenchido()
    {
        const string payload = "<PedidoEnvioRPS>...</PedidoEnvioRPS>";

        var ex = new NfeRequestException(HttpStatusCode.BadRequest, payload, null);

        Assert.Equal(payload, ex.RequestPayload);
    }

    [Fact]
    public void ConstrutorInterno_ResponseBodyPreenchido()
    {
        const string body = "<RetornoEnvio><Erro>...</Erro></RetornoEnvio>";

        var ex = new NfeRequestException(HttpStatusCode.BadRequest, null, body);

        Assert.Equal(body, ex.ResponseBody);
    }

    [Fact]
    public void ConstrutorInterno_RequestPayloadNulo_PropriedadeEhNula()
    {
        var ex = new NfeRequestException(HttpStatusCode.NotFound, null, "corpo");

        Assert.Null(ex.RequestPayload);
    }

    [Fact]
    public void ConstrutorInterno_ResponseBodyNulo_PropriedadeEhNula()
    {
        var ex = new NfeRequestException(HttpStatusCode.NotFound, "payload", null);

        Assert.Null(ex.ResponseBody);
    }

    [Fact]
    public void ConstrutorInterno_TodosOsPayloads_PreenchidosSimultaneamente()
    {
        const string payload = "<xml>requisicao</xml>";
        const string body = "<xml>resposta</xml>";

        var ex = new NfeRequestException(HttpStatusCode.InternalServerError, payload, body);

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.Equal(payload, ex.RequestPayload);
        Assert.Equal(body, ex.ResponseBody);
    }

    [Fact]
    public void ConstrutorInterno_InnerExceptionEhNulo()
    {
        var ex = new NfeRequestException(HttpStatusCode.BadRequest, "payload", "body");

        Assert.Null(ex.InnerException);
    }

    // ============================================
    // Comportamento como exceção — pode ser capturada
    // ============================================

    [Fact]
    public void NfeRequestException_PodeSerLancadaECapturada()
    {
        NfeRequestException? capturada;
        try
        {
            throw new NfeRequestException(HttpStatusCode.ServiceUnavailable, null, null);
        }
        catch (NfeRequestException ex)
        {
            capturada = ex;
        }

        Assert.NotNull(capturada);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, capturada.StatusCode);
    }

    [Fact]
    public void NfeRequestException_PodeSerCapturadaComoException()
    {
        Exception? capturada;
        try
        {
            throw new NfeRequestException(HttpStatusCode.Forbidden, null, null);
        }
        catch (Exception ex)
        {
            capturada = ex;
        }

        _ = Assert.IsType<NfeRequestException>(capturada);
    }
}