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

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-25
-- Description:	add EBL code
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-29
-- Description:	modify WPC
-- =============================================

-- =============================================
-- Author:		Hakim
-- Create date: 2016-08-29
-- Description:	change WPC algorith
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
	DECLARE @LatestIDFlow INT
	DECLARE @IDFlows TABLE (idFlow INT)
	DECLARE @Temp INT

	SET @Ret = 0;
	
	if(@TransactionCode like '%FEE%')
	BEGIN
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
	END
	ELSE IF((@TransactionCode like '%WPC%') OR (@TransactionCode like '%EBL%'))
	BEGIN
		
		SELECT TOP 1 @LatestIDFlow = IDFlow 
		FROM [dbo].[UtilTransactionLogs] 
		WHERE TransactionCode like '%' + @TransactionCode + '%'
		ORDER BY UpdatedDate DESC

		SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole

		-- Production Card checker
		IF(@Page = 'ProductionCard')
		BEGIN
			IF(@Button = 'Save' AND @LatestIDFlow in(17,18,19,21,57))
			BEGIN
				INSERT INTO @IDFlows VALUES (17)
				INSERT INTO @IDFlows VALUES (18)
				INSERT INTO @IDFlows VALUES (19)
				INSERT INTO @IDFlows VALUES (21)
				INSERT INTO @IDFlows VALUES (57)
			END
			ELSE IF(@Button = 'Submit' AND @LatestIDFlow = 21)
			BEGIN
				INSERT INTO @IDFlows VALUES (21)
			END
			ELSE IF(@Button = 'Cancel Submit' AND @LatestIDFlow in (22,23,69) )
			BEGIN
				INSERT INTO @IDFlows VALUES (22)
				INSERT INTO @IDFlows VALUES (23)
				INSERT INTO @IDFlows VALUES (69)
			END
		END

		ELSE IF(@Page = 'ProductionCardApprovalDetail')
		BEGIN
			IF(@Button = 'Approve' AND @LatestIDFlow in (22,23,69,24) )
			BEGIN
				INSERT INTO @IDFlows VALUES (22)
				INSERT INTO @IDFlows VALUES (23)
				INSERT INTO @IDFlows VALUES (24)
				INSERT INTO @IDFlows VALUES (69)
			END
			ELSE IF(@Button = 'Complete' AND @LatestIDFlow in (25,56))
			BEGIN
				INSERT INTO @IDFlows VALUES (25)
				INSERT INTO @IDFlows VALUES (56)
			END
			ELSE IF(@Button = 'Return' AND @LatestIDFlow in (25,56))
			BEGIN
				INSERT INTO @IDFlows VALUES (25)
				INSERT INTO @IDFlows VALUES (56)
			END
		END

		ELSE IF(@Page = 'EblekRelease')
		BEGIN
			IF(@Button = 'SendApproval' AND @LatestIDFlow = 26 )
			BEGIN
				INSERT INTO @IDFlows VALUES (26)
			END
		END

		ELSe IF(@Page = 'EblekReleaseApproval')
		BEGIN
			IF(@Button = 'Approve' AND @LatestIDFlow in(27,28))
			BEGIN
				INSERT INTO @IDFlows VALUES (27)
				INSERT INTO @IDFlows VALUES (28)
			END
			ELSE IF(@Button = 'Return' AND @LatestIDFlow in(29,30))
			BEGIN
				INSERT INTO @IDFlows VALUES (29)
				INSERT INTO @IDFlows VALUES (30)
			END
		END

		select @Temp = count(*) from UtilFlowFunctionView where DestinationFunctionForm = @Page and DestinationRolesCodes = @RoleCode and IDFlow IN (select * from @IDFlows)

		IF(@Temp > 0)
		BEGIN
			SET @Ret = 1
		END
	END
	-- Return the result of the function
	INSERT INTO @ButtonState SELECT @Ret
	RETURN;

END;
