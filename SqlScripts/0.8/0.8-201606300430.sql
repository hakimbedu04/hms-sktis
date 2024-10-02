;WITH CTE
AS (
select MAX(a.ProductionEntryCode) as TransCode,MAX(b.IDFlow) as IDFlow
from dbo.ExeTPOProductionEntryVerification as a
join dbo.UtilTransactionLogs as b on b.TransactionCode = a.ProductionEntryCode and b.IDFlow IN (36,37,38,39,40)
where a.VerifySystem = 0 or a.VerifyManual = 0 
group by a.ProductionEntryCode
)

UPDATE 
dbo.ExeTPOProductionEntryVerification
SET VerifySystem = 1, VerifyManual = 1
FROM dbo.ExeTPOProductionEntryVerification a
INNER JOIN CTE b on a.ProductionEntryCode = b.TransCode