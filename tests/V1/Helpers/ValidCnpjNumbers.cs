namespace Nfe.Paulistana.Tests.V1.Helpers;

internal class ValidCnpjNumbers : TheoryData<long>
{
    public ValidCnpjNumbers()
    {
        // Random valid CNPJs generated with https://www.4devs.com.br/gerador_de_cnpj
        Add(TestConstants.ValidCnpj);
        Add(84067820000190);
        Add(33579346000145);
        Add(23461695000104);
        Add(24507735000174);
        Add(27344118000193);
        Add(57090291000173);
        Add(40483206000134);
        Add(05895697000120);
        Add(21771685000140);
    }
}