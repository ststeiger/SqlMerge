
-- Requires SQLCMD mode (Query > SQLCMD Mode)
-- Extras -> Optionen -> Abfrageausführung -> Standardmässig neue Abfragen im SQLCMD-Modus öffnen 
-- In SSMS, go to Tools -> Options -> Query Execution > By default, open new queries in SQLCMD mode.
:ON ERROR EXIT

-- script1
PRINT 'script1';
GO

-- script2
USE BasicReports1; -- This will fail
GO

-- script3
PRINT 'script3'; -- This will NOT run
GO
