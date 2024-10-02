/****** Object:  UserDefinedFunction [dbo].[RoleButtonChecker]    Script Date: 23-Aug-16 3:04:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Robby Prima
-- Create date: 2016-08-11
-- Description:	Check if user can do button action
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-23
-- Description:	Check if user can do button action / part WPC(wages)
-- =============================================
ALTER FUNCTION [dbo].[RoleButtonChecker]
(
--	@TPOFeeCode VARCHAR(250),
	@TransactionCode VARCHAR(250),
	@IDRole INT,
	@Page VARCHAR(250),
	@Button VARCHAR(250)
)
RETURNS @ButtonState TABLE (IsEnabled INT)
AS
BEGIN
	DECLARE @DestinationRole INT
	DECLARE @Ret BIT
	DECLARE @RC INT
	DECLARE @RoleCode VARCHAR(10)

	SET @Ret = 0;
	
	if(@TransactionCode like '%FEE%')
	
		SET @DestinationRole = (SELECT DestinationRole FROM TPOFeeApprovalView WHERE TPOFeeCode = @TransactionCode)
		SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole

		if (@DestinationRole = @IDRole)
		BEGIN
			SELECT @RC = COUNT(*) FROM UtilFlowFunctionView WHERE SourceRolesCodes = @RoleCode AND SourceFunctionForm = @Page AND FunctionName = @Button

			if (@RC > 0)
			BEGIN
				SET @Ret = 1
			END
		END

	ELSE IF(@TransactionCode like '%WPC%')
		
		SET @DestinationRole = 
		(
			SELECT COUNT(b.DestinationRolesCodes) from 
			(
				SELECT TOP 1 IDFlow FROM UtilTransactionLogs 
				WHERE TransactionCode like @TransactionCode
				order by UpdatedDate desc
			) a
			join UtilFlowFunctionView b
			on b.IDFlow = a.IDFlow
			join UtilRoles c
			on c.IDRole = @IDROle
			where DestinationRolesCodes = c.RolesCode
		)
		
		SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole
		
		if (@DestinationRole > 0)
		BEGIN
			SELECT @RC = COUNT(*) FROM UtilFlowFunctionView WHERE SourceRolesCodes = @RoleCode AND SourceFunctionForm = @Page AND FunctionName = @Button
			
			if (@RC > 0)
			BEGIN
				SET @Ret = 1
			END
		END

	-- Return the result of the function
	INSERT INTO @ButtonState SELECT @Ret
	RETURN;

END;