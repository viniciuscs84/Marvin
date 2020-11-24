using System.Configuration;

namespace Marvin.Config
{
    /// <summary>
    /// Seção de configuração do Essentials
    /// </summary>
    public class Settings : ConfigurationSection
    {
        /// <summary>
        /// Coleções de classes de conexão para bancos de dados
        /// </summary>
        [ConfigurationProperty("registerDatabaseConnectionClasses")]
        public DatabaseConnectionElementCollection DatabaseConnectionClasses
        {
            get { return ((DatabaseConnectionElementCollection)(base["registerDatabaseConnectionClasses"])); }
            set { base["registerDatabaseConnectionClasses"] = value; }
        }


        [ConfigurationProperty("xmlModelsPath", DefaultValue = "models.xml", IsKey = false, IsRequired = false)]
        public string XmlModelsPath
        {
            get { return (string)base["xmlModelsPath"]; }
            set { base["xmlModelsPath"] = value; }
        }
    }

    /// <summary>
    /// Collection de elementos que representam classes de conexão do banco de dados
    /// </summary>
    [ConfigurationCollection(typeof(DatabaseConnectionElement))]
    public class DatabaseConnectionElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseConnectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DatabaseConnectionElement)(element)).ProviderName;
        }

        public DatabaseConnectionElement GetDatabaseConnectionClass(string key)
        {
            return (DatabaseConnectionElement)BaseGet(key);
        }
    }

    /// <summary>
    /// Elementos que representa classe de conexão do banco de dados
    /// </summary>
    public class DatabaseConnectionElement : ConfigurationElement
    {
        /// <summary>
        /// Nome da classe
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Tipo do provider
        /// </summary>
        [ConfigurationProperty("providerName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ProviderName
        {
            get { return (string)base["providerName"]; }
            set { base["providerName"] = value; }
        }
    }
}
