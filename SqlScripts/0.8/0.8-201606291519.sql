/* Transaction History */
ALTER FUNCTION [dbo].[GetTransactionHistory]
(	
	@transactionCode VARCHAR(128)
)
RETURNS TABLE 
AS
RETURN 
(
	--SELECT TOP 100
	--	utl.CreatedBy name,
	--	ur.RolesName [role], 
	--	uf.MessageText [action],
	--	utl.CreatedDate [date],
	--	utl.Comments note 
	--FROM UtilTransactionLogs utl
	--INNER JOIN MstADTemp mat ON mat.UserAD = utl.CreatedBy
	--INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
	--INNER JOIN UtilResponsibility urp on urp.IDResponsibility = uur.IDResponsibility
	--INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
	--INNER JOIN UtilFlows uf ON uf.IDFlow = utl.IDFlow
	--WHERE utl.TransactionCode like '%'+@transactionCode+'%'
	--ORDER BY utl.CreatedDate desc
	SELECT TOP 100 MAX(mat.Name) AS name,
       MAX(ur.RolesName) AS role,
       (MAX(ufn.FunctionName)) AS action,
       MAX(utl.CreatedDate) AS date,
       MAX(utl.Comments) AS note
	FROM dbo.UtilTransactionLogs AS utl
	INNER JOIN dbo.UtilFlows AS uf ON uf.IDFlow = utl.IDFlow
	INNER JOIN dbo.MstADTemp AS mat ON mat.UserAD = utl.CreatedBy
	INNER JOIN dbo.UtilFlowFunctionView AS ufn ON ufn.IDFlow = uf.IDFlow
	AND ufn.UserAD = mat.UserAD
	INNER JOIN UtilUsersResponsibility uur ON uur.UserAD = mat.UserAD
	INNER JOIN UtilResponsibility urp ON urp.IDResponsibility = uur.IDResponsibility
	INNER JOIN UtilRoles ur ON ur.IDRole = urp.IDRole
	WHERE utl.TransactionCode LIKE '%'+@transactionCode+'%'
	GROUP BY ufn.UserAD,ufn.FunctionName
	ORDER BY max(utl.TransactionDate) DESC
)

GO


