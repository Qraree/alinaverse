namespace Shared.DapperCustomAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ColumnAttribute : Attribute
{
    public string Name { get; set; }
}