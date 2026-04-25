namespace Nfe.Paulistana.Tests.V2.Helpers;

internal class ValidCnpjStrings : TheoryData<string, string>
{
    public ValidCnpjStrings()
    {
        // Random valid CNPJs generated with https://servicos.receitafederal.gov.br/servico/cnpj-alfa/simular
        Add(TestConstants.ValidFormattedCnpj, TestConstants.ValidUnformattedCnpj);
        Add("XA.412.263/0001-70", "XA412263000170");
        Add("CP.34N.HRC/0001-08", "CP34NHRC000108");
        Add("T0.RNE.N3Y/H59P-76", "T0RNEN3YH59P76");
        Add("PM.0CS.LBV/0001-12", "PM0CSLBV000112");
        Add("VN.23J.9KA/16ZX-10", "VN23J9KA16ZX10");
        Add("XN.33Y.9Z5/0001-92", "XN33Y9Z5000192");
        Add("WR.37L.4M2/0001-38", "WR37L4M2000138");
        Add("Z3.JZZ.C80/0001-73", "Z3JZZC80000173");
        Add("0S.P3X.4H5/0001-29", "0SP3X4H5000129");
    }
}