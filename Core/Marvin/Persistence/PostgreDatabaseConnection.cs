using System;

namespace Marvin.Persistence
{
    public class PostgreDatabaseConnection : DatabaseConnection
    {
        public PostgreDatabaseConnection(string dataBaseName = null) : base(dataBaseName) { }

        public override DatabaseConstantValue GetSequenceNextValue(string sequenceName)
        {
            return new DatabaseConstantValue() { Value = "nextval(" + sequenceName + ")" };
        }
        
        public override DatabaseConstantValue GetSequenceCurrentValue(string sequenceName)
        {
            return new DatabaseConstantValue() { Value = "currval(" + sequenceName + ")" };
        }

        public static string GetDataBaseEngineType(DataAnnotations.ERBridge.DataType dataType, int length = 0, int precision = 0)
        {
            throw new NotImplementedException("Not Implemented");
        }
    }
}
