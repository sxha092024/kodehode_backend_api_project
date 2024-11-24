using System.Data;
using Dapper;

public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
        parameter.Value = guid.ToString();
    }

    public override Guid Parse(object value)
    {
        if (value is Guid guid)
        {
            return guid;
        }

        return new Guid((string)value);
    }
}