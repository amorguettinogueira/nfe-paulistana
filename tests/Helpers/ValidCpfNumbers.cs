namespace Nfe.Paulistana.Tests.Helpers;

internal class ValidCpfNumbers : ValidCpfNumber
{
    public ValidCpfNumbers()
    {
        // Random valid CPFs generated with https://www.4devs.com.br/gerador_de_cpf
        Add(63596780047);
        Add(86290818210);
        Add(94471862030);
        Add(31547161574);
        Add(94840839867);
        Add(89450471827);
        Add(64295333280);
        Add(76233669391);
        Add(80606975527);
    }
}