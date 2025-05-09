﻿
namespace SqlMerge
{


    class BadMerge
    {


        public static void Merge()
        {
            // Test.ReplaceTestFileContent();
            string strExeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strPath = System.IO.Path.GetDirectoryName(strExeLocation);
            string strPathRoot = System.IO.Path.GetPathRoot(strPath);
        }


        // https://www.gaijin.at/en/infos/ascii-ansi-character-table#:~:text=Overview,the%20unchanged%20ASCII%20character%20set.
        public static void BrokenEncoding()
        {
            // https://stackoverflow.com/questions/700187/unicode-utf-ascii-ansi-format-differences

            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("iso-8859-1");

            string fileName = @"D:\username\Documents\Visual Studio 2017\GitLab\COR-Basic-V4\Portal\Portal_Share\0\VWS.Legend.Load.sql";

            string content = System.IO.File.ReadAllText(fileName, System.Text.Encoding.UTF8);

            // Text encoded as iso
            byte[] bca = enc.GetBytes(content);
            // text wrongly decoded as utf8 
            content = new System.Text.UTF8Encoding(false).GetString(bca);


            // expected reversal 
            // bca = new System.Text.UTF8Encoding(false).GetBytes(content);
            // content = enc.GetString(bca);


            // This happens on browser:
            bca = enc.GetBytes(content);
            content = new System.Text.UTF8Encoding(false).GetString(bca);

            System.Console.WriteLine(content);
        }


        public static void TestEncodingQuirks()
        {
            string TESTSTRING = null;
            string testString = "Hello äöüÄÖÜ";
            testString = "abcäáàâëéèêïíìîöóòôüúùÿñçşğıiœææøåß";
            TESTSTRING = "ABCÄÁÀÂËÉÈÊÏÍÌÎÖÓÒÔÜÚÙŸÑÇŞĞIIŒÆÆØÅSS";
            System.Diagnostics.Trace.WriteLine(TESTSTRING);

            testString = "Die erste Spalte zählt als Primärschlüssel\r\nRevêtement de sol";

            byte[] ba = System.IO.File.ReadAllBytes(@"D:\Stefan.Steiger\Desktop\foohoo\002_Anpassungen (VWS).sql");


            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("iso-8859-1");
            System.Text.Encoding enc2 = new System.Text.UTF8Encoding(false);
            // decode[7]   65533 '�'   char

            enc = new System.Text.UTF8Encoding(false);
            enc2 = System.Text.Encoding.GetEncoding("iso-8859-1");
            // decode[6]   195 'Ã' char
            // decode[7]   164 '¤' char
            // decode[8]   195 'Ã' char

            byte[] bytes = enc.GetBytes(testString);
            string decode = enc2.GetString(bytes);
            System.Console.WriteLine(decode);
        } // End Sub BadTests 


