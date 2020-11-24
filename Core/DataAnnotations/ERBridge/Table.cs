using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define as informações da tabela da entidade no banco de dados.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Table : Attribute
    {
        #region Atributos e Propiedades
        public string Name { get; private set; }

        public string DatabaseName { get; private set; }

        public InheritanceMode InheritanceMode { get; private set; }

        public string ParentTableReferenceColumn { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo atributo de tabela.
        /// </summary>
        /// <param name="name">Nome da tabela.</param>
        /// <param name="name">Nome do banco de dados em que a tabela se encontra</param>
        public Table(string name, string databaseName = null, InheritanceMode inheritanceMode = InheritanceMode.UniqueTable, string parentTableReferenceColumn = null)
        {
            Name = name;
            DatabaseName = databaseName;
            InheritanceMode = inheritanceMode;
            ParentTableReferenceColumn = parentTableReferenceColumn;
        }
        #endregion
    }
}
