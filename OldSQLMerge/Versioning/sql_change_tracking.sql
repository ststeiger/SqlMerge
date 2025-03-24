
-- schema_update_history
-- db_migration_log
-- schema_change_log
-- script_execution_log
-- deployment_log
-- change_tracking 
-- sql_change_log

-- Yes, it's generally a best practice to separate schema updates (DDL) from data updates (DML) in SQL scripts. Here’s why:
--   - Schema changes (e.g., ALTER TABLE) can fail due to constraints, dependencies, or locks.
--   - Data updates (e.g., UPDATE, DELETE) might depend on a schema that hasn’t changed yet.
--   - If both are in the same script, a failure in one part can leave the database in an inconsistent state.
-- However, data-changes depend on schema-changes, so separating the two is actually a bad practice 
-- Whats worse, schema-changes may in turn depend on data-changes 
-- Better to have all in one and follow the original sequence of events. 

IF NOT EXISTS(
	SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'deployment' 
)
BEGIN
	EXECUTE('CREATE SCHEMA deployment; ');
END



IF NOT EXISTS
(
	SELECT * FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_TYPE = 'BASE TABLE' 
	AND TABLE_SCHEMA = 'deployment' 
	AND TABLE_NAME = 'sql_change_tracking' 
)
BEGIN
	EXECUTE(N'CREATE TABLE deployment.sql_change_tracking 
( 
	 ct_uid uniqueidentifier NOT NULL 
	,ct_script_sequence_no int 
	,ct_script_sub_sequence_no int 
	,ct_script_name national character varying(4000) NOT NULL 
	,ct_folder_name national character varying(4000) NOT NULL 
	,ct_executed_by national character varying(1000) NOT NULL 
	,ct_executed_at datetime2 CONSTRAINT df_ct_executed_at DEFAULT CURRENT_TIMESTAMP 
	,ct_succeeded bit NOT NULL CONSTRAINT df_ct_succeeded DEFAULT ''false'' 
	,ct_error_message national character varying(MAX) -- Stores error details if the script fails 
	,CONSTRAINT pk_sql_change_tracking PRIMARY KEY ( ct_uid ) 
); 

'); 
END


GO 


DECLARE @__script_sqeuence_number int; 
DECLARE @__script_name national character varying(4000); 
DECLARE @__script_path national character varying(4000); 

SET @__script_sqeuence_number = 123; 
SET @__script_name = N'Test'; 
SET @__script_path = N'Test123'; 



INSERT INTO deployment.sql_change_tracking 
( 
	 ct_uid 
	,ct_script_sequence_no 
	,ct_script_name 
	,ct_folder_name 
	,ct_executed_by 
	,ct_executed_at 
	,ct_succeeded 
	,ct_error_message 
) 
SELECT 
	 NEWID() AS ct_uid -- uniqueidentifier 
	,@__script_sqeuence_number AS ct_script_sequence_no -- int 
	,@__script_name AS ct_script_name -- nvarchar(4000)
	,@__script_path AS ct_folder_name -- nvarchar(4000) 
	,SUSER_SNAME() AS ct_executed_by -- nvarchar(1000) 
	,CURRENT_TIMESTAMP AS ct_executed_at -- datetime2 
	,N'true' AS ct_succeeded -- bit 
	,NULLIF(N'hello', N'') AS ct_error_message -- nvarchar(MAX) 
; 


TRUNCATE TABLE deployment.sql_change_tracking; 
