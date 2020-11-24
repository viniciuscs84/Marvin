using System;
using System.Collections.Generic;
using System.Text;

using Marvin.Commons.Extensions;

namespace Marvin.Persistence
{
    public interface ICriteria
    {
        #region Filtros
        /// <summary>
        /// Adiciona condição de igualdade à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddEqualTo(string col, object value);

        /// <summary>
        /// Adiciona condição de diferença à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor excluido</param>
        void AddNotEqualTo(string col, object value);

        /// <summary>
        /// Adiciona condição de menor que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddLessThan(string col, object value);

        /// <summary>
        /// Adiciona condição de igualdade ou menor que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddLessOrEqualTo(string col, object value);

        /// <summary>
        /// Adiciona condição de maior que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddGreaterThan(string col, object value);

        /// <summary>
        /// Adiciona condição de igualdade ou maior que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddGreaterOrEqualTo(string col, object value);

        /// <summary>
        /// Adiciona uma expressão de like.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddLike(string col, string value);

        /// <summary>
        /// Adiciona uma expressão de like à esquerda.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddLeftLike(string col, string value);

        /// <summary>
        /// Adiciona uma expressão de like à direita.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddRightLike(string col, string value);

        /// <summary>
        /// Adiciona uma expressão de like exata.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddExactLike(string col, string value);

        /// <summary>
        /// Adiciona uma expressão de like exata.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        void AddBetween(string col, object startValue, object finshValue);

        /// <summary>
        /// Adiciona uma expressão entre.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="values">conjunto de valores</param>
        void AddIn(string col, List<object> values);

        /// <summary>
        /// Adiciona uma expressão não entre.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="values">conjunto de valores</param>
        void AddNotIn(string col, List<object> values);

        /// <summary>
        /// Adiciona condição para valor nulo
        /// </summary>
        /// <param name="col">Nome da coluna</param>
        void AddIsNull(string col);

        /// <summary>
        /// Adiciona condição para valor não nulo
        /// </summary>
        /// <param name="col">Nome da coluna</param>
        void AddIsNotNull(string col);

        /// <summary>
        /// Adiciona uma expressão ao filtro
        /// </summary>
        /// <param name="expression">nova expressão</param>
        void AddExpression(string expression);

        /// <summary>
        /// Realiza operação de ou com outro critério de busca.
        /// </summary>
        /// <param name="criteria">Outro critério de busca.</param>	
        void AddOrCriteria(ICriteria criteria);
        #endregion

        #region Joins
        /// <summary>
        /// Adiciona uma expressão de join
        /// </summary>
        /// <param name="table">Nome da tabela</param>
        /// <param name="expression">Expressão do JOIN</param>
        /// <param name="type">Tipo do JOIN</param>
        void AddJoin(string table, string expression, JoinType type = JoinType.INNER);

        /// <summary>
        /// Adiciona uma expressão de INNER JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        void AddInnerJoin(string table, string mainKey, string foreignKey);

        /// <summary>
        /// Adiciona uma expressão de LEFT JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        void AddLeftJoin(string table, string mainKey, string foreignKey);

        /// <summary>
        /// Adiciona uma expressão de RIGHT JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        void AddRightJoin(string table, string mainKey, string foreignKey);

        /// <summary>
        /// Adiciona uma expressão de FULL JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        void AddFullJoin(string table, string mainKey, string foreignKey);
        #endregion        

        /// <summary>
        /// Monta expressão de busca (clasula where). 
        /// </summary>
        /// <returns>Expressão</returns>
        string MountsExpression();

        /// <summary>
        /// Retorna o filtro da consulta.
        /// </summary>
        /// <returns>as clausulas da consulta.</returns>
        string GetClauses();

        /// <summary>
        /// Formata um valor para que ele atenda a formatação do tipo no banco de dados. 
        /// </summary>
        /// <param name="objValue">Objeto que deve ser formatados</param>
        /// <returns>Valor formatado</returns>
        string ParseFormatValue(object objValue);

        /// <summary>
        /// Adiciona a clasulas à consulta
        /// </summary>
        /// <param name="sql">sql sem clasulas</param>
        /// <returns>sql com clasulas</returns>
        string CompleteSQL(string sql);
    }
    
    /// <summary>
    /// Classe que traz abstração das clasulas de filtro em consultas sql.	
    /// </summary>
    public class Criteria : ICriteria
    {
        #region Propriedades e Atributos
        protected string OrderBy { get; set; }

        protected string GroupBy { get; set; }

        /// <summary>
        /// Armazena o conjunto de expressões do critério.
        /// </summary>
        protected List<string> _expressions = new List<string>();

        /// <summary>
        /// Armazena outros critérios para realiza operações de OU;
        /// </summary>
        protected List<ICriteria> _orCriterias = new List<ICriteria>();

        /// <summary>
        /// Armazena o conjunto de junções da consulta.
        /// </summary>
        protected List<string> _joins = new List<string>();
        #endregion

