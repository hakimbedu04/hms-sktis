-- Description: insert wip detail conflic
-- Ticket: http://tp.voxteneo.co.id/entity/8661
-- Author: Sidiq

ALTER TABLE dbo.PlanPlantWIPDetail drop CONSTRAINT PK_PlanPlantWIPDetail;

ALTER TABLE dbo.PlanPlantWIPDetail ALTER COLUMN UnitCode VARCHAR(4) NOT NULL;

ALTER TABLE dbo.PlanPlantWIPDetail ADD CONSTRAINT PK_PlanPlantWIPDetail 
PRIMARY KEY CLUSTERED (
	KPSYear,
	KPSWeek,
	ProcessGroup,
	UnitCode,
	LocationCode,
	BrandCode
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];