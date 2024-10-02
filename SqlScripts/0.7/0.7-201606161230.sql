-- =============================================
-- Description: Change Data Type
-- Ticket: http://tp.voxteneo.co.id/entity/7097, http://tp.voxteneo.co.id/entity/7096
-- Author: AZKA
-- Updated: 1.0 - 2016/06/16
-- =============================================

ALTER TABLE [dbo].[ExePlantProductionEntry]
   ALTER COLUMN ProdCapacity DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity3 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity5 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity6 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity7 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity8 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity9 DECIMAL(18,3)

ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]
   ALTER COLUMN HoursCapacity10 DECIMAL(18,3)

ALTER TABLE [dbo].[ExePlantProductionEntryVerification]
   ALTER COLUMN TotalCapacityValue DECIMAL(18,3)