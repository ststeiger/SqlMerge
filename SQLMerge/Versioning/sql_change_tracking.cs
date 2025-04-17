
namespace SqlMerge
{


    public class sql_change_tracking
    {
        public System.Guid ct_uid { get; set; }
        public int ct_script_sequence_no { get; set; }
        public int ct_script_sub_sequence_no { get; set; }
        public string ct_script_name { get; set; }
        public string ct_folder_name { get; set; }
        public string ct_executed_by { get; set; }
        public System.DateTime ct_executed_at { get; set; }
        public bool ct_succeeded { get; set; }
        public string ct_error_message { get; set; }
    }


}
