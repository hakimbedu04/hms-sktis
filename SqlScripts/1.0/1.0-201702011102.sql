/****** Object:  StoredProcedure [dbo].[BaseExeReportByProcessGenerator]    Script Date: 2/1/2017 7:08:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Opun Buds
-- Create date: 14-04-2016
-- Description:	Store Procedure For insert or update to ExeReportByProcess
-- Edited by: Indra Permana
-- Description: Add brandcode, process, kpsyear, kpsweek, productiondate for parameter
-- Edited by: Ardi Siskayana
-- Description: condition beginningStock from endingstock
-- Edited by: Dwi Yudha
-- Description: extended SP condition
-- =============================================

-- =============================================
-- Description: edit rounding on SUM(TotalActualValue)
-- Author: Azka
-- Update: 11/05/2016
-- =============================================

-- =============================================
-- Description: Change update to delete + insert
-- Author: Robby
-- Update: 27/07/2016
-- =============================================

-- =============================================
-- Description: Adding WRAPPING condition, add unitCode to parameter
-- Author: azka
-- Update: 23/08/2016
-- =============================================

-- =============================================
-- Description: add  edit rounding on beginning stock, production, endingstock, 
-- Author: azka
-- Update: 14/10/2016
-- ticket: http://tp.voxteneo.co.id/entity/10449
-- =============================================

-- =============================================
-- Description: add unitcode on delete first and insert from #SOURCE
-- Author: azka
-- Update: 01/11/2016
-- ticket: http://tp.voxteneo.co.id/entity/10794
-- =============================================

ALTER PROCEDURE [dbo].[BaseExeReportByProcessGenerator]
	@LocationCode varchar(8),
	@BrandCode varchar(16),
	@ProcessGroup varchar(16),
	@KPSYear varchar(4),
	@KPSWeek varchar(2), 
	@CreatedBy varchar(50),
	@UpdatedBy varchar(50),
	@ProductionDate date,
	@UnitCode varchar(10)
AS
BEGIN

DECLARE @Description varchar(50);
DECLARE @UOMOrder INT;
DECLARE @ProcessOrder INT;
DECLARE @Production INT;
DECLARE @KeluarBersih INT;
DECLARE @RejectSample INT
DECLARE @BeginningStock INT;
DECLARE @EndingStock INT;
DECLARE @AdjCut INT;
DECLARE @AdjPack INT;

DECLARE @brandGroupCode varchar(20);
DECLARE @isExistExeReportByProcess INT

SET @Description =
CASE 
	WHEN @ProcessGroup = 'ROLLING' THEN 'UnCutCigarette' 
	WHEN @ProcessGroup = 'CUTTING' THEN 'CutCigarette' 
	WHEN @ProcessGroup = 'FOILROLL' THEN 'Alufoil' 
	WHEN @ProcessGroup = 'STICKWRAPPING' THEN 'WrappedCigarette' 
	WHEN @ProcessGroup = 'PACKING' THEN 'UnStampPack' 
	WHEN @ProcessGroup = 'STAMPING' THEN 'StampPack'
	WHEN @ProcessGroup = 'WRAPPING' THEN 'StampPack'
END;

SET @UOMOrder = 
CASE 
	WHEN @ProcessGroup = 'ROLLING' THEN 1 
	WHEN @ProcessGroup = 'CUTTING' THEN 2 
	WHEN @ProcessGroup = 'FOILROLL' THEN 3 
	WHEN @ProcessGroup = 'STICKWRAPPING' THEN 4 
	WHEN @ProcessGroup = 'PACKING' THEN 5 
	WHEN @ProcessGroup = 'STAMPING' THEN 7 
	WHEN @ProcessGroup = 'WRAPPING' THEN 7 
	ELSE 12			
END;

set @ProcessOrder = (select ProcessOrder from dbo.MstGenProcess
			where ProcessGroup = @ProcessGroup);

SET @Production =
(SELECT isnull(sum([Production]),0)
  FROM [SKTIS].[dbo].[ExeReportByGroups]
  where [LocationCode] = @LocationCode
      AND [UnitCode] = @UnitCode
      AND [ProcessGroup] = @ProcessGroup 
      AND [BrandCode] = @BrandCode
      AND [ProductionDate] = @ProductionDate);
      
SET @BeginningStock =
(SELECT isnull([EndingStock],0)
  FROM [SKTIS].[dbo].[ExeReportByProcess]
  where [LocationCode] = @LocationCode
      AND [UnitCode] = @UnitCode
      AND [ProcessGroup] = @ProcessGroup 
      AND [BrandCode] = @BrandCode
      AND [ProductionDate] = DATEADD(day, -1, @ProductionDate) AND UOMOrder = @UOMOrder);
 
IF EXISTS(SELECT * FROM ExeReportByGroups WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND ProductionDate = @ProductionDate AND BrandCode = @BrandCode
AND ProcessGroup = 'STICKWRAPPING')
BEGIN
SET @KeluarBersih =
(SELECT isnull(sum([Production]),0)
  FROM dbo.ExeReportByGroups x
  where [LocationCode] = @LocationCode
      AND [UnitCode] = @UnitCode
      AND [ProcessGroup] = (
			select isnull(ProcessGroup,@ProcessGroup) from dbo.MstGenProcess a
			where a.ProcessOrder = (
			select min(w.ProcessOrder) from dbo.MstGenProcess w
			where w.ProcessOrder > CASE WHEN @ProcessGroup = 'CUTTING' THEN (SELECT TOP 1 ProcessOrder FROM MstGenProcess WHERE ProcessGroup = 'STICKWRAPPING') ELSE @ProcessOrder END
			and exists (select 1 from dbo.MstGenProcessSettings
			where ProcessGroup = w.ProcessGroup
			and BrandGroupCode = x.BrandGroupCode)
			and exists (select 1 from dbo.ProcessSettingsAndLocationView
			where ProcessGroup = a.ProcessGroup
			and BrandGroupCode = x.BrandGroupCode
			and LocationCode = x.LocationCode))) 
      AND [BrandCode] = @BrandCode
      AND [ProductionDate] = @ProductionDate);

END
ELSE
BEGIN
SET @KeluarBersih =
(SELECT isnull(sum([Production]),0)
  FROM dbo.ExeReportByGroups x
  where [LocationCode] = @LocationCode
      AND [UnitCode] = @UnitCode
      AND [ProcessGroup] = (
			select isnull(ProcessGroup,@ProcessGroup) from dbo.MstGenProcess a
			where a.ProcessOrder = (
			select min(w.ProcessOrder) from dbo.MstGenProcess w
			where w.ProcessOrder > @ProcessOrder
			and exists (select 1 from dbo.MstGenProcessSettings
			where ProcessGroup = w.ProcessGroup
			and BrandGroupCode = x.BrandGroupCode)
			and exists (select 1 from dbo.ProcessSettingsAndLocationView
			where ProcessGroup = a.ProcessGroup
			and BrandGroupCode = x.BrandGroupCode
			and LocationCode = x.LocationCode))) 
      AND [BrandCode] = @BrandCode
      AND [ProductionDate] = @ProductionDate);

END


IF(@UOMOrder=7)
begin
SET @RejectSample =isnull((Select AdjustmentValue FROM ProductAdjustment
where AdjustmentType like '%Panel%'
AND [LocationCode] = @LocationCode
AND [UnitCode] = @UnitCode
AND [BrandCode] = @BrandCode
AND [ProductionDate] = @ProductionDate),0)

set @BeginningStock = 0;
set @EndingStock = 0;
end
Else
SET @RejectSample = 0;

IF @ProcessGroup = 'WRAPPING'
BEGIN
IF(@ProcessOrder = 6 OR @ProcessOrder = 5)
Set @KeluarBersih = @Production-@RejectSample;
END
ELSE
BEGIN
	IF(@ProcessOrder = 6)
Set @KeluarBersih = @Production-@RejectSample;
END


SET @AdjCut = isnull((SELECT AdjustmentValue FROM ProductAdjustment
WHERE AdjustmentType like '2%'
AND [LocationCode] = @LocationCode
AND [UnitCode] = @UnitCode
AND [BrandCode] = @BrandCode
AND [ProductionDate] = @ProductionDate),0);    

SET @AdjPack = isnull((SELECT AdjustmentValue FROM ProductAdjustment
WHERE AdjustmentType like '3%'
AND [LocationCode] = @LocationCode
AND [UnitCode] = @UnitCode
AND [BrandCode] = @BrandCode
AND [ProductionDate] = @ProductionDate),0);   

IF(@ProcessGroup = 'ROLLING')
BEGIN
	SET @KeluarBersih = @KeluarBersih + @AdjCut + @AdjPack;
	END 
IF(@ProcessGroup = 'CUTTING')
BEGIN
	SET @KeluarBersih = @KeluarBersih + @AdjPack;  
	END
IF(@ProcessGroup = 'FOILROLL')
BEGIN
	SET @KeluarBersih = @KeluarBersih + @AdjPack;
	END  
IF(@ProcessGroup = 'STICKWRAPPING')
BEGIN
	SET @KeluarBersih = @KeluarBersih + @AdjPack; 
END

SET @EndingStock = @BeginningStock + @Production - @KeluarBersih - @RejectSample;	 

IF EXISTS (SELECT 1 FROM dbo.ExeReportByProcess WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProcessGroup = @ProcessGroup AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND ProductionDate = @ProductionDate)
	BEGIN
		DELETE FROM dbo.ExeReportByProcess WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProcessGroup = @ProcessGroup and ProductionDate = @ProductionDate AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek
	END

INSERT INTO dbo.ExeReportByProcess
	(
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
	) 
	VALUES
	( 
		@LocationCode,
		@UnitCode,
		@BrandCode,
		@KPSYear,
		@KPSWeek,
		@ProductionDate,
		@ProcessGroup,
		@ProcessOrder,
		1,
		@Description,
		'Stick',
		@UOMOrder,
		@Production,
		isnull(@KeluarBersih,0),
		isnull(@RejectSample,0),
		isnull(@BeginningStock,0),
		isnull(@EndingStock,0),
		GETDATE(),
		'SYSTEM',
		GETDATE(),
		'SYSTEM'
	)

if(@ProcessGroup = 'WRAPPING')
BEGIN
SET @UOMOrder = 5;

INSERT INTO dbo.ExeReportByProcess
	(
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
	) 
	VALUES
	( 
		@LocationCode,
		@UnitCode,
		@BrandCode,
		@KPSYear,
		@KPSWeek,
		@ProductionDate,
		@ProcessGroup,
		@ProcessOrder,
		1,
		'UnStampPack',
		'Stick',
		@UOMOrder,
		@Production,
		isnull(@Production,0),
		0,
		isnull(@BeginningStock,0),
		isnull(@EndingStock,0),
		GETDATE(),
		'SYSTEM',
		GETDATE(),
		'SYSTEM')
END

if(@ProcessGroup = 'STAMPING' OR @ProcessGroup = 'PACKING' OR @ProcessGroup = 'WRAPPING')
BEGIN
	EXEC [dbo].ExtendedExeReportByProcess @LocationCode,@BrandCode,@CreatedBy,@UpdatedBy,@ProductionDate,@ProcessGroup, @UnitCode;
END
END