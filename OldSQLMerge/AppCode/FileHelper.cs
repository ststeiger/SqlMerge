
namespace SQLMerge
{


    internal class FileHelper
    {

        //public static System.IO.FileInfo[] GetSqlFilesSorted(string directoryPath)
        //{
        //    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(directoryPath);
        //    return directory.GetFiles("*.sql")
        //                  .OrderBy(f => GetSortNumber(f.Name))
        //                  .ThenBy(f => f.Name)
        //                  .ToArray();
        //}

        private static int? GetSortNumber(string fileName)
        {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(fileName, @"^\d+");
            if (match.Success)
            {
                try
                {
                    return int.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }
            return null;
        }

        public static System.IO.FileInfo[] GetSqlFilesSorted(string directoryPath)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(directoryPath);
            System.IO.FileInfo[] files = directory.GetFiles("*.sql");

            if (files.Length == 0)
            {
                return files; // Return empty array if no files found
            }



            System.Array.Sort(files, delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (!string.IsNullOrEmpty(f1.Name) && !string.IsNullOrEmpty(f2.Name))
                {
                    return f1.Name.CompareTo(f2.Name);
                }
                else if (!string.IsNullOrEmpty(f1.Name))
                {
                    return 1; // sortNumber1 is greater
                }
                else if (!string.IsNullOrEmpty(f2.Name))
                {
                    return -1; // sortNumber2 is greater
                }

                return 0;
            });


            System.Array.Sort(files, delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                int? sortNumber1 = GetSortNumber(f1.Name);
                int? sortNumber2 = GetSortNumber(f2.Name);


                if (sortNumber1.HasValue && sortNumber2.HasValue)
                {
                    int sortNumberComparison = sortNumber1.Value.CompareTo(sortNumber2.Value);
                    if (sortNumberComparison != 0)
                    {
                        return sortNumberComparison;
                    }
                }
                else if (sortNumber1.HasValue)
                {
                    return 1; // sortNumber1 is greater
                }
                else if (sortNumber2.HasValue)
                {
                    return -1; // sortNumber2 is greater
                }


                return 0;
            });

            return files;
        }

        private static string NormalizeLineEndings(string content)
        {
            // Remove the newlines when created using sp_RPT_DEBUG_PrintVarcharMax 
            content = content.Replace("#@NEW@#_@#LINE@#\r\n", "");
            content = content.Replace("#@NEW@#_@#LINE@#\n", "");
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\n", "\r\n");
            return content;
        }


        public static ScriptSplitter LoadSqlScript(System.IO.FileInfo fileInfo, System.Text.Encoding encoding)
        {
            try
            {
                string content = System.IO.File.ReadAllText(fileInfo.FullName, encoding);
                content = NormalizeLineEndings(content);
                return new ScriptSplitter(content);
            }
            catch (System.Exception ex)
            {
                throw new System.IO.IOException($"Failed to read SQL file: {fileInfo.Name}", ex);
            }
        }


        public static void WriteToFile(string filePath, string content, System.Text.Encoding encoding)
        {
            try
            {
                System.IO.File.WriteAllText(filePath, content, encoding);
            }
            catch (System.Exception ex)
            {
                throw new System.IO.IOException($"Failed to write to file: {filePath}", ex);
            }
        }


        internal static System.IO.FileInfo[] GetSqlFilesSorted111(string strPath)
        {
            //System.IO.Directory.GetFiles(strPath, "*.sql", System.IO.SearchOption.TopDirectoryOnly);
            System.IO.FileInfo[] afiSQLfiles = GetSqlFiles(strPath);
            System.Collections.Generic.List<System.IO.FileInfo> afi = new System.Collections.Generic.List<System.IO.FileInfo>();
            afi.AddRange(afiSQLfiles);
            for (int i = afi.Count - 1; i > -1; --i)
            {
                bool isSoftDelete = System.Text.RegularExpressions.Regex.IsMatch(afi[i].Name, "^(x{3})+_", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (isSoftDelete)
                    afi.RemoveAt(i);
            }

            afi.Sort(StringHelper.SortFileNames);
            afiSQLfiles = afi.ToArray();

            // Array.Sort(afiSQLfiles);
            //System.Array.Sort(afiSQLfiles, StringHelper.SortFileNames);

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
            string[] filters = new string[] { "sql", "psql" };
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


    } // End Class FileHelper 


}
