using System.Data;

namespace Marvin.AppCore.Entities
{
    /// <summary>
    /// This entity represents a Configuration Setting in the application
    /// </summary>
    [DataAnnotations.ERBridge.Table("sys_setting")]
    //Insert Procedure
    [DataAnnotations.ERBridge.InsertProcedure("usp_sys_setting_insert")]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Application.Id", "@id_application", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Key", "@key", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Value", "@vl_setting", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32, direction: ParameterDirection.Output)]
    //Update Procedure
    [DataAnnotations.ERBridge.UpdateProcedure("usp_sys_setting_update")]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Application.Id", "@id_application", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Key", "@key", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Value", "@vl_setting", DataAnnotations.ERBridge.DataType.String)]
    //Select Procedure
    [DataAnnotations.ERBridge.SelectProcedure("usp_sys_setting_select")]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Application", "@id_application", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Key", "@key", DataAnnotations.ERBridge.DataType.String)]
    //Search Procedure
    [DataAnnotations.ERBridge.SearchProcedure("usp_sys_setting_search")]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Application", "@id_application", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("PageSize", "@page_size", DataAnnotations.ERBridge.DataType.Int32, 20)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Page", "@page", DataAnnotations.ERBridge.DataType.Int32, 1)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Sort", "@order_by", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("SortType", "@order_method", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureTotalRowsOutParameter("@total_rows")]
    //Delete Procedure
    [DataAnnotations.ERBridge.DeleteProcedure("usp_sys_setting_delete")]
    [DataAnnotations.ERBridge.DeleteProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    public class Setting : Layers.DefaultEntity
    {
        [DataAnnotations.ERBridge.Column("id_application", DataAnnotations.ERBridge.DataType.Int32, required: true)]
        [DataAnnotations.ERBridge.Reference("Id", fetchType: DataAnnotations.ERBridge.Fetch.EAGER)]
        public virtual Application Application { get; set; }

        /// <summary>
        /// Gets or Sets the Setting's Key.
        /// </summary>
        [DataAnnotations.ERBridge.Column("key", DataAnnotations.ERBridge.DataType.String, length: 25)]
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or Sets the Setting's Value.
        /// </summary>
        [DataAnnotations.ERBridge.Column("vl_setting", DataAnnotations.ERBridge.DataType.UnlimitedString)]
        public virtual string Value { get; set; }
    }
}
