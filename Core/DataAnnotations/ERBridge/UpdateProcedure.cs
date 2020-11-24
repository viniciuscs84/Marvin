using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos da procedure de atualização da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UpdateProcedure : Procedure
    {
        public UpdateProcedure(string name) : base(DefaultProceduresKeys.UPDATE.ToString(), name) { }
    }

    public class UpdateProcedureParameter : ProcedureParameter
    {
        public UpdateProcedureParameter(string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
            : base(DefaultProceduresKeys.UPDATE.ToString(), field, name, dataType, defaultValue, direction, size) { }
    }
}