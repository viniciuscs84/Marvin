using System.Data;

namespace Marvin.DataAnnotations.ERBridge
{
    public enum DataType
    {
        [Commons.Extensions.EnumValue(DbType.Binary)]
        Binary,

        [Commons.Extensions.EnumValue(DbType.Binary)]
        Image,

        [Commons.Extensions.EnumValue(DbType.Boolean)]
        Boolean,

        [Commons.Extensions.EnumValue(DbType.Byte)]
        Byte,

        [Commons.Extensions.EnumValue(DbType.DateTime)]
        DateTime,

        [Commons.Extensions.EnumValue(DbType.Decimal)]
        Decimal,

        [Commons.Extensions.EnumValue(DbType.Decimal)]
        Money,

        [Commons.Extensions.EnumValue(DbType.Double)]
        Double,

        [Commons.Extensions.EnumValue(DbType.Guid)]
        Guid,

        [Commons.Extensions.EnumValue(DbType.Int16)]
        Int16,

        [Commons.Extensions.EnumValue(DbType.Int32)]
        Int32,

        [Commons.Extensions.EnumValue(DbType.Int64)]
        Int64,

        [Commons.Extensions.EnumValue(DbType.Object)]
        Object,

        [Commons.Extensions.EnumValue(DbType.Single)]
        Single,

        [Commons.Extensions.EnumValue(DbType.String)]
        Text,

        [Commons.Extensions.EnumValue(DbType.String)]
        String,

        [Commons.Extensions.EnumValue(DbType.String)]
        UnlimitedString,

        [Commons.Extensions.EnumValue(DbType.Time)]
        Time,

        [Commons.Extensions.EnumValue(DbType.Xml)]
        Xml
    }

    /// <summary>
    /// Enum que define os tipos de busca de uma entidade referênciada
    /// </summary>
    public enum Fetch
    {
        /// <summary>
        /// Busca "preguiçosa". A entidade será montada apenas com a informação do seu campo chave. As demais informações devem ser consultadas explicitamente.
        /// </summary>
        LAZY,
        /// <summary>
        /// Busca "apressada". A entidade referenciada será montada com todos os seus dados.
        /// </summary>
        EAGER
    }

    /// <summary>
    /// Tipos padrões de procedures
    /// </summary>
    public enum DefaultProceduresKeys
    {
        SEARCH,
        SELECT,
        INSERT,
        UPDATE,
        DELETE
    }

    public enum InheritanceMode
    {
        NoInheritance,
        UniqueTable,
        JoinTables
    }
}
