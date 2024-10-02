IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetTransactionFlow]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetTransactionFlow]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetTransactionFlow]
(	
	@functionName VARCHAR(64)
)
RETURNS @TempTable TABLE (IDFlow INT NOT NULL, RolesName VARCHAR(64) NULL, MessageText VARCHAR(64) NULL)
AS
BEGIN
	DECLARE @tempfunctionname VARCHAR(256);

	SET @tempfunctionname = @functionName;

	IF(@tempfunctionname = 'ProductionCardApprovalDetail')
	BEGIN
		INSERT INTO @TempTable
		SELECT uflow.IDFlow,urole.RolesName,uflow.MessageText FROM UtilFunctions ufunc
		INNER JOIN UtilFlows uflow ON uflow.FormSource = ufunc.IDFunction
		INNER JOIN UtilRoles urole ON urole.IDRole = uflow.DestinationRole
		WHERE IDFlow = 24
		and ufunc.FunctionName = @functionName
		UNION ALL
		SELECT uflow.IDFlow,urole.RolesName,uflow.MessageText FROM UtilFunctions ufunc
		INNER JOIN UtilFlows uflow ON uflow.FormSource = ufunc.IDFunction
		INNER JOIN UtilRoles urole ON urole.IDRole = uflow.DestinationRole
		WHERE IDFlow = 70
		and ufunc.FunctionName = @functionName
		UNION ALL
		SELECT uflow.IDFlow,urole.RolesName,uflow.MessageText FROM UtilFunctions ufunc
		INNER JOIN UtilFlows uflow ON uflow.FormSource = ufunc.IDFunction
		INNER JOIN UtilRoles urole ON urole.IDRole = uflow.DestinationRole
		WHERE IDFlow = 56
		and ufunc.FunctionName = @functionName
		UNION ALL
		SELECT uflow.IDFlow,urole.RolesName,uflow.MessageText FROM UtilFunctions ufunc
		INNER JOIN UtilFlows uflow ON uflow.FormSource = ufunc.IDFunction
		INNER JOIN UtilRoles urole ON urole.IDRole = uflow.DestinationRole
		WHERE IDFlow = 25
		and ufunc.FunctionName = @functionName
	END
	ELSE
	BEGIN
		INSERT INTO @TempTable
		SELECT uflow.IDFlow,urole.RolesName,uflow.MessageText FROM UtilFunctions ufunc
		INNER JOIN UtilFlows uflow ON uflow.FormSource = ufunc.IDFunction
		INNER JOIN UtilRoles urole ON urole.IDRole = uflow.DestinationRole
		WHERE ufunc.FunctionName = @functionName;
	END
RETURN;
END