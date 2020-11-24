using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Marvin.Commons.Extensions;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Marvin.Persistence
{
    //TODO: Implementar o uso de transactions
    /// <summary>
    /// Interface para conexão com Banco de dados
    /// </summary>
    public interface IDatabaseConnection
    {
        /*/// <summary>
        /// Retorna uma conexão com o banco de dados
        /// </summary>
        /// <param name="name">Nome da conexão</param>
        /// <returns>Conexão criada</returns>
        ///IDbConnection GetConnection(string name = null);*/

        /// <summary>
        /// Executa uma query no banco de dados, retornando um DataReader
        /// </summary>
        /// <param name="command">Objeto da query que será executada</param>
        /// <returns>DataReader com resultado da consulta</returns>
        IDataReader ExecuteReaderCommand(IDbCommand command);

        /// <summary>
        /// Executa um comando no banco de dados, retornando um valor escalar
        /// </summary>
        /// <param name="command">Objeto do comando que será executado</param>
        /// <returns>Objeto resultante</returns>
        object ExecuteScalarCommand(IDbCommand command);

        /// <summary>
        /// Executa comando no banco de dados
        /// </summary>
        /// <param name="command">Comando que será executado</param>
        /// <returns>Retorno do comando</returns>
        int ExecuteNonQueryCommand(IDbCommand command);

        /// <summary>
        /// Executa uma query no banco de dados, retornando um DataSet
        /// </summary>
        /// <param name="command">Objeto da query que será executada</param>
        /// <returns>DataSet com resultado da consulta</returns>
        DataSet ExecuteDataSetCommand(IDbCommand command);

        /// <summary>
        /// Retorna um objeto de comando do banco de dados com base em um sql
        /// </summary>
        /// <param name="sql">Comando SQL</param>
        /// <param name="commandType">Tipo de comando</param>
        /// <returns></returns>
        IDbCommand GetCommand(string sql, DatabaseCommandType commandType = DatabaseCommandType.SQL);

        /// <summary>
        /// Retorna um novo objeto Criteria
        /// </summary>
        /// <returns>Novo objeto Criteria</returns>
        ICriteria GetNewCriteria();

        /// <summary>
        /// Adiciona parâmetros em um comando
        /// </summary>
        /// <param name="command">Comando que terá parâmetros adicionados</param>
        /// <param name="parameters">Dicionários de parâmetros</param>
        void AddParameters(IDbCommand command, List<ModelMaps.ProcedureParameter> parameters);

        string GetSelectSQL(string table, List<string> columns, Dictionary<string, object> filters);
        string GetInsertSQL(string table, Dictionary<string, object> values, Dictionary<string, string> sequenceKeys);
        string GetUpdateSQL(string table, Dictionary<string, object> values, Dictionary<string, object> filters);
        string GetDeleteSQL(string table, Dictionary<string, object> filters);

        DatabaseConstantValue GetSequenceNextValue(string sequenceName);
        DatabaseConstantValue GetSequenceCurrentValue(string sequenceName);
    }

    /// <summary>
    /// Classe abstrata para conexão com banco de dados
    /// </summary>
    public abstract class DatabaseConnection : IDatabaseConnection
    {
        #region Atributos estáticos       
        /// <summary>
        /// Retorna string para formatação de comandos INSERT
        /// INSERT INTO [Tabela] [(Colunas)] VALUES([Valores])
        /// </summary>
        public virtual string InsertFormat
        {
            get { return "INSERT INTO {0} ({1}) VALUES({2})"; }
        }

        /// <summary>
        /// Retorna string para formatação de comandos UPDATE
        /// UPDATE [Tabela] SET [Colunas e Vlaores] [Cláusulas]
        /// </summary>
        public virtual string UpdateFormat
        {
            get { return "UPDATE {0} SET {1} {2}"; }
        }

        /// <summary>
        /// Retorna string para formatação de comandos DELETE
        /// DELETE FROM [Tabela] [Cláusulas]
        /// </summary>
        public virtual string DeleteFormat
        {
            get { return "DELETE FROM {0} {1}"; }
        }

        /// <summary>
        /// Retorna string para formatação de comandos SELECT
        /// SELECT [Colunas] FROM [Tabela] [Cláusulas]
        /// </summary>
        public virtual string SelectFormat
        {
            get { return "SELECT {1} FROM {0} {2}"; }
        }

        private static DatabaseProviderFactory _databaseProviderFactory;
        protected static DatabaseProviderFactory DatabaseProviderFactory
        {
            get
            {
                if (_databaseProviderFactory == null)
                    _databaseProviderFactory = new DatabaseProviderFactory();
                return _databaseProviderFactory;
            }
        }
        #endregion

        protected string _dataBaseName;

        protected bool _useTransaction;

        protected int _defaultTimeOut;

        /// <summary>
        /// Objeto do banco de dados.
        /// </summary>
        protected Database _db;

        public DatabaseConnection(string dataBaseName = null){
            _dataBaseName = dataBaseName;
            //DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
            _db = DatabaseProviderFactory.Create(dataBaseName);
        }

        /// <summary>
        /// Retorna um novo objeto Criteria
        /// </summary>
        /// <returns>Novo objeto Criteria</returns>
        public virtual ICriteria GetNewCriteria()
        {
            return new Criteria();
        }

        /// <summary>
        /// Retorna um objeto de comando do banco de dados com base em um sql
        /// </summary>
        /// <param name="sql">Comando SQL</param>
        /// <param name="type">Tipo de comando</param>
        /// <returns></returns>
        public virtual IDbCommand GetCommand(string sql, DatabaseCommandType commandType = DatabaseCommandType.SQL)
        {
            if (commandType == DatabaseCommandType.PROCEDURE)
                return _db.GetStoredProcCommand(sql);
            return _db.GetSqlStringCommand(sql);
        }

        /// <summary>
        /// Executa uma query no banco de dados, retornando um DataReader
        /// </summary>
        /// <param name="command">Objeto da query que será executada</param>
        /// <returns>DataReader com resultado da consulta</returns>
        public virtual IDataReader ExecuteReaderCommand(IDbCommand command)
        {
            return _db.ExecuteReader((command as DbCommand));
        }

        /// <summary>
        /// Executa um comando no banco de dados, retornando um valor escalar
        /// </summary>
        /// <param name="command">Objeto do comando que será executado</param>
        /// <returns>Objeto resultante</returns>
        public virtual object ExecuteScalarCommand(IDbCommand command)
        {
            return _db.ExecuteScalar((command as DbCommand));
        }

        /// <summary>
        /// Executa comando no banco de dados
        /// </summary>
        /// <param name="command">Comando que será executado</param>
        /// <returns>Retorno do comando</returns>
        public virtual int ExecuteNonQueryCommand(IDbCommand command)
        {
            return _db.ExecuteNonQuery((command as DbCommand));
        }

        /// <summary>
        /// Executa uma query no banco de dados, retornando um DataSet
        /// </summary>
        /// <param name="command">Objeto da query que será executada</param>
        /// <returns>DataSet com resultado da consulta</returns>
        public virtual DataSet ExecuteDataSetCommand(IDbCommand command)
        {
            return _db.ExecuteDataSet((command as DbCommand));
        }
        
        /// <summary>
        /// Adiciona parâmetros em um comando
        /// </summary>
        /// <param name="command">Comando que terá parâmetros adicionados</param>
        /// <param name="parameters">Dicionários de parâmetros</param>
        public virtual void AddParameters(IDbCommand command, List<ModelMaps.ProcedureParameter> parameters)
        {
            foreach (ModelMaps.ProcedureParameter parameter in parameters)
            {
                switch (parameter.Direction)
                {
                    case ParameterDirection.Input:
                        _db.AddInParameter((command as DbCommand), parameter.Name, CastDataType(parameter.DataType), parameter.Value);
                        break;
                    case ParameterDirection.Output:
                        _db.AddOutParameter((command as DbCommand), parameter.Name, CastDataType(parameter.DataType), parameter.Size);
                        break;
                    case ParameterDirection.InputOutput:
                    case ParameterDirection.ReturnValue:
                        _db.AddParameter((command as DbCommand), parameter.Name, CastDataType(parameter.DataType), parameter.Direction, null, DataRowVersion.Default, parameter.Value);
                        break;
                }
            }
        }

        public virtual string GetSelectSQL(string table, List<string> columns = null, Dictionary<string, object> filters = null)
        {
            string cols = "*";
            if (columns != null && columns.Count > 0)
                cols = string.Join(",", columns.Select(column => "\"" + column + "\""));
            ICriteria criteria = GetNewCriteria();
            if (filters != null)
            {
                foreach (string col in filters.Keys)
                {
                    criteria.AddEqualTo("\"" + col + "\"", filters[col]);
                }
            }
            return string.Format(SelectFormat, "\"" + table + "\"", cols, criteria.GetClauses());
        }

        public virtual string GetInsertSQL(string table, Dictionary<string, object> values, Dictionary<string, string> sequenceKeys = null)
        {
            ICriteria criteria = GetNewCriteria();
            string cols = sequenceKeys != null ? string.Join(",", sequenceKeys.Keys.Select(key => "\"" + key + "\"")) : "";
            if (!string.IsNullOrEmpty(cols))
                cols += ",";
            cols += string.Join(",", values.Keys.Select(key => "\"" + key + "\""));
            string vals = sequenceKeys != null ? string.Join(", ", sequenceKeys.Values.Select(value => GetSequenceNextValue(value).Value)) : "";
            if (!string.IsNullOrEmpty(vals))
                vals += ",";            
            vals += string.Join(", ", values.Values.Select(value => criteria.ParseFormatValue(value)));
            return string.Format(InsertFormat, "\"" + table + "\"", cols, vals);
        }

        public virtual string GetUpdateSQL(string table, Dictionary<string, object> values, Dictionary<string, object> filters = null)
        {
            ICriteria criteria = GetNewCriteria();
            if (filters != null)
            {
                foreach (string col in filters.Keys)
                {
                    criteria.AddEqualTo("\"" + col + "\"", filters[col]);
                }
            }
            string sets = string.Join(", ", values.Keys.Select(key => "\"" + key + "\" = " + criteria.ParseFormatValue(values[key])));
            return string.Format(UpdateFormat, "\"" + table + "\"", sets, criteria.GetClauses());
        }

        public virtual string GetDeleteSQL(string table, Dictionary<string, object> filters = null)
        {
            ICriteria criteria = GetNewCriteria();
            if (filters != null)
            {
                foreach (string col in filters.Keys)
                {
                    criteria.AddEqualTo("\"" + col + "\"", filters[col]);
                }
            }

            return string.Format(DeleteFormat, "\"" + table + "\"", criteria.GetClauses());
        }

        public DbType CastDataType(DataAnnotations.ERBridge.DataType dataType)
        {
            return (DbType)dataType.GetEnumValue();
        }

        public abstract DatabaseConstantValue GetSequenceNextValue(string sequenceName);
        public abstract DatabaseConstantValue GetSequenceCurrentValue(string sequenceName);
        //static String GetDataBaseEngineType(ModelAttributes.DataType dataType, int length, int precision);
    }

    /// <summary>
    /// Tipos de comandos do Banco de Dados
    /// </summary>
    public enum DatabaseCommandType
    {
        SQL,
        PROCEDURE,
        FUNCTION,
        NONQUERY
    }

    public struct DatabaseConstantValue
    {
        public string Value { get; set; }
    }
}
