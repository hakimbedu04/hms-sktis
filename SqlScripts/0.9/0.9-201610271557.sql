SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_ACV]
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@tmpProductionDate DATETIME
AS
BEGIN
	SET DATEFIRST 1;

	DECLARE @absentType VARCHAR(128);
	SET @absentType = 'Multiskill Out';
	
	DECLARE @oldLocationCode VARCHAR(8);
	DECLARE @oldUnitCode VARCHAR(4);
	DECLARE @oldGroupCode VARCHAR(4);
	DECLARE @oldProcess VARCHAR(16);
	DEClARE @oldEmpNumber VARCHAR(6);
	DECLARE @oldStatus VARCHAR(64);

	-- Get process Order
	DECLARE @processOrder INT;
	SELECT @processOrder = ProcessOrder from mstgenprocess WHERE ProcessGroup = @empUpd_ProcessSettingsCode;

	-- Get old data employee
	SELECT @oldLocationCode = LocationCode, @oldUnitCode = UnitCode, @oldGroupCode = GroupCode, @oldProcess = ProcessSettingsCode,
			@oldEmpNumber = EmployeeNumber, @oldStatus = Status
	FROM MstPlantEmpJobsDataAcv  WHERE EmployeeID = @empUpd_ID 

	-- Get KpsYear, KpsWeek
	DECLARE @kpsWeekEffDate INT;
	DECLARE @kpsYearEffDate INT;
	SELECT TOP 1 @kpsWeekEffDate = Week, @kpsYearEffDate = Year
	FROM MstGenWeek WHERE @tmpProductionDate BETWEEN StartDate AND EndDate

	-- Get BrandCode, Shift
	DECLARE @brandCodeUpd VARCHAR(11);
	DECLARE @shiftUpd INT;
	SELECT TOP 1 @brandCodeUpd = BrandCode, @shiftUpd = Shift  
	FROM ExePlantProductionEntryVerification WHERE ProductionDate = @tmpProductionDate and LocationCode = @oldLocationCode;

	-- Old Production Entry Code
	DECLARE @productionEntryCodeOLD VARCHAR(64);
	SET @productionEntryCodeOLD = 'EBL/' + @oldLocationCode 
									+ '/' + CONVERT(varchar, @shiftUpd) 
									+ '/' + @oldUnitCode 
									+ '/' + @oldGroupCode 
									+ '/' + @brandCodeUpd 
									+ '/' + CONVERT(varchar,@kpsYearEffDate) 
									+ '/' + CONVERT(varchar,@kpsWeekEffDate)
									+ '/' + CONVERT(varchar,(select datepart(dw, @tmpProductionDate)));

	-- Update Production Entry Code
	DECLARE @productionEntryCodeUpd VARCHAR(64);
	SET @productionEntryCodeUpd = 'EBL/' + @empUpd_LocationCode 
									+ '/' + CONVERT(varchar, @shiftUpd) 
									+ '/' + @empUpd_UnitCode 
									+ '/' + @empUpd_GroupCode 
									+ '/' + @brandCodeUpd 
									+ '/' + CONVERT(varchar,@kpsYearEffDate) 
									+ '/' + CONVERT(varchar,@kpsWeekEffDate)
									+ '/' + CONVERT(varchar,(select datepart(dw, @tmpProductionDate)));

	-- Check Entry Verification
	IF NOT EXISTS
	(
		SELECT * FROM ExePlantProductionEntryVerification WHERE ProductionEntryCode = @productionEntryCodeUpd
	)
	BEGIN
		DECLARE @workhour INT;
		SET @workhour = 0;
		IF EXISTS(SELECT * FROM MstGenHoliday WHERE HolidayDate = @tmpProductionDate AND LocationCode = @empUpd_LocationCode)
		BEGIN 
			SELECT @workhour = JknHour FROM MstGenStandardHours WHERE DayType = 'Holiday' AND [Day] = CONVERT(int,(select datepart(dw, @tmpProductionDate)))
		END
		ELSE
		BEGIN
			SELECT @workhour = JknHour FROM MstGenStandardHours WHERE DayType = 'Non-Holiday' AND [Day] = CONVERT(int,(select datepart(dw, @tmpProductionDate)))
		END

		--- insert verification upd
		INSERT INTO [dbo].[ExePlantProductionEntryVerification]
			   ([ProductionEntryCode]
			   ,[LocationCode]
			   ,[UnitCode]
			   ,[Shift]
			   ,[ProcessGroup]
			   ,[ProcessOrder]
			   ,[GroupCode]
			   ,[BrandCode]
			   ,[KPSYear]
			   ,[KPSWeek]
			   ,[ProductionDate]
			   ,[WorkHour]
			   ,[TPKValue]
			   ,[TotalTargetValue]
			   ,[TotalActualValue]
			   ,[TotalCapacityValue]
			   ,[VerifySystem]
			   ,[VerifyManual]
			   ,[Remark]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   ,[Flag_Manual])
		VALUES
			   (@productionEntryCodeUpd
			   ,@empUpd_LocationCode
			   ,@empUpd_UnitCode
			   ,1
			   ,@empUpd_ProcessSettingsCode
			   ,@processOrder
			   ,@empUpd_GroupCode
			   ,@brandCodeUpd
			   ,@kpsYearEffDate
			   ,@kpsWeekEffDate
			   ,@tmpProductionDate
			   ,@workhour
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,GETDATE()
			   ,'SYSTEM'
			   ,GETDATE()
			   ,'SYSTEM'
			   ,0)
	END
	
	--- temp Old Entry ------
	DECLARE @tmpEntryOld TABLE
	(
		[ProductionEntryCode] [varchar](64) NOT NULL,
		[EmployeeID] [varchar](64) NOT NULL,
		[EmployeeNumber] [varchar](11) NULL,
		[StatusEmp] [varchar](64) NOT NULL,
		[StatusIdentifier] [int] NOT NULL,
		[StartDateAbsent] [datetime] NULL,
		[AbsentType] [varchar](128) NULL,
		[ProdCapacity] [decimal](18, 3) NULL,
		[ProdTarget] [real] NULL,
		[ProdActual] [real] NULL,
		[AbsentRemark] [varchar](256) NULL,
		[AbsentCodeEblek] [varchar](128) NULL,
		[AbsentCodePayroll] [varchar](128) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [varchar](64) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[UpdatedBy] [varchar](64) NOT NULL,
		[IsFromAbsenteeism] [bit] NULL,
		[ProductionDate] [datetime] NULL
	);

	IF EXISTS
	(
		SELECT e.*, v.ProductionDate FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		where v.ProductionDate = @tmpProductionDate AND EmployeeID = @empUpd_ID AND e.AbsentType = @absentType
		AND v.ProductionEntryCode = @productionEntryCodeOLD
	)
	BEGIN
		INSERT INTO @tmpEntryOld
		SELECT e.*, v.ProductionDate FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		where v.ProductionDate = @tmpProductionDate AND EmployeeID = @empUpd_ID AND e.AbsentType = @absentType
		AND v.ProductionEntryCode = @productionEntryCodeOLD

		DELETE e 
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		where v.ProductionDate = @tmpProductionDate AND EmployeeID = @empUpd_ID AND e.AbsentType = @absentType
		AND v.ProductionEntryCode = @productionEntryCodeOLD

		IF NOT EXISTS
		(
			SELECT * FROM ExePlantProductionEntry WHERE EmployeeID = @empUpd_ID AND ProductionEntryCode = @productionEntryCodeUpd
		)
		BEGIN
			INSERT INTO [dbo].[ExePlantProductionEntry]
						([ProductionEntryCode]
						,[EmployeeID]
						,[EmployeeNumber]
						,[StatusEmp]
						,[StatusIdentifier]
						,[StartDateAbsent]
						,[AbsentType]
						,[ProdCapacity]
						,[ProdTarget]
						,[ProdActual]
						,[AbsentRemark]
						,[AbsentCodeEblek]
						,[AbsentCodePayroll]
						,[CreatedDate]
						,[CreatedBy]
						,[UpdatedDate]
						,[UpdatedBy]
						,[IsFromAbsenteeism])
					SELECT
						 @productionEntryCodeUpd as [ProductionEntryCode]
						,@empUpd_ID as [EmployeeID]
						,@empUpd_Number as [EmployeeNumber]
						,[StatusEmp]
						,[StatusIdentifier]
						,[StartDateAbsent]
						,[AbsentType]
						,[ProdCapacity]
						,[ProdTarget]
						,[ProdActual]
						,[AbsentRemark]
						,[AbsentCodeEblek]
						,[AbsentCodePayroll]
						,GETDATE() as [CreatedDate]
						,'System' as [CreatedBy]
						,GETDATE() as [UpdatedDate]
						,'System' as [UpdatedBy]
						,[IsFromAbsenteeism]
			FROM @tmpEntryOld WHERE EmployeeID = @empUpd_ID and ProductionDate = @tmpProductionDate and ProductionEntryCode = @productionEntryCodeOLD
		END
		
		DELETE @tmpEntryOld;
	END
	ELSE
	BEGIN
		DECLARE @SktAbsentCode		VARCHAR(11);
		DECLARE @PayrollAbsentCode	VARCHAR(11);
		DECLARE @isFromAbsenteeism BIT;

		SELECT @SktAbsentCode = SktAbsentCode, @PayrollAbsentCode = PayrollAbsentCode,
		@isFromAbsenteeism = 1
		FROM MstPlantAbsentType WHERE AbsentType = @absentType

		IF NOT EXISTS
		(
			SELECT * FROM ExePlantProductionEntry WHERE EmployeeID = @empUpd_ID AND ProductionEntryCode = @productionEntryCodeUpd
		)
		BEGIN
			INSERT INTO [dbo].[ExePlantProductionEntry]
					   ([ProductionEntryCode]
					   ,[EmployeeID]
					   ,[EmployeeNumber]
					   ,[StatusEmp]
					   ,[StatusIdentifier]
					   ,[StartDateAbsent]
					   ,[AbsentType]
					   ,[ProdCapacity]
					   ,[ProdTarget]
					   ,[ProdActual]
					   ,[AbsentRemark]
					   ,[AbsentCodeEblek]
					   ,[AbsentCodePayroll]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy]
					   ,[IsFromAbsenteeism])
				 VALUES
					   (@productionEntryCodeUpd
					   ,@empUpd_ID
					   ,@empUpd_Number
					   ,'Resmi'
					   ,1
					   ,@tmpProductionDate
					   ,@absentType
					   ,0
					   ,0
					   ,0
					   ,NULL
					   ,@SktAbsentCode
					   ,@PayrollAbsentCode
					   ,GETDATE()
					   ,'SYSTEM'
					   ,GETDATE()
					   ,'SYSTEM'
					   ,@isFromAbsenteeism)
		END
	END
END
