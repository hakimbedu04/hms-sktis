IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[CREATE_BY_PROCESS_SHEDULER_PER_DAY]'))
	DROP PROCEDURE [dbo].[CREATE_BY_PROCESS_SHEDULER_PER_DAY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CREATE_BY_PROCESS_SHEDULER_PER_DAY]
AS
BEGIN
DECLARE @ErrorMessage   NVARCHAR(1000);
DECLARE @ErrorState	INT;
DECLARE @ErrorSeverity  INT;

BEGIN TRANSACTION tran2
BEGIN TRY

	DECLARE	@LocationCode		VARCHAR(50);
	DECLARE	@UnitCode         VARCHAR(50);
	DECLARE	@BrandCode        VARCHAR(50);
	DECLARE @maxProductionDate DATETIME;
	DECLARE @currDate DATETIME
	
	SET @currDate = CONVERT(date, GETDATE());
	
	DECLARE @cursor1 CURSOR;
	BEGIN
		SET @cursor1 = CURSOR FOR
		select LocationCode, UnitCode, BrandCode, max(ProductionDate) as LastProductionDate
		from exereportbyprocess
		where (UOMOrder = 1 OR UOMOrder = 2 OR UOMOrder = 5 OR UOMOrder = 6 OR UOMOrder = 7 OR UOMOrder = 8 OR 
		UOMOrder = 9 OR UOMOrder = 10 OR UOMOrder = 11 OR UOMOrder = 12 OR UOMOrder = 13 OR UOMOrder = 14)
		AND EndingStock > 0
		group by 
		LocationCode, UnitCode, BrandCode
	
		OPEN @cursor1
		FETCH NEXT FROM @cursor1
		INTO @LocationCode, @UnitCode, @BrandCode, @maxProductionDate
	
		WHILE @@FETCH_STATUS = 0
		BEGIN
			---START LOOP----
	
			IF EXISTS
			(
				SELECT * FROM ExeReportByProcess 
				WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProductionDate = @currDate
			)
			BEGIN
				DELETE ExeReportByProcess
				WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProductionDate = @currDate
			END;
	
			DECLARE @week INT;
			DECLARE @year int;
	
			SELECT TOP 1 @week = [Week], @year = [Year] FROM MstGenWeek where @currDate between StartDate and EndDate;
	
			-- INSERT by process get value beginning stock from previous max date
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
			SELECT [LocationCode]
				   ,[UnitCode]
				   ,[BrandCode]
				   ,@year as [KPSYear]
				   ,@week as [KPSWeek]
				   ,@currDate as [ProductionDate]
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
				   ,[BeginningStock] as [EndingStock]
				   ,GETDATE() as [CreatedDate]
				   ,'SYSTEM' as [CreatedBy]
				   ,GETDATE() as [UpdatedDate]
				   ,'SYSTEM' as [UpdatedBy]
			FROM ExeReportByProcess
			WHERE 
			LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProductionDate = @maxProductionDate
			AND @maxProductionDate < @currDate;
	
			---END LOOP------
			FETCH NEXT FROM @cursor1
			INTO @LocationCode, @UnitCode, @BrandCode, @maxProductionDate
		END
		
		CLOSE @cursor1 ;
	    DEALLOCATE @cursor1;
	END

COMMIT TRANSACTION tran2
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION tran2
		SET @ErrorMessage  = ERROR_MESSAGE();
		SET	@ErrorState    = ERROR_STATE();
		SET @ErrorSeverity = ERROR_SEVERITY();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END