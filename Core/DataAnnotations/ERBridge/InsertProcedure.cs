using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos da procedure de inserção da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InsertProcedure : Procedure
    {
        public InsertProcedure(string name) : base(DefaultProceduresKeys.INSERT.ToString(), name) { }
    }

    public class InsertProcedureParameter : ProcedureParameter
    {
        public InsertProcedureParameter(string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
            : base(DefaultProceduresKeys.INSERT.ToString(), field, name, dataType, defaultValue, direction, size) { }
    }
}