/****** Object:  UserDefinedFunction [dbo].[GetAbsent]    Script Date: 5/26/2016 1:41:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: get absent modify
-- Ticket: http://tp.voxteneo.co.id/entity/3029
-- Author: Azka
-- Update: 14/03/2016
-- =============================================
-- =============================================
-- Description: add PG, SB,LO,LP,LL
-- Ticket: http://tp.voxteneo.co.id/entity/3029
-- Author: Ardi
-- Update: 21/04/2016
-- =============================================

-- =============================================
-- Description: add T (Terminasi)
-- Ticket: http://tp.voxteneo.co.id/entity/6725
-- Author: Azka
-- Update: 5/26/2016
-- =============================================

ALTER FUNCTION [dbo].[GetAbsent](
                @ProductionEntryCode VARCHAR(50),
                @AbsentCode          VARCHAR(50))
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
				WHERE eppe.AbsentCodeEblek IN ('SL4', 'SLP') AND eppe.ProductionEntryCode = @ProductionEntryCode;
			END
			ELSE
				BEGIN
				  SELECT @result = COUNT(*) FROM dbo.ExePlantProductionEntry eppe WHERE eppe.AbsentCodeEblek = @AbsentCode AND eppe.ProductionEntryCode = @ProductionEntryCode;
				END	
		END
     ELSE	    
	    BEGIN
		  SELECT @result = COUNT(*) FROM dbo.ExePlantProductionEntry eppe 
			 WHERE eppe.AbsentCodeEblek IS NOT NULL AND eppe.ProductionEntryCode = @ProductionEntryCode
			 --AND eppe.AbsentCodeEblek IN ('LL', 'SKR');
			 AND eppe.AbsentCodeEblek NOT IN ('A', 'I', 'C', 'CH', 'CT', 'SLS', 'SLP', 'MO', 'S', 'SL4','PG','SB','LO','LP','LL', 'T');
	    END

     

     RETURN @result;
     END;