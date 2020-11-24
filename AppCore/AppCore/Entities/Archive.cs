using System;
using System.Data;


namespace Marvin.AppCore.Entities
{
    [DataAnnotations.ERBridge.Table("sys_archive")]
    //Insert Procedure
    [DataAnnotations.ERBridge.InsertProcedure("usp_sys_archive_insert")]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Name", "@nm_archive", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Type", "@type", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Path", "@path", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Size", "@size", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Date", "@dt_archive", DataAnnotations.ERBridge.DataType.DateTime)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("ProviderClass", "nm_provider", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("ProviderArgs", "provider_args", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("StorageAddress", "fullpath", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("StorageFileName", "nm_storage", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("StorageStrategy", "fl_strategy", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("LockKey", "vl_key", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.InsertProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32, direction: ParameterDirection.Output)]
    //Update Procedure
    [DataAnnotations.ERBridge.UpdateProcedure("usp_sys_archive_update")]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Name", "@nm_archive", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Type", "@type", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Path", "@path", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Size", "@size", DataAnnotations.ERBridge.DataType.Int32)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("Date", "@dt_archive", DataAnnotations.ERBridge.DataType.DateTime)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("ProviderClass", "nm_provider", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("ProviderArgs", "provider_args", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("StorageAddress", "fullpath", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("StorageFileName", "nm_storage", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("StorageStrategy", "fl_strategy", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.UpdateProcedureParameter("LockKey", "vl_key", DataAnnotations.ERBridge.DataType.String)]
    //Select Procedure
    [DataAnnotations.ERBridge.SelectProcedure("usp_sys_archive_select")]
    [DataAnnotations.ERBridge.SelectProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    //Search Procedure
    [DataAnnotations.ERBridge.SearchProcedure("usp_sys_archive_search")]
    [DataAnnotations.ERBridge.SearchProcedureParameter("PageSize", "@page_size", DataAnnotations.ERBridge.DataType.Int32, 20)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Page", "@page", DataAnnotations.ERBridge.DataType.Int32, 1)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("Sort", "@order_by", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureParameter("SortType", "@order_method", DataAnnotations.ERBridge.DataType.String)]
    [DataAnnotations.ERBridge.SearchProcedureTotalRowsOutParameter("@total_rows")]
    //Delete Procedure
    [DataAnnotations.ERBridge.DeleteProcedure("usp_sys_archive_delete")]
    [DataAnnotations.ERBridge.DeleteProcedureParameter("Id", "@id", DataAnnotations.ERBridge.DataType.Int32)]
    public class Archive : Layers.DefaultModel
    {
        [DataAnnotations.ERBridge.Column("nm_archive", DataAnnotations.ERBridge.DataType.String, required: true, length: 100)]
        public string Name { get; set; }

        [DataAnnotations.ERBridge.Column("type", DataAnnotations.ERBridge.DataType.String, required: true, length: 50)]
        public string Type { get; set; }

        [DataAnnotations.ERBridge.Column("path", DataAnnotations.ERBridge.DataType.String, required: true, length: 200)]
        public string Path { get; set; }

        [DataAnnotations.ERBridge.Column("size", DataAnnotations.ERBridge.DataType.Int32, required: true)]
        public int Size { get; set; }

        [DataAnnotations.ERBridge.Column("dt_archive", DataAnnotations.ERBridge.DataType.DateTime, required: true)]
        public DateTime Date { get; set; }

        [DataAnnotations.ERBridge.Column("nm_provider", DataAnnotations.ERBridge.DataType.String, required: true, length: 100)]
        public string ProviderClass { get; set; }

        [DataAnnotations.ERBridge.Column("provider_args", DataAnnotations.ERBridge.DataType.String, length: 200)]
        public string ProviderArgs { get; set; }

        [DataAnnotations.ERBridge.Column("fullpath", DataAnnotations.ERBridge.DataType.String, required: true, length: 200)]
        public string StorageAddress { get; set; }

        [DataAnnotations.ERBridge.Column("nm_storage", DataAnnotations.ERBridge.DataType.String, required: true, length: 100)]
        public string StorageFileName { get; set; }

        [DataAnnotations.ERBridge.Column("fl_strategy", DataAnnotations.ERBridge.DataType.String, required: true, length: 1)]
        public StorageStrategy StorageStrategy { get; set; }

        [DataAnnotations.ERBridge.Column("vl_key", DataAnnotations.ERBridge.DataType.String, length: 100)]
        public string LockKey { get; set; }
    }

    public enum StorageStrategy
    {
        [Commons.Extensions.EnumValue('O')]
        [System.Xml.Serialization.XmlEnum("O")]
        Original,
        [Commons.Extensions.EnumValue('C')]
        [System.Xml.Serialization.XmlEnum("C")]
        Compressed,
        [Commons.Extensions.EnumValue('L')]
        [System.Xml.Serialization.XmlEnum("L")]
        Locked,
        [Commons.Extensions.EnumValue('V')]
        [System.Xml.Serialization.XmlEnum("V")]
        Versioned
    }
}
