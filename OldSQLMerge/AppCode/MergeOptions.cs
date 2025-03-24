
namespace SQLMerge
{


    public class MergeOptions
    {
        public string OutputFileName { get; set; } = "AllInOne.sql";
        public string BatchSeparator { get; set; } = "GO";
        public bool IncludeVersionTable { get; set; } = false;
        public bool IncludeAfterMergeProcedure { get; set; } = true;
        public string VersionTableName { get; set; } = "T_COR_Version";
        public string AfterMergeProcedureName { get; set; } = "stp_COR_afterSQLMerge";
        public string VersionKey { get; set; } = "SQL";
        public string VersionValue { get; set; }
        public string VersionUser { get; set; }
        public string VersionNote { get; set; }
        public System.DateTime VersionDate { get; set; } = System.DateTime.Now;

        public static MergeOptions Default => new MergeOptions();
    } // End Class MergeOptions 


} // End Namespace 
