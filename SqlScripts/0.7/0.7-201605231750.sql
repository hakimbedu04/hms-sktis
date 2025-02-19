/*
 * Author: Robby Prima Suherlan
 * Ticket: http://tp.voxteneo.co.id/entity/6627
 */

ALTER TABLE dbo.MntcInventoryAdjustment
 DROP CONSTRAINT FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM
GO
ALTER TABLE dbo.MstMntcItem SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.MntcInventoryAdjustment
 DROP CONSTRAINT FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT
GO
ALTER TABLE dbo.MstPlantUnit SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.Tmp_MntcInventoryAdjustment
 (
 AdjustmentDate datetime NOT NULL,
 LocationCode varchar(8) NOT NULL,
 UnitCode varchar(4) NOT NULL,
 UnitCodeDestination varchar(4) NOT NULL,
 ItemCode varchar(20) NOT NULL,
 ItemStatusFrom varchar(32) NOT NULL,
 ItemStatusTo varchar(32) NOT NULL,
 AdjustmentValue int NOT NULL,
 AdjustmentType varchar(32) NULL,
 Remark varchar(256) NULL,
 CreatedDate datetime NULL,
 CreatedBy varchar(64) NULL,
 UpdatedDate datetime NULL,
 UpdatedBy varchar(64) NULL
 )  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_MntcInventoryAdjustment SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.MntcInventoryAdjustment)
  EXEC('INSERT INTO dbo.Tmp_MntcInventoryAdjustment (AdjustmentDate, LocationCode, UnitCode, UnitCodeDestination, ItemCode, ItemStatusFrom, ItemStatusTo, AdjustmentValue, AdjustmentType, Remark, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
  SELECT AdjustmentDate, LocationCode, UnitCode, UnitCodeDestination, ItemCode, ItemStatusFrom, ItemStatusTo, AdjustmentValue, AdjustmentType, Remark, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy FROM dbo.MntcInventoryAdjustment WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.MntcInventoryAdjustment
GO
EXECUTE sp_rename N'dbo.Tmp_MntcInventoryAdjustment', N'MntcInventoryAdjustment', 'OBJECT' 
GO
ALTER TABLE dbo.MntcInventoryAdjustment ADD CONSTRAINT
 PK_MntcInventoryAdjustment PRIMARY KEY CLUSTERED 
 (
 AdjustmentDate,
 LocationCode,
 UnitCode,
 UnitCodeDestination,
 ItemCode,
 ItemStatusFrom,
 ItemStatusTo
 ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MntcInventoryAdjustment ADD CONSTRAINT
 FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT FOREIGN KEY
 (
 UnitCode,
 LocationCode
 ) REFERENCES dbo.MstPlantUnit
 (
 UnitCode,
 LocationCode
 ) ON UPDATE  NO ACTION 
  ON DELETE  NO ACTION 
 
GO
ALTER TABLE dbo.MntcInventoryAdjustment ADD CONSTRAINT
 FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM FOREIGN KEY
 (
 ItemCode
 ) REFERENCES dbo.MstMntcItem
 (
 ItemCode
 ) ON UPDATE  NO ACTION 
  ON DELETE  NO ACTION 
 
GO