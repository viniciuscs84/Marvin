using System;
using System.Collections.Generic;
using System.Data;

namespace Marvin.Layers
{
    //TODO: Implementar FETCH
    public class ModelDataAccess
    {
        protected virtual Persistence.Repository GetDefaultRepository(Persistence.ModelMaps.ModelMap modelMap)
        {
            return Persistence.Repository.GetRepository(modelMap.DatabaseName);
        }        
        
        #region Commands
        protected virtual IDbCommand CreateProcedureCommand(Persistence.ModelMaps.ModelMap modelMap, Dictionary<string, object> parameters, string procedureNameKey)
        {
            //TODO:Tratar exceções
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureNameKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            IDbCommand command = repository.DataBaseConnection.GetCommand(procedureMap.Name, Persistence.DatabaseCommandType.PROCEDURE);
            List<Persistence.ModelMaps.ProcedureParameter> procedureParameters = modelMap.GetProcedureParameters(procedureNameKey, parameters);
            repository.DataBaseConnection.AddParameters(command, procedureParameters);
            return command;
        }

        protected virtual IDbCommand CreateSQLSelectCommand(Persistence.ModelMaps.ModelMap modelMap, Dictionary<string, object> parameters)
        {
            //TODO:Tratar exceções
            Dictionary<string, object> filters = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (string parameter in parameters.Keys)
                {
                    Persistence.ModelMaps.ColumnMap columnMap = modelMap.GetColumnMap(parameter);
                    if (columnMap != null)
                        filters.Add(columnMap.Column, parameters[parameter]);
                    else
                        filters.Add(parameter, parameters[parameter]);
                }
            }
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            string sql = repository.DataBaseConnection.GetSelectSQL(modelMap.Table, filters: filters);
            IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.SQL);
            return command;
        }

        protected virtual IDbCommand CreateSQLInsertCommand(Persistence.ModelMaps.ModelMap modelMap, Dictionary<string, object> parameterValues)
        {
            //TODO:Tratar exceções
            Dictionary<string, object> values = new Dictionary<string, object>();
            Dictionary<string, string> sequenceKeys = new Dictionary<string, string>();
            foreach (string parameter in parameterValues.Keys)
            {
                Persistence.ModelMaps.ColumnMap columnMap = modelMap.GetColumnMap(parameter);
                if (columnMap != null)
                {
                    if (!columnMap.IsAutoIncremented)
                        values.Add(columnMap.Column, parameterValues[parameter]);
                    else if (!string.IsNullOrEmpty(columnMap.SequenceName))
                        sequenceKeys.Add(columnMap.Column, columnMap.SequenceName);
                }
            }         
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            string sql = repository.DataBaseConnection.GetInsertSQL(modelMap.Table, values: values, sequenceKeys: sequenceKeys);
            IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.NONQUERY);
            return command;
        }

        protected virtual IDbCommand CreateSQLUpdateCommand(Persistence.ModelMaps.ModelMap modelMap, Dictionary<string, object> parameterValues, Dictionary<string, object> parameters)
        {
            //TODO:Tratar exceções
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (string parameter in parameterValues.Keys)
            {
                Persistence.ModelMaps.ColumnMap columnMap = modelMap.GetColumnMap(parameter);
                if (columnMap != null)
                    values.Add(columnMap.Column, parameterValues[parameter]);
            }

            Dictionary<string, object> filters = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (string parameter in parameters.Keys)
                {
                    Persistence.ModelMaps.ColumnMap columnMap = modelMap.GetColumnMap(parameter);
                    if (columnMap != null)
                        filters.Add(columnMap.Column, parameters[parameter]);
                    else
                        filters.Add(parameter, parameters[parameter]);
                }
            }

            Persistence.Repository repository = GetDefaultRepository(modelMap);
            string sql = repository.DataBaseConnection.GetUpdateSQL(modelMap.Table, values, filters: filters);
            IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.NONQUERY);
            return command;
        }

        protected virtual IDbCommand CreateSQLDeleteCommand(Persistence.ModelMaps.ModelMap modelMap, Dictionary<string, object> parameters)
        {
            //TODO:Tratar exceções            
            Dictionary<string, object> filters = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (string parameter in parameters.Keys)
                {
                    Persistence.ModelMaps.ColumnMap columnMap = modelMap.GetColumnMap(parameter);
                    if (columnMap != null)
                        filters.Add(columnMap.Column, parameters[parameter]);
                    else
                        filters.Add(parameter, parameters[parameter]);
                }
            }
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            string sql = repository.DataBaseConnection.GetDeleteSQL(modelMap.Table, filters: filters);
            IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.NONQUERY);
            return command;
        }
        #endregion

        protected void FetchReferences<TModel>(TModel model, bool forceToEager = false)
            where TModel : class, IModel
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(model.GetType());
            foreach (Persistence.ModelMaps.ReferenceMapInfo reference in modelMap.GetReferencesMapList())
            {
                if (forceToEager || reference.FetchType == DataAnnotations.ERBridge.Fetch.EAGER)
                {
                    object referencedModel = model.GetType().GetProperty(reference.Property).GetValue(model, null);
                    if (referencedModel != null)
                    {
                        object keyValue = referencedModel.GetType().GetProperty(reference.ReferenceKey).GetValue(referencedModel, null);
                        referencedModel = Select<IModel>(new Dictionary<string, object>() { { reference.ReferenceKey, keyValue } }, modelType: reference.ReferencedModelType);
                        model.GetType().GetProperty(reference.Property).SetValue(model, referencedModel, null);
                    }
                }
            }

            foreach (Persistence.ModelMaps.CollectionReferenceMapInfo collReference in modelMap.GetCollectionsReferencesMapList())
            {
                if (forceToEager || collReference.FetchType == DataAnnotations.ERBridge.Fetch.EAGER)
                {
                    FillCollectionReference(model, collReference.Property);
                }
            }
        }

        public ModelCollection<TModel> Search<TModel>(Dictionary<string, object> parameters = null, Type modelType = null, string procedureKey = null, bool forceNonProcedureCommand = false)
            where TModel : class, IModel
        {
            //TODO:Tratar exceções
            modelType = (modelType == null) ? typeof(TModel) : modelType;
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(modelType);
            //Verifica se a entidade foi configurada com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.SEARCH.ToString() : procedureKey;
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command = (procedureMap == null || forceNonProcedureCommand) ? CreateSQLSelectCommand(modelMap, parameters) : CreateProcedureCommand(modelMap, parameters, procedureKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            ModelCollection<TModel> result = repository.SearchModels<TModel>(command, modelType);

            if (procedureMap != null && procedureMap.Parameters.ContainsKey("TotalRows"))
            {
                Dictionary<string, object> returnedValues = repository.ReadOutputParameters(command);
                if (returnedValues.ContainsKey(procedureMap.Parameters["TotalRows"].Name))
                    result.TotalRows = int.Parse(returnedValues[procedureMap.Parameters["TotalRows"].Name].ToString());
            }
            return result;
        }

        public TModel Select<TModel>(Dictionary<string, object> parameters, bool forceToEager = false, Type modelType = null, string procedureKey = null, bool forceNonProcedureCommand = false)
            where TModel : class, IModel
        {
            //TODO:Tratar exceções
            modelType = (modelType == null) ? typeof(TModel) : modelType;
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(modelType);
            //Verifica se a entidade foi configurada com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.SELECT.ToString() : procedureKey;
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command = (procedureMap == null || forceNonProcedureCommand) ? CreateSQLSelectCommand(modelMap, parameters) : CreateProcedureCommand(modelMap, parameters, procedureKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            TModel result = repository.SelectModel<TModel>(command, modelType);
            if(result != null)
                FetchReferences(result, forceToEager);
            return result;
        }        

        public void Insert<TModel>(TModel model, string procedureKey = null, bool forceNonProcedureCommand = false)
        {
            //TODO:Tratar exceções
            //TODO:Recuperar ID
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(model.GetType());
            Dictionary<string, object> modelValues = Persistence.ModelMaps.ModelMap.GetModelValues(model);
            //Verifica se a entidade foi configurada com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.INSERT.ToString() : procedureKey;
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command = (procedureMap == null || forceNonProcedureCommand) ? CreateSQLInsertCommand(modelMap, modelValues) : CreateProcedureCommand(modelMap, modelValues, procedureKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            repository.DataBaseConnection.ExecuteScalarCommand(command);
            if (procedureMap != null)
            {
                Dictionary<string, object> returnedValues = repository.ReadOutputParameters(command);
                UpdateProcedureReturnedFields(model, returnedValues, procedureMap);
            }
        }

        public void Update<TModel>(TModel model, string procedureKey = null, bool forceNonProcedureCommand = false)
        {
            //TODO:Tratar exceções
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(model.GetType());
            //Verifica se a entidade foi configurada com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.UPDATE.ToString() : procedureKey;
            Dictionary<string, object> modelValues = Persistence.ModelMaps.ModelMap.GetModelValues(model);
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command;
            if (procedureMap == null || forceNonProcedureCommand)
            {
                Dictionary<string, object> keyParameters = new Dictionary<string, object>();
                foreach (string key in modelMap.GetKeys())
                {
                    keyParameters.Add(key, modelValues[key]);
                    modelValues.Remove(key);                    
                }
                command = CreateSQLUpdateCommand(modelMap, modelValues, keyParameters);
            }
            else
                command = CreateProcedureCommand(modelMap, modelValues, procedureKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            repository.DataBaseConnection.ExecuteNonQueryCommand(command);

            if (procedureMap != null)
            {
                Dictionary<string, object> returnedValues = repository.ReadOutputParameters(command);
                UpdateProcedureReturnedFields(model, returnedValues, procedureMap);
            }
        }


        public void Delete<TModel>(Dictionary<string, object> parameters, Type modelType = null, string procedureKey = null, bool forceNonProcedureCommand = false)
        {
            //TODO:Tratar exceções
            modelType = (modelType == null) ? typeof(TModel) : modelType;
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(modelType);            
            //Verifica se a entidade foi configurado com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.DELETE.ToString() : procedureKey;
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command = (procedureMap == null || forceNonProcedureCommand) ? CreateSQLDeleteCommand(modelMap, parameters) : CreateProcedureCommand(modelMap, parameters, procedureKey);            
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            repository.DataBaseConnection.ExecuteNonQueryCommand(command);
        }

        public void DeleteModel<TModel>(TModel model, string procedureKey = null, bool forceNonProcedureCommand = false)
        {
            //TODO:Tratar exceções
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(model.GetType());
            Dictionary<string, object> modelValues = Persistence.ModelMaps.ModelMap.GetModelValues(model);
            //Verifica se a entidade foi configurada com uma procedure padronizada de busca
            procedureKey = string.IsNullOrEmpty(procedureKey) ? DataAnnotations.ERBridge.DefaultProceduresKeys.DELETE.ToString() : procedureKey;
            Persistence.ModelMaps.ProcedureMapInfo procedureMap = modelMap.GetProcedureMap(procedureKey);
            IDbCommand command;
            if (procedureMap == null || forceNonProcedureCommand)
            {
                Dictionary<string, object> keyParameters = new Dictionary<string, object>();
                foreach (string key in modelMap.GetKeys())
                {
                    keyParameters.Add(key, modelValues[key]);
                }
                command = CreateSQLDeleteCommand(modelMap, keyParameters);
            }
            else
                command = CreateProcedureCommand(modelMap, modelValues, procedureKey);
            Persistence.Repository repository = GetDefaultRepository(modelMap);
            repository.DataBaseConnection.ExecuteNonQueryCommand(command);

            if (procedureMap != null)
            {
                Dictionary<string, object> returnedValues = repository.ReadOutputParameters(command);
                UpdateProcedureReturnedFields(model, returnedValues, procedureMap);
            }
        }

        public void FillCollectionReference<TModel>(TModel model, string collectionName, Type collectionItemType = null)
            where TModel : class, IModel
        {
            //TODO:Tratar exceções
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));            
            Persistence.ModelMaps.CollectionReferenceMapInfo collReferenceMap = modelMap.GetCollectionReferenceMap(collectionName);
            if (collReferenceMap != null)
            {
                if (collectionItemType == null)
                    collectionItemType = collReferenceMap.CollectionItemType;

                Persistence.ModelMaps.ModelMap itemModellMap = Persistence.ModelMaps.ModelMap.GetModelMap(collectionItemType);

                object modelKey = typeof(TModel).GetProperty(modelMap.GetKeys()[0]).GetValue(model, null);

                Persistence.Criteria criteria = new Persistence.Criteria();

                if (string.IsNullOrEmpty(collReferenceMap.AssociationTableName))
                    criteria.AddEqualTo("\"" + itemModellMap.GetColumnName(collReferenceMap.ItemReferenceKey) + "\"", modelKey);
                else
                {
                    criteria.AddInnerJoin(collReferenceMap.AssociationTableName, "\"" + itemModellMap.Table + "\".\"" + itemModellMap.GetColumnName(itemModellMap.GetKeys()[0]) + "\"", "\"" + collReferenceMap.AssociationTableName + "\".\"" + collReferenceMap.SecundaryColumnKey + "\"");
                    criteria.AddEqualTo("\"" + collReferenceMap.AssociationTableName + "\".\"" + collReferenceMap.MainColumnKey + "\"", modelKey);
                }

                Persistence.Repository repository = GetDefaultRepository(itemModellMap);
                string sql = repository.DataBaseConnection.GetSelectSQL(itemModellMap.Table);
                sql = criteria.CompleteSQL(sql);
                IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.SQL);

                //Type collectionType = typeof(ModelCollection<>).MakeGenericType(collectionItemType);
                //object collection = Activator.CreateInstance(collectionType);

                ModelCollection<IModel> result = repository.SearchModels<IModel>(command, collectionItemType);                

                typeof(TModel).GetProperty(collectionName).SetValue(model, Activator.CreateInstance(typeof(TModel).GetProperty(collectionName).PropertyType));

                result.ForEach(item => typeof(TModel).GetProperty(collectionName).PropertyType.GetMethod("Add").Invoke(typeof(TModel).GetProperty(collectionName).GetValue(model, null), new object[] { item }));

                //typeof(TModel).GetProperty(collectionName).PropertyType.GetMethod("Add").Invoke(typeof(TModel).GetProperty(collectionName).GetValue(model, null), new object[] { repository.SearchModels<object>(command, collectionItemType) });
            }
        }

        public void AddIntoCollectionReference<TModel, TItem>(TModel model, TItem item, string collectionName)
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));            
            Persistence.ModelMaps.CollectionReferenceMapInfo collReferenceMap = modelMap.GetCollectionReferenceMap(collectionName);
            if (collReferenceMap != null)
            {
                Persistence.ModelMaps.ModelMap itemModelMap = Persistence.ModelMaps.ModelMap.GetModelMap(collReferenceMap.CollectionItemType);
                if (string.IsNullOrEmpty(collReferenceMap.AssociationTableName))
                {
                    typeof(TItem).GetProperty(collReferenceMap.ItemReferenceKey).SetValue(item, model, null);
                    Update(item);
                }
                else
                {
                    object modelKey = typeof(TModel).GetProperty(modelMap.GetKeys()[0]).GetValue(model, null);
                    object itemKey = typeof(TItem).GetProperty(itemModelMap.GetKeys()[0]).GetValue(item, null);

                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add(collReferenceMap.MainColumnKey, modelKey);
                    values.Add(collReferenceMap.SecundaryColumnKey, itemKey);

                    Persistence.Repository repository = GetDefaultRepository(modelMap);
                    string sql = repository.DataBaseConnection.GetInsertSQL(collReferenceMap.AssociationTableName, values: values);
                    IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.NONQUERY);
                    repository.DataBaseConnection.ExecuteScalarCommand(command);
                }
            }           
        }

        public void RemoveFromCollectionReference<TModel, TItem>(TModel model, TItem item, string collectionName)
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));
            Persistence.ModelMaps.CollectionReferenceMapInfo collReferenceMap = modelMap.GetCollectionReferenceMap(collectionName);
            if (collReferenceMap != null)
            {
                Persistence.ModelMaps.ModelMap itemModelMap = Persistence.ModelMaps.ModelMap.GetModelMap(collReferenceMap.CollectionItemType);
                if (string.IsNullOrEmpty(collReferenceMap.AssociationTableName))
                {
                    typeof(TItem).GetProperty(collReferenceMap.ItemReferenceKey).SetValue(item, null, null);
                    Update(item);
                }
                else
                {
                    object modelKey = typeof(TModel).GetProperty(modelMap.GetKeys()[0]).GetValue(model, null);
                    object itemKey = typeof(TItem).GetProperty(itemModelMap.GetKeys()[0]).GetValue(item, null);

                    Dictionary<string, object> filters = new Dictionary<string, object>();
                    filters.Add(collReferenceMap.MainColumnKey, modelKey);
                    filters.Add(collReferenceMap.SecundaryColumnKey, itemKey);

                    Persistence.Repository repository = GetDefaultRepository(modelMap);
                    string sql = repository.DataBaseConnection.GetDeleteSQL(collReferenceMap.AssociationTableName, filters: filters);
                    IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.NONQUERY);
                    repository.DataBaseConnection.ExecuteScalarCommand(command);
                }
            }           
        }

        public void ClearCollectionReference<TModel>(TModel model, string collectionName)
        {
            //TODO:Tratar exceções
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));
            Persistence.ModelMaps.CollectionReferenceMapInfo collReferenceMap = modelMap.GetCollectionReferenceMap(collectionName);
            if (collReferenceMap != null)
            {
                Persistence.ModelMaps.ModelMap itemModelMap = Persistence.ModelMaps.ModelMap.GetModelMap(collReferenceMap.CollectionItemType);

                object modelKey = typeof(TModel).GetProperty(modelMap.GetKeys()[0]).GetValue(model, null);

                Persistence.Criteria criteria = new Persistence.Criteria();
                Persistence.Repository repository = GetDefaultRepository(itemModelMap);

                string table = string.IsNullOrEmpty(collReferenceMap.AssociationTableName) ? itemModelMap.Table : collReferenceMap.AssociationTableName;
                string column = string.IsNullOrEmpty(collReferenceMap.AssociationTableName) ? itemModelMap.GetColumnName(collReferenceMap.ItemReferenceKey) : collReferenceMap.MainColumnKey;
                string sql = repository.DataBaseConnection.GetDeleteSQL(table);

                criteria.AddEqualTo(column, modelKey);
                
                sql = criteria.CompleteSQL(sql);
                IDbCommand command = repository.DataBaseConnection.GetCommand(sql, Persistence.DatabaseCommandType.SQL);
                repository.DataBaseConnection.ExecuteNonQueryCommand(command);
                
                Type collectionType = typeof(ModelCollection<>).MakeGenericType(collReferenceMap.CollectionItemType);
                object collection = Activator.CreateInstance(collectionType);
                typeof(TModel).GetProperty(collectionName).SetValue(model, collection, null);
            }
        }

        public void UpdateProcedureReturnedFields<TModel>(TModel model, Dictionary<string, object> returnedValues, Persistence.ModelMaps.ProcedureMapInfo procedureMap)
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(model.GetType());
            foreach (string key in procedureMap.Parameters.Keys)
            {
                Persistence.ModelMaps.ProcedureParameter parameter = procedureMap.Parameters[key];
                if (parameter.Direction != ParameterDirection.Input && returnedValues.ContainsKey(parameter.Name) && modelMap.GetColumnMap(key) != null)
                {
                    typeof(TModel).GetProperty(key).SetValue(model, returnedValues[parameter.Name], null);
                }
            }
        }
    }
}
