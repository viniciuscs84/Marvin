using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Configuration;

namespace Marvin.Persistence.ModelMaps
{
     
    /// <summary>
    /// Classe que realiza o mapeamento entre uma Entidade e uma tabela do banco.
    /// </summary>
    public class ModelMap
    {
        /// <summary>
        /// Dicionário estático de entidades mapeadas
        /// </summary>
        private static Dictionary<Type, ModelMap> _modelsMap = new Dictionary<Type, ModelMap>();

        /// <summary>
        /// Retorna um mapeamento da entidade em questão
        /// </summary>
        /// <param name="type">Tipo da entidade a ser mapeada</param>
        /// <returns>Mapeamento da entidade</returns>
        public static ModelMap GetModelMap(Type type)
        {
            ModelMap modelMap = null;
            if (_modelsMap.ContainsKey(type))
                modelMap = _modelsMap[type];
            else
            {
                try
                {
                    modelMap = LoadModelXml(type);

                    if (modelMap == null)
                        modelMap = LoadModelAttributes(type);

                    if (modelMap != null)
                    {
                        modelMap._baseModelMap = GetModelMap(type.BaseType);
                        _modelsMap.Add(type, modelMap);

                        if (modelMap.InheritanceMode == DataAnnotations.ERBridge.InheritanceMode.UniqueTable)
                        {
                            if (modelMap._baseModelMap == null && type.BaseType.IsSubclassOf(typeof(Layers.Model)))
                                _modelsMap.Add(type.BaseType, modelMap);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Commons.Utilities.Logger.Error(ex);
                }
            }

            return modelMap;
        }

        /// <summary>
        /// Preenche o mapeamentos das colunas da entidade
        /// </summary>
        /// <param name="type">Tipo da entidade</param>
        /// <param name="modelMap">Mapeamento da entidade</param>
        public static void FillColumnMap(Type type, ModelMap modelMap)
        {
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {

                object[] att = property.GetCustomAttributes(typeof(DataAnnotations.ERBridge.Column), true);
                DataAnnotations.ERBridge.Column columnAtt = (att != null && att.Length > 0) ? (DataAnnotations.ERBridge.Column)att[0] : null;

                att = property.GetCustomAttributes(typeof(DataAnnotations.ERBridge.Reference), true);
                DataAnnotations.ERBridge.Reference referenceAtt = (att != null && att.Length > 0) ? (DataAnnotations.ERBridge.Reference)att[0] : null;

                att = property.GetCustomAttributes(typeof(DataAnnotations.ERBridge.CollectionReference), true);
                DataAnnotations.ERBridge.CollectionReference collectionAtt = (att != null && att.Length > 0) ? (DataAnnotations.ERBridge.CollectionReference)att[0] : null;

                att = property.GetCustomAttributes(typeof(KeyAttribute), true);
                KeyAttribute keyAtt = (att != null && att.Length > 0) ? (KeyAttribute)att[0] : null;

                if (columnAtt != null)
                {
                    ColumnMap columnMap = new ColumnMap(property.Name, columnAtt.Name, columnAtt.DataType, columnAtt.AutoIncremented, columnAtt.SequenceName, columnAtt.Required, (keyAtt != null), (referenceAtt != null), columnAtt.Length, columnAtt.Precision);
                    modelMap.ColumnsMap.Add(columnMap.Property, columnMap);                    

                    if (columnMap.IsReference)
                    {
                        ReferenceMapInfo referenceMap = new ReferenceMapInfo(property.Name, property.PropertyType, referenceAtt.ReferenceKey, referenceAtt.FetchType);
                        modelMap.ReferencesMap.Add(referenceMap.Property, referenceMap);
                        modelMap.Columns.Add(columnMap.Column, columnMap.Property + "." + referenceMap.ReferenceKey);
                        modelMap.ColumnsMap.Add(columnMap.Property + "." + referenceMap.ReferenceKey, columnMap);
                        if (columnMap.IsKey)
                            modelMap.Keys.Add(columnMap.Property + "." + referenceMap.ReferenceKey);
                    }
                    else
                    {
                        modelMap.Columns.Add(columnMap.Column, columnMap.Property);
                        if (columnMap.IsKey)
                            modelMap.Keys.Add(columnMap.Property);
                    }
                }

                if (collectionAtt != null)
                {
                    Type itemType = (property.PropertyType.IsGenericType) ? property.PropertyType.GetGenericArguments()[0] : null;
                    if (itemType != null)
                    {
                        CollectionReferenceMapInfo collectionMap = new CollectionReferenceMapInfo(property.Name, itemType, collectionAtt.ItemReferenceKey, collectionAtt.FetchType,  collectionAtt.AssociationTableName, collectionAtt.MainColumnKey, collectionAtt.SecundaryColumnKey);
                        modelMap.CollectionsReferencesMap.Add(property.Name, collectionMap);
                    }
                }
            }
        }

        public static Dictionary<string, object> GetModelValues<TModel>(TModel model)
        {
            //TODO:Tratar exceções
            Type modelType = model.GetType();
            ModelMap modelMap = ModelMap.GetModelMap(modelType);
            Dictionary<string, object> modelValues = new Dictionary<string, object>();
            foreach (string propertyName in modelMap.GetPropertiesNamesList())
            {
                try
                {
                    object value = modelType.GetProperty(propertyName).GetValue(model, null);
                    if (value != null && modelMap.GetColumnMap(propertyName).DataType == DataAnnotations.ERBridge.DataType.Xml)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.InnerXml = Commons.Utilities.ClassToXML.SerializeObject(value, propertyName);
                        value = xml.FirstChild.InnerXml;
                    }
                    else if (modelMap.GetColumnMap(propertyName).DataType == DataAnnotations.ERBridge.DataType.DateTime && DateTime.MinValue.Equals(value))
                        value = null;
                    
                    ReferenceMapInfo referenceMap = modelMap.GetReferencesMap(propertyName);
                    if (value != null && referenceMap != null)
                    {
                        object keyValue = value.GetType().GetProperty(referenceMap.ReferenceKey).GetValue(value, null);
                        modelValues.Add(propertyName + "." + referenceMap.ReferenceKey, keyValue);
                    }
                    else
                    {
                        modelValues.Add(propertyName, value);
                    }
                }
                catch (Exception ex){
                    Commons.Utilities.Logger.Error(ex);
                }
            }
            return modelValues;
        }

        private static ModelMap LoadModelXml(Type type)
        {
            try
            {
                Config.Settings essentialsSettings = (Config.Settings)ConfigurationManager.GetSection("essentialsSettings");
                XmlDocument xml = new XmlDocument();
                xml.Load(essentialsSettings.XmlModelsPath);
                XmlNode modelNode = xml.SelectSingleNode("/Models/Model[@type='" + type.FullName + "']");

                modelNode = (modelNode == null) ? xml.SelectSingleNode("/Models/Model[@baseModelType='" + type.FullName + "']") : modelNode;
                
                if (modelNode != null)
                {
                    ModelMap modelMap = new ModelMap(table: modelNode.Attributes["table"].Value);
                    if (modelNode.Attributes["dataBaseName"] != null)
                        modelMap._databaseName = modelNode.Attributes["dataBaseName"].Value;
                    if (modelNode.Attributes["inheritanceMode"] != null && modelNode.Attributes["inheritanceMode"].Value == "JoinTables")
                    {
                        modelMap.InheritanceMode = DataAnnotations.ERBridge.InheritanceMode.JoinTables;
                        modelMap.ParentTableReferenceColumn = modelNode.Attributes["parentTableReferenceColumn"].Value;
                    }

                    //Colunas
                    foreach (XmlNode columnNode in modelNode.SelectNodes("ColumnMap"))
                    {
                        if (type.GetProperty(columnNode.Attributes["property"].Value) != null)
                        {
                            ColumnMap columnMap = new ColumnMap(
                                property: columnNode.Attributes["property"].Value,
                                column: columnNode.Attributes["column"].Value,
                                dataType: (DataAnnotations.ERBridge.DataType)Enum.Parse(typeof(DataAnnotations.ERBridge.DataType),
                                columnNode.Attributes["dbType"].Value),
                                isAutoIncremented: (columnNode.Attributes["isAutoIncremented"] != null ? bool.Parse(columnNode.Attributes["isAutoIncremented"].Value) : false),
                                sequenceName: (columnNode.Attributes["sequenceName"] != null ? columnNode.Attributes["sequenceName"].Value : null),
                                isRequired: (columnNode.Attributes["isRequired"] != null ? bool.Parse(columnNode.Attributes["isRequired"].Value) : false),
                                isKey: (columnNode.Attributes["isKey"] != null ? bool.Parse(columnNode.Attributes["isKey"].Value) : false),
                                isReference: (columnNode.Attributes["isReference"] != null ? bool.Parse(columnNode.Attributes["isReference"].Value) : false));

                            modelMap.ColumnsMap.Add(columnMap.Property, columnMap);

                            if (columnMap.IsKey)
                                modelMap.Keys.Add(columnMap.Property);

                            //Referências
                            if (columnMap.IsReference)
                            {
                                XmlNode referenceNode = modelNode.SelectSingleNode("ReferenceMap[@property='" + columnMap.Property + "']");
                                ReferenceMapInfo referenceMap = new ReferenceMapInfo(
                                    property: referenceNode.Attributes["property"].Value,
                                    referenceKey: referenceNode.Attributes["referenceKey"].Value,
                                    fetchType: columnNode.Attributes["fetchType"] != null ? (DataAnnotations.ERBridge.Fetch)Enum.Parse(typeof(DataAnnotations.ERBridge.Fetch), columnNode.Attributes["fetchType"].Value) : DataAnnotations.ERBridge.Fetch.LAZY,
                                    referencedModelType: type.GetProperty(referenceNode.Attributes["property"].Value).PropertyType
                                );
                                modelMap.ReferencesMap.Add(referenceMap.Property, referenceMap);
                                modelMap.ColumnsMap.Add(columnMap.Property + "." + referenceMap.ReferenceKey, columnMap);
                                modelMap.Columns.Add(columnMap.Column, columnMap.Property + "." + referenceMap.ReferenceKey);
                            }
                            else
                            {
                                modelMap.Columns.Add(columnMap.Column, columnMap.Property);
                            }
                        }
                    }

                    //Coleções
                    foreach (XmlNode collectionNode in modelNode.SelectNodes("ColletionReferenceMap"))
                    {
                        if (type.GetProperty(collectionNode.Attributes["property"].Value) != null)
                        {
                            System.Reflection.PropertyInfo property = type.GetProperty(collectionNode.Attributes["property"].Value);
                            Type itemType = (property.PropertyType.IsGenericType) ? property.PropertyType.GetGenericArguments()[0] : null;
                            if (itemType != null)
                            {
                                CollectionReferenceMapInfo collectionMap = new CollectionReferenceMapInfo(
                                    property: property.Name,
                                    collectionItemType: itemType,
                                    itemReferenceKey: collectionNode.Attributes["itemReferenceKey"].Value,
                                    collectionFetchType: collectionNode.Attributes["fetchType"] != null ? (DataAnnotations.ERBridge.Fetch)Enum.Parse(typeof(DataAnnotations.ERBridge.Fetch), collectionNode.Attributes["fetchType"].Value) : DataAnnotations.ERBridge.Fetch.LAZY,
                                    associationTableName: (collectionNode.Attributes["associationTable"] != null ? collectionNode.Attributes["associationTable"].Value : null),
                                    mainColumnKey: (collectionNode.Attributes["mainColumnKey"] != null ? collectionNode.Attributes["mainColumnKey"].Value : null),
                                    secundaryColumnKey: (collectionNode.Attributes["secundaryColumnKey"] != null ? collectionNode.Attributes["secundaryColumnKey"].Value : null)
                                );
                                modelMap.CollectionsReferencesMap.Add(property.Name, collectionMap);
                            }
                        }
                    }

                    //Procedures
                    foreach (XmlNode procedureNode in modelNode.SelectNodes("ProcedureMap"))
                    {
                        ProcedureMapInfo procedureMap = new ProcedureMapInfo(procedureNode.Attributes["name"].Value);
                        foreach (XmlNode parameterNode in procedureNode.SelectNodes("Parameter"))
                        {
                            ProcedureParameter parameter = new ProcedureParameter();
                            parameter.Name = parameterNode.Attributes["name"].Value;
                            parameter.DataType = (DataAnnotations.ERBridge.DataType)Enum.Parse(typeof(DataAnnotations.ERBridge.DataType), parameterNode.Attributes["dbType"].Value);
                            parameter.DefaultValue = parameterNode.Attributes["defaultValue"] != null ? parameterNode.Attributes["defaultValue"].Value : null;
                            parameter.Direction = parameterNode.Attributes["direction"] != null ? (ParameterDirection)Enum.Parse(typeof(ParameterDirection), parameterNode.Attributes["direction"].Value): ParameterDirection.Input;
                            parameter.Size = parameterNode.Attributes["size"] != null ? int.Parse(parameterNode.Attributes["size"].Value) : 0;
                            procedureMap.Parameters.Add(parameterNode.Attributes["field"].Value, parameter);
                        }
                        modelMap.ProceduresMap.Add(procedureNode.Attributes["procedureKey"].Value, procedureMap);
                    }                
                    return modelMap;
                }
            }
            catch(Exception ex) 
            {
                Console.Write(ex);
            }
            return null;            
        }

        private static ModelMap LoadModelAttributes(Type type)
        {
            object[] att = type.GetCustomAttributes(typeof(DataAnnotations.ERBridge.Table), true);
            DataAnnotations.ERBridge.Table tableAtt = (att != null && att.Length > 0) ? (DataAnnotations.ERBridge.Table)att[0] : null;
            ModelMap baseModelMap = GetModelMap(type.BaseType);
            if (tableAtt != null || baseModelMap != null)
            {
                ModelMap modelMap = new ModelMap();
                if (tableAtt != null)
                {
                    modelMap._table = tableAtt.Name;
                    modelMap._databaseName = tableAtt.DatabaseName;
                    if (tableAtt.InheritanceMode == DataAnnotations.ERBridge.InheritanceMode.JoinTables && baseModelMap != null)
                    {
                        modelMap.InheritanceMode = tableAtt.InheritanceMode;
                        modelMap.ParentTableReferenceColumn = tableAtt.ParentTableReferenceColumn;
                    }
                }

                FillColumnMap(type, modelMap);

                att = type.GetCustomAttributes(typeof(DataAnnotations.ERBridge.Procedure), true);
                if (att != null)
                {
                    foreach (DataAnnotations.ERBridge.Procedure proc in att)
                    {
                        if (!modelMap.ProceduresMap.ContainsKey(proc.ProcedureKey))
                            modelMap.ProceduresMap.Add(proc.ProcedureKey, new ProcedureMapInfo(proc.Name));
                    }
                }

                att = type.GetCustomAttributes(typeof(DataAnnotations.ERBridge.ProcedureParameter), true);
                if (att != null)
                {
                    foreach (DataAnnotations.ERBridge.ProcedureParameter param in att)
                    {
                        if (!modelMap.ProceduresMap[param.ProcedureKey].Parameters.ContainsKey(param.Field))                       
                            modelMap.ProceduresMap[param.ProcedureKey].Parameters.Add(param.Field, new ProcedureParameter() { Name = param.Name, DataType = param.DataType, DefaultValue = param.DefaultValue, Direction = param.Direction, Size = param.Size });
                    }
                }
                return modelMap;
            }
            return null;
        }        

        /// <summary>
        /// Nome da tabela no banco de dados
        /// </summary>
        private string _table;
        /// <summary>
        /// Retorna o nome da tabela no banco de dados
        /// </summary>
        public string Table
        {
            get { return  (string.IsNullOrEmpty(_table) && _baseModelMap != null) ? _baseModelMap._table : _table; }
        }

        /// <summary>
        /// Armazena o nome do banco de dados em que a tabela se encontra.
        /// </summary>
        private string _databaseName;
        /// <summary>
        /// Devolve o nome do banco de dados em que a tabela se encontra.
        /// </summary>
        public string DatabaseName
        {
            get { return (string.IsNullOrEmpty(_databaseName) && _baseModelMap != null) ? _baseModelMap._databaseName : _databaseName; }
        }

        private DataAnnotations.ERBridge.InheritanceMode InheritanceMode { get; set; }

        private string ParentTableReferenceColumn { get; set; }

        protected Dictionary<string, ColumnMap> ColumnsMap { get; private set; }

        protected Dictionary<string, string> Columns { get; private set; }

        protected List<string> Keys { get; private set; }

        protected Dictionary<string, ReferenceMapInfo> ReferencesMap { get; private set; }

        protected Dictionary<string, CollectionReferenceMapInfo> CollectionsReferencesMap { get; private set; }

        protected Dictionary<string, ProcedureMapInfo> ProceduresMap { get; private set; }

        private ModelMap _baseModelMap;

        /// <summary>
        /// Cria um novo mapeamento para uma entidade
        /// </summary>
        /// <param name="table">Nome da tabela</param>
        /// <param name="columnsMap">Dicionário com mapeamento das colunas "Nome da propriedade" x ColumnMap</param>
        /// <param name="columns">Dicionário coluna x propriedade</param>
        /// <param name="keys">Lista de propriedades chaves da entidade</param>
        /// <param name="referencesMap">Dicionário com mapa de referências  da entidade "Nome da propriedade" x ReferenceMapInfo</param>
        /// <param name="collectionsReferencesMap">Dicionário com o mapa de referÊncias das coleções  da entidade "Nome da propriedade" x CollectionReferenceMapInfo</param>
        /// <param name="databaseName">Nome do banco onde a tabela está armazenada</param>
        private ModelMap(string table = null, Dictionary<string, ColumnMap> columnsMap = null, Dictionary<string, string> columns = null, List<string> keys = null, Dictionary<string, ReferenceMapInfo> referencesMap = null, Dictionary<string, CollectionReferenceMapInfo> collectionsReferencesMap = null, string databaseName = null, Dictionary<string, ProcedureMapInfo> proceduresMap = null, DataAnnotations.ERBridge.InheritanceMode inheritanceMode = DataAnnotations.ERBridge.InheritanceMode.NoInheritance, string parentTableReferenceColumn = null)
        {
            _table = table;
            _databaseName = databaseName;
            ColumnsMap = columnsMap == null ? new Dictionary<string, ColumnMap>() : columnsMap;
            Columns = columns == null ? new Dictionary<string, string>() : columns;
            Keys = keys == null ? new List<string>() : keys;
            ReferencesMap = referencesMap == null ? new Dictionary<string, ReferenceMapInfo>() : referencesMap;
            CollectionsReferencesMap = collectionsReferencesMap == null ? new Dictionary<string, CollectionReferenceMapInfo>() : collectionsReferencesMap;
            ProceduresMap = proceduresMap == null ? new Dictionary<string, ProcedureMapInfo>() : proceduresMap;
            InheritanceMode = inheritanceMode;
            ParentTableReferenceColumn = parentTableReferenceColumn;
        }

        /// <summary>
        /// Retorna o conjunto de parâmetros da procedure com os respectivos valores
        /// </summary>
        /// <param name="procudureMapKey">Chave do Mapa da procedure</param>
        /// <param name="fieldParameters">Dicionários de campos e valores</param>
        /// <returns>Dicionários de parâmetros e valores</returns>
        public List<ProcedureParameter> GetProcedureParameters(string procudureMapKey, Dictionary<string, object> fieldParameters)
        {
            //TODO:Tratar exceções
            List<ProcedureParameter> procedureParameters = new List<ProcedureParameter>();
            ProcedureMapInfo procedureMap = GetProcedureMap(procudureMapKey);
            foreach (string field in procedureMap.Parameters.Keys)
            {
                ProcedureParameter parameter = procedureMap.Parameters[field];
                if ((fieldParameters != null && fieldParameters.ContainsKey(field)) || parameter.DefaultValue != null || parameter.Direction != ParameterDirection.Input)
                {
                    ProcedureParameter newParameter = new ProcedureParameter() { Name = parameter.Name, DataType = parameter.DataType, Value = parameter.DefaultValue, Direction = parameter.Direction, Size = parameter.Size };
                    if (fieldParameters != null && fieldParameters.ContainsKey(field) && fieldParameters[field] != null)
                        if (fieldParameters[field].GetType() != typeof(DateTime) || Convert.ToDateTime(fieldParameters[field]) != DateTime.MinValue)
                            newParameter.Value = fieldParameters[field];
                    procedureParameters.Add(newParameter);
                }
            }
            return procedureParameters;
        }

        public ProcedureMapInfo GetProcedureMap(string procedureKey)
        {
            if (ProceduresMap.ContainsKey(procedureKey))
                return ProceduresMap[procedureKey];
            if (_baseModelMap != null)
                return _baseModelMap.GetProcedureMap(procedureKey);
            return null;
        }

        public List<ProcedureMapInfo> GetProceduresMapList() {
            List<ProcedureMapInfo> list = ProceduresMap.Values.ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetProceduresMapList());
            return list;
        }

