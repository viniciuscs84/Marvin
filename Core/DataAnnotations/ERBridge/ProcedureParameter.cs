using System;

namespace Marvin.DataAnnotations.ERBridge
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ProcedureParameter : Attribute
    {
        #region Atributos e Propiedades
        public string ProcedureKey { get; private set; }

        public string Name { get; private set; }

        public string Field { get; private set; }

        public object DefaultValue { get; private set; }

        public DataType DataType { get; private set; }

        public System.Data.ParameterDirection Direction { get; private set; }

        public int Size { get; private set; }
        #endregion

        #region Construtores
        public ProcedureParameter(string procedureKey, string field, string name, DataType dataType, object defaultValue = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input, int size = 0)
        {
            ProcedureKey = procedureKey;
            Field = field;
            Name = name;
            DataType = dataType;
            DefaultValue = defaultValue;
            Direction = direction;
            Size = size;
        }
        #endregion
    }

}