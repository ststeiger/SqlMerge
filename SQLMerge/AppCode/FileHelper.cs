
namespace SQLMerge
{


    internal class FileHelper
    {


        internal static System.IO.FileInfo[] GetSqlFilesSorted(string strPath)
        {
            //System.IO.Directory.GetFiles(strPath, "*.sql", System.IO.SearchOption.TopDirectoryOnly);
            System.IO.FileInfo[] afiSQLfiles = GetSqlFiles(strPath);

            // Array.Sort(afiSQLfiles);
            System.Array.Sort(afiSQLfiles, StringHelper.SortFileNames);

            // System.Array.Sort(afiSQLfiles, delegate(System.IO.FileInfo f1, System.IO.FileInfo f2)
            // {
            //     return System.StringComparer.InvariantCultureIgnoreCase.Compare(f1.Name, f2.Name);
            // });



            // In C# 3 this becomes slightly simpler:
            // Array.Sort(afiSQLfiles, (f1, f2) => f1.Name.CompareTo(f2.Name));
            // Or you can use a StringComparer if you want to use a case-insensitive sort order:
            // Array.Sort(afiSQLfiles, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));


            //System.IO.FileInfo[] afiForDebug = new System.IO.FileInfo[] { 
            //    new System.IO.FileInfo(
            //        System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sqltestfile.txt")
            //    ) 
            //};

            //return afiForDebug;

            return afiSQLfiles;
        } // End Function GetSqlFilesSorted


        // SQLMerge.FileHelper.GetSqlFiles(@"");
        internal static System.IO.FileInfo[] GetSqlFiles(string searchFolder)
        {
            return GetSqlFiles(searchFolder, System.IO.SearchOption.TopDirectoryOnly);
        } // End Function GetRasterImages


        // SQLMerge.FileHelper.GetSqlFiles();
        internal static System.IO.FileInfo[] GetSqlFiles(string searchFolder, System.IO.SearchOption searchOption)
        {
            string[] filters = new string[] { "sql", "psql"};
            return GetFilesFrom(searchFolder, filters, searchOption);
        } // End Function GetRasterImages


        // Linux case-insensitive multi-extension search
        internal static System.IO.FileInfo[] GetFilesFrom(string searchFolder, string[] filters, System.IO.SearchOption searchOption)
        {
            System.IO.FileInfo[] arrRes = null;
            System.Collections.Generic.List<System.IO.FileInfo> filesFound = new System.Collections.Generic.List<System.IO.FileInfo>();

            System.IO.DirectoryInfo diDirInfo = new System.IO.DirectoryInfo(searchFolder);

            System.IO.FileInfo[] filez = diDirInfo.GetFiles("*.*", searchOption);

            for (int i = 0; i < filters.Length; ++i)
            {
                string extension = "." + filters[i];

                for (int j = 0; j < filez.Length; ++j)
                {
                    if (filez[j].Name.EndsWith(extension, System.StringComparison.InvariantCultureIgnoreCase))
                        filesFound.Add(filez[j]);
                } // Next j 

            } // Next i

            arrRes = filesFound.ToArray();
            filesFound.Clear();
            filesFound = null;

            return arrRes;
        } // End Function GetFilesFrom


    }


}
