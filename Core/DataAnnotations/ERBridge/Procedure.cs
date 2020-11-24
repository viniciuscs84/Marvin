using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe abstrata que define os atributos de procedures padrões da entidade
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class Procedure : Attribute
    {
        #region Atributos e Propiedades
        public string ProcedureKey { get; private set; }

        public string Name { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo atributo de procedure padronizada
        /// </summary>
        /// <param name="procedureKey">Chave de identificação</param>
        /// <param name="name">Nome da procedure</param>
        /// <param name="parameters">dicionário de parâmetros da procedure (campo, parâmetro)</param>
        public Procedure(string procedureKey, string name)
        {
            //TODO: Tratar exceções
            ProcedureKey = procedureKey;
            Name = name;
        }
        #endregion
    }
}