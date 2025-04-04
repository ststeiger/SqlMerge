
-- REM: Wird am Ende von "SQLMerge.exe" augerufen 
ALTER PROCEDURE dbo.stp_COR_afterSQLMerge 
	@XML xml -- REM: Parameter (=<SQL><![CDATA[0001_basicLink.sql" in "P:\COR_Basic\Release\V4\v404 (CAFM)]]></SQL>)
AS 
BEGIN 
	-- REM: Version aktualisieren 
	INSERT INTO dbo.T_COR_Version ( CV_Key, CV_Value, CV_Note  ) 
	SELECT 
		 'SQL' AS CV_Key 
		,[row].c.value('SQL[1]', 'varchar(255)') AS CV_Value 
		,'[dbo].[stp_COR_afterSQLMerge]' AS CV_Note 
	FROM @XML.nodes('.') AS [row](c) 
	WHERE @XML.exist('SQL[1]') = 1 
	; 

	-- REM: Release aktualisieren 
	UPDATE dbo.T_FMS_Configuration 
		SET FC_Value = CONVERT(varchar(10), CURRENT_TIMESTAMP, 104) 
	WHERE FC_Key = 'Release' 
	; 
END;

