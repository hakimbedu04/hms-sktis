/****** Object:  UserDefinedFunction [dbo].[GetAbsentAssignmentOnEntry]    Script Date: 8/3/2016 7:57:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: function to calculate absent when there are assignment
-- Ticket: http://tp.voxteneo.co.id/entity/8473
-- Author: AZKA
-- Updated: 2016/08/03
-- =============================================

CREATE FUNCTION [dbo].[GetAbsentAssignmentOnEntry](
                @ProductionEntryCode VARCHAR(50),
                @AbsentCode          VARCHAR(50),
				@StatusEmp			 VARCHAR(64))
RETURNS INT
AS
     BEGIN
     DECLARE @column VARCHAR(50),
             @result INT = 0;
     IF @AbsentCode IS NOT NULL
		BEGIN
			IF @AbsentCode = 'SLP'
			BEGIN
				SELECT @result = COUNT(*) FROM dbo.ExePlantProductionEntry eppe 
				WHERE eppe.AbsentCodeEblek IN ('SL4', 'SLP') AND eppe.ProductionEntryCode = @ProductionEntryCode AND StatusEmp = @StatusEmp;
			END
			ELSE
				BEGIN
				  SELECT @result = COUNT(*) FROM dbo.ExePlantProductionEntry eppe WHERE eppe.AbsentCodeEblek = @AbsentCode AND eppe.ProductionEntryCode = @ProductionEntryCode AND StatusEmp = @StatusEmp;
				END	
		END
     ELSE	    
	    BEGIN
		  SELECT @result = COUNT(*) FROM dbo.ExePlantProductionEntry eppe 
			 WHERE eppe.AbsentCodeEblek IS NOT NULL AND eppe.ProductionEntryCode = @ProductionEntryCode AND StatusEmp = @StatusEmp
			 --AND eppe.AbsentCodeEblek IN ('LL', 'SKR');
			 AND eppe.AbsentCodeEblek NOT IN ('A', 'I', 'C', 'CH', 'CT', 'SLS', 'SLP', 'MO', 'S', 'SL4','PG','SB','LO','LP','LL', 'T');
	    END
     RETURN @result;
     END;