        #region Filtros
        /// <summary>
        /// Adiciona condição de igualdade à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddEqualTo(string col, object value)
        {
            if (value == null)
                AddIsNull(col);
            else
                AddExpression(col + " = " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona condição de diferença à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor excluido</param>
        public void AddNotEqualTo(string col, object value)
        {
            if (value == null)
                AddIsNotNull(col);
            else
                AddExpression(col + " <> " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona condição de menor que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddLessThan(string col, object value)
        {
            AddExpression(col + " < " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona condição de igualdade ou menor que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddLessOrEqualTo(string col, object value)
        {
            AddExpression(col + " <= " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona condição de maior que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddGreaterThan(string col, object value)
        {
            AddExpression(col + " > " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona condição de igualdade ou maior que à consulta. 
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddGreaterOrEqualTo(string col, object value)
        {
            AddExpression(col + " >= " + ParseFormatValue(value));
        }

        /// <summary>
        /// Adiciona uma expressão de like.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddLike(string col, string value)
        {
            AddExpression(col + " like '%" + value + "%'");
        }

        /// <summary>
        /// Adiciona uma expressão de like à esquerda.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddLeftLike(string col, string value)
        {
            AddExpression(col + " like '" + value + "%'");
        }

        /// <summary>
        /// Adiciona uma expressão de like à direita.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddRightLike(string col, string value)
        {
            AddExpression(col + " like '%" + value + "'");
        }

        /// <summary>
        /// Adiciona uma expressão de like exata.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddExactLike(string col, string value)
        {
            AddExpression(col + " like '" + value + "'");
        }

        /// <summary>
        /// Adiciona uma expressão de like exata.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="value">valor pretendido</param>
        public void AddBetween(string col, object startValue, object finshValue)
        {
            AddExpression(col + " between " + ParseFormatValue(startValue) + " AND " + ParseFormatValue(finshValue));
        }

        /// <summary>
        /// Adiciona uma expressão entre.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="values">conjunto de valores</param>
        public void AddIn(string col, List<object> values)
        {
            //TODO: Usar string.Join
            if (values != null && values.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerator<object> iValues = values.GetEnumerator();
                iValues.MoveNext();
                sb.Append(col + " in (" + ParseFormatValue(iValues.Current));
                while (iValues.MoveNext())
                {
                    sb.AppendFormat(", {0}", ParseFormatValue(iValues.Current));
                }
                sb.Append(")");
                AddExpression(sb.ToString());
            }
        }

        /// <summary>
        /// Adiciona uma expressão não entre.
        /// </summary>
        /// <param name="col">coluna alvo</param>
        /// <param name="values">conjunto de valores</param>
        public void AddNotIn(string col, List<object> values)
        {
            if (values != null && values.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerator<object> iValues = values.GetEnumerator();
                iValues.MoveNext();
                sb.Append(col + " not in (" + ParseFormatValue(iValues.Current));
                while (iValues.MoveNext())
                {
                    sb.AppendFormat(", {0}", ParseFormatValue(iValues.Current));
                }
                sb.Append(")");
                AddExpression(sb.ToString());
            }
        }

        /// <summary>
        /// Adiciona condição para valor nulo
        /// </summary>
        /// <param name="col">Nome da coluna</param>
        public void AddIsNull(string col)
        {
            string expression = col + " IS NULL";
            AddExpression(expression);
        }

        /// <summary>
        /// Adiciona condição para valor não nulo
        /// </summary>
        /// <param name="col">Nome da coluna</param>
        public void AddIsNotNull(string col)
        {
            string expression = col + " IS NOT NULL";
            AddExpression(expression);
        }

        /// <summary>
        /// Adiciona uma expressão ao filtro
        /// </summary>
        /// <param name="expression">nova expressão</param>
        public void AddExpression(string expression)
        {
            _expressions.Add(expression);
        }

        /// <summary>
        /// Realiza operação de ou com outro critério de busca.
        /// </summary>
        /// <param name="criteria">Outro critério de busca.</param>	
        public void AddOrCriteria(ICriteria criteria)
        {
            _orCriterias.Add(criteria);
        }
        #endregion

        #region Joins
        /// <summary>
        /// Adiciona uma expressão de join
        /// </summary>
        /// <param name="table">Nome da tabela</param>
        /// <param name="expression">Expressão do JOIN</param>
        /// <param name="type">Tipo do JOIN</param>
        public void AddJoin(string table, string expression, JoinType type = JoinType.INNER)
        {
            _joins.Add(type.GetEnumValue() + " JOIN \"" + table + "\" ON " + expression);
        }

        /// <summary>
        /// Adiciona uma expressão de INNER JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        public void AddInnerJoin(string table, string mainKey, string foreignKey)
        {
            AddJoin(table, mainKey + " = " + foreignKey, JoinType.INNER);
        }

        /// <summary>
        /// Adiciona uma expressão de LEFT JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        public void AddLeftJoin(string table, string mainKey, string foreignKey)
        {
            AddJoin(table, mainKey + " = " + foreignKey, JoinType.LEFT);
        }

        /// <summary>
        /// Adiciona uma expressão de RIGHT JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        public void AddRightJoin(string table, string mainKey, string foreignKey)
        {
            AddJoin(table, mainKey + " = " + foreignKey, JoinType.RIGHT);
        }

        /// <summary>
        /// Adiciona uma expressão de FULL JOIN
        /// </summary>
        /// <param name="table">Tabela do JOIN</param>
        /// <param name="mainKey">chave da tabela principal</param>
        /// <param name="foreignKey">chave da tabela do JOIN</param>
        public void AddFullJoin(string table, string mainKey, string foreignKey)
        {
            AddJoin(table, mainKey + " = " + foreignKey, JoinType.FULL);
        }
        #endregion       
        
        /// <summary>
        /// Monta expressão de busca (clasula where). 
        /// </summary>
        /// <returns>Expressão</returns>
        public string MountsExpression()
        {
            //TODO: Usar string.Join
            string exp = "";
            StringBuilder sb = new StringBuilder();
            if (_expressions != null && _expressions.Count != 0)
            {
                IEnumerator<string> iExpressions = _expressions.GetEnumerator();
                iExpressions.MoveNext();
                sb.Append(iExpressions.Current);
                while (iExpressions.MoveNext())
                {
                    sb.AppendFormat(" AND {0}", iExpressions.Current);
                }
            }
            exp = sb.ToString();
            sb.Clear();
            if (_orCriterias != null && _orCriterias.Count != 0)
            {
                IEnumerator<ICriteria> iCriterias = _orCriterias.GetEnumerator();
                iCriterias.MoveNext();
                sb.Append(iCriterias.Current.MountsExpression());
                while (iCriterias.MoveNext())
                {
                    sb.AppendFormat(" OR ({0})", iCriterias.Current.MountsExpression());
                }
                exp = string.IsNullOrEmpty(exp) ? exp : exp + " OR ";
                exp += sb.ToString();
            }            
            return exp;
        }

        /// <summary>
        /// Retorna o filtro da consulta.
        /// </summary>
        /// <returns>as clausulas da consulta.</returns>
        public string GetClauses()
        {
            //TODO: Usar string.Join
            StringBuilder sb = new StringBuilder();
            if (_joins != null && _joins.Count > 0)
            {
                foreach (string join in _joins)
                {
                    sb.AppendFormat("{0} ", join);
                }
            }
            
            if (!string.IsNullOrEmpty(MountsExpression()))
                sb.AppendFormat("WHERE {0} ", MountsExpression());

            if (!string.IsNullOrEmpty(GroupBy))
                sb.AppendFormat("GROUP BY {0} ", GroupBy);

            if (!string.IsNullOrEmpty(OrderBy))
                sb.AppendFormat("ORDER BY {0} ", OrderBy);
            return sb.ToString();
        }

        /// <summary>
        /// Adiciona a clasulas à consulta
        /// </summary>
        /// <param name="sql">sql sem clasulas</param>
        /// <returns>sql com clasulas</returns>
        public string CompleteSQL(string sql)
        {
            return sql + GetClauses();
        }

        /// <summary>
        /// Formata um valor para que ele atenda a formatação do tipo no banco de dados. 
        /// </summary>
        /// <param name="objValue">Objeto que deve ser formatados</param>
        /// <returns>Valor formatado</returns>
        public virtual string ParseFormatValue(object objValue)
        {
            if (objValue == null)
                return "null";
            double num;
            if (double.TryParse(objValue.ToString(), out num) && objValue.GetType() != typeof(string))
                return num.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (objValue.GetType() == typeof(DateTime))
            {
                if((DateTime)objValue != DateTime.MinValue)
                    return "'" + ((DateTime)objValue).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                return "null";
            }
            if (objValue.GetType() == typeof(DatabaseConstantValue))
                return ((DatabaseConstantValue)objValue).Value;
            return "'" + objValue.ToString().Replace("'", "''") + "'";
        }

        public Criteria Clone()
        {
            Criteria newCriteria = new Criteria();
            newCriteria.OrderBy = OrderBy;
            newCriteria.GroupBy = GroupBy;
            newCriteria._expressions.AddRange(_expressions);
            newCriteria._orCriterias.AddRange(_orCriterias);
            newCriteria._joins.AddRange(_joins);
            return newCriteria;
        }
    }

    public class OracleCriteria : Criteria
    {
        public override string ParseFormatValue(object objValue)
        {
            if (objValue != null && objValue.GetType() == typeof(DateTime) && (DateTime)objValue != DateTime.MinValue)
                return "TO_DATE('" + objValue.ToString() + "','DD/MM/YYYY HH24:MI:SS')";
            return base.ParseFormatValue(objValue);
        }
    }

    /// <summary>
    /// Tipos de join
    /// </summary>
    public enum JoinType
    {
        [EnumValue("INNER", valueType: typeof(string))]
        INNER,

        [EnumValue("LEFT", valueType: typeof(string))]
        LEFT,

        [EnumValue("RIGHT", valueType: typeof(string))]
        RIGHT,

        [EnumValue("FULL", valueType: typeof(string))]
        FULL
    }
}
