using System.Data;

namespace Marvin.AppCore.Entities
{
    [DataAnnotations.ERBridge.Table("sys_application")]
    //Insert Procedure
    [DataAnnotations.ERBridge.InsertProcedure("usp_sys_application_insert")]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Name", "@nm_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Code", "@cd_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Key", "@key", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Description", "@ds_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Title", "@title", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("IsActive", "@fl_active", DataAnnotations.ERBridge.DataType.Boolean)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32, direction: ParameterDirection.Output)]
    //Update Procedure
    [DataAnnotations.ERBridge.UpdateProcedure("usp_sys_application_update")]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Name", "@nm_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Code", "@cd_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Description", "@ds_application", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Title", "@title", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("IsActive", "@fl_active", DataAnnotations.ERBridge.DataType.Boolean)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Key", "@key", DataAnnotations.ERBridge.DataType.String)]
    //Select Procedure
    [DataAnnotations.ERBridge.SelectProcedure("usp_sys_application_select")]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Code", "@cd_application", DataAnnotations.ERBridge.DataType.String)]
    //Search Procedure
    [DataAnnotations.ERBridge.SearchProcedure("usp_sys_application_search")]
    [DataAnnotations.ERBridge.SearchProcedureParameter("PageSize", "@page_size", DataAnnotations.ERBridge.DataType.Int32, 20)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Page", "@page", DataAnnotations.ERBridge.DataType.Int32, 1)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Sort", "@order_by", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("SortType", "@order_method", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureTotalRowsOutParameter("@total_rows")]
    //Delete Procedure
    [DataAnnotations.ERBridge.DeleteProcedure("usp_sys_application_delete")]
    [DataAnnotations.ERBridge.DeleteProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    public class Application : Layers.DefaultEntity
    {
        [DataAnnotations.ERBridge.Column("nm_application", DataAnnotations.ERBridge.DataType.String, required: true, length: 100)]
        public virtual string Name { get; set; }

        [DataAnnotations.ERBridge.Column("cd_application", DataAnnotations.ERBridge.DataType.String, required: true, length: 50)]
        public virtual string Code { get; set; }

        [DataAnnotations.ERBridge.Column("key", DataAnnotations.ERBridge.DataType.String, required: true, length: 50)]
        public virtual string Key { get; set; }

        [DataAnnotations.ERBridge.Column("title", DataAnnotations.ERBridge.DataType.String, length: 100)]
        //[StringLength(100), Display(Name = "Title", ResourceType = typeof(Globalization.AppCore.General), Order = 3)]
        public virtual string Title { get; set; }

        [DataAnnotations.ERBridge.Column("ds_application", DataAnnotations.ERBridge.DataType.String)]
        //[Display(Name = "Description", ResourceType = typeof(Globalization.AppCore.General), Order = 4)]
        public virtual string Description { get; set; }

        [DataAnnotations.ERBridge.Column("fl_active", DataAnnotations.ERBridge.DataType.Boolean, required: true)]
        public virtual bool IsActive { get; set; }
    }
}
