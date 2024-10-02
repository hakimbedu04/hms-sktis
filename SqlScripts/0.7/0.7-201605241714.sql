/* CREATE Maintenance Repair Item Usage View */
ALTER VIEW [dbo].[MntcRepairItemUsageView]
AS
SELECT        
	mriu.TransactionDate, 
	mriu.LocationCode, 
	mriu.ItemCodeSource, 
	mriu.ItemCodeDestination, 
	mmi.ItemDescription, 
	mmi.UOM, 
	mmc.QtyConvert, 
	SUM(mriu.QtyUsage) AS Quantity
FROM            
	dbo.MntcRepairItemUsage AS mriu INNER JOIN
    dbo.MstMntcItem AS mmi ON mmi.ItemCode = mriu.ItemCodeDestination INNER JOIN
    dbo.MstMntcConvert AS mmc ON mmc.ItemCodeDestination = mriu.ItemCodeDestination AND mmc.ItemCodeSource = mriu.ItemCodeSource
WHERE mriu.UnitCode = 'MTNC'
GROUP BY mriu.UnitCode, mriu.TransactionDate, mriu.LocationCode, mriu.ItemCodeSource, mriu.ItemCodeDestination, mmi.ItemDescription, mmi.UOM, mmc.QtyConvert

GO


