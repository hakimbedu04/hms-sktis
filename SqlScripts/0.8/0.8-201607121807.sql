
ALTER TABLE [dbo].[ExePlantWorkerAbsenteeism] DROP CONSTRAINT [FK_ExePlantWorkerAbsenteeism_MstPlantEmpJobsDataAcv]
ALTER TABLE [dbo].[ExePlantWorkerAssignment] DROP CONSTRAINT [FK_ExePlantWorkerAssignment_MstPlantEmpJobsDataAcv]
ALTER TABLE [dbo].[MstPlantProductionGroup] DROP CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_97_MSTPLANTEMPJOBSDATAACV]
ALTER TABLE [dbo].[MstPlantProductionGroup] DROP CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_98_MSTPLANTEMPJOBSDATAACV]
ALTER TABLE [dbo].[MstPlantProductionGroup] DROP CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_99_MSTPLANTEMPJOBSDATAACV]
ALTER TABLE [dbo].[PlanPlantAllocation] DROP CONSTRAINT [FK_PLANPLANTALLOCATION_REFERENCE_141_MSTPLANTEMPJOBSDATAACV]
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours] DROP CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_112_MSTPLANTEMPJOBSDATAACV]