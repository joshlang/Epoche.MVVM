using System.Collections;
using FluentValidation;

namespace Epoche.MVVM.Models;

public abstract class ValidatingModelBase<T> : ModelBase, INotifyDataErrorInfo, IDataErrorInfo where T : ValidatingModelBase<T>
{
    static readonly DataErrorsChangedEventArgs NullDataErrorsChangedEventArgs = new(null);
    static readonly PropertyChangedEventArgs ErrorPropertyChangedEventArgs = new(nameof(Error));
    static readonly PropertyChangedEventArgs HasErrorsPropertyChangedEventArgs = new(nameof(HasErrors));

    string[] ModelValidationFailures = Array.Empty<string>();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    IValidator<T> Validator { get; }
    public IReadOnlyDictionary<string, ValidatingModelErrors> BindableErrors { get; }

    protected ValidatingModelBase(IValidator<T> validator)
    {
        Validator = validator ?? throw new ArgumentNullException(nameof(validator));

        BindableErrors = Validator
            .CreateDescriptor()
            .GetMembersWithValidators()
            .Select(x => x.Key)
            .ExcludeNull()
            .ToDictionary(x => x, x => new ValidatingModelErrors());
    }

    bool hasErrors;
    public bool HasErrors
    {
        get => hasErrors;
        private set
        {
            if (hasErrors != value)
            {
                hasErrors = value;
                RaisePropertyChanged(HasErrorsPropertyChangedEventArgs);
            }
        }
    }

    public IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName)
        ? BindableErrors
            .Select(x => x.Value)
            .SelectMany(x => x.Errors)
            .Concat(ModelValidationFailures)
        : BindableErrors.TryGetValue(propertyName, out var errors)
        ? errors.Errors
        : Array.Empty<string>();

    public bool ValidateAll(params string[] ruleSets)
    {
        var anyChange = false;
        var validationResults = Validator
            .Validate((T)this, options =>
            {
                if (ruleSets.Length > 0)
                {
                    options.IncludeRuleSets(ruleSets);
                }
            })
            .Errors
            .GroupBy(x => x.PropertyName?.Split('[').FirstOrDefault() ?? string.Empty)
            .ToDictionary(x => x.Key, x => x.ToArray());

        foreach (var property in BindableErrors.Keys)
        {
            var currentErrors = BindableErrors[property];

            if (validationResults.TryGetValue(property, out var newErrors))
            {
                var newErrorMessages = newErrors.Select(x => x.ErrorMessage).ToArray();

                if (currentErrors.Errors.SequenceEqual(newErrorMessages))
                {
                    continue;
                }

                anyChange = true;
                currentErrors.Errors = newErrorMessages;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
            }
            else
            {
                if (currentErrors.Errors?.Length > 0)
                {
                    anyChange = true;
                    currentErrors.Errors = Array.Empty<string>();
                }
            }

            validationResults.Remove(property);
        }

        var modelFailures = validationResults.SelectMany(x => x.Value).ToArray();

        if (!modelFailures.Select(x => x.ErrorMessage).SequenceEqual(ModelValidationFailures))
        {
            ModelValidationFailures = modelFailures.Select(x => x.ErrorMessage).ToArray();
            anyChange = true;
            ErrorsChanged?.Invoke(this, NullDataErrorsChangedEventArgs);
        }
        if (anyChange)
        {
            HasErrors = ModelValidationFailures.Length > 0 || BindableErrors.Any(x => x.Value.Errors.Length > 0);
            RaisePropertyChanged(ErrorPropertyChangedEventArgs);
        }
        return !HasErrors;
    }

    public string Error =>
        ModelValidationFailures
        .Concat(
            BindableErrors
            .Select(x => x.Value.Error))
        .FirstOrDefault(x => x is not null)
        ?? "";


    public string this[string columnName] =>
        (string.IsNullOrEmpty(columnName)
        ? ModelValidationFailures.FirstOrDefault()
        : BindableErrors.TryGetValue(columnName ?? "", out var errors)
        ? errors.Error
        : null
        ) ?? "";

    /// <summary>
    /// Adds an error (if it doesn't exist), optionally relating to a specific property.
    /// The next call to ValidateAll() will remove the error.
    /// </summary>
    public void AddError(string? columnName, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            return;
        }
        if (!string.IsNullOrEmpty(columnName) && BindableErrors.TryGetValue(columnName, out var bindableErrors))
        {
            if (bindableErrors.Errors.Contains(error))
            {
                return;
            }
            bindableErrors.Errors = bindableErrors.Errors.Concat(new[] { error }).ToArray();
        }
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(columnName));

        if (!ModelValidationFailures.Contains(error))
        {
            ModelValidationFailures = ModelValidationFailures.Concat(new[] { error }).ToArray();
            ErrorsChanged?.Invoke(this, NullDataErrorsChangedEventArgs);
        }

        HasErrors = true;

        RaisePropertyChanged(ErrorPropertyChangedEventArgs);
    }

    public void ClearErrors()
    {
        var anyChanges = false;
        foreach (var error in BindableErrors.Where(x => x.Value.Errors.Length > 0))
        {
            anyChanges = true;
            error.Value.Errors = Array.Empty<string>();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(error.Key));
        }
        if (ModelValidationFailures.Length > 0)
        {
            anyChanges = true;
            ModelValidationFailures = Array.Empty<string>();
            ErrorsChanged?.Invoke(this, NullDataErrorsChangedEventArgs);
        }

        HasErrors = false;

        if (anyChanges)
        {
            RaisePropertyChanged(ErrorPropertyChangedEventArgs);
        }
    }
}
