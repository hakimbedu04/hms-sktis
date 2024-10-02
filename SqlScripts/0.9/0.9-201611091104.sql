--script add button complete on approval page By Wahyu

if not exists(select top 1 * from [SKTIS].[dbo].[UtilFunctions]
              where [ParentIDFunction] = 399 AND [FunctionName] = 'Complete' AND [FunctionType] = 'Button' ) 
begin
    INSERT INTO [SKTIS].[dbo].[UtilFunctions]
           ([ParentIDFunction]
           ,[FunctionName]
           ,[FunctionType]
           ,[FunctionDescription]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (399
           ,'Complete'
           ,'Button'
           ,NULL
           ,'2016-11-09 12:00:00.000'
           ,'PMI\bkristom'
           ,'2016-11-09 12:00:00.000'
           ,'PMI\bkristom')
end
GO


if not exists(select * from [SKTIS].[dbo].[UtilRolesFunction] 
              where [IDFunction] = (select top 1 IDFunction from [SKTIS].[dbo].[UtilFunctions]
              where [ParentIDFunction] = 399 AND [FunctionName] = 'Complete' AND [FunctionType] = 'Button') AND				  [IDRole] = 20 ) 
begin

	INSERT INTO [SKTIS].[dbo].[UtilRolesFunction]
			   ([IDFunction]
			   ,[IDRole]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		 VALUES
			   ((select top 1 IDFunction from [SKTIS].[dbo].[UtilFunctions]
                 where [ParentIDFunction] = 399 AND [FunctionName] = 'Complete' AND [FunctionType] = 'Button')
			   ,20
			   ,'2016-11-09 10:01:58.770'
			   ,'PMI\bkristom'
			   ,'2016-11-09 10:01:58.770'
			   ,'PMI\bkristom')
end
GO

