using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Marvin.Layers
{
    public interface IModel
    {
        bool IsRecorded { get; }

        bool IsReady { get; set; }
    }

    /// <summary>
    /// Classe base das entidades utilizadas no sistema.
    /// </summary>
    public abstract class Model : IModel
    {
        [ReadOnly(true), ScaffoldColumn(false)]
        public abstract bool IsRecorded { get; }

        public override string ToString()
        {
            //TODO: Usar string.Join
            StringBuilder stringValue = new StringBuilder();
            stringValue.Append("'" + GetType().FullName + "': { ");
            foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
            {
                if (property.GetType().IsSubclassOf(typeof(Model)))
                    stringValue.Append("'" + property.Name + "': " + property.GetValue(this, null) + ",");
                else
                    stringValue.Append("'" + property.Name + "': '" + property.GetValue(this, null) + "',");
            }
            stringValue.Remove(stringValue.ToString().LastIndexOf(','), 1);
            stringValue.Append("}");
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

        [ReadOnly(true), ScaffoldColumn(false)]
        public bool IsReady { get; set; }
    }

    public abstract class DefaultModel : Model
    {
        [Key]
        [DataAnnotations.ERBridge.Column("id", DataAnnotations.ERBridge.DataType.Int32, autoIncremented: true)]
        public int Id { get; set; }

        public override bool IsRecorded
        {
            get { return Id != 0; }
        }
    }
}
