-- =============================================
-- Description: Delete MstGenLocStatus Where status is "Beginner" or "Intermediate" and LocationCode is not "ID26"
-- Ticket: http://tp.voxteneo.co.id/entity/7188
-- Author: Enrile
-- Created: 2016/06/21
-- =============================================

DELETE FROM MstGenLocStatus WHERE StatusEmp in ('Beginner', 'Intermediate') and LocationCode != 'ID26'