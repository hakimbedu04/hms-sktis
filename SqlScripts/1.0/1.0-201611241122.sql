/****** Object:  UserDefinedFunction [dbo].[GetReportByStatus]    Script Date: 11/24/2016 10:39:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Get Report By Status Function Table
-- Ticket: http://tp.voxteneo.co.id/entity/11217
-- Author: Azka
-- Update: 11/17/2016
-- =============================================

ALTER FUNCTION [dbo].[GetReportByStatus]
(	
	@paramLocationCode	VARCHAR(10),
	@paramUnitCode		VARCHAR(10),
	@paramShift			INT,
	@paramBrandGroup	VARCHAR(20),
	@paramBrandCode		VARCHAR(20),
	@paramProdDateFrom	DATETIME,
	@paramProdDateTo	DATETIME
)
RETURNS @result TABLE 
(
	[ProcessGroup]			VARCHAR(16),
	[StatusEmp]				VARCHAR(16),
	[ActualWorker]			INT,
	[ActualAbsWorker]		INT,
	[ActualWorkHourPerDay]	DECIMAL(20, 2),
	[ProductionStick]		BIGINT,
	[StickHourPeople]		DECIMAL(20, 2),
	[StickHour]				DECIMAL(20, 2),
	PRIMARY KEY CLUSTERED ([ProcessGroup], [StatusEmp])
)
AS
BEGIN
	DECLARE @countChild INT;
	SELECT @countChild = COUNT(*) FROM [dbo].[GetLastChildLocation](@paramLocationCode);
	
	IF (@paramBrandCode = 'All')
		SET @paramBrandCode = NULL;

	IF (@countChild > 1) -- Parent Location SKT, TPO, REG1-4
	BEGIN
		INSERT INTO @result
		SELECT 
			g.ProcessGroup,
			g.StatusEmp,
			SUM(ISNULL(g.ActualWorker, 0)) AS ActualWorker,
			ROUND(SUM(ISNULL(g.Absence_C, 0) + ISNULL(g.Absence_CH, 0) + ISNULL(g.Absence_CT, 0) + ISNULL(g.Absence_S, 0) + ISNULL(g.Absence_ETC, 0) 
					+ ISNULL(g.Absence_I, 0) + ISNULL(g.Absennce_A, 0) + ISNULL(g.Multi_CUTT, 0) + ISNULL(g.Multi_FWRP, 0) 
					+ ISNULL(g.Multi_GEN, 0) + ISNULL(g.Multi_PACK, 0) + ISNULL(g.Multi_ROLL, 0) + ISNULL(g.Multi_STAMP, 0) 
					+ ISNULL(g.Multi_SWRP, 0) + ISNULL(g.Multi_TPO, 0) + ISNULL(g.Multi_WRP, 0)
				), 2) AS ActualAbsWorker,
			CAST(ROUND(AVG(ISNULL(g.WorkHour, 0)), 2) as DECIMAL(20,2)) AS [ActualWorkHourPerDay], 
			SUM(CAST(ISNULL(g.Production, 0) as BIGINT)) as [ProductionStick],
			CAST(0 as DECIMAL(20,2)) AS [StickHourPeople],
			CAST(0 as DECIMAL(20,2)) AS [StickHour]
		FROM ExeReportByGroups g
		INNER JOIN MstGenProcess p on p.ProcessGroup = g.ProcessGroup
		WHERE g.LocationCode IN (SELECT LocationCode FROM [dbo].[GetLastChildLocation](@paramLocationCode)) AND g.ProductionDate BETWEEN @paramProdDateFrom AND @paramProdDateTo
				AND g.BrandGroupCode = @paramBrandGroup AND g.BrandCode = COALESCE(@paramBrandCode, g.BrandCode) AND g.Shift = @paramShift
		GROUP BY
			g.ProcessGroup,
			g.StatusEmp,
			p.ProcessOrder
		ORDER BY p.ProcessOrder
		OPTION(RECOMPILE)

		DECLARE @processGroup VARCHAR(16), @statusEmp VARCHAR(16), @actual INT, @prodstick BIGINT;

		DECLARE csr CURSOR LOCAL FOR
		SELECT ProcessGroup, StatusEmp, ProductionStick, ActualWorker FROM @result

		OPEN csr
		FETCH NEXT FROM csr INTO @processGroup, @statusEmp, @prodstick, @actual
		WHILE @@FETCH_STATUS = 0 
		BEGIN
			DECLARE @actualWorkHourPerDay DECIMAL(10,2);
			SELECT @actualWorkHourPerDay = CAST(ROUND(COALESCE(SUM(ISNULL(ActualWorker,0) * ISNULL(WorkHour,0)) / NULLIF(SUM(ISNULL(ActualWorker, 0)), 0), 0) ,2) as DECIMAL(20,2))
			FROM ExeReportByGroups g
			WHERE g.LocationCode IN (SELECT LocationCode FROM [dbo].[GetLastChildLocation](@paramLocationCode)) AND g.ProductionDate BETWEEN @paramProdDateFrom AND @paramProdDateTo
				AND g.BrandGroupCode = @paramBrandGroup AND g.BrandCode = COALESCE(@paramBrandCode, g.BrandCode) AND g.ProcessGroup = @processGroup AND g.StatusEmp = @statusEmp
				AND g.Shift = @paramShift
			OPTION(RECOMPILE)
			
			UPDATE @result 
			SET
				ActualWorkHourPerDay = CAST(@actualWorkHourPerDay as DECIMAL(20,2)),
				StickHourPeople = CAST(ROUND(COALESCE(@prodstick / NULLIF(@actualWorkHourPerDay, 0) / NULLIF(@actual,0), 0), 2) as DECIMAL(20,2)),
				StickHour = CAST(ROUND(COALESCE(@prodstick / NULLIF(@actualWorkHourPerDay, 0), 0), 2) as DECIMAL(20,2))
			WHERE StatusEmp = @statusEmp AND ProcessGroup = @processGroup

			FETCH NEXT FROM csr INTO @processGroup, @statusEmp, @prodstick, @actual
		END 

		CLOSE csr
		DEALLOCATE csr
	END
	ELSE
	BEGIN
		IF (@paramUnitCode = 'All')
			SET @paramUnitCode = NULL;

		INSERT INTO @result
		SELECT 
			g.ProcessGroup,
			g.StatusEmp,
			SUM(ISNULL(g.ActualWorker, 0)) AS ActualWorker,
			ROUND(SUM(ISNULL(g.Absence_C, 0) + ISNULL(g.Absence_CH, 0) + ISNULL(g.Absence_CT, 0) + ISNULL(g.Absence_S, 0) + ISNULL(g.Absence_ETC, 0) 
					+ ISNULL(g.Absence_I, 0) + ISNULL(g.Absennce_A, 0) + ISNULL(g.Multi_CUTT, 0) + ISNULL(g.Multi_FWRP, 0) 
					+ ISNULL(g.Multi_GEN, 0) + ISNULL(g.Multi_PACK, 0) + ISNULL(g.Multi_ROLL, 0) + ISNULL(g.Multi_STAMP, 0) 
					+ ISNULL(g.Multi_SWRP, 0) + ISNULL(g.Multi_TPO, 0) + ISNULL(g.Multi_WRP, 0)
				), 2) AS ActualAbsWorker,
			CAST(ROUND(AVG(ISNULL(g.WorkHour, 0)), 2) as DECIMAL(20, 2)) AS [ActualWorkHourPerDay], 
			SUM(ISNULL(g.Production, 0)) as [ProductionStick],
			CAST(ROUND(COALESCE(SUM(ISNULL(g.Production, 0)) / NULLIF(ROUND(AVG(ISNULL(g.WorkHour, 0)), 2), 0) / NULLIF(SUM(ISNULL(g.ActualWorker, 0)),0), 0), 2) as DECIMAL(20,2)) AS [StickHourPeople],
			CAST(ROUND(COALESCE(SUM(ISNULL(g.Production, 0)) / NULLIF(ROUND(AVG(ISNULL(g.WorkHour, 0)), 2),0), 0), 2) as DECIMAL(20,2)) AS [StickHour]
		FROM ExeReportByGroups g
		INNER JOIN MstGenProcess p on p.ProcessGroup = g.ProcessGroup
		WHERE g.LocationCode = @paramLocationCode AND g.UnitCode = COALESCE(@paramUnitCode, g.UnitCode) AND g.ProductionDate BETWEEN @paramProdDateFrom AND @paramProdDateTo
				AND g.BrandGroupCode = @paramBrandGroup AND g.BrandCode = COALESCE(@paramBrandCode, g.BrandCode) AND g.Shift = @paramShift
		GROUP BY
			g.ProcessGroup,
			g.StatusEmp,
			p.ProcessOrder
		ORDER BY p.ProcessOrder
		OPTION(RECOMPILE)
	END
	RETURN;
END;