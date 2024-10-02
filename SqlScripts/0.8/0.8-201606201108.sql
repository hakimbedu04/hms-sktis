
-- =============================================
-- Description: delete foreign key constraint
-- Ticket: http://tp.voxteneo.co.id/entity/7160
-- Author: Azka
-- Update: 2016/06/20
-- =============================================

ALTER TABLE [dbo].[ExePlantWorkerAssignment] DROP CONSTRAINT FK_EXEPLANTWORKERASSIGNMENT_REFERENCE_128_MSTPLANTPRODUCTIONGROUP;

ALTER TABLE [dbo].[ExePlantWorkerAssignment] DROP CONSTRAINT FK_EXEPLANTWORKERASSIGNMENT_RELATIONSHIP_45_MSTPLANTPRODUCTIONGROUP;