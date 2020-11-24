using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos da procedure de seleção da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SelectProcedure : Procedure
    {
        public SelectProcedure(string name) : base(DefaultProceduresKeys.SELECT.ToString(), name) { }
    }

    public class SelectProcedureParameter : ProcedureParameter
    {
        public SelectProcedureParameter(string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
            : base(DefaultProceduresKeys.SELECT.ToString(), field, name, dataType, defaultValue, direction, size) { }
    }
}
