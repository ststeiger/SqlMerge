
-- DECLARE @xml XML; 
-- SET @xml = ( SELECT ( 

-- SELECT *  FROM T_Benutzer  

-- FOR XML PATH('row'), ROOT('table'),  ELEMENTS xsinil) AS outerXml ) 
-- SELECT @xml 


DECLARE @xml xml; 
SET @xml = N'<?xml version="1.0" encoding="utf-16"?>
<DeploymentTracking xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <sql_change_tracking>
    <ct_uid>2a555424-6175-4eec-b666-9946c9d12fc9</ct_uid>
    <ct_script_sequence_no>1</ct_script_sequence_no>
    <ct_script_sub_sequence_no>1</ct_script_sub_sequence_no>
    <ct_script_name>Script1.sql</ct_script_name>
    <ct_folder_name>Folder1</ct_folder_name>
    <ct_executed_by>User1</ct_executed_by>
    <ct_executed_at>2025-03-24T15:00:33.1214666+01:00</ct_executed_at>
    <ct_succeeded>true</ct_succeeded>
  </sql_change_tracking>

  <sql_change_tracking>
    <ct_uid>6849bbf6-cb5f-4322-b622-add209a6c0e0</ct_uid>
    <ct_script_sequence_no>2</ct_script_sequence_no>
    <ct_script_sub_sequence_no>1</ct_script_sub_sequence_no>
    <ct_script_name>Script2.sql</ct_script_name>
    <ct_folder_name>Folder1</ct_folder_name>
    <ct_executed_by>User2</ct_executed_by>
    <ct_executed_at>2025-03-23T15:00:33.1234845+01:00</ct_executed_at>
    <ct_succeeded>false</ct_succeeded>
    <ct_error_message>Error occurred</ct_error_message>
  </sql_change_tracking>

</DeploymentTracking>'; 


;WITH XMLNAMESPACES ('http://www.w3.org/2001/XMLSchema-instance' AS xsi) 
SELECT 
 --    'SQL' AS CV_Key 
 --   -- ,t_row_data.xml_row.value('SQL[1]', 'varchar(255)') AS CV_Value 
	---- ,t_row_data.xml_row.value('(SQL[not(@xsi:nil = "true")])[1]', 'varchar(255)') AS CV_Value 
	
	--,ISNULL((OBJECT_SCHEMA_NAME(@@PROCID) + N'.' + OBJECT_NAME(@@PROCID)), 'dbo.stp_COR_afterSQLMerge') AS CV_Note 
	--,SUSER_SNAME() AS sy 
	
	 t_row_data.xml_row.query('.') AS q 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_uid"][not(@*[local-name()="nil" and . ="true"])])[1]', 'uniqueidentifier') AS ct_uid 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_script_sequence_no"][not(@*[local-name()="nil" and . ="true"])])[1]', 'int') AS ct_script_sequence_no 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_script_sub_sequence_no"][not(@*[local-name()="nil" and . ="true"])])[1]', 'int') AS ct_script_sub_sequence_no 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_script_name"][not(@*[local-name()="nil" and . ="true"])])[1]', 'national character varying(4000)') AS ct_script_name 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_folder_name"][not(@*[local-name()="nil" and . ="true"])])[1]', 'national character varying(4000)') AS ct_folder_name 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_executed_by"][not(@*[local-name()="nil" and . ="true"])])[1]', 'national character varying(4000)') AS ct_executed_by 

	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_executed_at"][not(@*[local-name()="nil" and . ="true"])])[1]', 'datetime2') AS ct_executed_at 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_succeeded"][not(@*[local-name()="nil" and . ="true"])])[1]', 'bit') AS ct_succeeded 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "ct_error_message"][not(@*[local-name()="nil" and . ="true"])])[1]', 'national character varying(4000)') AS ct_error_message 
FROM @xml.nodes('//sql_change_tracking') AS t_row_data(xml_row) 
WHERE (1=1) 
-- AND t_row_data.xml_row.exist('ct_uid[1]') = 1 
-- AND t_row_data.xml_row.exist('(./*[lower-case(local-name(.)) = "ct_uid"][not(@*[local-name()="nil" and . ="true"])])[1]') = 1 
;
