-- =============================================
-- Description: change label Total Biaya Produksi & Jasa Maklon for generated data
-- Ticket: http://tp.voxteneo.co.id/entity/9980
-- Author: Abdurrahman Hakim
-- Date  : 2016-09-08 16:54
-- Version: 2.9
-- =============================================
update TPOFeeCalculation set ProductionFeeType = 'Total Biaya Produksi & Jasa Manajemen' where OrderFeeType = 8