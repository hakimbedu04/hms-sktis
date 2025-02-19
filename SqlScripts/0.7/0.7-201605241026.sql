/*
 * Tuesday, May 24, 201610:05:18 AM
 * Author: Robby Prima Suherlan
 */

ALTER TABLE dbo.MntcInventoryAdjustment
	DROP CONSTRAINT PK_MntcInventoryAdjustment
GO
ALTER TABLE dbo.MntcInventoryAdjustment ADD CONSTRAINT
	PK_MntcInventoryAdjustment_1 PRIMARY KEY CLUSTERED 
	(
	AdjustmentDate,
	LocationCode,
	UnitCode,
	UnitCodeDestination,
	ItemCode
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MntcInventoryAdjustment SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.MntcInventoryAdjustment
	ALTER COLUMN ItemStatusFrom VARCHAR(32) null
GO
ALTER TABLE dbo.MntcInventoryAdjustment
	ALTER COLUMN ItemStatusTo VARCHAR(32) null
Go
