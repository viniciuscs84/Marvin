using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvin.Layers
{
    /// <summary>
    /// Classe base para coleções das entidades utilizadas no sistema.
    /// </summary>
    public class ModelCollection<TModel> : List<TModel>
        where TModel : IModel
    {
        #region Propriedades
        public int TotalRows { get; set; }
        #endregion

        public override string ToString()
        {
            //TODO: Usar string.Join
            StringBuilder stringValue = new StringBuilder();
            stringValue.Append("'" + GetType().FullName + "': [");
            foreach (TModel model in this.ToList<TModel>())
            {
                stringValue.Append(model.ToString() + ",");
            }
            stringValue.Remove(stringValue.ToString().LastIndexOf(','), 1);
            stringValue.Append("]");
            return "{" + stringValue.ToString() + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ToString().Equals(obj.ToString());
        }

        public ModelCollection<IModel> GetNonTypedCollection()
        {
            ModelCollection<IModel> copy = new ModelCollection<IModel>();
            ForEach(e => copy.Add(e));
            copy.TotalRows = TotalRows;
            return copy;
        }
    }
}
