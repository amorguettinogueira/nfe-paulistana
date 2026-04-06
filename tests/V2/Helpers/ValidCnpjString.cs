namespace Nfe.Paulistana.Tests.V2.Helpers;

internal class ValidCnpjString : TheoryData<string, string>
{
    public ValidCnpjString() =>
        // Random valid CNPJs generated with https://servicos.receitafederal.gov.br/servico/cnpj-alfa/simular
        Add("BX.5S4.X0C/0001-76", "BX5S4X0C000176");
}