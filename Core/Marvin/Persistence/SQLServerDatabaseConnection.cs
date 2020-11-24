using System.Collections.Generic;

namespace Marvin.Persistence
{
    public class SQLServerDatabaseConnection : DatabaseConnection
    {
        protected static Dictionary<DataAnnotations.ERBridge.DataType, string> _dataBaseEngineTypeDictionary = new Dictionary<DataAnnotations.ERBridge.DataType, string>(){
            {DataAnnotations.ERBridge.DataType.Binary, "binary"},
            {DataAnnotations.ERBridge.DataType.Image, "image"},
            {DataAnnotations.ERBridge.DataType.Boolean, "bit"},
            {DataAnnotations.ERBridge.DataType.Byte, "tinyint"},
            {DataAnnotations.ERBridge.DataType.DateTime, "datetime"},
            {DataAnnotations.ERBridge.DataType.Decimal, "decimal"},
            {DataAnnotations.ERBridge.DataType.Money, "money"},
            {DataAnnotations.ERBridge.DataType.Double, "float"},
            {DataAnnotations.ERBridge.DataType.Guid, "uniqueidentifier"},
            {DataAnnotations.ERBridge.DataType.Int16, "smallint"},
            {DataAnnotations.ERBridge.DataType.Int32, "int"},
            {DataAnnotations.ERBridge.DataType.Int64, "bigint"},
            {DataAnnotations.ERBridge.DataType.Object, "sql_variant"},
            {DataAnnotations.ERBridge.DataType.Single, "real"},
            {DataAnnotations.ERBridge.DataType.Text, "text"},
            {DataAnnotations.ERBridge.DataType.String, "varchar"},
            {DataAnnotations.ERBridge.DataType.UnlimitedString, "varchar(max)"},
            {DataAnnotations.ERBridge.DataType.Time, "time"},
            {DataAnnotations.ERBridge.DataType.Xml, "xml"}            
        };

        public SQLServerDatabaseConnection(string dataBaseName = null) : base(dataBaseName) { }

        public override DatabaseConstantValue GetSequenceNextValue(string sequenceName)
        {
            throw new Commons.Exceptions.EssentialsException("This database does not support sequences", "NotImplemented");
        }
        
        public override DatabaseConstantValue GetSequenceCurrentValue(string sequenceName)
        {
            throw new Commons.Exceptions.EssentialsException("This database does not support sequences", "NotImplemented");
        }

        public static string GetDataBaseEngineType(DataAnnotations.ERBridge.DataType dataType, int length = 0, int precision = 0)
        {
            if (_dataBaseEngineTypeDictionary.ContainsKey(dataType))
            {
                string typeName = _dataBaseEngineTypeDictionary[dataType];
                if (length > 0)
                {
                    typeName += "(" + length;
                    if (precision > 0)
                        typeName += "," + precision;
                    typeName += ")";
                }
                return typeName;
            }
            return null;
        }
    }
}
