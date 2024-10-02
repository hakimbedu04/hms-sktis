/****** Object:  UserDefinedFunction [dbo].[MaintenanceExecutionInventoryFunction]    Script Date: 11/11/2016 10:16:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MntcEquipmentStockFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[MntcEquipmentStockFunction]
GO

-- =============================================
-- [MaintenanceExecutionInventoryFunction]
-- Description: function for accessing data on [TemporaryTableEquipmentStock]
-- Author: WAHYU
-- Updated: 1.0 - 10/14/2016
-- =============================================


CREATE FUNCTION [dbo].[MntcEquipmentStockFunction]
(
	@InventoryDate  DATETIME,
	@LocationCode   VARCHAR(16),
	@UnitCode       VARCHAR(16),
	@UserAD         VARCHAR(50)
)
RETURNS TABLE 
AS
RETURN
(
SELECT 
	*
FROM	TemporaryTableEquipmentStock WITH (NOLOCK)
         
WHERE InventoryDate = @InventoryDate 
AND LocationCode = @LocationCode
AND UnitCode LIKE + '%'+@UnitCode+'%' 
AND QParam LIKE + '%'+@UserAD+'%'

GROUP BY InventoryDate,
         LocationCode,
         UnitCode,
         ItemCode,
         ItemDescription,
         RowID,
         InTransit,
         QI,
         ReadyToUse,
         BadStock,
         TotalStockMntc,
         Used,
         Repair,
         TotalStockProd,
         QParam          
) 



GO


