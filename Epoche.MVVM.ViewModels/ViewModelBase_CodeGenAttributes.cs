namespace Epoche.MVVM.ViewModels;
public abstract partial class ViewModelBase
{
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
