namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Calculadora estática compartilhada para o algoritmo de Módulo 11 duplo.
/// Aceita dígitos como <see cref="ReadOnlySpan{Int32}"/> em ordem da esquerda para a direita,
/// permitindo validação tanto de valores numéricos (CPF, CNPJ v1) quanto alfanuméricos (CNPJ v2).
/// </summary>
internal static class ModulusElevenCalculator
{
    /// <summary>
    /// Valida os dois dígitos verificadores de um número usando o algoritmo de Módulo 11 duplo.
    /// </summary>
    /// <param name="values">
    /// Dígitos do número em ordem da esquerda para a direita.
    /// Os dois últimos elementos são os dígitos verificadores.
    /// Deve conter pelo menos 3 elementos.
    /// </param>
    /// <param name="weights">
    /// Pesos cíclicos utilizados no cálculo. Deve conter pelo menos um elemento.
    /// </param>
    /// <returns><c>true</c> se ambos os dígitos verificadores estão corretos; <c>false</c> caso contrário.</returns>
    public static bool Validate(ReadOnlySpan<int> values, IReadOnlyList<byte> weights)
    {
        if (values.Length < 3 || weights.Count == 0)
        {
            return false;
        }

        int lastValue = values[^1];
        int penultimateValue = values[^2];

        int lastSum = penultimateValue * weights[0];
        int penultimateSum = 0;

        bool allSameValues = true;
        int currentValue = penultimateValue;
        int previousValue = lastValue;
        int weightIndex = 0;

        for (int i = values.Length - 3; i >= 0; i--)
        {
            allSameValues &= currentValue == previousValue;
            previousValue = currentValue;
            currentValue = values[i];

            penultimateSum += currentValue * weights[weightIndex];
            weightIndex = (weightIndex + 1) % weights.Count;
            lastSum += currentValue * weights[weightIndex];
        }

        penultimateSum %= 11;
        lastSum %= 11;

        return !allSameValues
            && penultimateValue == GetDigitFromRollingSum(penultimateSum)
            && lastValue == GetDigitFromRollingSum(lastSum);
    }

    /// <summary>
    /// Calcula um dígito a partir de uma soma em módulo 11.
    /// Retorna 0 quando a soma é menor que 2, caso contrário retorna 11 - soma.
    /// </summary>
    private static int GetDigitFromRollingSum(int sum) =>
        sum < 2 ? 0 : 11 - sum;
}