namespace Epoche.MVVM.Models;
public abstract partial class ModelBase
{
    [AttributeUsage(AttributeTargets.Field)]
    protected sealed class PropertyAttribute : Attribute
    {
        public string? Name { get; }
        public string? OnChange { get; set; }
        public string? EqualityComparer { get; set; }
        public bool TrackChanges { get; set; } = true;
        public bool PrivateSetter { get; set; }
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

    [AttributeUsage(AttributeTargets.Field)]
    protected sealed class FactoryInitializeAttribute : Attribute
    {
        public Type? Type { get; set; }
        public FactoryInitializeAttribute(Type? type = null)
        {
            Type = type;
        }
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
