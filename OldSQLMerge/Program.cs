#define WITH_AFTER_MERGE 
// #define WITH_T_COR_Version

namespace SQLMerge
{


    class Program
    {


        public static void Test()
        {
            string file = @"P:\COR_Basic\Release\V4\v401\331_Anpassungen auf Grund von JB.sql";
            System.Text.Encoding enc = EncodingDetector.DetectOrGuessEncoding(file);
            System.IO.File.ReadAllText(file, enc);

            System.Console.Write("File \"");
            System.Console.Write(file);
            System.Console.Write("\" - Detected Encoding ");
            System.Console.WriteLine(enc.WebName);
        } // End Sub Test 


        // https://github.com/AutoItConsulting/text-encoding-detect/blob/master/TextEncodingDetect-C%23/TextEncodingDetect/TextEncodingDetect.cs
        public static void Main(string[] args)
        {
            // Test();
            // Example.Test();
            // Merge();
            NewMerge();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


        private static void NewMerge()
        {
            try
            {
                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string directoryPath = System.IO.Path.GetDirectoryName(exeLocation);

                MergeOptions options = new MergeOptions
                {
                    VersionValue = $"From {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    VersionUser = System.Environment.UserName,
                    VersionNote = "SQL Merge Operation"
                };

                SqlMergeService mergeService = new SqlMergeService(options);
                mergeService.Merge(directoryPath);

                System.Console.WriteLine("\nPress any key to continue...");
                System.Console.ReadKey();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine("\nPress any key to exit...");
                System.Console.ReadKey();
            } // End Catch 

        } // End Sub TestNewMerge 


    } // End Class Program 


} // End Namespace SQLMerge 
