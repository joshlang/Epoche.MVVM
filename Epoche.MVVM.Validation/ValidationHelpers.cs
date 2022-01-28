using System.Collections.Concurrent;
using System.Reflection;

namespace Epoche.MVVM.Validation;

static class ValidatorHelpers
{
    static readonly ConcurrentDictionary<Assembly, ValidatorsInAssembly> ValidatorsByAssembly = new();

    sealed class ValidatorsInAssembly
    {
        readonly Dictionary<Type, Type> ValidatorTypeByType;
        readonly Dictionary<Type, IValidator> ValidatorsByType = new();

        public ValidatorsInAssembly(Assembly assembly)
        {
            ValidatorTypeByType = assembly
                .GetTypes()
                .Select(x => new
                {
                    Type = x,
                    Interfaces = x.GetGenericInterfaces(typeof(IValidator<>)).ToList()
                })
                .SelectMany(x => x.Interfaces.Select(i => new
                {
                    ValidatedType = i.GetGenericArguments()[0],
                    ValidatorType = x.Type
                }))
                .Where(x => !x.ValidatedType.IsGenericParameter)
                .GroupBy(x => x.ValidatedType)
                .Where(x => x.Count() == 1)
                .ToDictionary(x => x.Key, x => x.First().ValidatorType);
        }

        public IValidator GetOrCreateValidator(Type validatedType)
        {
            if (validatedType is null)
            {
                throw new ArgumentNullException(nameof(validatedType));
            }

            lock (ValidatorsByType)
            {
                if (ValidatorsByType.TryGetValue(validatedType, out var validator))
                {
                    return validator;
                }

                var validatorType = ValidatorTypeByType[validatedType];
                return ValidatorsByType[validatedType] = Activator.CreateInstance(validatorType) as IValidator ?? throw new InvalidOperationException($"Failed to create instance of {validatorType.Name}");
            }
        }
        public IEnumerable<(Type ValidatorInterface, Type ValidatorType)> FindValidators() => ValidatorTypeByType.Select(x => (typeof(IValidator<>).MakeGenericType(x.Key), x.Value));

    }

    static ValidatorsInAssembly GetValidatorsInAssembly(Assembly assembly) => ValidatorsByAssembly.GetOrAdd(
        assembly ?? throw new ArgumentNullException(nameof(assembly)),
        x => new ValidatorsInAssembly(assembly));

    internal static IValidator FindValidatorInAssembly(Assembly assembly, Type validatedType) => GetValidatorsInAssembly(assembly).GetOrCreateValidator(validatedType);

    internal static IEnumerable<(Type ValidatorInterface, Type ValidatorType)> FindValidatorsInAssembly(Assembly assembly) => GetValidatorsInAssembly(assembly).FindValidators();
}
