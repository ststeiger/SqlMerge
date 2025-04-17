
namespace SqlMerge
{


    internal class Program
    {


        // https://github.com/AutoItConsulting/text-encoding-detect/blob/master/TextEncodingDetect-C%23/TextEncodingDetect/TextEncodingDetect.cs
        public static async System.Threading.Tasks.Task<int> Main(string[] args)
        {
            // BadMerge.TestEncodingQuirks();
            // Example.Test();
            // BadMerge.Merge();
            await NewMerge();

            await System.Console.Out.WriteLineAsync(System.Environment.NewLine);
            await System.Console.Out.WriteLineAsync(" --- Press any key to continue --- ");
            System.Console.ReadKey();
            return 0;
        } // End Sub Main 


        private static async System.Threading.Tasks.Task NewMerge()
        {
            try
            {
                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string? directoryPath = System.IO.Path.GetDirectoryName(exeLocation);

                MergeOptions options = new MergeOptions
                {
                    VersionValue = $"From {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    VersionUser = System.Environment.UserName,
                    VersionNote = "SQL Merge Operation"
                };

                SqlMergeService mergeService = new SqlMergeService(options);
                mergeService.Merge(directoryPath!);
            }
            catch (System.Exception ex)
            {
                await System.Console.Out.WriteLineAsync($"Error: {ex.Message}");
                await System.Console.Out.WriteLineAsync(ex.StackTrace);
            } // End Catch 

        } // End Sub TestNewMerge 


    } // End Class Program 


} // End Namespace 
