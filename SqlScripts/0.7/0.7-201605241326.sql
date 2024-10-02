-- Description: [Maintenance] Equipment Repair Plant
-- Ticket: http://tp.voxteneo.co.id/entity/6625
-- Author: Yudha

ALTER TABLE dbo.MntcRepairItemUsage
	DROP CONSTRAINT FK_MNTCREPAIRITEMUSAGE_RELATIONSHIP_131_MNTCEQUIPMENTREPAIR
GO