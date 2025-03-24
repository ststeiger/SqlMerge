
-- DECLARE @xml XML; 
-- SET @xml = ( SELECT ( 

-- SELECT *  FROM T_Benutzer  

-- FOR XML PATH('row'), ROOT('table'),  ELEMENTS xsinil) AS outerXml ) 
-- SELECT @xml 


DECLARE @XML xml; 
SET @XML = N'<root xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <row>
        <SQL>foo</SQL>
        <BE_usePRT xsi:nil="true" />
    </row>
    <row>
        <sql>bar</sql>
        <BE_usePRT xsi:nil="true" />
    </row>
    <row>
        <SQL>baz</SQL>
        <BE_usePRT>hello</BE_usePRT>
    </row>
</root>'; 


;WITH XMLNAMESPACES ('http://www.w3.org/2001/XMLSchema-instance' AS xsi) 
SELECT 
     'SQL' AS CV_Key 
    -- ,t_row_data.xml_row.value('SQL[1]', 'varchar(255)') AS CV_Value 
	-- ,t_row_data.xml_row.value('(SQL[not(@xsi:nil = "true")])[1]', 'varchar(255)') AS CV_Value 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "sql"][not(@*[local-name()="nil" and . ="true"])])[1]', 'varchar(255)') AS CV_Value 
	,t_row_data.xml_row.value('(./*[lower-case(local-name(.)) = "be_useprt"][not(@*[local-name()="nil" and . ="true"])])[1]', 'varchar(255)') AS BE_usePRT 
	,ISNULL((OBJECT_SCHEMA_NAME(@@PROCID) + N'.' + OBJECT_NAME(@@PROCID)), 'dbo.stp_COR_afterSQLMerge') AS CV_Note 
	,SUSER_SNAME() AS sy 
FROM @XML.nodes('//row') AS t_row_data(xml_row) 
WHERE (1=1) 
-- AND t_row_data.xml_row.exist('SQL[1]') = 1 
AND t_row_data.xml_row.exist('(./*[lower-case(local-name(.)) = "sql"][not(@*[local-name()="nil" and . ="true"])])[1]') = 1 
;
