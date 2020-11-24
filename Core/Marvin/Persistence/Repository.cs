using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Xml;

using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

using Marvin.Commons.Extensions;

namespace Marvin.Persistence
{
    /// <summary>
    /// Classe de manipulação dos dados
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Dicionários estáticos dos repositórios criados
        /// </summary>
        private static Dictionary<string, Repository> _repositories = new Dictionary<string, Repository>();

        /// <summary>
        /// Seleciona o objeto de conexão seguindo o provider do banco selecionado
        /// </summary>
        /// <param name="dataBaseName">Nome do banco de dados</param>
        /// <returns>Objeto de conexão com o banco de dados</returns>
        private static DatabaseConnection SelectDatabaseConnection(string dataBaseName)
        {
            //TODO: Tratar exceções
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[dataBaseName];
            Config.Settings essentialsSettings = (Config.Settings)ConfigurationManager.GetSection("essentialsSettings");
            Config.DatabaseConnectionElement databaseConnectionElement = essentialsSettings.DatabaseConnectionClasses.GetDatabaseConnectionClass(connectionString.ProviderName);
            DatabaseConnection connection = (DatabaseConnection)Activator.CreateInstance(Type.GetType(databaseConnectionElement.Name), dataBaseName);
            return connection;
        }       

        /// <summary>
        /// Retorna um repositório
        /// </summary>
        /// <param name="dataBaseName">Nome do banco de dados</param>
        /// <returns>Retorna o objeto de repositório associado ao banco de dados</returns>
        public static Repository GetRepository(string dataBaseName = null){
            //TODO: Tratar exceções            
            if (string.IsNullOrEmpty(dataBaseName))
            {
                DatabaseSettings databaseSettings = (DatabaseSettings)ConfigurationManager.GetSection("dataConfiguration");
                dataBaseName = databaseSettings.DefaultDatabase;
            }
            if(!_repositories.ContainsKey(dataBaseName))
                _repositories.Add(dataBaseName, new Repository(dataBaseName));
            return _repositories[dataBaseName];
        }

        public DatabaseConnection DataBaseConnection { get; private set; }
        
        /// <summary>
        /// Construtor estático para garantir instâncias unicas do repositório para cada banco de dados
        /// </summary>
        /// <param name="dataBaseName"></param>
        private Repository(string dataBaseName)
        {
            DataBaseConnection = SelectDatabaseConnection(dataBaseName);
        }        

        /// <summary>
        /// Executa comando de busca de por registros da entidade
        /// </summary>
        /// <param name="command">Comando de busca</param>
        /// <param name="modelType">Tipo da entidade</param>
        /// <returns>Retorna coleção com objetos encontrados</returns>
        public Layers.ModelCollection<TModel> SearchEntities<TModel>(IDbCommand command, Type modelType = null)
            where TModel : class, Layers.IModel
        {
            //TODO:Tratar exceções
            Layers.ModelCollection<TModel> results = new Layers.ModelCollection<TModel>();
            int totalRows = -1;
            using (IDataReader dr = DataBaseConnection.ExecuteReaderCommand(command))
            {
                while (dr.Read() || (dr.NextResult() && dr.Read()))
                {
                    try
                    {
                        if (dr["totalRows"] != DBNull.Value)
                            totalRows = Convert.ToInt32(dr["totalRows"]);
                    }
                    catch { }
                    TModel item = GetModelObjectByDataReader<TModel>(dr, modelType);
                    if(item != null)
                        results.Add(item);
                }
            }
            results.TotalRows = (totalRows == -1) ? results.Count : totalRows;
            return results;
        }

        /// <summary>
        /// Executa comando de busca por um registro da entidade
        /// </summary>
        /// <param name="command">Comando de busca</param>
        /// <param name="modelType">Tipo da entidade</param>
        /// <returns>Objeto da entidade encontrada</returns>
        public TModel SelectModel<TModel>(IDbCommand command, Type modelType = null)
            where TModel : class, Layers.IModel
        {
            //TODO:Tratar exceções
            TModel result = default(TModel);
            using (IDataReader dr = DataBaseConnection.ExecuteReaderCommand(command))
            {
                if (dr.Read())
                    result = GetModelObjectByDataReader<TModel>(dr, modelType);
            }            
            return result;
        }        

        /// <summary>
        /// Retorna um objeto da entidade montada a partir de um DataReader e do seu mapeamento
        /// </summary>
        /// <param name="dr">DataReader</param>
        /// <param name="modelType">Tipo da entidade</param>
        /// <returns>Objeto da entidade</returns>
        public TModel GetModelObjectByDataReader<TModel>(IDataReader dr, Type modelType = null)
            where TModel : class, Layers.IModel
        {
            if (modelType == null)
                modelType = typeof(TModel);
            //TODO:Tratar exceções
            TModel result = default(TModel);
            ModelMaps.ModelMap modelMap = ModelMaps.ModelMap.GetModelMap(modelType);
            if (modelMap != null)
            {
                XmlDocument docXml = new XmlDocument();
                docXml.AppendChild(docXml.CreateXmlDeclaration("1.0", null, null));
                XmlElement root = docXml.CreateElement(modelType.GetTypeName());
                docXml.AppendChild(root);
                bool isNullRow = true;
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string propertyName = modelMap.GetMappedPropertyName(dr.GetName(i));
                    if (!string.IsNullOrEmpty(propertyName) && dr.GetValue(i) != DBNull.Value)
                    {
                        isNullRow = false;
                        XmlNode nodeToAppend = root;
                        string mappedNode = propertyName;

                        foreach (string nodeName in mappedNode.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (nodeToAppend.SelectSingleNode(nodeName) != null)
                                nodeToAppend = nodeToAppend.SelectSingleNode(nodeName);
                            else
                            {
                                nodeToAppend.AppendChild(docXml.CreateElement(nodeName));
                                nodeToAppend = nodeToAppend.LastChild;
                            }
                        }
                        string val = dr[i].ToString();
                        double num;
                        if (double.TryParse(val.ToString(), out num) && dr[i].GetType() != typeof(string))
                            val = num.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        else if (dr[i].GetType() == typeof(DateTime))
                            val = dr.GetDateTime(i).ToString("yyyy-MM-ddTHH:mm:ss");                        
                        else if (dr[i].GetType() == typeof(bool))
                            val = val.ToLower();
                        else if (dr[i].GetType() == typeof(byte[]))
                            val = Commons.Utilities.ClassToXML.SerealizeBytes((dr[i] as Byte[]));
                        if (modelMap.GetMappedColumnMap(propertyName).DataType == DataAnnotations.ERBridge.DataType.Xml)
                        {
                            try
                            {
                                nodeToAppend.InnerXml = val;
                            }
                            catch
                            {
                                nodeToAppend.InnerText = val;
                            }
                        }
                        else
                        {
                            nodeToAppend.InnerText = val;
                        }
                    }
                }
                if(!isNullRow)
                    result = (TModel)Commons.Utilities.ClassToXML.DeserializeXML(docXml.InnerXml, modelType);
                result.IsReady = true;
            }
            return result;
        }

        public virtual Dictionary<string, object> ReadOutputParameters(IDbCommand command)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (IDbDataParameter parameter in command.Parameters)
            {
                if (parameter.Direction != ParameterDirection.Input)
                    values.Add(parameter.ParameterName, parameter.Value);
            }
            return values;
        }
    }
}
