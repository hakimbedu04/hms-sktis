-- =============================================
-- Author:	WAHYU
-- Create date: 28-09-2016
-- Description:	Modify to make sure batch run script is run well
-- =============================================
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDefaultExeReportByProcess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDefaultExeReportByProcess]
GO


-- =============================================
-- Author:	HAKIM
-- Create date: 27-09-2016
-- Description:	Insert 0 to ExeReport By Process when no process before
-- =============================================
CREATE PROCEDURE [dbo].[InsertDefaultExeReportByProcess]
	@locationCode varchar(10),
	@brandCode varchar(20),
	@unitCode varchar(10),
	@kpsYear int,
	@kpsWeek int,
	@productionDate DATE,
	@createdBy varchar(20),
	@updatedBy varchar(20)
AS
BEGIN
	DECLARE @tempTable TABLE(
		ProcessGroup varchar(20),
		c Int Identity(1,1)
	);
	DECLARE @i INT;
	DECLARE @description varchar(50);
	DECLARE @uom varchar(15);
	DECLARE @ProcessGroupExist varchar(20);
	DECLARE @brandGroupCode varchar(20);
	DECLARE @processOrder INT;

	SET @brandGroupCode = (SELECT TOP 1 BrandGroupCode FROM dbo.MstGenBrand WHERE BrandCode = @brandCode);

	INSERT INTO @tempTable	
	SELECT DISTINCT ProcessGroup FROM ProcessSettingsAndLocationView WHERE BrandGroupCode = @brandGroupCode and LocationCode = @locationCode;
	
	SET @uom = 'Stick'
	SET @i = (SELECT COUNT(*) FROM @tempTable);
	DECLARE @uomOrder INT;
	WHILE ((SELECT COUNT(*) FROM @tempTable) > 0) 
	BEGIN
		SELECT @ProcessGroupExist = ProcessGroup from @tempTable where c = @i
		SET @i = @i - 1;
		IF(@ProcessGroupExist = 'ROLLING')
			SET @description = 'UnCutCigarette' 
		IF(@ProcessGroupExist = 'CUTTING')
			SET @description = 'CutCigarette'
		IF(@ProcessGroupExist = 'FOILROLL')
			SET @description = 'Alufoil'
		IF(@ProcessGroupExist = 'STICKWRAPPING')
			SET @description = 'WrappedCigarette'
		IF(@ProcessGroupExist = 'PACKING')
			SET @description = 'UnStampPack'
		IF(@ProcessGroupExist = 'STAMPING')
			SET @description = 'StampPack'
		IF(@ProcessGroupExist = 'WRAPPING')
			SET @description = 'StampPack'

		IF (@ProcessGroupEXIST = 'STAMPING' OR @ProcessGroupEXIST = 'WRAPPING')
		/*
		*STAMPING
		*/
		BEGIN
			SET @uomOrder = 7;
			WHILE @uomOrder > 0
			BEGIN
				if((@uomOrder = 9) OR (@uomOrder = 10))
					SET @description = 'DisplayCarton'
				if((@uomOrder = 11) OR (@uomOrder = 12))
					SET @description = 'InternalMove'
				if((@uomOrder = 13) OR (@uomOrder = 14))
					SET @description = 'ExternalMove'

				if(@uomOrder % 2) = 1
				begin
					set @uom = 'Stick'
				end
				else
				begin
					if(@uomOrder = 8)
						set @uom = 'Pack'
					if(@uomOrder = 10)
						set @uom = 'Slof'
					if(@uomOrder = 12) OR (@uomOrder = 14)
						set @uom = 'Box'
				end

				INSERT INTO dbo.ExeReportByProcess(
					LocationCode,
					UnitCode,
					BrandCode,
					KPSYear,
					KPSWeek,
					ProductionDate,
					ProcessGroup,
					ProcessOrder,
					Shift,
					Description,
					UOM,
					UOMOrder,
					Production,
					KeluarBersih,
					RejectSample,
					BeginningStock,
					EndingStock,
					CreatedDate,
					CreatedBy,
					UpdatedDate,
					UpdatedBy
				)VALUES(
				@locationCode,@unitCode,@brandCode,@kpsYear,@kpsWeek,@productionDate,@ProcessGroupExist,6,1,@description,@uom,@uomOrder,0,0,0,0,0,
				GETDATE(),@createdBy,GETDATE(),@updatedBy
				)
			
				IF(@uomOrder = 14)
					BREAK;

				SET @uomOrder = @uomOrder + 1;
			END
		END

		IF (@ProcessGroupEXIST = 'PACKING')
		/*
		*PACKING
		*/
		BEGIN
			SET @uomOrder = 5;
			WHILE @uomOrder > 0
			BEGIN
				SET @description = 'UnStampPack'

				if(@uomOrder % 2) = 1
				begin
					set @uom = 'Stick'
				end
				else
				begin
					set @uom = 'Pack'
				end

				INSERT INTO dbo.ExeReportByProcess(
					LocationCode,
					UnitCode,
					BrandCode,
					KPSYear,
					KPSWeek,
					ProductionDate,
					ProcessGroup,
					ProcessOrder,
					Shift,
					Description,
					UOM,
					UOMOrder,
					Production,
					KeluarBersih,
					RejectSample,
					BeginningStock,
					EndingStock,
					CreatedDate,
					CreatedBy,
					UpdatedDate,
					UpdatedBy
				)VALUES(
				@locationCode,@unitCode,@brandCode,@kpsYear,@kpsWeek,@productionDate,@ProcessGroupExist,5,1,@description,@uom,@uomOrder,0,0,0,0,0,
				GETDATE(),@createdBy,GETDATE(),@updatedBy
				)
			
				IF(@uomOrder = 6)
					BREAK;

				SET @uomOrder = @uomOrder + 1;
			END
		END

		IF (@ProcessGroupEXIST = 'CUTTING')
		/*
		*CUTTING
		*/
		BEGIN
			SET @uomOrder = 2;
			WHILE @uomOrder > 0
			BEGIN
				SET @description = 'CutCigarette'

				set @uom = 'Stick'

				INSERT INTO dbo.ExeReportByProcess(
					LocationCode,
					UnitCode,
					BrandCode,
					KPSYear,
					KPSWeek,
					ProductionDate,
					ProcessGroup,
					ProcessOrder,
					Shift,
					Description,
					UOM,
					UOMOrder,
					Production,
					KeluarBersih,
					RejectSample,
					BeginningStock,
					EndingStock,
					CreatedDate,
					CreatedBy,
					UpdatedDate,
					UpdatedBy
				)VALUES(
				@locationCode,@unitCode,@brandCode,@kpsYear,@kpsWeek,@productionDate,@ProcessGroupExist,2,1,@description,@uom,@uomOrder,0,0,0,0,0
				,GETDATE(),@createdBy,GETDATE(),@updatedBy
				)
			
				IF(@uomOrder = 2)
					BREAK;

				SET @uomOrder = @uomOrder + 1;
			END
		END

		IF (@ProcessGroupEXIST = 'ROLLING')
		/*
		*PACKING
		*/
		BEGIN
			SET @uomOrder = 1;
			WHILE @uomOrder > 0
			BEGIN
				SET @description = 'UnCutCigarette'

				if(@uomOrder % 2) = 1
				begin
					set @uom = 'Stick'
				end
				else
				begin
					set @uom = 'Pack'
				end

				INSERT INTO dbo.ExeReportByProcess(
					LocationCode,
					UnitCode,
					BrandCode,
					KPSYear,
					KPSWeek,
					ProductionDate,
					ProcessGroup,
					ProcessOrder,
					Shift,
					Description,
					UOM,
					UOMOrder,
					Production,
					KeluarBersih,
					RejectSample,
					BeginningStock,
					EndingStock,
					CreatedDate,
					CreatedBy,
					UpdatedDate,
					UpdatedBy
				)VALUES(
				@locationCode,@unitCode,@brandCode,@kpsYear,@kpsWeek,@productionDate,@ProcessGroupExist,1,1,@description,@uom,@uomOrder,0,0,0,0,0
				,GETDATE(),@createdBy,GETDATE(),@updatedBy
				)
			
				IF(@uomOrder = 1)
					BREAK;

				SET @uomOrder = @uomOrder + 1;
			END
		END

		IF (@i = 0)
			BREAK; 
	END
END