        public static void Merge(string strPath)
        {
            string strFileName = System.IO.Path.Combine(strPath, "AllInOne.sql");

            System.IO.FileInfo[] afiSQLfiles = FileHelper.GetSqlFilesSorted(strPath);
            System.Console.WriteLine("Total number of sql files to be merged: {0}", afiSQLfiles.Length);

            System.Collections.Generic.List<string> lsFileList = new System.Collections.Generic.List<string>();


            string strFileContent = string.Empty;
            foreach (System.IO.FileInfo fiThisFile in afiSQLfiles)
            {
                if (!System.StringComparer.InvariantCultureIgnoreCase.Equals(fiThisFile.FullName, strFileName))
                {
                    System.Console.WriteLine("Adding content of file " + fiThisFile.Name);
                    strFileContent = GetFileContent(fiThisFile, System.Text.Encoding.Default);

                    lsFileList.Add(strFileContent);
                } // End if (!StringComparer.OrdinalIgnoreCase.Equals(fiThisFile.Name, strFileName))
                else
                    System.Console.WriteLine("Preventing the probably wrong addition of content from file \"" + fiThisFile.Name + "\"");
            } // Next fiThisFile



            // while (System.StringComparer.InvariantCultureIgnoreCase.Equals(strPathRoot, @"P:\"))
            while (true)
            {
                if (lsFileList.Count == 0)
                    break;

                string firstFile = ""; // 
                string lastFile = "";
                string strInsertText = "";


                if (afiSQLfiles.Length > 0)
                    firstFile = afiSQLfiles[0].Name;



                // string strFirstFolder = @"P:\" + strPath.Split(System.IO.Path.DirectorySeparatorChar)[1];
                string strLastFolder = (new System.IO.DirectoryInfo(strPath)).Name;

                if (afiSQLfiles.Length > 1)
                {
                    lastFile = afiSQLfiles[lsFileList.Count - 1].Name;
                    strInsertText = string.Format("({0}) From \"{1}\" to \"{2}\" in \"{3}\"", strLastFolder, firstFile, lastFile, strPath);
                }
                else
                    strInsertText = string.Format("({0}) From \"{1}\" in \"{2}\"", strLastFolder, firstFile, strPath);



#if WITH_T_COR_Version

                lsFileList.Add(string.Format(@"


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
		,N'{0}' AS CV_Value -- nvarchar(200) 
	;
END



GO


PRINT 'Finsihed Writing entry into T_COR_Version' 


GO

", strInsertText.Replace("'", "''")));
#endif


                break;
            }



            if (System.IO.File.Exists(strFileName))
                System.IO.File.Delete(strFileName);

            WriteToFile(lsFileList, strFileName);

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main


        public static string GetFileContent(System.IO.FileInfo fiThisFile, System.Text.Encoding enc)
        {
            System.Text.StringBuilder sb = null;
            string strFileContent = string.Empty;
            string strBatchSeparator = "GO";

            try
            {
                // Specify file, instructions, and privelegdes
                using (System.IO.FileStream fs = new System.IO.FileStream(fiThisFile.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {

                    // Create a new stream to read from a file
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fs, enc))
                    {
                        string strFileName = fiThisFile.Name.Replace("'", "''");
                        sb = new System.Text.StringBuilder();

                        sb.Append(StringHelper.RepeatNewLine(1));
                        sb.Append("PRINT 'Begin Executing \"");
                        sb.Append(strFileName);
                        sb.Append("\"' ");
                        sb.Append(StringHelper.RepeatNewLine(2));
                        sb.Append(strBatchSeparator);
                        sb.Append(" ");
                        sb.Append(StringHelper.RepeatNewLine(3));

                        //strFileContent = strFileContent + sr.ReadToEnd(); // Read contents of file into a string
                        sb.Append(sr.ReadToEnd());

                        // Remove the newlines when created using sp_RPT_DEBUG_PrintVarcharMax 
                        sb = sb.Replace("#@NEW@#_@#LINE@#\r\n", "");

                        sb.Append(StringHelper.RepeatNewLine(2));
                        sb.Append(strBatchSeparator);
                        sb.Append(" ");
                        sb.Append(StringHelper.RepeatNewLine(3));
                        sb.Append("PRINT 'Done Executing \"");
                        sb.Append(strFileName);
                        sb.Append("\"' ");
                        sb.Append(StringHelper.RepeatNewLine(3));
                        sb.Append(strBatchSeparator);
                        sb.Append(" ");
                        sb.Append(StringHelper.RepeatNewLine(4));

                        strFileContent = sb.ToString();
                        sr.Close(); // Close StreamReader
                    } // End Using sr

                    fs.Close(); // Close file
                } // End Using fs

            } // End try
            catch (System.Exception ex)
            {
                System.Console.Write("Reading file: \"" + fiThisFile.Name + "\" has failed because: " + ex.Message);
                //MessageBox.Show("Reading of file: " + strFilename + " has failed because: " + ex.ToString());
            } // End catch

            sb.Length = 0;
            sb = null;
            return strFileContent;
        } // End Function GetFileContent


        public static void WriteToFile(System.Collections.Generic.List<string> lsInputFileList, string strFileName)
        {
            if (lsInputFileList == null || lsInputFileList.Count == 0)
            {
                System.Console.WriteLine("No files to be merged. Doing nothing.");
                return;
            }

            try
            {
                // Specify file, instructions, and privelegdes
                using (System.IO.FileStream fsOutputFile = new System.IO.FileStream(strFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
                {

                    // Create a new stream to write to the file
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fsOutputFile, System.Text.Encoding.UTF8))
                    {

                        foreach (string strThisFile in lsInputFileList)
                        {
                            // Write a string to the file
                            sw.WriteLine(strThisFile);
                        } // Next strThisFile

                        sw.Close(); // Close StreamWriter
                    } // End Using sw

                    fsOutputFile.Close(); // Close file
                } // End Using sw

            } // End try
            catch (System.Exception ex)
            {
                System.Console.Write("Writing to file: " + strFileName + " has failed because: " + ex.Message);
                //MessageBox.Show("Writing to file: " + strFileName + " has failed because: " + ex.ToString());
            } // End catch

        } // End Sub WriteToFile


    }


}
