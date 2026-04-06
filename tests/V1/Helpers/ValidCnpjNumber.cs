namespace Nfe.Paulistana.Tests.V1.Helpers;

internal class ValidCnpjNumber : TheoryData<long>
{
    public ValidCnpjNumber() =>
        // Random valid CNPJs generated with https://www.4devs.com.br/gerador_de_cnpj
        Add(00878130000121);
}