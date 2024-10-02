/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcEquipmentItemConvert]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcEquipmentItemConvert]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcInventory]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcInventory]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcEquipmentItemDisposal]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcEquipmentItemDisposal]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcEquipmentMovement]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcEquipmentMovement]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcEquipmentQualityInspection]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcEquipmentQualityInspection]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcMntcEquipmentRepair]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcMntcEquipmentRepair]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcInventoryAdjustment]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcInventoryAdjustment]
GO

/****** Object:  Trigger [trgAfterUpdateMntcMntcEquipmentRepair]    Script Date: 09/28/2016 14:45:51 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcRepairItemUsage]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcRepairItemUsage]
GO