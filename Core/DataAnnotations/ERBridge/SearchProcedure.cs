using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos da procedure de busca da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SearchProcedure : Procedure
    {
        public SearchProcedure(string name) : base(DefaultProceduresKeys.SEARCH.ToString(), name) { }
    }

    public class SearchProcedureParameter : ProcedureParameter
    {
        public SearchProcedureParameter(string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
            : base(DefaultProceduresKeys.SEARCH.ToString(), field, name, dataType, defaultValue, direction, size) { }
    }

    public class SearchProcedureTotalRowsOutParameter : SearchProcedureParameter
    {
        public SearchProcedureTotalRowsOutParameter(string name)
            : base("TotalRows", name, DataType.Int32, null, System.Data.ParameterDirection.Output) { }
    }
}
