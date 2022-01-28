﻿namespace Epoche.MVVM.Models;
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