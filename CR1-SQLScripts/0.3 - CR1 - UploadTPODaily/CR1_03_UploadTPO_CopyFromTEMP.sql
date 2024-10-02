-- =============================================
-- Description: CR-1 Upload TPO Daily Copy from Temp to TPO Verification and TPO Production Entry
-- Author: Azka
-- Ticket: http://tp.voxteneo.co.id/entity/18755
-- Update: 5/30/2017
-- =============================================

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('UPLOAD_TPODAILY_COPYFROMTEMP'))
DROP PROCEDURE [dbo].[UPLOAD_TPODAILY_COPYFROMTEMP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UPLOAD_TPODAILY_COPYFROMTEMP]
	@listProductionEntryCode VARCHAR(MAX), -- list string production entry split by ';'
	@username VARCHAR(20)
AS
BEGIN
	DECLARE @splitProdEntryCode VARCHAR(50);

	DECLARE cursor_splitProductionEntryCode CURSOR LOCAL FOR
	SELECT splitdata FROM [dbo].[fnSplitString](@listProductionEntryCode, ';')

	OPEN cursor_splitProductionEntryCode

	FETCH NEXT FROM cursor_splitProductionEntryCode   
	INTO @splitProdEntryCode

	WHILE @@FETCH_STATUS = 0  
	BEGIN

		IF NOT EXISTS (SELECT * FROM ExeTPOProductionEntryVerification WHERE ProductionEntryCode = @splitProdEntryCode)
		BEGIN
			INSERT INTO [dbo].[ExeTPOProductionEntryVerification]
						   ([ProductionEntryCode]
						   ,[LocationCode]
						   ,[ProcessGroup]
						   ,[ProcessOrder]
						   ,[BrandCode]
						   ,[KPSYear]
						   ,[KPSWeek]
						   ,[ProductionDate]
						   ,[WorkHour]
						   ,[TotalTPKValue]
						   ,[TotalActualValue]
						   ,[VerifySystem]
						   ,[VerifyManual]
						   ,[Remark]
						   ,[CreatedDate]
						   ,[CreatedBy]
						   ,[UpdatedDate]
						   ,[UpdatedBy]
						   ,[Flag_Manual])
			SELECT 
							[ProductionEntryCode]
						   ,[LocationCode]
						   ,[ProcessGroup]
						   ,[ProcessOrder]
						   ,[BrandCode]
						   ,[KPSYear]
						   ,[KPSWeek]
						   ,[ProductionDate]
						   ,[WorkHour]
						   ,[TotalTPKValue]
						   ,[TotalActualValue]
						   ,[VerifySystem]
						   ,[VerifyManual]
						   ,[Remark]
						   ,GETDATE() as [CreatedDate]
						   ,@username as [CreatedBy]
						   ,GETDATE() as [UpdatedDate]
						   ,@username as [UpdatedBy]
						   ,[Flag_Manual]
			FROM ExeTPOProductionEntryVerificationTemp WHERE ProductionEntryCode = @splitProdEntryCode
			
			DELETE ExeTPOProductionEntryVerificationTemp WHERE ProductionEntryCode = @splitProdEntryCode
		END
		ELSE

		DECLARE @StatusEmp [varchar](64);
		DECLARE @ProductionGroup [varchar](20);
		DECLARE @WorkerCount [int];
		DECLARE @Absent [int];
		DECLARE @ActualProduction [real];

		-- cursor ExeTpoProduction Temp
		DECLARE cursor_tpoEntryTemp CURSOR LOCAL FOR
		SELECT StatusEmp, ProductionGroup, WorkerCount, Absent, ActualProduction FROM ExeTPOProductionTemp WHERE ProductionEntryCode = @splitProdEntryCode

		OPEN cursor_tpoEntryTemp

		FETCH NEXT FROM cursor_tpoEntryTemp   
		INTO @StatusEmp, @ProductionGroup, @WorkerCount, @Absent, @ActualProduction

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			IF (@WorkerCount > 0)
			BEGIN
				IF NOT EXISTS (SELECT * FROM ExeTPOProduction WHERE ProductionEntryCode = @splitProdEntryCode AND StatusEmp = @StatusEmp AND ProductionGroup = @ProductionGroup)
				BEGIN
					INSERT INTO [dbo].[ExeTPOProduction]
									   ([ProductionEntryCode]
									   ,[StatusEmp]
									   ,[StatusIdentifier]
									   ,[ProductionGroup]
									   ,[WorkerCount]
									   ,[Absent]
									   ,[ActualProduction]
									   ,[CreatedDate]
									   ,[CreatedBy]
									   ,[UpdatedDate]
									   ,[UpdatedBy])
					SELECT [ProductionEntryCode]
							,[StatusEmp]
							,[StatusIdentifier]
							,[ProductionGroup]
							,[WorkerCount]
							,[Absent]
							,[ActualProduction]
							,GETDATE() as [CreatedDate]
							,@username as [CreatedBy]
							,GETDATE() as [UpdatedDate]
							,@username as [UpdatedBy]
					FROM ExeTPOProductionTemp WHERE ProductionEntryCode = @splitProdEntryCode AND StatusEmp = @StatusEmp AND ProductionGroup = @ProductionGroup
				END
				ELSE
				BEGIN
					UPDATE ExeTPOProduction
					SET WorkerCount = @WorkerCount,
						Absent = @Absent,
						ActualProduction = @ActualProduction,
						UpdatedBy = @username,
						UpdatedDate = GETDATE()
					WHERE ProductionEntryCode = @splitProdEntryCode AND StatusEmp = @StatusEmp AND ProductionGroup = @ProductionGroup
				END
			END	

			IF (@WorkerCount = 0)
			BEGIN
				DELETE [dbo].[ExeTPOProduction] WHERE ProductionEntryCode = @splitProdEntryCode AND StatusEmp = @StatusEmp AND ProductionGroup = @ProductionGroup
			END
			
			
			--DELETE ExeTPOProductionTemp WHERE ProductionEntryCode = @splitProdEntryCode AND StatusEmp = @StatusEmp AND ProductionGroup = @ProductionGroup

			FETCH NEXT FROM cursor_tpoEntryTemp   
			INTO @StatusEmp, @ProductionGroup, @WorkerCount, @Absent, @ActualProduction
		END

		CLOSE cursor_tpoEntryTemp;  
		DEALLOCATE cursor_tpoEntryTemp; 

		FETCH NEXT FROM cursor_splitProductionEntryCode   
		INTO @splitProdEntryCode
	END

	CLOSE cursor_splitProductionEntryCode;  
	DEALLOCATE cursor_splitProductionEntryCode; 
END