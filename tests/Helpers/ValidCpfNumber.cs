namespace Nfe.Paulistana.Tests.Helpers;

internal class ValidCpfNumber : TheoryData<long>
{
    public ValidCpfNumber() =>
        // Random valid CPFs generated with https://www.4devs.com.br/gerador_de_cpf
        Add(46381819618);
}