        public ColumnMap GetColumnMap(string propertyName)
        {
            if (ColumnsMap.ContainsKey(propertyName))
                return ColumnsMap[propertyName];
            if (_baseModelMap != null)
                return _baseModelMap.GetColumnMap(propertyName);
            return null;
        }

        public ColumnMap GetMappedColumnMap(string propertyName)
        {
            ColumnMap mappedColumnMap = GetColumnMap(propertyName);
            if (mappedColumnMap == null)
            {
                string[] propertyParts = propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (propertyParts.Length > 1)
                {
                    ReferenceMapInfo reference = GetReferencesMapList().FirstOrDefault(r => r.Property == propertyParts[0]);
                    if (reference != null)
                    {
                        ModelMap referenceModelMap = ModelMap.GetModelMap(reference.ReferencedModelType);
                        if (referenceModelMap != null)
                            mappedColumnMap = referenceModelMap.GetColumnMap(propertyName.Remove(0, (propertyParts[0] + ".").Length));
                    }
                }
            }
            return mappedColumnMap;
        }

        public List<ColumnMap> GetColumnsMapList()
        {
            List<ColumnMap> list = ColumnsMap.Values.Distinct().ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetColumnsMapList());
            return list;
        }

        public string GetPropertyName(string columnName)
        {
            if (Columns.ContainsKey(columnName))
                return Columns[columnName];
            if (_baseModelMap != null)
                return _baseModelMap.GetPropertyName(columnName);
            return null;
        }

