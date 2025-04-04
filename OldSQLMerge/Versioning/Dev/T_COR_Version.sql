
-- SELECT * FROM T_COR_Version; 


CREATE TABLE dbo.T_COR_Version 
( 
	 CV_UID uniqueidentifier NOT NULL 
	,CV_Key national character varying (200) NOT NULL 
	,CV_Value national character varying (MAX) NOT NULL 
	,CV_User national character varying (200) NULL 
	,CV_Note national character varying (200) NULL 
	,CV_Date datetime NOT NULL 
	,CONSTRAINT PK_T_COR_Version PRIMARY KEY ( CV_UID ) 
); 
GO 

ALTER TABLE dbo.T_COR_Version ADD CONSTRAINT DF_T_COR_Version_CV_UID DEFAULT( NEWID() ) FOR CV_UID 
GO 

ALTER TABLE dbo.T_COR_Version ADD CONSTRAINT DF_T_COR_Version_CV_User DEFAULT( SUSER_SNAME() ) FOR CV_User 
GO 

-- ALTER TABLE dbo.T_COR_Version ADD CONSTRAINT DF_T_COR_Version_CV_Date DEFAULT(GETDATE()) FOR CV_Date 
ALTER TABLE dbo.T_COR_Version ADD CONSTRAINT DF_T_COR_Version_CV_Date DEFAULT( CURRENT_TIMESTAMP ) FOR CV_Date 
GO 





IF NULLIF(@operation_type, '') IS NULL OR @operation_type NOT IN ('N', 'C', 'D') 
BEGIN 
	RAISERROR 
	( 
			N'operation_type nicht spezifiziert.' -- Message text. 
		,18 -- Severity 
		,1 -- State 
	); 
END 

