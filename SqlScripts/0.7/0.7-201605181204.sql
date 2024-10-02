/****** Object:  StoredProcedure [dbo].[WeeklyPlantWIPDetail]    Script Date: 5/18/2016 11:48:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Indra Permana>
-- Create date: <2016-02-09>
-- Description:	<Weekly Scheduler for PlantWIPDetail from ExeReportByProcess>
-- =============================================

-- =============================================
-- Author:		Azka
-- Create date: 5/18/2016
-- Description:	remove [SKT_DEV]
-- =============================================

ALTER PROCEDURE [dbo].[WeeklyPlantWIPDetail]
	-- Add the parameters for the stored procedure here
	@Date datetime, 
	@CreatedBy varchar(50),
	@UpdatedBy varchar(50)
AS
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.PlanPlantWIPDetail
	(
		[KPSYear]
      ,[KPSWeek]
      ,[ProcessGroup]
      ,[UnitCode]
      ,[LocationCode]
      ,[BrandCode]
      ,[WIPCurrentValue]
      ,[WIPPreviousValue]
      ,[WIPStock1]
      ,[WIPStock2]
      ,[WIPStock3]
      ,[WIPStock4]
      ,[WIPStock5]
      ,[WIPStock6]
      ,[WIPStock7]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
	) SELECT 
			(SELECT TOP 1 Year FROM  [dbo].[MstGenWeek]
			WHERE IDMstWeek > 
			(
			SELECT [IDMstWeek]
			  FROM [dbo].[MstGenWeek]
			WHERE Year = YEAR(@Date) AND Week = (SELECT Week FROM [dbo].[MstGenWeek] WHERE StartDate <= @Date AND EndDate >= @Date)
			)) AS KPSYear
		  ,(SELECT TOP 1 Week FROM  [dbo].[MstGenWeek]
			WHERE IDMstWeek > 
			(
			SELECT [IDMstWeek]
			  FROM [dbo].[MstGenWeek]
			WHERE Year = YEAR(@Date) AND Week = (SELECT Week FROM [dbo].[MstGenWeek] WHERE StartDate <= @Date AND EndDate >= @Date)
			)) AS KPSWeek
		  ,repProc.[ProcessGroup]
		  ,NULL AS [UnitCode]
		  ,repProc.[LocationCode]
		  ,repProc.[BrandCode]
		  ,NULL AS [WIPCurrentValue]
		  ,AVG(repProc.EndingStock) AS [WIPPreviousValue]
		  ,AVG(repProc.EndingStock) AS [WIPStock1]
		  ,AVG(repProc.EndingStock) AS [WIPStock2]
		  ,AVG(repProc.EndingStock) AS [WIPStock3]
		  ,AVG(repProc.EndingStock) AS [WIPStock4]
		  ,AVG(repProc.EndingStock) AS [WIPStock5]
		  ,AVG(repProc.EndingStock) AS [WIPStock6]
		  ,AVG(repProc.EndingStock) AS [WIPStock7]
		  ,GETDATE() AS [CreatedDate]
		  ,@CreatedBy AS [CreatedBy]
		  ,GETDATE() AS [UpdatedDate]
		  ,@UpdatedBy AS [UpdatedBy]
	  FROM [dbo].[ExeReportByProcess] repProc
	  INNER JOIN [dbo].[MstGenProcess] genProc on repProc.ProcessGroup = genProc.ProcessGroup AND genProc.WIP = 1
	  WHERE KPSWeek = (SELECT Week FROM [dbo].[MstGenWeek] WHERE StartDate <= @Date AND EndDate >= @Date) 
	  GROUP BY repProc.[KPSYear]
		  ,repProc.[KPSWeek]
		  ,repProc.[ProcessGroup]
		  ,repProc.[LocationCode]
		  ,repProc.[BrandCode]

END