        public string GetMappedPropertyName(string column)
        {
            string mappedPropertyName = GetPropertyName(column);
            if (string.IsNullOrEmpty(mappedPropertyName))
            {
                string[] columnParts = column.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (columnParts.Length > 1)
                {
                    foreach (ReferenceMapInfo reference in GetReferencesMapList())
                    {
                        ModelMap referenceModelMap = ModelMap.GetModelMap(reference.ReferencedModelType);
                        if (referenceModelMap != null && (referenceModelMap.Table == columnParts[0] || columnParts[0] == reference.Property))
                        {
                            mappedPropertyName = reference.Property;
                            mappedPropertyName += "." + referenceModelMap.GetMappedPropertyName(column.Remove(0, (columnParts[0] + ".").Length));
                            break;
                        }
                    }                    
                }
            }
            return mappedPropertyName;
        }

        public List<string> GetPropertiesNamesList()
        {
            List<string> list = ColumnsMap.Keys.ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetPropertiesNamesList());
            return list;
        }

        public string GetColumnName(string propertyName)
        {
            ColumnMap columnMap = GetColumnMap(propertyName);
            if (columnMap != null)
                return columnMap.Column;
            return null;
        }

        public List<string> ColumnsNamesList()
        {
            List<string> list = Columns.Keys.ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.ColumnsNamesList().Where(i => !list.Contains(i)));
            return list;
        }

        public CollectionReferenceMapInfo GetCollectionReferenceMap(string collectionName)
        {
            if (CollectionsReferencesMap.ContainsKey(collectionName))
                return CollectionsReferencesMap[collectionName];
            if (_baseModelMap != null)
                return _baseModelMap.GetCollectionReferenceMap(collectionName);
            return null;
        }

        public List<CollectionReferenceMapInfo> GetCollectionsReferencesMapList()
        {
            List<CollectionReferenceMapInfo> list = CollectionsReferencesMap.Values.ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetCollectionsReferencesMapList());
            return list;
        }

        public ReferenceMapInfo GetReferencesMap(string referenceName)
        {
            if (ReferencesMap.ContainsKey(referenceName))
                return ReferencesMap[referenceName];
            if (_baseModelMap != null)
                return _baseModelMap.GetReferencesMap(referenceName);
            return null;
        }

        public List<ReferenceMapInfo> GetReferencesMapList()
        {
            List<ReferenceMapInfo> list = ReferencesMap.Values.ToList();
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetReferencesMapList());
            return list;
        }

        public List<string> GetKeys(){
            List<string> list = new List<string>();
            list.AddRange(Keys);
            if (_baseModelMap != null)
                list.AddRange(_baseModelMap.GetKeys().Where(i => !list.Contains(i)));
            return list;
        }
    }
    
    /// <summary>
    /// Realiza o mapeamento entre um propriedade da entidade e uma coluna da tabela
    /// </summary>
    public class ColumnMap
    {
        #region Atributos e Propiedades
        public string Property { get; private set; }

        public string Column { get; private set; }

        public DataAnnotations.ERBridge.DataType DataType { get; private set; }

        public bool IsAutoIncremented { get; private set; }

        public string SequenceName { get; private set; }

        public bool IsRequired { get; private set; }

        public bool IsKey { get; private set; }

        public bool IsReference { get; private set; }

        public int Length { get; private set; }

        public int Precision { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria uma nova estrutura de mapeamento entro a propriedade da entidade e uma coluna da tabela.
        /// </summary>
        /// <param name="property">Nome da propriedade</param>
        /// <param name="column">Nome da coluna</param>
        /// <param name="dataType">Tipo de dado da coluna</param>
        /// <param name="isAutoIncremented">Define se a coluna é autoincrementada</param>
        /// <param name="sequenceName">Nome da sequência da coluna</param>
        /// <param name="isRequired">Define se a coluna é requerida</param>
        /// <param name="isKey">Define se a coluna é uma chave da tebela</param>
        /// <param name="isReference">Define a coluna é uma chave de refrência</param>
        public ColumnMap(string property, string column, DataAnnotations.ERBridge.DataType dataType, bool isAutoIncremented = false, string sequenceName = null, bool isRequired = false, bool isKey = false, bool isReference = false, int length = 0, int precision = 0)
        {
            Property = property;
            Column = column;
            DataType = dataType;
            IsAutoIncremented = isAutoIncremented;
            SequenceName = sequenceName;
            IsRequired = isRequired;
            IsKey = isKey;
            IsReference = isReference;
            Length = length;
            Precision = precision;
        }
        #endregion
    }

    /// <summary>
    /// Armazena informações de mapeamento para referência entre tabelas.
    /// </summary>
    public class ReferenceMapInfo
    {
        #region Atributos e Propiedades
        public string Property { get; private set; }

        public Type ReferencedModelType { get; private set; }

        public string ReferenceKey { get; private set; }

        public DataAnnotations.ERBridge.Fetch FetchType { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo mapeamento de refrência entre entidades
        /// </summary>
        /// <param name="property">Nome da propriedade</param>
        /// <param name="referencedModelType">Tipo da entidade referenciada</param>
        /// <param name="referenceKey">Propriedade chave na entidade referênciada</param>
        /// <param name="fetchType">Tipo de busca aplicada à entidade refrênciada</param>
        public ReferenceMapInfo(string property, Type referencedModelType, string referenceKey, DataAnnotations.ERBridge.Fetch fetchType = DataAnnotations.ERBridge.Fetch.LAZY)
        {
            Property = property;
            ReferencedModelType = referencedModelType;
            ReferenceKey = referenceKey;
            FetchType = fetchType;
        }
        #endregion
    }

    /// <summary>
    /// Armazena informações de mapeamento para referência em uma coleção de entidades.
    /// </summary>
    public class CollectionReferenceMapInfo
    {
        #region Atributos e Propiedades
        public string Property { get; private set; }

        public Type CollectionItemType { get; private set; }

        public DataAnnotations.ERBridge.Fetch FetchType { get; private set; }

        public string ItemReferenceKey { get; private set; }

        public string AssociationTableName { get; private set; }

        public string MainColumnKey { get; private set; }

        public string SecundaryColumnKey { get; private set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um novo mapeamento de referências em uma coleção de entidades.
        /// </summary>
        /// <param name="property">Nome da prpriedade</param>
        /// <param name="collectionItemType">Tipo do item da coleção</param>
        /// <param name="itemReferenceKey">Campo de referência dos itens da coleção para a entidade principal (quando não houver tabela de associação)</param> 
        /// <param name="collectionFetchType">Tipo de busca aplicada à coleção</param>
        /// <param name="itemFetchType">Tipo de busca aplicada aos itens da coleção</param>        
        /// <param name="associationTableName">Tabela de associação multipla entre as entidades</param>
        /// <param name="mainColumnKey">Nome da coluna chave que referencia a entidade principal (quando houver tabela de associação)</param>
        /// <param name="secundaryColumnKey">Nome da coluna chave que referencia os itens da coleção (quando houver tabela de associação)</param>
        public CollectionReferenceMapInfo(string property, Type collectionItemType, string itemReferenceKey, DataAnnotations.ERBridge.Fetch collectionFetchType = DataAnnotations.ERBridge.Fetch.LAZY, string associationTableName = null, string mainColumnKey = null, string secundaryColumnKey = null)
        {
            Property = property;
            CollectionItemType = collectionItemType;
            ItemReferenceKey = itemReferenceKey;
            FetchType = collectionFetchType;
            AssociationTableName = associationTableName;
            MainColumnKey = mainColumnKey;
            SecundaryColumnKey = secundaryColumnKey;
        }
        #endregion
    }

    /// <summary>
    /// Mapeamento de procedures padrão da entidade
    /// </summary>
    public class ProcedureMapInfo
    {
        public string Name { get; private set; }

        public Dictionary<string, ProcedureParameter> Parameters { get; private set; }

        /// <summary>
        /// Cria um novo mapeamento de precedures padrão da entidade
        /// </summary>
        /// <param name="name">Nome da preocedure</param>
        /// <param name="parameters">Parâmetros da preocedure</param>
        public ProcedureMapInfo(string name)
        {
            Name = name;
            Parameters = new Dictionary<string, ProcedureParameter>();
        }
    }

    public class ProcedureParameter
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public object Value { get; set; }
        public DataAnnotations.ERBridge.DataType DataType { get; set; }
        public ParameterDirection Direction { get; set; }
        public int Size { get; set; }
    }    
}
