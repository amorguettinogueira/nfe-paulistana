using Microsoft.Extensions.Configuration;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nfe.Paulistana.Integration.Sample.Host;

/// <summary>
/// Carrega e valida as configurações da aplicação a partir de User Secrets.
/// Única fonte de configuração: <c>Microsoft.Extensions.Configuration.UserSecrets</c>
/// (UserSecretsId vinculado ao tipo <see cref="AppSettings"/>).
/// </summary>
internal static class AppSettingsLoader
{
    /// <summary>
    /// Carrega as configurações via User Secrets, vincula-as a uma instância de
    /// <see cref="AppSettings"/> e executa validação recursiva completa do grafo de objetos.
    /// <para><b>Retorno em falha:</b> retorna <see langword="null"/>! (null-forgiving) quando
    /// a validação falha. O chamador é responsável por verificar o resultado antes de usar.</para>
    /// <para><b>Efeito colateral:</b> imprime cada erro de validação no console via
    /// <c>ConsolePresenter.Error</c> quando a validação falha; não produz saída em caso de sucesso.</para>
    /// <para><b>Notas:</b>
    /// <list type="bullet">
    ///   <item>A validação inclui <c>IValidatableObject</c> de <see cref="AppSettings"/>,
    ///         que tenta carregar o certificado <c>.pfx</c> do disco e produz um
    ///         <see cref="System.ComponentModel.DataAnnotations.ValidationResult"/> em caso de falha
    ///         (arquivo ausente, senha incorreta ou formato inválido).</item>
    ///   <item>Os membros com erro são prefixados com o caminho dotado da propriedade
    ///         (ex.: <c>Certificado.CaminhoArquivo</c>), facilitando o diagnóstico.</item>
    /// </list></para>
    /// </summary>
    /// <returns>
    /// <see cref="AppSettings"/> completamente validado, ou <see langword="null"/>! se houver
    /// falha de validação.
    /// </returns>
    public static AppSettings LoadAndValidate()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AppSettings>()
            .Build();

        var settings = new AppSettings();
        configuration.Bind(settings);

        var validationResults = new List<ValidationResult>();

        var isValid = TryValidateObjectRecursive(settings, validationResults);

        if (!isValid)
        {
            ConsolePresenter.Error("Erro: validação de configuração falhou:");
            foreach (var vr in validationResults)
            {
                ConsolePresenter.Error($"  - {vr.ErrorMessage}");
            }
            return null!; // caller should handle null
        }

        return settings;
    }

    /// <summary>
    /// Ponto de entrada da validação recursiva.
    /// Inicializa o conjunto de objetos visitados com <see cref="ReferenceEqualityComparer"/>
    /// para detecção de ciclos no grafo de objetos antes de delegar ao overload principal.
    /// </summary>
    private static bool TryValidateObjectRecursive(object? obj, ICollection<ValidationResult> results)
    {
        if (obj is null)
        {
            return true;
        }

        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        return TryValidateObjectRecursive(obj, results, visited, string.Empty);
    }

    /// <summary>
    /// Valida <paramref name="obj"/> com
    /// <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}, bool)"/>
    /// (<c>validateAllProperties: true</c>) e percorre recursivamente todas as propriedades
    /// públicas de instância cujo tipo não seja primitivo, enum, <see cref="string"/>,
    /// <see cref="decimal"/> ou pertencente ao namespace <c>System.*</c>.
    /// <para>Propriedades do tipo <see cref="System.Collections.IEnumerable"/> são
    /// iteradas com índice no prefixo (ex.: <c>Itens[0].Campo</c>).
    /// Objetos já presentes em <paramref name="visited"/> são ignorados para evitar loops.</para>
    /// <para><paramref name="prefix"/> é acumulado a cada nível e aplicado aos
    /// <c>MemberNames</c> dos <see cref="ValidationResult"/> adicionados a
    /// <paramref name="results"/>.</para>
    /// </summary>
    private static bool TryValidateObjectRecursive(object obj, ICollection<ValidationResult> results, HashSet<object> visited, string prefix)
    {
        if (obj is null)
        {
            return true;
        }

        if (visited.Contains(obj))
        {
            return true;
        }

        visited.Add(obj);

        var context = new ValidationContext(obj);
        var localResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(obj, context, localResults, validateAllProperties: true);

        foreach (var r in localResults)
        {
            var memberNames = r.MemberNames?.Select(m => string.IsNullOrWhiteSpace(prefix) ? m : $"{prefix}.{m}") ?? [];
            results.Add(new ValidationResult(r.ErrorMessage, memberNames));
        }

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var prop in properties)
        {
            var propType = prop.PropertyType;

            // Skip simple types
            if (propType.IsPrimitive || propType.IsEnum || propType == typeof(string) || propType == typeof(decimal))
            {
                continue;
            }

            // Skip system types
            if (propType.Namespace?.StartsWith("System") == true)
            {
                continue;
            }

            var value = prop.GetValue(obj);

            if (value is null)
            {
                continue;
            }

            if (value is IEnumerable enumerable and not string)
            {
                var i = 0;

                foreach (var element in enumerable)
                {
                    if (element is null)
                    {
                        i++;
                        continue;
                    }

                    var childPrefix = string.IsNullOrWhiteSpace(prefix) ? $"{prop.Name}[{i}]" : $"{prefix}.{prop.Name}[{i}]";
                    var childValid = TryValidateObjectRecursive(element, results, visited, childPrefix);
                    isValid = isValid && childValid;
                    i++;
                }
            }
            else
            {
                var childPrefix = string.IsNullOrWhiteSpace(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                var childValid = TryValidateObjectRecursive(value, results, visited, childPrefix);
                isValid = isValid && childValid;
            }
        }

        return isValid;
    }

    /// <summary>
    /// Comparador por identidade de referência para uso em <see cref="HashSet{T}"/>.
    /// Usa <see cref="System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode"/> e
    /// <see cref="object.ReferenceEquals"/> para evitar falsos positivos de ciclo
    /// em objetos que sobreescrevem <c>Equals</c>/<c>GetHashCode</c>.
    /// </summary>
    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>Instância única do comparador.</summary>
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}