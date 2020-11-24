using System;

namespace Marvin.Persistence
{
    public class OracleDatabaseConnection : DatabaseConnection
    {
        public OracleDatabaseConnection(string dataBaseName = null) : base(dataBaseName) { }

        public override DatabaseConstantValue GetSequenceNextValue(string sequenceName)
        {
            return new DatabaseConstantValue() { Value = sequenceName+ ".NEXTVAL" };
        }
        
        public override DatabaseConstantValue GetSequenceCurrentValue(string sequenceName)
        {
            return new DatabaseConstantValue() { Value = sequenceName + ".CURRVAL" };
        }

        public override ICriteria GetNewCriteria()
        {
            return new OracleCriteria();
        }

        public static string GetDataBaseEngineType(DataAnnotations.ERBridge.DataType dataType, int length = 0, int precision = 0)
        {
            throw new NotImplementedException("Not Implemented");
        }
    }
}
