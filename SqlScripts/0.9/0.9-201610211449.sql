/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV_V2]    Script Date: 10/21/2016 2:48:13 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV_V2]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV_V2]
GO

/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV_V2]    Script Date: 10/21/2016 2:48:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV_V2]
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@tmpProductionDate DATETIME,
	@dummyGroup VARCHAR(4)
AS
BEGIN
	SET DATEFIRST 1;

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
	FROM MstPlantEmpJobsDataAcvTemp  WHERE EmployeeID = @empUpd_ID 

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

	DECLARE @absentType VARCHAR(128);
	SET @absentType = 'Multiskill Out';

	-- Check Entry Verification
	IF NOT EXISTS
	(
		SELECT * FROM ExePlantProductionEntryVerification WHERE ProductionEntryCode = @productionEntryCodeUpd
	)
	BEGIN
		DECLARE @workhour INT;
		SET @workhour = 0;
		IF EXISTS(SELECT * FROM MstGenHoliday WHERE HolidayDate = @tmpProductionDate)
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
						,'Resmi' as [StatusEmp]
						,1 as [StatusIdentifier]
						,NULL as [StartDateAbsent]
						,NULL as [AbsentType]
						,[ProdCapacity]
						,[ProdTarget]
						,[ProdActual]
						,NULL as [AbsentRemark]
						,NULL as [AbsentCodeEblek]
						,NULL as [AbsentCodePayroll]
						,GETDATE() as [CreatedDate]
						,'System' as [CreatedBy]
						,GETDATE() as [UpdatedDate]
						,'System' as [UpdatedBy]
						,NULL as [IsFromAbsenteeism]
			FROM @tmpEntryOld WHERE EmployeeID = @empUpd_ID and ProductionDate = @tmpProductionDate and ProductionEntryCode = @productionEntryCodeOLD
		END
	
		DELETE @tmpEntryOld;
	END
	ELSE
	BEGIN
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
					   ,NULL
					   ,0
					   ,0
					   ,0
					   ,NULL
					   ,NULL
					   ,NULL
					   ,GETDATE()
					   ,'SYSTEM'
					   ,GETDATE()
					   ,'SYSTEM'
					   ,NULL)
		END
	END

	-- DELETE DUMMY
	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup
	)
	BEGIN
		DELETE e
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup
	END

	-- delete verification dummy if there is no entry dummy left
	DECLARE @countEnt INT;
	DECLARE @prodEntryCodeDummy VARCHAR(64);

	SELECT @countEnt = COUNT(*) 
	FROM ExePlantProductionEntry e
	inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
	WHERE v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup

	SELECT TOP 1 @prodEntryCodeDummy = v.ProductionEntryCode 
	FROM ExePlantProductionEntryVerification v
	WHERE v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup

	IF(@countEnt = 0)
	BEGIN
		DELETE ExePlantProductionEntryVerification
		WHERE ProductionEntryCode = @prodEntryCodeDummy;
	END
END

GO