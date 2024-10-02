CREATE PROCEDURE [dbo].[RECALCULATEREPORTBYGROUPWEEKLYMONTHLYNOGROUP]
	   @LocationCode VARCHAR(8),            	
	   @BrandCode       VARCHAR(11),	 
	   @Week         INT,
	   @Year         INT,	 
	   @Date        DATETIME
AS
BEGIN
	
	BEGIN TRANSACTION

	BEGIN TRY

	--ExeReportByGroupsWeekly

		--DELETE FIRST THE DATA
		DELETE FROM ExeReportByGroupsWeekly
		WHERE LocationCode = @LocationCode 			
			AND BrandCode = @BrandCode		
			AND KPSWeek = @Week
			AND KPSYear = @Year

		--THEN INSERT
		INSERT INTO ExeReportByGroupsWeekly
		(	[LocationCode],[UnitCode],[ProcessGroup],[GroupCode] ,[BrandGroupCode]
           ,[BrandCode],[StatusEmp],[KPSWeek],[KPSYear],[Shift],[Production]
           ,[TPKValue],[WorkHour],[WeekDay],[Register]
		   ,[Absennce_A],[Absence_I],[Absence_C],[Absence_CH],[Absence_CT]
           ,[Absence_SLS],[Absence_SLP],[Absence_ETC],[Multi_TPO]
           ,[Multi_ROLL],[Multi_CUTT],[Multi_PACK],[Multi_STAMP]
           ,[Multi_FWRP],[Multi_SWRP],[Multi_GEN],[Multi_WRP],[Out]
           ,[Attend],[CreatedDate] ,[CreatedBy],[UpdatedDate]
           ,[UpdatedBy],[EmpIn],[ValueHour],[ValuePeople]
           ,[ValuePeopleHour] ,[ActualWorker] ,[Absence_S])
		SELECT
			LocationCode,UnitCode,ProcessGroup,GroupCode,BrandGroupCode,
			BrandCode,StatusEmp,@Week ,	@Year,Shift,AVG(Production),
			AVG(TPKValue),AVG(WorkHour),AVG(WeekDay),AVG(Register),
			AVG(Absennce_A),AVG(Absence_I),	AVG(Absence_C),	AVG(Absence_CH),AVG(Absence_CT),
			AVG(Absence_SLS),AVG(Absence_SLP),AVG(Absence_ETC),	AVG(Multi_TPO),
			AVG(Multi_ROLL),AVG(Multi_CUTT),AVG(Multi_PACK),AVG(Multi_STAMP),
			AVG(Multi_FWRP),AVG(Multi_SWRP),AVG(Multi_GEN),	AVG(Multi_WRP),	AVG(Out),
			AVG(Attend),GETDATE(),'System',GETDATE(),
			'System',AVG(EmpIn),AVG(ValueHour),	AVG(ValuePeople),
			AVG(ValuePeopleHour),AVG(ActualWorker),	AVG(Absence_S)
		FROM ExeReportByGroups
		WHERE LocationCode = @LocationCode 
		--AND GroupCode = @GroupCode 
		AND BrandCode = @BrandCode		
		AND KPSWeek = @Week
		AND KPSYear = @Year
		GROUP BY
			LocationCode,
			UnitCode,
			ProcessGroup,
			GroupCode,
			BrandGroupCode,
			BrandCode,
			StatusEmp,
			KPSWeek, 
			KPSYear	,
			Shift	

		
		--ExeReportByGroupsMonthly
		--DELETE FIRST THE DATA
		DELETE FROM ExeReportByGroupsMonthly
		WHERE LocationCode = @LocationCode 
			--AND GroupCode = @GroupCode 
			AND BrandCode = @BrandCode			
			AND Month = DATEPART(MONTH, @Date)
			AND Year = @Year


		--THEN INSERT
		INSERT INTO ExeReportByGroupsMonthly
		(	[LocationCode],[UnitCode],[ProcessGroup],[GroupCode] ,[BrandGroupCode]
           ,[BrandCode],[StatusEmp],[Month],[Year],[Shift], [Production]
           ,[TPKValue],[WorkHour],[WeekDay],[Register]
		   ,[Absennce_A],[Absence_I],[Absence_C],[Absence_CH],[Absence_CT]
           ,[Absence_SLS],[Absence_SLP],[Absence_ETC],[Multi_TPO]
           ,[Multi_ROLL],[Multi_CUTT],[Multi_PACK],[Multi_STAMP]
           ,[Multi_FWRP],[Multi_SWRP],[Multi_GEN],[Multi_WRP],[Out]
           ,[Attend],[CreatedDate] ,[CreatedBy],[UpdatedDate]
           ,[UpdatedBy],[EmpIn],[ValueHour],[ValuePeople]
           ,[ValuePeopleHour] ,[ActualWorker] ,[Absence_S])
		SELECT
			LocationCode,UnitCode,ProcessGroup,GroupCode,BrandGroupCode,
			BrandCode,StatusEmp,DATEPART(MONTH,ProductionDate) ,@Year,Shift,AVG(Production),
			AVG(TPKValue),AVG(WorkHour),AVG(WeekDay),AVG(Register),
			AVG(Absennce_A),AVG(Absence_I),	AVG(Absence_C),	AVG(Absence_CH),AVG(Absence_CT),
			AVG(Absence_SLS),AVG(Absence_SLP),AVG(Absence_ETC),	AVG(Multi_TPO),
			AVG(Multi_ROLL),AVG(Multi_CUTT),AVG(Multi_PACK),AVG(Multi_STAMP),
			AVG(Multi_FWRP),AVG(Multi_SWRP),AVG(Multi_GEN),	AVG(Multi_WRP),	AVG(Out),
			AVG(Attend),GETDATE(),'System',GETDATE(),
			'System',AVG(EmpIn),AVG(ValueHour),	AVG(ValuePeople),
			AVG(ValuePeopleHour),AVG(ActualWorker),	AVG(Absence_S)
		FROM ExeReportByGroups
		WHERE LocationCode = @LocationCode 
		--AND GroupCode = @GroupCode 
		AND BrandCode = @BrandCode		
		AND DATEPART(MONTH, ProductionDate) = DATEPART(MONTH, @Date)
		AND KPSYear = @Year
		GROUP BY
			LocationCode,
			UnitCode,
			ProcessGroup,
			GroupCode,
			BrandGroupCode,
			BrandCode,
			StatusEmp,
			DATEPART(MONTH,ProductionDate), 
			KPSYear	,
			Shift	

	COMMIT TRANSACTION ;
	END TRY	
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH  

END