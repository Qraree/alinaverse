using Dapper;
using Shared.DapperCustomAttributes;

namespace Repository.DapperMapping;

public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
{
    public ColumnAttributeTypeMapper() 
        : base(new SqlMapper.ITypeMap[]
        {
            new CustomPropertyTypeMap(
                typeof(T),
                (type, columnName) => 
                    type.GetProperties().FirstOrDefault(prop =>
                        prop.GetCustomAttributes(false)
                            .OfType<ColumnAttribute>()
                            .Any(attr => attr.Name == columnName)
                        )
                    ),
            new DefaultTypeMap(typeof(T))
        })
     { }
}