
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


CREATE TABLE deployment.sql_change_tracking 
(
	 ct_uid uniqueidentifier NOT NULL 
	,ct_script_sequence_no int NOT NULL 
	,ct_script_name national character varying(4000) NOT NULL 
	,ct_folder_name national character varying(4000) NOT NULL 
	,ct_executed_by national character varying(1000) NOT NULL 
	,ct_executed_at datetime2 CONSTRAINT df_ct_executed_at DEFAULT CURRENT_TIMESTAMP 
	,ct_succeeded bit NOT NULL CONSTRAINT df_ct_succeeded DEFAULT 'false' 
	,ct_error_message national character varying(MAX) -- Stores error details if the script fails 
	,CONSTRAINT pk_sql_change_tracking PRIMARY KEY (ct_uid) 
);