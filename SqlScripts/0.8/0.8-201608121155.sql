-- =============================================
-- Author:		Robby Prima
-- Create date: 2016-08-11
-- Description:	Check if user can do button action
-- =============================================
CREATE FUNCTION [dbo].[RoleButtonChecker]
(
	@TPOFeeCode VARCHAR(250),
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

	SET @DestinationRole = (SELECT DestinationRole FROM TPOFeeApprovalView WHERE TPOFeeCode = @TPOFeeCode)
	SELECT TOP 1 @RoleCode = RolesCode FROM UtilRoles WHERE IDRole = @IDRole

	if (@DestinationRole = @IDRole)
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

