DECLARE	@return_value int

EXEC	@return_value = [dbo].[ScriptUpdateMigration]
		@FilePath = N'C:\MIGRATION_LIST.txt'

SELECT	'Return Value' = @return_value

GO
