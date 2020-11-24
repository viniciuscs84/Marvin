using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define os atributos das colunas de uma entidade no banco de dados.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Column : Attribute
    {
        #region Atributos e Propiedades
        public string Name { get; private set; }

        public DataType DataType { get; private set; }

        public bool Required { get; private set; }

        public bool AutoIncremented { get; private set; }

        public string SequenceName { get; private set; }

        public int Length { get; private set; }

        public int Precision { get; private set; }
        #endregion

        #region Construtor
        /// <summary>
        /// Cria um novo atributo de coluna.
        /// </summary>
        /// <param name="name">Nome da coluna no banco de dados.</param>
        /// <param name="dataType">Tipo de dado armazenado na coluna.</param>
        /// <param name="required">Aceita valores nulos ou não.</param>
        /// <param name="autoIncremented">Coluna com auto-incremento ou não.</param>
        /// <param name="sequence">Nome da sequência de auto-incremento da coluna.</param>
        /// <param name="length">Tamamnho do campo</param>
        public Column(string name, DataType dataType, bool required = false, bool autoIncremented = false, string sequenceName = null, int length = 0, int precision = 0)
        {
            Name = name;
            DataType = dataType;
            Required = required;
            AutoIncremented = autoIncremented;
            SequenceName = sequenceName;
            Length = length;
            Precision = precision;
        }
        #endregion        
    }
}