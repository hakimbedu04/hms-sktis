CREATE PROCEDURE [dbo].[ScriptUpdateMigration]
(
	@FilePath varchar(max)
) AS
BEGIN
	IF OBJECT_ID('tempdb..#TEMP_TABLE_MIGRATION') IS NOT NULL
	BEGIN
		DROP TABLE #TEMP_TABLE_MIGRATION
	END

	CREATE TABLE #TEMP_TABLE_MIGRATION (
		Filename varchar(max)
	)

	DECLARE @SQL_EXEC nvarchar(max)

	SET @SQL_EXEC = '
		BULK INSERT #TEMP_TABLE_MIGRATION FROM ''' + @FilePath + ''' 
		WITH(ROWTERMINATOR = ''\n'')
		MERGE into VT_SchemaChangeLog
		USING (
			SELECT
				SUBSTRING(Filename, 1, (CHARINDEX(''.'', Filename) - 1)) as MajorReleaseNumber,
				SUBSTRING(Filename, (CHARINDEX(''.'', Filename) + 1), (CHARINDEX(''-'', Filename) - (CHARINDEX(''.'', Filename) + 1))) as MinorReleaseNumber,
				Filename as ScriptName,
				GETDATE() as ScriptDateApplied
			FROM #TEMP_TABLE_MIGRATION
		) as tmp on VT_SchemaChangeLog.ScriptName = tmp.ScriptName
		WHEN NOT MATCHED THEN 
		INSERT (
			MajorReleaseNumber,
			MinorReleaseNumber,
			ScriptName,
			ScriptDateApplied
		) VALUES (
			tmp.MajorReleaseNumber,
			tmp.MinorReleaseNumber,
			tmp.ScriptName,
			tmp.ScriptDateApplied
		);'
	EXEC (@SQL_EXEC)
END