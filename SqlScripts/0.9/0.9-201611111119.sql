IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetDefaultWorkhour]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetDefaultWorkhour]
/****** Object:  UserDefinedFunction [dbo].[GetDefaultWorkhour]    Script Date: 11/11/2016 11:18:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetDefaultWorkhour]
(
	@paramLocationCode VARCHAR(10),
	@paramBrandGroupCode VARCHAR(20),
	@paramProcessGroup VARCHAR(10),
	@paramWorkhour INT
)
RETURNS DECIMAL(18,3)
AS
BEGIN
	DECLARE @result DECIMAL(18,3);

	SELECT TOP 1 @result = ((StdStickPerHour * @paramWorkhour) / UOMEblek) 
	FROM ProcessSettingsAndLocationView v 
	WHERE v.LocationCode = @paramLocationCode and BrandGroupCode = @paramBrandGroupCode and ProcessGroup = @paramProcessGroup

	return @result;
END