using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos da procedure de exclusão da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DeleteProcedure : Procedure
    {
        public DeleteProcedure(string name) : base(DefaultProceduresKeys.DELETE.ToString(), name) { }
    }

    public class DeleteProcedureParameter : ProcedureParameter
    {
        public DeleteProcedureParameter(string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
            : base(DefaultProceduresKeys.DELETE.ToString(), field, name, dataType, defaultValue, direction, size) { }
    }
}
