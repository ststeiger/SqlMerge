
namespace SqlMerge.Old
{


    internal class OldProgram 
    {


        public static void Merge()
        {
            string strExeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strPath = System.IO.Path.GetDirectoryName(strExeLocation);
            string strPathRoot = System.IO.Path.GetPathRoot(strPath);

            Merge(strPath);
        }

        private static int? GetSortNumber(string fileName)
        {
            int? result = null;

            System.Text.RegularExpressions.Match ma = System.Text.RegularExpressions.Regex.Match(fileName, @"^\d+");
            if (ma.Success)
            {
                try
                {
                    result = int.Parse(ma.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }

            return result;
        }


        public static void WriteFileContent(System.IO.FileInfo file, System.IO.TextWriter writer, System.Text.Encoding enc, string batchSeparator)
        {
            string fileContent = "";

            try
            {
                using (System.IO.FileStream fsReadFile = new System.IO.FileStream(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {

                    // Create a new stream to read from a file
                    using (System.IO.TextReader tr = new System.IO.StreamReader(fsReadFile, enc))
                    {
                        // Read contents of file into a string
                        fileContent = tr.ReadToEnd();
                        // Remove the newlines when created using sp_RPT_DEBUG_PrintVarcharMax 
                        fileContent = fileContent.Replace("#@NEW@#_@#LINE@#\r\n", "");
                        fileContent = fileContent.Replace("#@NEW@#_@#LINE@#\n", "");
                        fileContent = fileContent.Replace("\r\n", "\n");
                        fileContent = fileContent.Replace("\n", "\r\n");
                    } // End Using tr 

                } // End using fsReadFile

            } // End try
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Reading file: \"" + file.Name + "\" has failed because: " + ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                return;
            } // End catch


            writer.Write(StringHelper.RepeatNewLine(1));
            writer.Write("PRINT 'Begin Executing \"");
            writer.Write(file.Name.Replace("'", "''"));
            writer.Write("\"' ");
            writer.Write(StringHelper.RepeatNewLine(2));
            writer.Write(batchSeparator);
            writer.Write(" ");
            writer.Write(StringHelper.RepeatNewLine(3));

            writer.Write(fileContent);


            writer.Write(StringHelper.RepeatNewLine(2));
            writer.Write(batchSeparator);
            writer.Write(" ");
            writer.Write(StringHelper.RepeatNewLine(3));
            writer.Write("PRINT 'Done Executing \"");
            writer.Write(file.Name.Replace("'", "''"));
            writer.Write("\"' ");
            writer.Write(StringHelper.RepeatNewLine(3));
            writer.Write(batchSeparator);
            writer.Write(" ");
            writer.Write(StringHelper.RepeatNewLine(4));
        } // End Function WriteFileContent 


        public static void Merge(string strPath)
        {
            string mergeFileName = System.IO.Path.Combine(strPath, "AllInOne.sql");
            string batchSeparator = "GO";

            try
            {
                if (System.IO.File.Exists(mergeFileName))
                    System.IO.File.Delete(mergeFileName);

                System.IO.FileInfo[] fileInfos = FileHelper.GetSqlFilesSorted(strPath);
                System.Console.WriteLine("Total number of sql files to be merged: {0}", fileInfos.Length);

                if (fileInfos == null || fileInfos.Length == 0)
                {
                    System.Console.WriteLine("No files to be merged. Doing nothing.");
                    return;
                }

#if WITH_T_COR_Version
                string strLastFolder = (new System.IO.DirectoryInfo(strPath)).Name;
                string firstFile = fileInfos[0].Name;
                string lastFile = fileInfos[fileInfos.Length - 1].Name;

                string strInsertText = (fileInfos.Length > 1) ?
                      string.Format("({0}) From \"{1}\" to \"{2}\" in \"{3}\"", strLastFolder, firstFile, lastFile, strPath)
                    : string.Format("({0}) From \"{1}\" in \"{2}\"", strLastFolder, firstFile, strPath)
                ;
#endif 

                using (System.IO.Stream fs = System.IO.File.Create(mergeFileName))
                {
                    using (System.IO.TextWriter tw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8))
                    {
                        foreach (System.IO.FileInfo file in fileInfos)
                        {
                            if (System.StringComparer.InvariantCultureIgnoreCase.Equals(file.FullName, mergeFileName))
                                continue;

                            int? sortNumber = GetSortNumber(file.Name);

                            System.Text.Encoding enc = EncodingDetector.DetectOrGuessEncoding(file.FullName);
                            System.Console.Write("File \"");
                            System.Console.Write(file.Name);
                            System.Console.Write("\" - Detected Encoding ");
                            System.Console.WriteLine(enc.WebName);

                            WriteFileContent(file, tw, enc, batchSeparator);
                        } // Next file 

#if WITH_AFTER_MERGE
                        tw.Write(@"
if exists(select * from sys.objects where object_id = object_id(N'[stp_COR_afterSQLMerge]') and type in (N'P', N'PC')) begin
	execute [dbo].[stp_COR_afterSQLMerge] '<SQL><![CDATA[0001_basicLink.sql"" in ""P:\COR_Basic\Release\V4\v404 (CAFM)]]></SQL>'
end;
");
#endif

#if WITH_T_COR_Version

                        tw.Write(@"


GO


PRINT 'Writing entry into T_COR_Version'

GO



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_COR_Version]') AND type in (N'U'))
BEGIN
EXECUTE('
	CREATE TABLE [dbo].[T_COR_Version](
		[CV_UID] [uniqueidentifier] NOT NULL,
		[CV_Key] [nvarchar](200) NOT NULL,
		[CV_Value] [nvarchar](max) NOT NULL,
		[CV_User] [nvarchar](200) NULL,
		[CV_Note] [nvarchar](200) NULL,
		[CV_Date] [datetime] NOT NULL,
		 CONSTRAINT [PK_T_COR_Version] PRIMARY KEY CLUSTERED 
		(
			[CV_UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
');


	EXECUTE('ALTER TABLE [dbo].[T_COR_Version] ADD CONSTRAINT [DF_T_COR_Version_CV_UID]  DEFAULT (newid()) FOR [CV_UID]');
	EXECUTE('ALTER TABLE [dbo].[T_COR_Version] ADD  CONSTRAINT [DF_T_COR_Version_CV_User]  DEFAULT (suser_sname()) FOR [CV_User]');
	EXECUTE('ALTER TABLE [dbo].[T_COR_Version] ADD  CONSTRAINT [DF_T_COR_Version_CV_Date]  DEFAULT (getdate()) FOR [CV_Date]');
END 


GO




IF 0 < (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'T_COR_Version') 
BEGIN 
	INSERT INTO T_COR_Version(CV_Key, CV_Value) 
	SELECT 
		 N'SQL' AS CV_Key -- nvarchar(200) 
		,N'");
                        tw.Write(strInsertText.Replace("'", "''"));

                        tw.WriteLine(@"' AS CV_Value -- nvarchar(200) 
	;
END



GO


PRINT 'Finsihed Writing entry into T_COR_Version' 


GO

");
#endif

                        tw.Flush();
                        fs.Flush();
                    } // End Using tw 

                } // End Using fs 

            } // End try
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Writing to file: \"" + mergeFileName + "\" has failed because: " + ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                //MessageBox.Show("Writing to file: " + strFileName + " has failed because: " + ex.ToString());
            } // End catch

        } // End Sub Merge 



    }
}
