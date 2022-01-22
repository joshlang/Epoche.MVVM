namespace Epoche.MVVM.ViewModels;
public abstract partial class ViewModelBase
{
    [AttributeUsage(AttributeTargets.Field)]
    protected sealed class PropertyAttribute : Attribute
    {
        public string? Name { get; }
        public string? OnChange { get; set; }
        public string? EqualityComparer { get; set; }
        public bool TrackChanges { get; set; } = true;
        public PropertyAttribute(string? name = null)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    protected sealed class ChangedByAttribute : Attribute
    {
        public string[] Properties { get; } = Array.Empty<string>();
        public ChangedByAttribute(params string[] properties)
        {
            Properties = properties;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    protected sealed class CommandAttribute : Attribute
    {
        public string? Name { get; }
        public bool AllowConcurrency { get; set; }
        public string? CanExecute { get; set; }
        public string? TaskName { get; set; }
        public CommandAttribute(string? name = null)
        {
            Name = name;
        }
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class InjectAttribute : Attribute
{
    public Type Type { get; }
    public string? Name { get; set; }
    public string? AccessModifier { get; set; }
    public InjectAttribute(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class WithFactoryAttribute : Attribute
{
    public string? InterfaceName { get; }
    public string? FactoryName { get; }
    public string? InterfaceAccessModifier { get; }
    public string? FactoryAccessModifier { get; }
    public WithFactoryAttribute()
    {
    }
}