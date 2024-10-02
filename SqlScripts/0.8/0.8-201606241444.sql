-- =============================================
-- Description: INSERT IDFLOW 58 CANCEL SUBMIT
-- Updated: 2016/06/24
-- =============================================

with ut1 as 
(
 select * from UtilTransactionLogs ut1 where IDFlow in (34, 35)
),
ut2 as
(
 select * from UtilTransactionLogs ut2 where IDFlow in (38,39,40)
)
insert into UtilTransactionLogs(TransactionCode, TransactionDate, IDFlow, Comments, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate)
select distinct 
	ut1.TransactionCode, DATEADD(ss, 5, ut1.TransactionDate) as TransactionDate, 
	58 as IDFlow, 
	ut1.Comments, 
	ut1.CreatedDate, 
	ut1.CreatedBy, 
	ut1.UpdatedBy, 
	ut1.UpdatedDate 
from ut1 inner join ut2 on ut2.TransactionCode = ut1.TransactionCode where ut1.CreatedDate > ut2.CreatedDate