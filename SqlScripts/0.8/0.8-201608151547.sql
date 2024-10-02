/****** Object:  UserDefinedFunction [dbo].[GetAbsentMultiskillOut]    Script Date: 8/15/2016 12:29:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: get absent plant entry multiskillout
-- Ticket: http://tp.voxteneo.co.id/entity/3029
-- Author: Azka
-- Update: 14/03/2016
-- =============================================

ALTER FUNCTION [dbo].[GetAbsentMultiskillOut](
                @ProductionEntryCode	VARCHAR(50),
				@SourceLocationCode		VARCHAR(10),
				@SourceShift			INT,
				@SourceUnitCode			VARCHAR(10),
				@SourceGroupCode		VARCHAR(10),
				@SourceBrandCode		VARCHAR(20),
				@DestinationProcess		VARCHAR(20),
                @AbsentCode				VARCHAR(50)
				)
RETURNS INT
AS
BEGIN
     DECLARE @result INT = 0;
	 DECLARE @startDate DATETIME;
	 
	 SELECT DISTINCT @startDate = StartDateAbsent from ExePlantProductionEntry where ProductionEntryCode = @ProductionEntryCode and AbsentCodeEblek = @AbsentCode;

	 IF @DestinationProcess = 'TPO'
		 BEGIN
	 		 SELECT  @result = Count(*) FROM ExePlantWorkerAssignment WHERE EmployeeNumber IN 
			 (
				SELECT EmployeeNumber FROM ExePlantProductionEntry WHERE ProductionEntryCode = @ProductionEntryCode and AbsentCodeEblek = @AbsentCode
			 ) 
			 AND SourceLocationCode = @SourceLocationCode 
			 AND SourceGroupCode = @SourceGroupCode 
			 AND SourceUnitCode = @SourceUnitCode
			 AND SourceBrandCode = @SourceBrandCode 
			 AND SourceShift = @SourceShift 
			 AND @startDate >= StartDate AND @startDate <= EndDate
			 AND DestinationLocationCode IN (select LocationCode from GetLastChildLocation('TPO'))
		 END

	 ELSE
		 BEGIN
			 SELECT  @result = Count(*) FROM ExePlantWorkerAssignment WHERE EmployeeNumber IN 
			 (
				SELECT EmployeeNumber FROM ExePlantProductionEntry WHERE ProductionEntryCode = @ProductionEntryCode and AbsentCodeEblek = @AbsentCode
			 ) 
			 AND SourceLocationCode = @SourceLocationCode 
			 AND SourceGroupCode = @SourceGroupCode 
			 AND SourceUnitCode = @SourceUnitCode
			 AND SourceBrandCode = @SourceBrandCode 
			 AND SourceShift = @SourceShift 
			 AND @startDate >= StartDate AND @startDate <= EndDate
			 AND DestinationProcessGroup = @DestinationProcess
		 END

     RETURN @result;
END;