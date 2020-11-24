using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define as informações de referência de uma entidade em outra.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Reference : Attribute
    {
        #region Atributos e Propiedades
        public string ReferenceKey { get; private set; }

        public Fetch FetchType { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo atributo de referência
        /// </summary>
        /// <param name="referenceKey">Campo chave da referência na entidade referenciada</param>
        /// <param name="fetchType">Tipo de busca aplicada à entidade referenciada</param>
        public Reference(string referenceKey, Fetch fetchType = Fetch.LAZY)
        {
            ReferenceKey = referenceKey;
            FetchType = fetchType;
        }
        #endregion        
    }
}