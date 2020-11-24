using System;

namespace Marvin.DataAnnotations.ERBridge
{
    /// <summary>
    /// Classe que define as informações de referência de uma collection para uma entidade.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CollectionReference : Attribute
    {
        #region Atributos e Propiedades
        public Fetch FetchType { get; private set; }

        public string ItemReferenceKey { get; private set; }

        public string AssociationTableName { get; private set; }

        public string MainColumnKey { get; private set; }

        public string SecundaryColumnKey { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo atributo de referência de uma coleção.
        /// </summary>
        /// <param name="collectionFetchType">Tipo de busca aplicada à coleção</param>
        /// <param name="itemFetchType">Tipo de busca aplicada aos itens da coleção</param>
        /// <param name="itemReferenceKey">Campo de referência dos itens da coleção para a entidade principal (quando não houver tabela de associação)</param>
        /// <param name="associationTableName">Tabela de associação multipla entre as entidade</param>
        /// <param name="mainColumnKey">Nome da coluna chave que referencia a entidade principal (quando houver tabela de associação)</param>
        /// <param name="secundaryColumnKey">Nome da coluna chave que referencia os itens da coleção (quando houver tabela de associação)</param>
        public CollectionReference(Fetch collectionFetchType = Fetch.LAZY, string itemReferenceKey = null, string associationTableName = null, string mainColumnKey = null, string secundaryColumnKey = null)
        {
            FetchType = collectionFetchType;
            ItemReferenceKey = itemReferenceKey;
            AssociationTableName = associationTableName;
            MainColumnKey = mainColumnKey;
            SecundaryColumnKey = secundaryColumnKey;
        }
        #endregion
    }
}