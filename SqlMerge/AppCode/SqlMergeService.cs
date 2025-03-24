
namespace SQLMerge
{


    public class SqlMergeService
    {

        private readonly MergeOptions m_options;


        public SqlMergeService(MergeOptions options = null)
        {
            this.m_options = options ?? MergeOptions.Default;
        } // End Class SqlMergeService 

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
        } // End Function GetSortNumber 

        public void Merge(string directoryPath)
        {
            string outputPath = System.IO.Path.Combine(directoryPath, this.m_options.OutputFileName);
            
            try
            {
                // Delete existing output file if it exists
                if (System.IO.File.Exists(outputPath))
                {
                    System.IO.File.Delete(outputPath);
                } // End if (System.IO.File.Exists(outputPath)) 

                System.IO.FileInfo[] sqlFiles = FileHelper.GetSqlFilesSorted(directoryPath);
                if (sqlFiles.Length == 0)
                {
                    System.Console.WriteLine("No SQL files found to merge.");
                    return;
                } // End if (sqlFiles.Length == 0) 

                System.Console.WriteLine($"Found {sqlFiles.Length} SQL files to merge.");

                System.Text.StringBuilder mergedContent = new System.Text.StringBuilder();
                foreach (System.IO.FileInfo thisFile in sqlFiles)
                {
                    if (string.Equals(thisFile.FullName, outputPath, System.StringComparison.OrdinalIgnoreCase))
                        continue;

                    System.Text.Encoding encoding = EncodingDetector.DetectOrGuessEncoding(thisFile.FullName);
                    System.Console.WriteLine($"Processing file: {thisFile.Name} (Encoding: {encoding.WebName})");

                    int? sortNumber = GetSortNumber(thisFile.Name);

                    ScriptSplitter scriptCollection = FileHelper.LoadSqlScript(thisFile, encoding);

                    foreach (string scriptContent in scriptCollection)
                    {
                        mergedContent.AppendLine(scriptContent);
                        mergedContent.AppendLine(this.m_options.BatchSeparator);
                    }

                } // Next thisFile 

                if (this.m_options.IncludeAfterMergeProcedure)
                {
                    mergedContent.AppendLine(GenerateAfterMergeProcedure());
                } // End if (this.m_options.IncludeAfterMergeProcedure) 

                if (this.m_options.IncludeVersionTable)
                {
                    mergedContent.AppendLine(GenerateVersionTableScript());
                } // End if (this.m_options.IncludeVersionTable) 

                FileHelper.WriteToFile(outputPath, mergedContent.ToString(), System.Text.Encoding.UTF8);
                System.Console.WriteLine($"Successfully merged SQL files into: {outputPath}");
            } // End Try 
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error during merge: {ex.Message}");
                throw;
            } // End Catch 

        } // End Sub Merge 


        private string GenerateAfterMergeProcedure()
        {
            return $@"
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = object_id(N'[{this.m_options.AfterMergeProcedureName}]') AND type IN (N'P', N'PC')) 
BEGIN 
    EXECUTE dbo.[{this.m_options.AfterMergeProcedureName}] '<SQL><![CDATA[{this.m_options.VersionValue}]]></SQL>'
END; ";
        } // End Function GenerateAfterMergeProcedure 


        private string GenerateVersionTableScript()
        {
            return $@"
GO

PRINT 'Writing entry into {this.m_options.VersionTableName}'

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.[{this.m_options.VersionTableName}]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.[{this.m_options.VersionTableName}]
    (
         CV_UID uniqueidentifier NOT NULL DEFAULT ( NEWID() ) 
        ,CV_Key national character varying(200) NOT NULL
        ,CV_Value national character varying(MAX) NOT NULL
        ,CV_User national character varying(200) NULL DEFAULT ( SUSER_SNAME() )
        ,CV_Note national character varying(200) NULL
        ,CV_Date datetime NOT NULL DEFAULT ( CURRENT_TIMESTAMP )
        ,CONSTRAINT [PK_{this.m_options.VersionTableName}] PRIMARY KEY (CV_UID) 
    )
END

GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{this.m_options.VersionTableName}')
BEGIN 
    INSERT INTO {this.m_options.VersionTableName}(CV_Key, CV_Value, CV_User, CV_Note, CV_Date)
    VALUES (
        N'{this.m_options.VersionKey}',
        N'{this.m_options.VersionValue?.Replace("'", "''")}',
        N'{this.m_options.VersionUser?.Replace("'", "''")}',
        N'{this.m_options.VersionNote?.Replace("'", "''")}',
        '{this.m_options.VersionDate:yyyy-MM-dd HH:mm:ss}'
    );
END

GO

PRINT 'Finished writing entry into {this.m_options.VersionTableName}'

GO";
        } // End Function GenerateVersionTableScript 


    } // End Class SqlMergeService 


} // End Namespace 
