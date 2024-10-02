IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[CREATE_BY_PROCESS_SWITCHING_BRAND_WPP]'))
	DROP PROCEDURE [dbo].[CREATE_BY_PROCESS_SWITCHING_BRAND_WPP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CREATE_BY_PROCESS_SWITCHING_BRAND_WPP]
AS
BEGIN
DECLARE @ErrorMessage   NVARCHAR(1000);
DECLARE @ErrorState	INT;
DECLARE @ErrorSeverity  INT;

BEGIN TRANSACTION tran1
BEGIN TRY

DECLARE @CurrWeek INT;
DECLARE @currYear INT;
DECLARE @currDate DATETIME;
SET @currDate = CAST(GETDATE() as date);

SELECT @CurrWeek = Week, @currYear = Year from MstGenWeek where @currDate between StartDate and EndDate

IF EXISTS
(
	select * from PlanWeeklyProductionPlanning where KPSWeek = @CurrWeek and KPSYear = @currYear and Value1 = 0 and Value2 > 0
)
BEGIN
	DECLARE @locationCode VARCHAR(4);
	DECLARE @brandCode VARCHAR(11);
	DECLARE @processGroup VARCHAR(16);
	DECLARE @unitcode VARCHAR(10);

	DECLARE MY_CURSOR CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
	SELECT LocationCode, BrandCode FROM PlanWeeklyProductionPlanning 
	where KPSWeek = @CurrWeek and KPSYear = @currYear and Value1 = 0 and Value2 > 0
	
	OPEN MY_CURSOR
	FETCH NEXT FROM MY_CURSOR INTO @locationCode, @brandCode
	WHILE @@FETCH_STATUS = 0 BEGIN 
		DECLARE @brandGroupCode VARCHAR(20);
		SELECT @brandGroupCode = BrandGroupCode FROM MstGenBrand WHERE BrandCode = @brandCode;

		select top 1 @processGroup = v.processgroup from ProcessSettingsAndLocationView v
		inner join mstgenprocess p on p.ProcessGroup = v.ProcessGroup
		where v.locationcode = @locationCode and v.BrandGroupCode = @brandGroupCode
		order by p.ProcessOrder desc

		DECLARE MY_CURSOR1 CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
		select distinct u.UnitCode from ProcessSettingsAndLocationView v
		inner join MstPlantUnit u on u.LocationCode = v.LocationCode
		where u.UnitCode <> 'MTNC' and  v.locationcode = @locationCode
		
		OPEN MY_CURSOR1
		FETCH NEXT FROM MY_CURSOR1 INTO @unitcode
		WHILE @@FETCH_STATUS = 0 BEGIN 

			IF NOT EXISTS
			(
				SELECT * FROM ExeReportByProcess 
				WHERE LocationCode = @locationCode AND UnitCode = @unitcode AND BrandCode = @brandCode AND ProductionDate = @currDate
			)
			BEGIN
				IF(@processGroup = 'STAMPING')
				BEGIN
					
						INSERT INTO [dbo].[ExeReportByProcess]
							   ([LocationCode]
							   ,[UnitCode]
							   ,[BrandCode]
							   ,[KPSYear]
							   ,[KPSWeek]
							   ,[ProductionDate]
							   ,[ProcessGroup]
							   ,[ProcessOrder]
							   ,[Shift]
							   ,[Description]
							   ,[UOM]
							   ,[UOMOrder]
							   ,[Production]
							   ,[KeluarBersih]
							   ,[RejectSample]
							   ,[BeginningStock]
							   ,[EndingStock]
							   ,[CreatedDate]
							   ,[CreatedBy]
							   ,[UpdatedDate]
							   ,[UpdatedBy])
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'ROLLING', 1, 1,  'UnCutCigarette', 'Stick', 1, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'CUTTING', 2, 1,  'CutCigarette', 'Stick', 2, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'PACKING', 5, 1,  'UnStampPack', 'Stick', 5, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'PACKING', 5, 1,  'UnStampPack', 'Pack', 6, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'StampPack', 'Stick', 7, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'StampPack', 'Pack', 8, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'DisplayCarton', 'Stick', 9, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'DisplayCarton', 'Slof', 10, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'InternalMove', 'Stick', 11, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'InternalMove', 'Box', 12, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'ExternalMove', 'Stick', 13, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'STAMPING', 6, 1, 'ExternalMove', 'Box', 14, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM'	  
					
				END
				ELSE IF(@processGroup = 'WRAPPING')
				BEGIN
					INSERT INTO [dbo].[ExeReportByProcess]
							   ([LocationCode]
							   ,[UnitCode]
							   ,[BrandCode]
							   ,[KPSYear]
							   ,[KPSWeek]
							   ,[ProductionDate]
							   ,[ProcessGroup]
							   ,[ProcessOrder]
							   ,[Shift]
							   ,[Description]
							   ,[UOM]
							   ,[UOMOrder]
							   ,[Production]
							   ,[KeluarBersih]
							   ,[RejectSample]
							   ,[BeginningStock]
							   ,[EndingStock]
							   ,[CreatedDate]
							   ,[CreatedBy]
							    ,[UpdatedDate]
								,[UpdatedBy])
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'ROLLING', 1, 1,  'UnCutCigarette', 'Stick', 1, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'CUTTING', 2, 1,  'CutCigarette', 'Stick', 2, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1,  'StampPack', 'Stick', 7, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1,  'StampPack', 'Pack', 8, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'DisplayCarton', 'Stick', 9, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'DisplayCarton', 'Pack', 10, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'InternalMove', 'Stick', 11, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'InternalMove', 'Slof', 12, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'ExternalMove', 'Stick', 13, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM' UNION ALL
						SELECT @LocationCode, @unitcode, @brandCode, @currYear, @CurrWeek, @currDate,'WRAPPING', 5, 1, 'ExternalMove', 'Box', 14, 0, 0, 0, 0, 0, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM'  
				END
			END
		FETCH NEXT FROM MY_CURSOR1 INTO @unitcode
		END
		CLOSE MY_CURSOR1
		DEALLOCATE MY_CURSOR1

	FETCH NEXT FROM MY_CURSOR INTO @locationCode, @brandCode
	END
	CLOSE MY_CURSOR
	DEALLOCATE MY_CURSOR
END
	COMMIT TRANSACTION tran1
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION tran1
		SET @ErrorMessage  = ERROR_MESSAGE();
		SET	@ErrorState    = ERROR_STATE();
		SET @ErrorSeverity = ERROR_SEVERITY();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END