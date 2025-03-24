
namespace SQLMerge
{

    internal class Test
    {


        internal static void ReplaceTestFileContent()
        {
            string strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            strPath = System.IO.Path.Combine(strPath, "../..");
            strPath = System.IO.Path.GetFullPath(strPath);

            string[] filez = System.IO.Directory.GetFiles(strPath, "*.sql");

            foreach (string file in filez)
            {
                string strSQL = string.Format("SELECT '{0}' AS test ", System.IO.Path.GetFileName(file).Replace("'", "''"));
                System.IO.File.WriteAllText(file, strSQL, System.Text.Encoding.UTF8);
            } // Next file

        }


    }


}
