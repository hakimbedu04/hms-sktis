USE [master]
GO
/****** Object:  Database [SKT-IS-V1.1.7]    Script Date: 10/16/2015 2:56:31 PM ******/
CREATE DATABASE [SKT-IS-V1.1.7]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SKT-IS-V1.1.7', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\SKT-IS-V1.1.7.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SKT-IS-V1.1.7_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\SKT-IS-V1.1.7_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SKT-IS-V1.1.7].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ARITHABORT OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET RECOVERY FULL 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET  MULTI_USER 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'SKT-IS-V1.1.7', N'ON'
GO
USE [SKT-IS-V1.1.7]
GO
/****** Object:  StoredProcedure [dbo].[GetMstGenLocationsByParentCode]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetMstGenLocationsByParentCode]
-- Add the parameters for the stored procedure here
      @parentLocationCode VARCHAR(8) = 'SKT',
      @sortColumn         VARCHAR(30) = NULL,
      @sortOrder          VARCHAR(4) = 'ASC'
AS
    BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        -- interfering with SELECT statements.
        SET NOCOUNT ON;
        SET FMTONLY OFF;
        -- Insert statements for procedure here
        DECLARE @sql NVARCHAR(MAX);
        IF @parentLocationCode IS NULL
            BEGIN
                SET @parentLocationCode = 'SKT';
            END;
            SET @sql = 'WITH LocationDetail( LocationCode,
                        LocationName,
                        CostCenter,
                        ABBR,
                        Shift,
				    ParentLocationCode,
                        UMK,
                        KPPBC,
                        Address,
                        City,
                        Region,
                        Phone,
                        StatusActive,
                        Remark,
                        CreatedDate,
                        CreatedBy,
                        UpdatedDate,
                        UpdatedBy)
            AS (
            SELECT LocationCode,
                   LocationName,
                   CostCenter,
                   ABBR,
                   Shift,
                   ParentLocationCode,
                   UMK,
                   KPPBC,
                   Address,
                   City,
                   Region,
                   Phone,
                   StatusActive,
                   Remark,
                   CreatedDate,
                   CreatedBy,
                   UpdatedDate,
                   UpdatedBy
            FROM MstGenLocation
            WHERE LocationCode = ''' + @parentLocationCode + ''' UNION ALL
            SELECT l.LocationCode,
                   l.LocationName,
                   l.CostCenter,
                   l.ABBR,
                   l.Shift,
                   l.ParentLocationCode,
                   l.UMK,
                   l.KPPBC,
                   l.Address,
                   l.City,
                   l.Region,
                   l.Phone,
                   l.StatusActive,
                   l.Remark,
                   l.CreatedDate,
                   l.CreatedBy,
                   l.UpdatedDate,
                   l.UpdatedBy
            FROM MstGenLocation l
                 INNER JOIN LocationDetail AS ld ON l.ParentLocationCode = ld.LocationCode )
        SELECT LocationCode,
               LocationName,
               CostCenter,
               ABBR,
               Shift,
			ParentLocationCode,
               UMK,
               KPPBC,
               Address,
               City,
               Region,
               Phone,
               StatusActive,
               Remark,
               CreatedDate,
               CreatedBy,
               UpdatedDate,
               UpdatedBy
        FROM LocationDetail';
            IF @sortColumn IS NOT NULL
           AND @sortColumn <> ''
                BEGIN
                    SET @sql = @sql + ' ORDER BY ' + @sortColumn + ' ' + @sortOrder;
                END;
                PRINT @sql;
                EXEC sp_executesql
                     @sql;
            --   WITH LocationDetail( LocationCode,
            --                LocationName,
            --                CostCenter,
            --                ABBR,
            --                Shift,
            --			 ParentLocationCode,
            --                UMK,
            --                KPPBC,
            --                Address,
            --                City,
            --                Region,
            --                Phone,
            --                StatusActive,
            --                Remark,
            --                CreatedDate,
            --                CreatedBy,
            --                UpdatedDate,
            --                UpdatedBy)
            --    AS (
            --    SELECT LocationCode,
            --           LocationName,
            --           CostCenter,
            --           ABBR,
            --           Shift,
            --           ParentLocationCode,
            --           UMK,
            --           KPPBC,
            --           Address,
            --           City,
            --           Region,
            --           Phone,
            --           StatusActive,
            --           Remark,
            --           CreatedDate,
            --           CreatedBy,
            --           UpdatedDate,
            --           UpdatedBy
            --    FROM MstGenLocation
            --    WHERE LocationCode = 'SKT'--@locationcode
            --    UNION ALL
            --    SELECT l.LocationCode,
            --           l.LocationName,
            --           l.CostCenter,
            --           l.ABBR,
            --           l.Shift,
            --           l.ParentLocationCode,
            --           l.UMK,
            --           l.KPPBC,
            --           l.Address,
            --           l.City,
            --           l.Region,
            --           l.Phone,
            --           l.StatusActive,
            --           l.Remark,
            --           l.CreatedDate,
            --           l.CreatedBy,
            --           l.UpdatedDate,
            --           l.UpdatedBy
            --    FROM MstGenLocation l
            --         INNER JOIN LocationDetail AS ld ON l.ParentLocationCode = ld.LocationCode )
            --SELECT LocationCode,
            --       LocationName,
            --       CostCenter,
            --       ABBR,
            --       Shift,
            --       ParentLocationCode,
            --       UMK,
            --       KPPBC,
            --       Address,
            --       City,
            --       Region,
            --       Phone,
            --       StatusActive,
            --       Remark,
            --       CreatedDate,
            --       CreatedBy,
            --       UpdatedDate,
            --       UpdatedBy
            --FROM LocationDetail
            --ORDER BY LocationCode ASC;
    END;









GO
/****** Object:  StoredProcedure [dbo].[GetReportSummaryDailyProductionTargets]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetReportSummaryDailyProductionTargets] 
	@Year int,
	@Week int,
	@Decimal int
AS
BEGIN
    DECLARE @DateFrom DATETIME, @DateTo DATETIME

    SELECT @DateFrom = dbo.MstGenWeek.StartDate,	@DateTo = dbo.MstGenWeek.EndDate FROM dbo.MstGenWeek WHERE dbo.MstGenWeek.[Year] = @Year AND dbo.MstGenWeek.Week = @Week

    SELECT BrandCode, LocationCode,
	    SUM(Coalesce(Targetmanual1, 0)) AS TargetManual1,
	    SUM(Coalesce(Targetmanual2, 0)) AS TargetManual2,
	    SUM(Coalesce(Targetmanual3, 0)) AS TargetManual3,
	    SUM(Coalesce(Targetmanual4, 0)) AS TargetManual4,
	    SUM(Coalesce(Targetmanual5, 0)) AS TargetManual5,
	    SUM(Coalesce(Targetmanual6, 0)) AS TargetManual6,
	    SUM(Coalesce(Targetmanual7, 0)) AS TargetManual7
    FROM PlanTPOTargetProductionKelompok AS ptpk WHERE (ProcessGroup = 'GILING') AND ptpk.TPKTPOStartProductionDate >= @DateFrom AND ptpk.TPKTPOStartProductionDate <= @DateTo
    GROUP BY BrandCode, LocationCode
    UNION
    SELECT BrandCode, LocationCode,
	    SUM(Coalesce(Targetmanual1, 0)) AS TargetManual1,
	    SUM(Coalesce(Targetmanual2, 0)) AS TargetManual2,
	    SUM(Coalesce(Targetmanual3, 0)) AS TargetManual3,
	    SUM(Coalesce(Targetmanual4, 0)) AS TargetManual4,
	    SUM(Coalesce(Targetmanual5, 0)) AS TargetManual5,
	    SUM(Coalesce(Targetmanual6, 0)) AS TargetManual6,
	    SUM(Coalesce(Targetmanual7, 0)) AS TargetManual7
    FROM dbo.PlanTargetProductionUnit ptpu WHERE ptpu.ProductionStartDate >= @DateFrom AND ptpu.ProductionStartDate <= @DateTo
    GROUP BY BrandCode, LocationCode

--    SELECT BrandCode,
--	LocationCode,
--	SUM(Coalesce(TargetSystem1, 0)) AS TargetManual1,
--	SUM(Coalesce(TargetSystem2, 0)) AS TargetManual2,
--	SUM(Coalesce(TargetSystem3, 0)) AS TargetManual3,
--	SUM(Coalesce(TargetSystem4, 0)) AS TargetManual4,
--	SUM(Coalesce(TargetSystem5, 0)) AS TargetManual5,
--	SUM(Coalesce(TargetSystem6, 0)) AS TargetManual6,
--	SUM(Coalesce(TargetSystem7, 0)) AS TargetManual7
--FROM dbo.PlanPlantTargetProductionKelompok AS pptpk
--WHERE pptpk.TPKPlantStartProductionDate >= @DateFrom AND pptpk.TPKPlantStartProductionDate <= @DateTo
--GROUP BY BrandCode,
--	LocationCode

END






GO
/****** Object:  StoredProcedure [dbo].[GetReportSummaryProcessTargets]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetReportSummaryProcessTargets]
	@Location varchar(50),
	@Year int,
	@Week int,
	@DateFrom datetime,
	@DateTo date,
	@Decimal int
AS
BEGIN

    DECLARE @StarDateWeek date, @EndDateWeek date
    -- Get StartDate and EndDate from MstGenWeek
    SELECT @StarDateWeek = mgw.StartDate, @EndDateWeek = mgw.EndDate FROM dbo.MstGenWeek mgw WHERE mgw.Week = @Week AND mgw.Year = @Year

    DECLARE @int int =  1, 
		  @query1 varchar(max) = 'SELECT pptpk.LocationCode, pptpk.BrandCode, pptpk.ProcessGroup, pptpk.KPSYear, pptpk.KPSWeek,',
		  @query2 varchar(max) = '';
    SELECT  @int = DATEDIFF(DAY, @StarDateWeek, @DateFrom)

    WHILE @DateFrom <= @DateTo
    BEGIN   
	  SET @query2 = @query2 + ' ISNULL(pptpk.TargetManual' + CAST(@int + 1 AS varchar) +', 0) +'
	  SET @int = @int + 1
	  SET @DateFrom = DateAdd(day, 1, @DateFrom)
    END

    DECLARE @pptpkTable AS TABLE (LocationCode varchar(50), UnitCode varchar(50), BrandCode varchar(50), ProcessGroup varchar(50), KPSYear int, KPSWeek int, TargetManual real)
    DECLARE @pttpkTable AS TABLE (LocationCode varchar(50), UnitCode varchar(50), BrandCode varchar(50), ProcessGroup varchar(50), KPSYear int, KPSWeek int, TargetManual real)

    INSERT INTO @pptpkTable ( LocationCode, BrandCode, ProcessGroup, KPSYear, KPSWeek, TargetManual, UnitCode )
    EXEC (@query1 + @query2 + '0 AS TargetManual, pptpk.UnitCode FROM PlanPlantTargetProductionKelompok pptpk' )

    INSERT INTO @pttpkTable ( LocationCode, BrandCode, ProcessGroup, KPSYear, KPSWeek, TargetManual, UnitCode )
    EXEC (@query1 + @query2 + '0 AS TargetManual, ''TPO'' UnitCode FROM PlanTPOTargetProductionKelompok pptpk')

    SET @int = 1;
    DECLARE @indexWIPDetail int = 1;
    WHILE @StarDateWeek <= @EndDateWeek
    BEGIN
	   IF @StarDateWeek = @DateTo
   	    BEGIN
     	    SET @indexWIPDetail = @int
	    END
	    SET @int = @int + 1
	    SET @StarDateWeek = DateAdd(day, 1, @StarDateWeek)
    END

    SET @query1 = 'SELECT ppw.LocationCode, ppw.UnitCode,ppw.BrandCode,''WIPGunting'' AS ProcessGroup, ppw.KPSYear, ppw.KPSWeek, ppw.WIPStock' + CAST(@indexWIPDetail AS varchar) + ' FROM dbo.PlanPlantWIPDetail ppw WHERE ppw.ProcessGroup =''GUNTING'' '
    SET @query2 = 'SELECT ppw.LocationCode, ppw.UnitCode,ppw.BrandCode,''WIPPak'' AS ProcessGroup, ppw.KPSYear, ppw.KPSWeek, ppw.WIPStock' + CAST(@indexWIPDetail AS varchar) + ' FROM dbo.PlanPlantWIPDetail ppw WHERE ppw.ProcessGroup =''Pak'' '
    INSERT INTO @pptpkTable ( LocationCode, UnitCode, BrandCode, ProcessGroup, KPSYear, KPSWeek, TargetManual )
    EXEC (@query1)
    INSERT INTO @pptpkTable ( LocationCode, UnitCode, BrandCode, ProcessGroup, KPSYear, KPSWeek, TargetManual )
    EXEC (@query2)

    SELECT Result.LocationCode,
	    Result.UnitCode,
	    Result.BrandCode,
	    ISNULL(Result.Giling, 0) Giling,
	    ISNULL(Result.Gunting, 0) Gunting,
	    ISNULL(Result.Pak, 0) Pak,
	    ISNULL(Result.WIPGunting, 0) WIPGunting,
	    ISNULL(Result.WIPPak, 0) WIPPak,
	    ISNULL(Result.Banderol, 0) AS Banderol,
	    (ISNULL(Result.Giling, 0) + ISNULL(Result.Gunting, 0) + ISNULL(Result.Pak, 0) + ISNULL(Result.WIPGunting, 0) + ISNULL(Result.WIPPak, 0) + ISNULL(Result.Banderol, 0)) / mgbg.BalPerBox AS Box
    FROM (
	    SELECT [LocationCode], [UnitCode], [BrandCode], [Giling], [Gunting], [Pak], [WRAPPING] + [Banderol] AS Banderol, [WIPGunting], [WIPPak]
	    FROM (
		    SELECT * FROM (
			    SELECT pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup, SUM(pptpk.TargetManual) AS TargetManual1 FROM @pptpkTable pptpk
			    WHERE ( pptpk.LocationCode IN ( SELECT LocationCode FROM [dbo].[GetLocations](@Location, - 1) ) ) AND pptpk.KPSYear = @Year AND pptpk.KPSWeek = @Week
			    GROUP BY pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup ) AS A
		    ) AS B
	    PIVOT(SUM(TargetManual1) FOR [ProcessGroup] IN ( [Giling], [Gunting], [Pak], [WRAPPING], [Banderol], [WIPGunting], [WIPPak] )) AS P
	
	    UNION
	
	    SELECT [LocationCode], [UnitCode], [BrandCode], [Giling], [Gunting], [Pak], [WRAPPING] + [Banderol] AS Banderol, 0 AS WIPGunting, 0 WIPPak
	    FROM (
		    SELECT * FROM (
			    SELECT pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup, SUM(pptpk.TargetManual) AS TargetManual1 FROM @pttpkTable pptpk
			    WHERE ( pptpk.LocationCode IN ( SELECT LocationCode FROM [dbo].[GetLocations](@Location, - 1) ) ) AND pptpk.KPSYear = @Year AND pptpk.KPSWeek = @Week
			    GROUP BY pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup ) AS A
		    ) AS B
	    PIVOT(SUM(TargetManual1) FOR [ProcessGroup] IN ( [Giling], [Gunting], [Pak], [WRAPPING], [Banderol] )) AS P
	    ) AS Result
    LEFT JOIN dbo.MstGenBrand mgb	ON Result.BrandCode = mgb.BrandCode
    INNER JOIN dbo.MstGenBrandGroup mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode

    --SELECT Result.LocationCode,
	   -- Result.UnitCode,
	   -- Result.BrandCode,
	   -- ISNULL(Result.Giling, 0) Giling,
	   -- ISNULL(Result.Gunting, 0) Gunting,
	   -- ISNULL(Result.Pak, 0) Pak,
	   -- ISNULL(Result.WIPGunting, 0) WIPGunting,
	   -- ISNULL(Result.WIPPak, 0) WIPPak,
	   -- ISNULL(Result.Banderol, 0) AS Banderol,
	   -- (ISNULL(Result.Giling, 0) + ISNULL(Result.Gunting, 0) + ISNULL(Result.Pak, 0) + ISNULL(Result.WIPGunting, 0) + ISNULL(Result.WIPPak, 0) + ISNULL(Result.Banderol, 0)) / mgbg.BalPerBox AS Box
    --FROM (
	   -- SELECT [LocationCode], [UnitCode], [BrandCode], [Giling], [Gunting], [Pak], [WRAPPING] AS Banderol, 0 AS WIPGunting, 0 WIPPak
	   -- FROM (
		  --  SELECT * FROM (
			 --   SELECT pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup, SUM(pptpk.TargetManual1) AS TargetManual1
			 --   FROM dbo.PlanPlantTargetProductionKelompok pptpk
			 --   WHERE (pptpk.LocationCode IN (SELECT LocationCode FROM [dbo].[GetLocations] ( @Location ,-1))) AND pptpk.TPKPlantStartProductionDate >= @DateFrom AND pptpk.TPKPlantStartProductionDate <= @DateTo
			 --   GROUP BY pptpk.LocationCode, pptpk.UnitCode, pptpk.BrandCode, pptpk.ProcessGroup ) AS A
		  --  ) AS B
	   -- PIVOT(SUM(TargetManual1) FOR [ProcessGroup] IN ( [Giling], [Gunting], [Pak], [WRAPPING] )) AS P
	
	   -- UNION
	
	   -- SELECT [LocationCode], [UnitCode], [BrandCode], [Giling], [Gunting], [Pak], [WRAPPING] AS Banderol, 0 AS WIPGunting, 0 WIPPak
	   -- FROM (
		  --  SELECT * FROM (
			 --   SELECT pptpk.LocationCode, 'TPO' UnitCode, pptpk.BrandCode, pptpk.ProcessGroup, sum(pptpk.TargetManual1) AS TargetManual1
			 --   FROM dbo.PlanTPOTargetProductionKelompok pptpk
			 --   WHERE (pptpk.LocationCode IN (SELECT LocationCode FROM [dbo].[GetLocations] ( @Location ,-1))) AND pptpk.TPKTPOStartProductionDate >= @DateFrom AND pptpk.TPKTPOStartProductionDate <= @DateTo
			 --   GROUP BY pptpk.LocationCode, pptpk.BrandCode, pptpk.ProcessGroup ) AS A
		  --  ) AS B
	   -- PIVOT(SUM(TargetManual1) FOR [ProcessGroup] IN ( [Giling], [Gunting], [Pak], [WRAPPING] )) AS P
	   -- ) AS Result
    --LEFT JOIN dbo.MstGenBrand mgb ON Result.BrandCode = mgb.BrandCode
    --INNER JOIN dbo.MstGenBrandGroup mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
END






GO
/****** Object:  StoredProcedure [dbo].[GetWorkerBrandAssignmentPlanningPlantTPK]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetWorkerBrandAssignmentPlanningPlantTPK]
    @GroupCode			   VARCHAR(20),
    @ProcessSettingsCode	   VARCHAR(50),
    @Year				   INT,
    @Week				   INT,
    @UnitCode			   varchar(20),
    @LocationCode		   varchar(50),
    @BrandCode			   varchar(50),
    @Shift			   varchar(2),
    @TPKPlantStartProdDate  date 
AS
BEGIN

DECLARE @StartDate DATE, 
	   @EndDate   DATE; 

SELECT @StartDate = mgw.StartDate, @EndDate = mgw.EndDate FROM dbo.MstGenWeek mgw WHERE mgw.Week = @Week AND mgw.[Year] = @Year;

SELECT mpejda.EmployeeID EmployeeNumber,
	  CASE
           WHEN EXISTS( SELECT ppa.EmployeeID FROM dbo.PlanPlantAllocation ppa 
				    WHERE	  ppa.EmployeeID = mpejda.EmployeeID AND
						  ppa.KPSYear = @Year AND
						  ppa.KPSWeek = @Week AND
						  ppa.GroupCode = @GroupCode AND
						  ppa.UnitCode = @UnitCode AND
						  ppa.TPKPlantStartProductionDate = @TPKPlantStartProdDate)
				THEN '1'
                    ELSE '0'
           END Status
FROM dbo.MstPlantEmpJobsDataAcv mpejda
    INNER JOIN dbo.ExePlantWorkerAbsenteeism epwa ON epwa.EmployeeID=mpejda.EmployeeID
WHERE mpejda.GroupCode = @GroupCode
  AND mpejda.ProcessSettingsCode = @ProcessSettingsCode
  AND epwa.StartDateAbsent <= @StartDate
  AND epwa.EndDate >= @EndDate;

END;

GO
/****** Object:  StoredProcedure [dbo].[RunSSISPlantTPK]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RunSSISPlantTPK]
	@UserName NVarchar(64),
	@KPSYear int,
	@KPSWeek int,
	@LocationCode varchar(8),
	@BrandCode varchar(11),
	@Shift int
AS
BEGIN
declare @param varchar(max)

set @param = N' /SET "\Package.Variables[User::UserName].Properties[Value]";' + @UserName +
			  ' /SET "\Package.Variables[User::KPSYear].Properties[Value]";' + cast(@KPSYear as varchar(4)) +
			  ' /SET "\Package.Variables[User::KPSWeek].Properties[Value]";' + cast(@KPSWeek as varchar(2)) +
			  ' /SET "\Package.Variables[User::LocationCode].Properties[Value]";' + @LocationCode +
			  ' /SET "\Package.Variables[User::BrandCode].Properties[Value]";' + @BrandCode +
			  ' /SET "\Package.Variables[User::Shift].Properties[Value]";' + cast(@Shift as varchar(2))

EXEC	[dbo].[usp_ExecAdhocJob]
		@Param = @param,
		@dtsxName = N'PlantTPK'
END






GO
/****** Object:  StoredProcedure [dbo].[RunSSISProductionEntryPlant]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RunSSISProductionEntryPlant]
	@UserName NVarchar(64),
	@KPSYear int,
	@KPSWeek int,
	@LocationCode varchar(8),
	@BrandCode varchar(11),
	@UnitCode varchar(4)
AS
BEGIN
declare @param varchar(max)

set @param = N' /SET "\Package.Variables[User::UserName].Properties[Value]";' + @UserName +
			  ' /SET "\Package.Variables[User::KPSYear].Properties[Value]";' + cast(@KPSYear as varchar(4)) +
			  ' /SET "\Package.Variables[User::KPSWeek].Properties[Value]";' + cast(@KPSWeek as varchar(2)) +
			  ' /SET "\Package.Variables[User::LocationCode].Properties[Value]";' + @LocationCode +
			  ' /SET "\Package.Variables[User::BrandCode].Properties[Value]";' + @BrandCode +
			  ' /SET "\Package.Variables[User::UnitCode].Properties[Value]";' + @UnitCode

EXEC	[dbo].[usp_ExecAdhocJob]
		@Param = @param,
		@dtsxName = N'ProductionEntryPlant'
END


GO
/****** Object:  StoredProcedure [dbo].[RunSSISProductionEntryTPO]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RunSSISProductionEntryTPO]
	@UserName NVarchar(64),
	@KPSYear int,
	@KPSWeek int,
	@LocationCode varchar(8),
	@BrandCode varchar(11)
AS
BEGIN
declare @param varchar(max)

set @param = N' /SET "\Package.Variables[User::UserName].Properties[Value]";' + @UserName +
			  ' /SET "\Package.Variables[User::KPSYear].Properties[Value]";' + cast(@KPSYear as varchar(4)) +
			  ' /SET "\Package.Variables[User::KPSWeek].Properties[Value]";' + cast(@KPSWeek as varchar(2)) +
			  ' /SET "\Package.Variables[User::LocationCode].Properties[Value]";' + @LocationCode +
			  ' /SET "\Package.Variables[User::BrandCode].Properties[Value]";' + @BrandCode 

EXEC	[dbo].[usp_ExecAdhocJob]
		@Param = @param,
		@dtsxName = N'ProductionEntryTPO'
END






GO
/****** Object:  StoredProcedure [dbo].[RunSSISWPP]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RunSSISWPP]
	@UserName NVarchar(64)
AS
BEGIN

declare @param varchar(max)

set @param = N' /SET "\Package.Variables[User::UserName].Properties[Value]";' + @UserName

EXEC	[dbo].[usp_ExecAdhocJob]
		@Param = @param,
		@dtsxName = N'WPP'

END



GO
/****** Object:  StoredProcedure [dbo].[TransactionLog]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TransactionLog] 
	-- Add the parameters for the stored procedure here
	@Separator			   VARCHAR(20),
    @Page				   VARCHAR(16),
    @Year				   INT,
    @Week				   INT,
	@code_1				   VARCHAR(16),
	@code_2				   VARCHAR(16),
	@code_3				   VARCHAR(16),
	@code_4				   VARCHAR(16),
	@code_5				   VARCHAR(16),
	@code_6				   VARCHAR(16),
	@code_7				   VARCHAR(16),
	@code_8				   VARCHAR(16),
	@code_9				   VARCHAR(16),
	@Transaction_Date	   datetime	,
	@ActionButton		   VARCHAR(16),
	@ActionTime			   datetime,
	@UserName				   VARCHAR(32)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @IDFunct int, @IDButton int, @IDFlow int, @TransactionCode  VARCHAR(128),@MessageText VARCHAR(128)
	-- checking the  
	if(@Page Like 'WPP%')
	Begin
	Set @TransactionCode = Convert(nvarchar(50),@code_1)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_2)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_3)
	Select     @IDFunct =IDFunction from UtilFunctions ufun where ufun.FunctionName Like 'Weekly%'
	End
	if(@Page Like 'TPU%')
	Begin
	Set @TransactionCode = Convert(nvarchar(50),@code_1)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_2)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_3)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_4)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_5)
	Select     @IDFunct =IDFunction from UtilFunctions ufun where ufun.FunctionName Like 'TargetProductionUnit%'
	End
	if(@Page Like 'TPKPLANT%')
	Begin
	Set @TransactionCode = Convert(nvarchar(50),@code_1)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_2)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_3)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_4)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_5)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_6)
	Select  @IDFunct =IDFunction from UtilFunctions ufun where ufun.FunctionName Like 'PlantTargetProductionGroup%'
	End
	if(@Page Like 'TPKTPO%')
	Begin
	Set @TransactionCode = Convert(nvarchar(50),@code_1)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_2)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_3)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_4)+Convert(nvarchar(50),@Separator)+Convert(nvarchar(50),@code_5)
	Select     @IDFunct =IDFunction from UtilFunctions ufun where ufun.FunctionName Like 'TPOTargetProductionGroup%'
	End
	-- insert into transaction log table
	Begin
	Select     @IDButton =IDFunction from UtilFunctions ufun where ufun.ParentIDFunction = @IDFunct and ufun.FunctionName = @ActionButton
	Select     @IDFlow = IDFlow,@MessageText=MessageText from UtilFlows uflow where uflow.FormSource = @IDFunct and uflow.ActionButton = @IDButton
	INSERT INTO UtilTransactionLogs( TransactionCode,TransactionDate,IDFlow,Comments,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(@TransactionCode,@Transaction_Date,@IDFlow,@MessageText,@ActionTime,@UserName,GETDATE(),@UserName) 
	End
	    --Insert statements for procedure here	
END

GO
/****** Object:  StoredProcedure [dbo].[usp_ExecAdhocJob]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_ExecAdhocJob]
        @Param VARCHAR(max),@dtsxName VARCHAR(10)

AS
DECLARE @jobId BINARY(16), @tmp_jobname SYSNAME, @cmd VARCHAR(8000), @database varchar(64)

SELECT @tmp_jobname = 'SKTIS_exec_' + @dtsxName + '_' 
	+ CAST(YEAR(GETDATE()) AS VARCHAR(4)) + RIGHT('00'+CAST(MONTH(GETDATE()) AS VARCHAR(2)),2) + RIGHT('00'+CAST(DAY(GETDATE()) AS VARCHAR(2)),2)
	+ RIGHT('00' + CAST(DATEPART(HOUR,GETDATE()) AS VARCHAR(2)),2) + RIGHT('00' + CAST(DATEPART(MINUTE,GETDATE()) AS VARCHAR(2)),2) + RIGHT('00' + CAST(DATEPART(SECOND,GETDATE()) AS VARCHAR(2)),2)

SET @database = N'SKT-IS-V1.1.7'
SET @cmd = '/FILE "D:\Voxteneo\Project\Sampoerna SKTIS\dotnet-hms-skt\SSIS Package\2008\SKT-IS\SKT-IS\'+@dtsxName+'.dtsx" /CHECKPOINTING OFF '+@Param+' /REPORTING E'

EXEC msdb.dbo.sp_add_job @job_name=@tmp_jobname, 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_netsend=2, 
		@notify_level_page=2, 
		@delete_level=1, 
		--@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@job_id = @jobId OUTPUT
		
EXEC msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name= @dtsxName, 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'SSIS', 
		@command=@cmd,
		@database_name= @database,
		@flags=0
		
EXEC msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1

EXEC msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = @tmp_jobname)	
EXEC msdb.dbo.sp_start_job @job_name=@tmp_jobname
RETURN



GO
/****** Object:  StoredProcedure [dbo].[VT_DBUpdatesExecScript]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[VT_DBUpdatesExecScript] (	@ScriptText VARCHAR(MAX)) AS	BEGIN		DECLARE @AddingScript VARCHAR(20) = 'AddingScriptTransaction'			EXEC (@ScriptText) END
GO
/****** Object:  StoredProcedure [dbo].[VT_DBUpdatesSaveScriptExecution]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[VT_DBUpdatesSaveScriptExecution] (	@ScriptName VARCHAR(100),	@MajorReleaseNumber VARCHAR(2),	@MinorReleaseNumber VARCHAR(2),	@ReturnValue VARCHAR(200) OUTPUT) AS	BEGIN		BEGIN TRY		    INSERT INTO VT_SchemaChangeLog (MajorReleaseNumber, MinorReleaseNumber, ScriptName) 		    VALUES (@MajorReleaseNumber, @MinorReleaseNumber, @ScriptName)			IF @@ERROR = 0				        SET @ReturnValue = 'OK: Script execution successfull'		END TRY		BEGIN CATCH			SET @ReturnValue = 'ERROR: ' + ERROR_MESSAGE()		END CATCH		 END
GO
/****** Object:  UserDefinedFunction [dbo].[GetLastChildLocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetLastChildLocation]( @LocationCode varchar(8))
RETURNS  @child table ( LocationCode varchar(8), LocationName varchar(32))
AS
BEGIN

		Declare @Location Table
		(
			row int,
			locationcode varchar(8),
			LocationName varchar(32),
			ParentLocationCode varchar(8)
		)

		insert into @location
		select ROW_NUMBER() OVER(ORDER BY LocationCode DESC) AS Row,LocationCode, LocationName, ParentLocationCode from GetLocations(@LocationCode,-1);


		declare @row int
		set @row = 1

		declare @maxrow int
		select @maxrow = max(row) from @Location

		declare @Loc varchar(8)
		declare @LocName varchar(32)
		declare @shift int 

		while (@row <= @maxrow)
		begin
			select @Loc = locationCode, @LocName = LocationName from @Location where row = @row

			if not (EXISTS(SELECT * FROM @Location WHERE ParentLocationCode = @Loc))
			begin
				insert into @child values(@Loc,@LocName)
			end

			set @row =@row + 1

		end

		 return
END






GO
/****** Object:  UserDefinedFunction [dbo].[GetLocationShift]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create FUNCTION [dbo].[GetLocationShift]()
RETURNS  @Shifts table ( LocationCode varchar(8),Shift int)
AS
BEGIN

		insert into @Shifts
		select LocationCode,Shift from MstGenLocation where Shift = 1



		Declare @tmp table
		(
			rownumber int,
			location varchar(8),
			shift int
		)


		insert into @tmp
		select ROW_NUMBER() OVER(ORDER BY LocationCode DESC) AS Row,LocationCode,Shift from MstGenLocation where Shift > 1

		declare @location varchar(8)
		declare @shift int

		declare @row int
		declare @maxrow int
		set @row = 1
		select @maxrow = max(rownumber) from @tmp

		While (@row <= @maxrow)
		begin
			select @location = location from @tmp where rownumber = @row
			select @shift = shift from @tmp where rownumber = @row

			declare @shiftcount int
			set @shiftcount = 1

			while (@shiftcount <= @shift)
			begin
				insert into @Shifts values(@location,@shiftcount)
				set @shiftcount = @shiftcount + 1
			end

			set @row = @row + 1
		end

		 return
END






GO
/****** Object:  UserDefinedFunction [dbo].[GetPastWeek]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetPastWeek]
(	
	  @currentYear int,
      @currentWeek int,
      @length int
)
RETURNS  @weeks table ( year int,week int)
AS
BEGIN

		insert into @weeks
		select	Year,Week 
		from	MstGenWeek
		where	Year = @currentYear and (week < @currentWeek and week >= (@currentWeek - @length))

		declare @lengthTable int
		select  @lengthTable = COUNT(*) from @weeks

		if ( @lengthTable <> @length ) 
		begin
			set @currentYear = @currentYear - 1
			select @currentWeek = max(week) from MstGenWeek where Year = @currentYear

			insert into @weeks
			select	Year,Week 
			from	MstGenWeek
			where	Year = @currentYear and (week <= @currentWeek and week >= (@currentWeek - (@length - @lengthTable) + 1))

		end

		 return
END






GO
/****** Object:  Table [dbo].[ExeActualWorkHours]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeActualWorkHours](
	[WorkHoursDate] [date] NOT NULL,
	[IDProcess] [int] NULL,
	[ProcessIdentifier] [char](1) NULL,
	[LocationCode] [varchar](8) NULL,
	[StatusEmp] [varchar](16) NULL,
	[TimeIn] [time](7) NULL,
	[TimeOut] [time](7) NULL,
	[BreakTime] [time](7) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_EXEACTUALWORKHOURS] PRIMARY KEY NONCLUSTERED 
(
	[WorkHoursDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExeMaterialUsage]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeMaterialUsage](
	[BrandGroupCode] [varchar](20) NULL,
	[MaterialCode] [varchar](11) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[MaterialUsageDate] [datetime] NULL,
	[Sisa] [int] NULL,
	[Ambil1] [int] NULL,
	[Ambil2] [int] NULL,
	[Ambil3] [int] NULL,
	[TobFM] [int] NULL,
	[TobStem] [int] NULL,
	[TobSapon] [int] NULL,
	[UncountableWaste] [int] NULL,
	[CountableWaste] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantCKSubmit]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantCKSubmit](
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[BrandCode] [varchar](11) NULL,
	[CKNumber] [varchar](64) NULL,
	[CKDate] [datetime] NULL,
	[GTValue] [int] NULL,
	[BDSktis] [int] NULL,
	[BDP1] [int] NULL,
	[Stock] [int] NULL,
	[RowNumber] [int] NULL,
	[Number] [numeric](16, 0) NULL,
	[CKRemark] [varchar](64) NULL,
	[Status] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantProductionEntry]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantProductionEntry](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[StartDateAbsent] [datetime] NULL,
	[AbsentType] [varchar](128) NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](11) NULL,
	[ProductionEntryCode] [varchar](64) NULL,
	[ProdCapacity] [int] NULL,
	[ProdTarget] [int] NULL,
	[ProdActual] [int] NULL,
	[AbsentRemark] [varchar](8) NULL,
	[AbsentCodeEblek] [varchar](8) NULL,
	[AbsentCodePayroll] [varchar](8) NULL,
	[ProductionDate] [datetime] NULL,
	[CurrentApproval] [varchar](32) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_EXEPLANTPRODUCTIONENTRY] PRIMARY KEY CLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantProductionEntryVerification]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantProductionEntryVerification](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[KPSWeek] [int] NULL,
	[TPKValue] [int] NULL,
	[TotalTargetValue] [int] NULL,
	[TotalActualValue] [int] NULL,
	[TotalCapacityValue] [int] NULL,
	[VerifySystem] [bit] NULL,
	[VerifyManual] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[KPSYear] [int] NULL,
	[Remark] [varchar](256) NULL,
	[WorkHours] [int] NOT NULL,
 CONSTRAINT [PK_EXEPLANTPRODUCTIONENTRYVERIFICATION] PRIMARY KEY NONCLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantWorkerAbsenteeism]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantWorkerAbsenteeism](
	[StartDateAbsent] [datetime] NOT NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[AbsentType] [varchar](128) NOT NULL,
	[EndDate] [datetime] NULL,
	[SktAbsentCode] [varchar](11) NULL,
	[PayrollAbsentCode] [varchar](11) NULL,
	[ePaf] [varchar](64) NULL,
	[Attachment] [varchar](64) NULL,
	[AttachmentPath] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](64) NULL,
 CONSTRAINT [PK_EXEPLANTWORKERABSENTEEISM] PRIMARY KEY NONCLUSTERED 
(
	[StartDateAbsent] ASC,
	[EmployeeID] ASC,
	[AbsentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantWorkerAssignment]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantWorkerAssignment](
	[SourceGroupCode] [varchar](4) NOT NULL,
	[SourceUnitCode] [varchar](4) NOT NULL,
	[SourceLocationCode] [varchar](8) NOT NULL,
	[SourceProcessGroup] [varchar](16) NOT NULL,
	[DestinationGroupCode] [varchar](4) NOT NULL,
	[DestinationUnitCode] [varchar](4) NOT NULL,
	[DestinationLocationCode] [varchar](8) NOT NULL,
	[DestinationProcessGroup] [varchar](16) NOT NULL,
	[EmployeeID] [nchar](64) NOT NULL,
	[BrandCode] [varchar](11) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[DestinationGroupCodeDummy] [varchar](6) NULL,
 CONSTRAINT [PK_ExePlantWorkerAssignment] PRIMARY KEY CLUSTERED 
(
	[SourceGroupCode] ASC,
	[SourceUnitCode] ASC,
	[SourceLocationCode] ASC,
	[SourceProcessGroup] ASC,
	[DestinationGroupCode] ASC,
	[DestinationUnitCode] ASC,
	[DestinationLocationCode] ASC,
	[DestinationProcessGroup] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExeProductionEntryRelease]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeProductionEntryRelease](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[Remark] [varchar](256) NULL,
	[EblekDate] [datetime] NULL,
	[IsLocked] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_EXEPRODUCTIONENTRYRELEASE] PRIMARY KEY CLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExeTPOProduction]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeTPOProduction](
	[TPKTPOStartProductionDate] [date] NOT NULL,
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[ProdGroup] [char](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[StatusEmp] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[TPOProductionEntryCode] [varchar](255) NULL,
	[Process] [int] NULL,
	[Status] [varchar](64) NULL,
	[Absent] [varchar](6) NULL,
	[WorkerCount] [int] NULL,
	[ActualProduction] [int] NULL,
	[TPOProductionEntryDate] [datetime] NULL,
	[TPOProductionEntryStatus] [int] NULL,
	[TPOProductionEntryType] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_ExeTPOProduction] PRIMARY KEY CLUSTERED 
(
	[TPKTPOStartProductionDate] ASC,
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[ProdGroup] ASC,
	[ProcessGroup] ASC,
	[LocationCode] ASC,
	[StatusEmp] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentFulfillment]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentFulfillment](
	[FulFillmentDate] [date] NOT NULL,
	[RequestDate] [date] NULL,
	[ItemCode] [varchar](20) NULL,
	[LocationCode] [varchar](8) NULL,
	[RequestNumber] [varchar](11) NOT NULL,
	[RequestToQty] [numeric](16, 0) NULL,
	[PurchaseQty] [numeric](16, 0) NULL,
	[PurchaseNumber] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[MntcReqFullFillment] [varchar](256) NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTFULFILLMENT] PRIMARY KEY NONCLUSTERED 
(
	[FulFillmentDate] ASC,
	[RequestNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentItemConvert]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentItemConvert](
	[TransactionDate] [date] NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ItemCodeSource] [varchar](20) NOT NULL,
	[ItemCodeDestination] [varchar](20) NOT NULL,
	[SourceStock] [int] NOT NULL,
	[DestinationStock] [int] NOT NULL,
	[QtyGood] [int] NULL,
	[QtyDisposal] [int] NULL,
	[ConversionType] [bit] NOT NULL,
	[Shift] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[SourceStatus] [varchar](16) NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTITEMCONVERT] PRIMARY KEY NONCLUSTERED 
(
	[TransactionDate] ASC,
	[LocationCode] ASC,
	[ItemCodeSource] ASC,
	[ItemCodeDestination] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentItemDisposal]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentItemDisposal](
	[TransactionDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[QtyDisposal] [int] NOT NULL,
	[Shift] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTITEMDISPOSAL] PRIMARY KEY NONCLUSTERED 
(
	[TransactionDate] ASC,
	[ItemCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentMovement]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentMovement](
	[TransferDate] [date] NOT NULL,
	[ReceiveDate] [date] NULL,
	[LocationCodeSource] [varchar](8) NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[UnitCodeDestination] [varchar](4) NOT NULL,
	[LocationCodeDestination] [varchar](8) NOT NULL,
	[QtyTransfer] [int] NULL,
	[QtyReceive] [int] NULL,
	[TransferNote] [varchar](32) NULL,
	[ReceiveNote] [varchar](32) NULL,
	[Shift] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[MntcMovementCode] [varchar](256) NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTMOVEMENT] PRIMARY KEY NONCLUSTERED 
(
	[TransferDate] ASC,
	[LocationCodeSource] ASC,
	[ItemCode] ASC,
	[UnitCodeDestination] ASC,
	[LocationCodeDestination] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentQualityInspection]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentQualityInspection](
	[TransactionDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[PurchaseNumber] [varchar](64) NOT NULL,
	[RequestNumber] [varchar](20) NOT NULL,
	[DeliveryNote] [varchar](32) NULL,
	[Comments] [varchar](256) NULL,
	[Supplier] [varchar](20) NULL,
	[PreviousOutstanding] [varchar](20) NULL,
	[QTYTransit] [int] NULL,
	[QtyReceiving] [int] NULL,
	[QtyPass] [int] NULL,
	[QtyReject] [int] NULL,
	[QtyOutstanding] [int] NULL,
	[QtyReturn] [int] NULL,
	[Shift] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[MntcQICode] [varchar](256) NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTQUALITYINSPECTION] PRIMARY KEY NONCLUSTERED 
(
	[TransactionDate] ASC,
	[ItemCode] ASC,
	[LocationCode] ASC,
	[PurchaseNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentRepair]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentRepair](
	[TransactionDate] [datetime] NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[Shift] [int] NULL,
	[PreviousOutstanding] [varchar](20) NULL,
	[QtyRepairRequest] [int] NULL,
	[QtyCompletion] [int] NULL,
	[QtyOutstanding] [int] NULL,
	[QtyBadStock] [int] NULL,
	[QtyTakenByUnit] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTREPAIR] PRIMARY KEY NONCLUSTERED 
(
	[TransactionDate] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentRequest]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcEquipmentRequest](
	[RequestDate] [date] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[RequestNumber] [varchar](11) NOT NULL,
	[Qty] [numeric](16, 0) NOT NULL,
	[ApprovedQty] [numeric](16, 0) NULL,
	[FullfillmentQty] [numeric](16, 0) NULL,
	[OutstandingQty] [numeric](16, 0) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MNTCEQUIPMENTREQUEST] PRIMARY KEY NONCLUSTERED 
(
	[RequestDate] ASC,
	[ItemCode] ASC,
	[LocationCode] ASC,
	[RequestNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcInventory]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcInventory](
	[InventoryDate] [datetime] NOT NULL,
	[ItemStatus] [varchar](16) NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ItemType] [varchar](16) NULL,
	[UOM] [varchar](8) NULL,
	[BeginningStock] [int] NULL,
	[StockIn] [int] NULL,
	[StockOut] [int] NULL,
	[EndingStock] [int] NULL,
	[UnitCode] [varchar](4) NULL,
 CONSTRAINT [PK_MNTCINVENTORY] PRIMARY KEY NONCLUSTERED 
(
	[InventoryDate] ASC,
	[ItemStatus] ASC,
	[ItemCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcInventoryAdjustment]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcInventoryAdjustment](
	[AdjustmentDate] [datetime] NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[UnitCodeDestination] [varchar](4) NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[ItemStatusFrom] [varchar](16) NOT NULL,
	[ItemStatusTo] [varchar](16) NOT NULL,
	[AdjustmentValue] [int] NOT NULL,
	[AdjustmentType] [varchar](32) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_MntcInventoryAdjustment] PRIMARY KEY CLUSTERED 
(
	[AdjustmentDate] ASC,
	[LocationCode] ASC,
	[UnitCode] ASC,
	[UnitCodeDestination] ASC,
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcRepairItemUsage]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcRepairItemUsage](
	[TransactionDate] [datetime] NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ItemCodeSource] [varchar](20) NOT NULL,
	[ItemCodeDestination] [varchar](20) NOT NULL,
	[QtyUsage] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_MNTCREPAIRITEMUSAGE] PRIMARY KEY CLUSTERED 
(
	[TransactionDate] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ItemCodeSource] ASC,
	[ItemCodeDestination] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcRequestToLocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcRequestToLocation](
	[FulFillmentDate] [date] NOT NULL,
	[RequestNumber] [varchar](11) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[QtyFromLocation] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MntcRequestToLocation_1] PRIMARY KEY CLUSTERED 
(
	[FulFillmentDate] ASC,
	[RequestNumber] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstADTemp]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstADTemp](
	[UserAD] [varchar](64) NOT NULL,
	[Name] [varchar](32) NULL,
	[Email] [varchar](64) NULL,
	[StatusActive] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTADTEMP] PRIMARY KEY NONCLUSTERED 
(
	[UserAD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenBrand]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrand](
	[BrandCode] [varchar](11) NOT NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[Description] [varchar](64) NULL,
	[EffectiveDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENBRAND] PRIMARY KEY NONCLUSTERED 
(
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenBrandGroup]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandGroup](
	[BrandGroupCode] [varchar](20) NOT NULL,
	[BrandFamily] [varchar](3) NOT NULL,
	[PackType] [varchar](8) NOT NULL,
	[ClassType] [varchar](8) NOT NULL,
	[StickPerPack] [int] NOT NULL,
	[PackPerSlof] [int] NOT NULL,
	[SlofPerBal] [int] NOT NULL,
	[BalPerBox] [int] NOT NULL,
	[SKTBrandCode] [varchar](11) NOT NULL,
	[Description] [varchar](64) NULL,
	[CapacityPackage] [int] NOT NULL,
	[CigarreteWeight] [float] NULL,
	[EmpPackage] [int] NOT NULL,
	[StickPerBox] [int] NULL,
	[StickPerSlof] [int] NULL,
	[StickPerBal] [int] NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENBRANDGROUP] PRIMARY KEY NONCLUSTERED 
(
	[BrandGroupCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenBrandPackageItem]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandPackageItem](
	[ItemCode] [varchar](20) NOT NULL,
	[BrandGroupCode] [varchar](20) NOT NULL,
	[Qty] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENBRANDPACKAGEITEM] PRIMARY KEY CLUSTERED 
(
	[ItemCode] ASC,
	[BrandGroupCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenBrandPkgMapping]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandPkgMapping](
	[BrandGroupCodeSource] [varchar](20) NOT NULL,
	[BrandGroupCodeDestination] [varchar](20) NOT NULL,
	[MappingValue] [real] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENBRANDPKGMAPPING] PRIMARY KEY CLUSTERED 
(
	[BrandGroupCodeSource] ASC,
	[BrandGroupCodeDestination] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenEmpStatus]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenEmpStatus](
	[StatusEmp] [varchar](16) NOT NULL,
	[StatusIdentifier] [char](1) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENEMPSTATUS] PRIMARY KEY NONCLUSTERED 
(
	[StatusEmp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenHoliday]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenHoliday](
	[HolidayDate] [datetime] NOT NULL,
	[HolidayType] [varchar](32) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[Description] [varchar](64) NOT NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENHOLIDAY] PRIMARY KEY NONCLUSTERED 
(
	[HolidayDate] ASC,
	[HolidayType] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenList]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenList](
	[ListGroup] [varchar](32) NOT NULL,
	[ListDetail] [varchar](32) NOT NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENLIST] PRIMARY KEY NONCLUSTERED 
(
	[ListGroup] ASC,
	[ListDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenLocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenLocation](
	[LocationCode] [varchar](8) NOT NULL,
	[LocationName] [varchar](32) NOT NULL,
	[ParentLocationCode] [varchar](8) NULL,
	[CostCenter] [varchar](20) NULL,
	[ABBR] [varchar](4) NULL,
	[Shift] [int] NULL,
	[UMK] [numeric](18, 0) NULL,
	[KPPBC] [varchar](16) NULL,
	[Address] [varchar](255) NULL,
	[City] [varchar](32) NULL,
	[Region] [varchar](32) NULL,
	[Phone] [varchar](32) NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENLOCATION] PRIMARY KEY NONCLUSTERED 
(
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenLocStatus]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenLocStatus](
	[LocationCode] [varchar](8) NOT NULL,
	[StatusEmp] [varchar](16) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENLOCSTATUS] PRIMARY KEY NONCLUSTERED 
(
	[LocationCode] ASC,
	[StatusEmp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenMaterial]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenMaterial](
	[MaterialCode] [varchar](11) NOT NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[LocationCode] [varchar](8) NULL,
	[MaterialName] [varchar](32) NULL,
	[Description] [varchar](64) NULL,
	[UOM] [varchar](8) NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENMATERIAL] PRIMARY KEY NONCLUSTERED 
(
	[MaterialCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenProcess]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenProcess](
	[ProcessGroup] [varchar](16) NOT NULL,
	[ProcessIdentifier] [char](1) NOT NULL,
	[ProcessOrder] [int] NOT NULL,
	[StatusActive] [bit] NULL,
	[WIP] [bit] NOT NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENPROCESS] PRIMARY KEY NONCLUSTERED 
(
	[ProcessGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenProcessSettings]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenProcessSettings](
	[IDProcess] [int] IDENTITY(1,1) NOT NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[ProcessIdentifier] [char](1) NOT NULL,
	[StdStickPerHour] [int] NULL,
	[MinStickPerHour] [int] NULL,
	[UOMEblek] [int] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENPROCESSSETTINGS] PRIMARY KEY NONCLUSTERED 
(
	[IDProcess] ASC,
	[ProcessIdentifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenProcessSettingsLocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenProcessSettingsLocation](
	[IDProcess] [int] NOT NULL,
	[ProcessIdentifier] [char](1) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[MaxWorker] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENPROCESSSETTINGSLOCATION] PRIMARY KEY CLUSTERED 
(
	[IDProcess] ASC,
	[ProcessIdentifier] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenStandardHours]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenStandardHours](
	[DayType] [varchar](11) NOT NULL,
	[Day] [int] NOT NULL,
	[DayName] [varchar](12) NOT NULL,
	[JknHour] [int] NOT NULL,
	[Jl1Hour] [int] NOT NULL,
	[Jl2Hour] [int] NOT NULL,
	[Jl3Hour] [int] NOT NULL,
	[Jl4Hour] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENSTANDARDHOURS] PRIMARY KEY NONCLUSTERED 
(
	[DayType] ASC,
	[Day] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstGenWeek]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenWeek](
	[IDMstWeek] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Week] [int] NULL,
	[Month] [int] NULL,
	[Year] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTGENWEEK] PRIMARY KEY NONCLUSTERED 
(
	[IDMstWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstMntcConvert]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstMntcConvert](
	[ItemCodeSource] [varchar](20) NOT NULL,
	[ItemCodeDestination] [varchar](20) NOT NULL,
	[ConversionType] [bit] NULL,
	[QtyConvert] [int] NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_MSTMNTCCONVERT] PRIMARY KEY CLUSTERED 
(
	[ItemCodeSource] ASC,
	[ItemCodeDestination] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstMntcItem]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstMntcItem](
	[ItemCode] [varchar](20) NOT NULL,
	[ItemDescription] [varchar](256) NULL,
	[ItemType] [varchar](16) NULL,
	[UOM] [varchar](8) NULL,
	[PriceType] [varchar](8) NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_MSTMNTCITEM] PRIMARY KEY NONCLUSTERED 
(
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstMntcItemLocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstMntcItemLocation](
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[BufferStock] [int] NOT NULL,
	[MinOrder] [int] NOT NULL,
	[StockReadyToUse] [int] NULL,
	[StockAll] [int] NULL,
	[Remark] [varchar](256) NULL,
	[AVGWeeklyUsage] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[ItemType] [varchar](16) NULL,
 CONSTRAINT [PK_MSTMNTCITEMLOCATION] PRIMARY KEY CLUSTERED 
(
	[ItemCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantAbsentType]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantAbsentType](
	[AbsentType] [varchar](128) NOT NULL,
	[SktAbsentCode] [varchar](128) NOT NULL,
	[PayrollAbsentCode] [varchar](128) NOT NULL,
	[AlphaReplace] [varchar](128) NULL,
	[MaxDay] [int] NOT NULL,
	[ActiveInAbsent] [bit] NULL,
	[ActiveInProductionEntry] [bit] NULL,
	[PrdTargetFg] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTABSENTTYPE] PRIMARY KEY NONCLUSTERED 
(
	[AbsentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantEmpJobsDataAcv]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantEmpJobsDataAcv](
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[EmployeeName] [varchar](64) NULL,
	[JoinDate] [datetime] NULL,
	[Title_id] [varchar](12) NULL,
	[ProcessSettingsCode] [varchar](12) NULL,
	[Status] [varchar](64) NULL,
	[CCT] [varchar](64) NULL,
	[CCTDescription] [varchar](64) NULL,
	[HCC] [varchar](64) NULL,
	[LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[Loc_id] [varchar](8) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTEMPJOBSDATAACV] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantEmpJobsDataAll]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantEmpJobsDataAll](
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[EmployeeName] [varchar](64) NULL,
	[JoinDate] [datetime] NULL,
	[TitleID] [varchar](12) NULL,
	[ProcessSettingsCode] [varchar](12) NULL,
	[Status] [varchar](64) NULL,
	[CCT] [varchar](64) NULL,
	[CCTDescription] [varchar](64) NULL,
	[HCC] [varchar](64) NULL,
	[LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[Loc_id] [varchar](8) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTEMPJOBSDATAALL] PRIMARY KEY NONCLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantEmpUpd]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantEmpUpd](
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[Name] [varchar](64) NULL,
	[JoinDate] [datetime] NULL,
	[TitleID] [varchar](12) NULL,
	[ProcessSettingsCode] [varchar](12) NULL,
	[Status] [varchar](64) NULL,
	[CCT] [varchar](64) NULL,
	[CCTDescription] [varchar](64) NULL,
	[HCC] [varchar](64) NULL,
	[LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[CompletedDate] [datetime] NULL,
	[AcvSts] [bit] NULL,
	[EffDate] [datetime] NULL,
	[Action] [varchar](64) NULL,
	[RowVersion] [int] NULL,
	[FlagCompleted] [int] NULL,
	[Loc_id] [varchar](8) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTEMPUPD] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantEmpUpdTmp]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantEmpUpdTmp](
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[Name] [varchar](64) NULL,
	[JoinDate] [datetime] NULL,
	[TitleID] [varchar](12) NULL,
	[ProcessSettingsCode] [varchar](12) NULL,
	[Status] [varchar](64) NULL,
	[CCT] [varchar](64) NULL,
	[CCTDescription] [varchar](64) NULL,
	[HCC] [varchar](64) NULL,
	[LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[CompletedDate] [datetime] NULL,
	[AcvSts] [bit] NULL,
	[EffDate] [datetime] NULL,
	[Action] [varchar](64) NULL,
	[RowVersion] [int] NULL,
	[FlagCompleted] [int] NULL,
	[Loc_id] [varchar](8) NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTEMPUPDTMP] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantProductionGroup]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantProductionGroup](
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[InspectionLeader] [varchar](64) NULL,
	[Leader1] [varchar](64) NULL,
	[Leader2] [varchar](64) NULL,
	[NextGroupCode] [varchar](4) NULL,
	[Mst_UnitCode] [varchar](4) NULL,
	[Mst_LocationCode] [varchar](8) NULL,
	[Mst_ProcessGroup] [varchar](16) NULL,
	[AvailableNumbers] [varchar](512) NULL,
	[WorkerCount] [int] NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTPRODUCTIONGROUP] PRIMARY KEY CLUSTERED 
(
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstPlantUnit]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstPlantUnit](
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[UnitName] [varchar](64) NOT NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTUNIT] PRIMARY KEY NONCLUSTERED 
(
	[UnitCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstTPOFeeRate]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOFeeRate](
	[EffectiveDate] [date] NOT NULL,
	[BrandGroupCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ExpiredDate] [date] NOT NULL,
	[JKN] [numeric](10, 0) NOT NULL,
	[JL1] [numeric](10, 0) NOT NULL,
	[Jl2] [numeric](10, 0) NOT NULL,
	[Jl3] [numeric](10, 0) NOT NULL,
	[Jl4] [numeric](10, 0) NOT NULL,
	[ManagementFee] [numeric](20, 0) NOT NULL,
	[ProductivityIncentives] [numeric](20, 0) NULL,
	[MemoRef] [varchar](32) NULL,
	[MemoFile] [varchar](32) NULL,
	[MemoPath] [varchar](255) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTTPOFEERATE] PRIMARY KEY NONCLUSTERED 
(
	[EffectiveDate] ASC,
	[BrandGroupCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstTPOInfo]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOInfo](
	[LocationCode] [varchar](8) NOT NULL,
	[TPORank] [varchar](8) NULL,
	[Owner] [varchar](32) NULL,
	[VendorNumber] [varchar](18) NOT NULL,
	[VendorName] [varchar](32) NULL,
	[BankType] [varchar](32) NULL,
	[BankAccountNumber] [varchar](32) NULL,
	[BankAccountName] [varchar](32) NULL,
	[BankBranch] [varchar](32) NULL,
	[Established] [datetime] NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTTPOINFO] PRIMARY KEY CLUSTERED 
(
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstTPOPackage]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOPackage](
	[LocationCode] [varchar](8) NOT NULL,
	[BrandGroupCode] [varchar](20) NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[ExpiredDate] [date] NOT NULL,
	[Package] [real] NOT NULL,
	[MemoRef] [varchar](32) NULL,
	[MemoFile] [varchar](32) NULL,
	[MemoPath] [varchar](255) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTTPOPACKAGE] PRIMARY KEY NONCLUSTERED 
(
	[EffectiveDate] ASC,
	[BrandGroupCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstTPOProductionGroup]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOProductionGroup](
	[ProdGroup] [char](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[StatusEmp] [varchar](16) NOT NULL,
	[StatusActive] [bit] NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[WorkerCount] [int] NULL,
 CONSTRAINT [PK_MSTTPOPRODUCTIONGROUP] PRIMARY KEY CLUSTERED 
(
	[ProdGroup] ASC,
	[ProcessGroup] ASC,
	[LocationCode] ASC,
	[StatusEmp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NewsInfo]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NewsInfo](
	[NewsId] [int] IDENTITY(1,1) NOT NULL,
	[TitleID] [nvarchar](150) NULL,
	[TitleEN] [nvarchar](150) NULL,
	[ShortDescriptionID] [nvarchar](500) NULL,
	[ShortDescriptionEN] [nvarchar](500) NULL,
	[ContentID] [nvarchar](max) NULL,
	[ContentEN] [nvarchar](max) NULL,
	[Image] [nvarchar](255) NULL,
	[IsSlider] [bit] NULL,
	[IsInformatips] [bit] NULL,
	[Location] [varchar](8) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdateBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_NewsInfo] PRIMARY KEY CLUSTERED 
(
	[NewsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantAllocation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantAllocation](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[Shift] [int] NOT NULL,
	[TPKPlantStartProductionDate] [date] NOT NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTALLOCATION] PRIMARY KEY CLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[GroupCode] ASC,
	[ProcessGroup] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[BrandCode] ASC,
	[Shift] ASC,
	[TPKPlantStartProductionDate] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantGroupShift]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantGroupShift](
	[StartDate] [date] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[EndDate] [date] NOT NULL,
	[Shift] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PlanPlantGroupShift_1] PRIMARY KEY CLUSTERED 
(
	[StartDate] ASC,
	[GroupCode] ASC,
	[ProcessGroup] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantIndividualCapacityReference]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantIndividualCapacityReference](
	[BrandGroupCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[MinCapacity] [int] NULL,
	[MaxCapacity] [int] NULL,
	[AvgCapacity] [int] NULL,
	[MedCapacity] [int] NULL,
	[LatestCapacity] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTINDIVIDUALCAPACITY] PRIMARY KEY CLUSTERED 
(
	[BrandGroupCode] ASC,
	[EmployeeID] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantIndividualCapacityWorkHours]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantIndividualCapacityWorkHours](
	[BrandGroupCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[HoursCapacity3] [int] NULL,
	[HoursCapacity5] [int] NULL,
	[HoursCapacity6] [int] NULL,
	[HoursCapacity7] [int] NULL,
	[HoursCapacity8] [int] NULL,
	[HoursCapacity9] [int] NULL,
	[HoursCapacity10] [int] NULL,
	[StatusActive] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTINDIVIDUALCAPACITYWORKHOURS] PRIMARY KEY CLUSTERED 
(
	[BrandGroupCode] ASC,
	[EmployeeID] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantLineBalancing]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantLineBalancing](
	[ProductionDate] [datetime] NOT NULL,
	[IDProcess] [int] NULL,
	[ProcessIdentifier] [char](1) NULL,
	[Mst_LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[CurrentValue] [int] NULL,
	[AfterBalancing] [int] NULL,
	[SystemRecommendation] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTLINEBALANCING] PRIMARY KEY NONCLUSTERED 
(
	[ProductionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantTargetProductionKelompok]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantTargetProductionKelompok](
	[TPKPlantStartProductionDate] [date] NOT NULL,
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[Shift] [int] NOT NULL,
	[TPKCode] [varchar](256) NULL,
	[WorkerRegister] [int] NULL,
	[WorkerAvailable] [int] NULL,
	[WorkerAllocation] [int] NULL,
	[WIP1] [int] NULL,
	[WIP2] [int] NULL,
	[WIP3] [int] NULL,
	[WIP4] [int] NULL,
	[WIP5] [int] NULL,
	[WIP6] [int] NULL,
	[WIP7] [int] NULL,
	[PercentAttendance1] [real] NULL,
	[PercentAttendance2] [real] NULL,
	[PercentAttendance3] [real] NULL,
	[PercentAttendance4] [real] NULL,
	[PercentAttendance5] [real] NULL,
	[PercentAttendance6] [real] NULL,
	[PercentAttendance7] [real] NULL,
	[HistoricalCapacityWorker1] [real] NULL,
	[HistoricalCapacityWorker2] [real] NULL,
	[HistoricalCapacityWorker3] [real] NULL,
	[HistoricalCapacityWorker4] [real] NULL,
	[HistoricalCapacityWorker5] [real] NULL,
	[HistoricalCapacityWorker6] [real] NULL,
	[HistoricalCapacityWorker7] [real] NULL,
	[HistoricalCapacityGroup1] [real] NULL,
	[HistoricalCapacityGroup2] [real] NULL,
	[HistoricalCapacityGroup3] [real] NULL,
	[HistoricalCapacityGroup4] [real] NULL,
	[HistoricalCapacityGroup5] [real] NULL,
	[HistoricalCapacityGroup6] [real] NULL,
	[HistoricalCapacityGroup7] [real] NULL,
	[TargetSystem1] [real] NULL,
	[TargetSystem2] [real] NULL,
	[TargetSystem3] [real] NULL,
	[TargetSystem4] [real] NULL,
	[TargetSystem5] [real] NULL,
	[TargetSystem6] [real] NULL,
	[TargetSystem7] [real] NULL,
	[TargetManual1] [real] NULL,
	[TargetManual2] [real] NULL,
	[TargetManual3] [real] NULL,
	[TargetManual4] [real] NULL,
	[TargetManual5] [real] NULL,
	[TargetManual6] [real] NULL,
	[TargetManual7] [real] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcessWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [real] NULL,
	[TotalTargetManual] [real] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[WIPStock] [int] NULL,
 CONSTRAINT [PK_PLANPLANTTARGETPRODUCTIONKELOMPOK] PRIMARY KEY NONCLUSTERED 
(
	[TPKPlantStartProductionDate] ASC,
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[GroupCode] ASC,
	[ProcessGroup] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[BrandCode] ASC,
	[Shift] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantWIPDetail]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantWIPDetail](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[WIPCurrentValue] [int] NULL,
	[WIPPreviousValue] [int] NULL,
	[WIPStock1] [int] NULL,
	[WIPStock2] [int] NULL,
	[WIPStock3] [int] NULL,
	[WIPStock4] [int] NULL,
	[WIPStock5] [int] NULL,
	[WIPStock6] [int] NULL,
	[WIPStock7] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PlanPlantWIPDetail] PRIMARY KEY CLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[GroupCode] ASC,
	[ProcessGroup] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantWorkerBalancing]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantWorkerBalancing](
	[BalancingDate] [datetime] NOT NULL,
	[ProductionDate] [datetime] NULL,
	[EmployeeID] [varchar](64) NULL,
	[GroupCodeSource] [varchar](4) NULL,
	[UnitCodeSource] [varchar](4) NULL,
	[LocationCodeSource] [varchar](8) NULL,
	[ProcessGroupSource] [varchar](16) NULL,
	[GroupCodeDestination] [varchar](4) NULL,
	[UnitCodeDestination] [varchar](4) NULL,
	[LocationCodeDestination] [varchar](8) NULL,
	[ProcessGroupDestination] [varchar](16) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTWORKERBALANCING] PRIMARY KEY NONCLUSTERED 
(
	[BalancingDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanTargetProductionUnit]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanTargetProductionUnit](
	[ProductionStartDate] [date] NOT NULL,
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[Shift] [int] NOT NULL,
	[TPUCode] [varchar](256) NULL,
	[WorkerRegister] [int] NULL,
	[WorkerAvailable] [int] NULL,
	[WorkerAlocation] [int] NULL,
	[PercentAttendance1] [real] NULL,
	[PercentAttendance2] [real] NULL,
	[PercentAttendance3] [real] NULL,
	[PercentAttendance4] [real] NULL,
	[PercentAttendance5] [real] NULL,
	[PercentAttendance6] [real] NULL,
	[PercentAttendance7] [real] NULL,
	[HistoricalCapacityWorker1] [real] NULL,
	[HistoricalCapacityWorker2] [real] NULL,
	[HistoricalCapacityWorker3] [real] NULL,
	[HistoricalCapacityWorker4] [real] NULL,
	[HistoricalCapacityWorker5] [real] NULL,
	[HistoricalCapacityWorker6] [real] NULL,
	[HistoricalCapacityWorker7] [real] NULL,
	[HistoricalCapacityGroup1] [real] NULL,
	[HistoricalCapacityGroup2] [real] NULL,
	[HistoricalCapacityGroup3] [real] NULL,
	[HistoricalCapacityGroup4] [real] NULL,
	[HistoricalCapacityGroup5] [real] NULL,
	[HistoricalCapacityGroup6] [real] NULL,
	[HistoricalCapacityGroup7] [real] NULL,
	[TargetSystem1] [real] NULL,
	[TargetSystem2] [real] NULL,
	[TargetSystem3] [real] NULL,
	[TargetSystem4] [real] NULL,
	[TargetSystem5] [real] NULL,
	[TargetSystem6] [real] NULL,
	[TargetSystem7] [real] NULL,
	[TargetManual1] [real] NULL,
	[TargetManual2] [real] NULL,
	[TargetManual3] [real] NULL,
	[TargetManual4] [real] NULL,
	[TargetManual5] [real] NULL,
	[TargetManual6] [real] NULL,
	[TargetManual7] [real] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcessWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [real] NULL,
	[TotalTargetManual] [real] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANTARGETPRODUCTIONUNIT] PRIMARY KEY NONCLUSTERED 
(
	[ProductionStartDate] ASC,
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[BrandCode] ASC,
	[LocationCode] ASC,
	[UnitCode] ASC,
	[Shift] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanTmpWeeklyProductionPlanning]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanTmpWeeklyProductionPlanning](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[Value1] [real] NOT NULL,
	[Value2] [real] NOT NULL,
	[Value3] [real] NOT NULL,
	[Value4] [real] NOT NULL,
	[Value5] [real] NOT NULL,
	[Value6] [real] NOT NULL,
	[Value7] [real] NOT NULL,
	[Value8] [real] NOT NULL,
	[Value9] [real] NOT NULL,
	[Value10] [real] NOT NULL,
	[Value11] [real] NOT NULL,
	[Value12] [real] NOT NULL,
	[Value13] [real] NOT NULL,
	[IsValid] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANTMPWEEKLYPRODUCTIONPLANNING] PRIMARY KEY CLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[BrandCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanTPOTargetProductionKelompok]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanTPOTargetProductionKelompok](
	[TPKTPOStartProductionDate] [date] NOT NULL,
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[ProdGroup] [char](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[StatusEmp] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[TPKCode] [varchar](256) NULL,
	[WorkerRegister] [int] NULL,
	[WorkerAvailable] [int] NULL,
	[WorkerAlocation] [int] NULL,
	[WIPStock] [int] NULL,
	[PercentAttendance1] [real] NULL,
	[PercentAttendance2] [real] NULL,
	[PercentAttendance3] [real] NULL,
	[PercentAttendance4] [real] NULL,
	[PercentAttendance5] [real] NULL,
	[PercentAttendance6] [real] NULL,
	[PercentAttendance7] [real] NULL,
	[HistoricalCapacityWorker1] [real] NULL,
	[HistoricalCapacityWorker2] [real] NULL,
	[HistoricalCapacityWorker3] [real] NULL,
	[HistoricalCapacityWorker4] [real] NULL,
	[HistoricalCapacityWorker5] [real] NULL,
	[HistoricalCapacityWorker6] [real] NULL,
	[HistoricalCapacityWorker7] [real] NULL,
	[HistoricalCapacityGroup1] [real] NULL,
	[HistoricalCapacityGroup2] [real] NULL,
	[HistoricalCapacityGroup3] [real] NULL,
	[HistoricalCapacityGroup4] [real] NULL,
	[HistoricalCapacityGroup5] [real] NULL,
	[HistoricalCapacityGroup6] [real] NULL,
	[HistoricalCapacityGroup7] [real] NULL,
	[TargetSystem1] [real] NULL,
	[TargetSystem2] [real] NULL,
	[TargetSystem3] [real] NULL,
	[TargetSystem4] [real] NULL,
	[TargetSystem5] [real] NULL,
	[TargetSystem6] [real] NULL,
	[TargetSystem7] [real] NULL,
	[TargetManual1] [real] NULL,
	[TargetManual2] [real] NULL,
	[TargetManual3] [real] NULL,
	[TargetManual4] [real] NULL,
	[TargetManual5] [real] NULL,
	[TargetManual6] [real] NULL,
	[TargetManual7] [real] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcessWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [real] NULL,
	[TotalTargetManual] [real] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[WIP1] [int] NULL,
	[WIP2] [int] NULL,
	[WIP3] [int] NULL,
	[WIP4] [int] NULL,
	[WIP5] [int] NULL,
	[WIP6] [int] NULL,
	[WIP7] [int] NULL,
 CONSTRAINT [PK_PLANTPOTARGETPRODUCTIONKELOMPOK] PRIMARY KEY NONCLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[TPKTPOStartProductionDate] ASC,
	[ProdGroup] ASC,
	[ProcessGroup] ASC,
	[LocationCode] ASC,
	[StatusEmp] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanWeeklyProductionPlanning]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanWeeklyProductionPlanning](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[Value1] [real] NOT NULL,
	[Value2] [real] NOT NULL,
	[Value3] [real] NOT NULL,
	[Value4] [real] NOT NULL,
	[Value5] [real] NOT NULL,
	[Value6] [real] NOT NULL,
	[Value7] [real] NOT NULL,
	[Value8] [real] NOT NULL,
	[Value9] [real] NOT NULL,
	[Value10] [real] NOT NULL,
	[Value11] [real] NOT NULL,
	[Value12] [real] NOT NULL,
	[Value13] [real] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANWEEKLYPRODUCTIONPLANNING] PRIMARY KEY NONCLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC,
	[BrandCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductAdjustment]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductAdjustment](
	[ProducttionDate] [datetime] NOT NULL,
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[BrandCode] [varchar](11) NULL,
	[Status] [varchar](64) NOT NULL,
	[StatusValue] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_PRODUCTADJUSTMENT] PRIMARY KEY NONCLUSTERED 
(
	[ProducttionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductionCard]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductionCard](
	[RevisionType] [int] NOT NULL,
	[BrandCode] [varchar](11) NULL,
	[EmployeeID] [varchar](64) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[Production] [decimal](5, 0) NULL,
	[ProductionDate] [datetime] NULL,
	[Absent] [varchar](6) NULL,
	[UpahLain] [decimal](5, 0) NULL,
	[EblekAbsentType] [varchar](8) NULL,
	[Shift] [int] NULL,
	[WorkHours] [int] NULL,
	[Remark] [varchar](256) NULL,
	[Comments] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PRODUCTIONCARD] PRIMARY KEY NONCLUSTERED 
(
	[RevisionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TPOFeeCalculation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TPOFeeCalculation](
	[ProductionFeeType] [int] NOT NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[Order] [int] NULL,
	[OutputProduction] [float] NULL,
	[OutputBiaya] [float] NULL,
	[Calculate] [float] NULL,
 CONSTRAINT [PK_TPOFEECALCULATION] PRIMARY KEY NONCLUSTERED 
(
	[ProductionFeeType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TPOFeeCalculationPlan]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TPOFeeCalculationPlan](
	[ProductionFeeType] [int] NOT NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[Order] [int] NULL,
	[OutputProduction] [float] NULL,
	[OutputBiaya] [float] NULL,
	[Calculate] [float] NULL,
 CONSTRAINT [PK_TPOFEECALCULATIONPLAN] PRIMARY KEY NONCLUSTERED 
(
	[ProductionFeeType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TPOFeeGLAccount]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeGLAccount](
	[GLAccount] [numeric](18, 0) NOT NULL,
	[Description] [varchar](64) NULL,
 CONSTRAINT [PK_TPOFEEGLACCOUNT] PRIMARY KEY NONCLUSTERED 
(
	[GLAccount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TPOFeeHdr]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeHdr](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandCode] [varchar](11) NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[LocationCode] [varchar](8) NULL,
	[TPOFeeTempID] [int] NULL,
	[TaxtNoProd] [varchar](32) NULL,
	[TaxtNoMgmt] [varchar](32) NULL,
	[CurrentApproval] [varchar](32) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[TPOFeeCode] [varchar](64) NULL,
	[Status] [varchar](64) NULL,
	[PengirimanL1] [varchar](64) NULL,
	[PengirimanL2] [varchar](64) NULL,
	[PengirimanL3] [varchar](64) NULL,
	[PengirimanL4] [varchar](64) NULL,
	[StickPerBox] [int] NULL,
	[TPOPackageValue] [float] NULL,
	[StickPerPackage] [float] NULL,
	[TotalProdStick] [float] NULL,
	[TotalProdBox] [float] NULL,
	[TotalProdJKN] [float] NULL,
	[TotalProdJl1] [float] NULL,
	[TotalProdJl2] [float] NULL,
	[TotalProdJl3] [float] NULL,
	[TotalProdJl4] [float] NULL,
 CONSTRAINT [PK_TPOFEEHDR] PRIMARY KEY NONCLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TPOFeeHdrPlan]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeHdrPlan](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandGroupCode] [varchar](20) NULL,
	[BrandCode] [varchar](11) NULL,
	[LocationCode] [varchar](8) NULL,
	[TPOFeeTempID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[TPOFeeCode] [varchar](64) NULL,
	[Status] [varchar](64) NULL,
	[StickPerBox] [int] NULL,
	[TPOPackageValue] [float] NULL,
	[TotalProdStick] [float] NULL,
	[TotalProdBox] [float] NULL,
	[TotalProdJKN] [float] NULL,
	[TotalProdJl1] [float] NULL,
	[TotalProdJl2] [float] NULL,
	[TotalProdJl3] [float] NULL,
	[TotalProdJl4] [float] NULL,
 CONSTRAINT [PK_TPOFEEHDRPLAN] PRIMARY KEY NONCLUSTERED 
(
	[KPSYear] ASC,
	[KPSWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TPOFeeProductionDaily]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeProductionDaily](
	[FeeDate] [datetime] NOT NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[OuputSticks] [float] NULL,
	[OutputBox] [float] NULL,
	[JKN] [numeric](10, 0) NULL,
	[JL1] [numeric](10, 0) NULL,
	[Jl2] [numeric](10, 0) NULL,
	[Jl3] [numeric](10, 0) NULL,
	[Jl4] [numeric](10, 0) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
	[JKNJam] [int] NULL,
	[JL1Jam] [int] NULL,
	[JL2Jam] [int] NULL,
	[JL3Jam] [int] NULL,
	[JL4Jam] [int] NULL,
 CONSTRAINT [PK_TPOFEEPRODUCTIONDAILY] PRIMARY KEY NONCLUSTERED 
(
	[FeeDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TPOFeeProductionDailyPlan]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeProductionDailyPlan](
	[FeeDate] [datetime] NOT NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[OuputSticks] [float] NULL,
	[OutputBox] [float] NULL,
	[JKN] [numeric](10, 0) NULL,
	[JL1] [numeric](10, 0) NULL,
	[Jl2] [numeric](10, 0) NULL,
	[Jl3] [numeric](10, 0) NULL,
	[Jl4] [numeric](10, 0) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_TPOFEEPRODUCTIONDAILYPLAN] PRIMARY KEY NONCLUSTERED 
(
	[FeeDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilDelegation]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilDelegation](
	[UserADTo] [varchar](64) NOT NULL,
	[IDResponsibility] [int] NOT NULL,
	[UserADFrom] [varchar](64) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILDELEGATION] PRIMARY KEY CLUSTERED 
(
	[UserADTo] ASC,
	[IDResponsibility] ASC,
	[UserADFrom] ASC,
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilFlows]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilFlows](
	[IDFlow] [int] IDENTITY(1,1) NOT NULL,
	[FormSource] [int] NOT NULL,
	[ActionButton] [int] NULL,
	[DestinationForm] [int] NULL,
	[DestinationRole] [int] NULL,
	[MessageText] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILFLOWS] PRIMARY KEY NONCLUSTERED 
(
	[IDFlow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilFunctions]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilFunctions](
	[IDFunction] [int] IDENTITY(1,1) NOT NULL,
	[ParentIDFunction] [int] NULL,
	[FunctionName] [varchar](64) NULL,
	[FunctionType] [varchar](64) NULL,
	[FunctionDescription] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILFUNCTIONS] PRIMARY KEY NONCLUSTERED 
(
	[IDFunction] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilResponsibility]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilResponsibility](
	[IDResponsibility] [int] IDENTITY(1,1) NOT NULL,
	[IDRole] [int] NULL,
	[ResponsibilityName] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[EffectiveDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
 CONSTRAINT [PK_UTILRESPONSIBILITY] PRIMARY KEY NONCLUSTERED 
(
	[IDResponsibility] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilResponsibilityRules]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilResponsibilityRules](
	[IDResponsibility] [int] NOT NULL,
	[IDRule] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILRESPONSIBILITYRULES] PRIMARY KEY CLUSTERED 
(
	[IDResponsibility] ASC,
	[IDRule] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilRoles]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilRoles](
	[IDRole] [int] IDENTITY(1,1) NOT NULL,
	[RolesCode] [varchar](8) NULL,
	[RolesName] [varchar](32) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILROLES] PRIMARY KEY NONCLUSTERED 
(
	[IDRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilRolesFunction]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilRolesFunction](
	[IDFunction] [int] NOT NULL,
	[IDRole] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILROLESFUNCTION] PRIMARY KEY CLUSTERED 
(
	[IDFunction] ASC,
	[IDRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilRules]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilRules](
	[IDRule] [int] IDENTITY(1,1) NOT NULL,
	[RulesName] [varchar](32) NULL,
	[Location] [varchar](64) NULL,
	[Unit] [varchar](64) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILRULES] PRIMARY KEY NONCLUSTERED 
(
	[IDRule] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilTransactionLogs]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilTransactionLogs](
	[TransactionCode] [varchar](128) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[IDFlow] [int] NULL,
	[Comments] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILTRANSACTIONLOGS] PRIMARY KEY NONCLUSTERED 
(
	[TransactionCode] ASC,
	[TransactionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UtilUsersResponsibility]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UtilUsersResponsibility](
	[IDResponsibility] [int] IDENTITY(1,1) NOT NULL,
	[UserAD] [varchar](64) NOT NULL,
	[EffectiveDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_UTILUSERSRESPONSIBILITY] PRIMARY KEY NONCLUSTERED 
(
	[IDResponsibility] ASC,
	[UserAD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VT_SchemaChangeLog]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VT_SchemaChangeLog](
	[SchemaId] [int] IDENTITY(1,1) NOT NULL,
	[MajorReleaseNumber] [varchar](10) NULL,
	[MinorReleaseNumber] [varchar](10) NULL,
	[ScriptName] [varchar](50) NOT NULL,
	[ScriptDateApplied] [datetime] NULL,
 CONSTRAINT [pk_VT_SchemaChangeLog] PRIMARY KEY CLUSTERED 
(
	[SchemaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[MntcInventoryDeltaView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
Create View [dbo].[MntcInventoryDeltaView] as
SELECT InventoryDate, LocationCode, UnitCode, ItemStatus,ItemCode,
Sum(DeltaStaw) as DBeginningStock ,
sum(DeltaStockIn) as DStockIn,
sum(DeltaStockOut) as DStockOut,
sum(DeltaStak) as DEndingStock
from (
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus, b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
sum(SourceStock) as DeltaStockOut, 
-sum(SourceStock) as DeltaStak  
  FROM [MntcEquipmentItemConvert] a,
  [MntcInventory] b
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and a.SourceStatus=b.ItemStatus
  and a.ItemCodeSource=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus, b.ItemCode,
-sum(SourceStock) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut,
-sum(SourceStock) as DeltaStak  
  FROM [MntcEquipmentItemConvert] a,
  [MntcInventory] b
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and a.SourceStatus=b.ItemStatus
  and a.ItemCodeSource=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate,b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode, 
0 DeltaStaw,
sum(QtyGood) as DeltaStockIn,
0 DeltaStockOut, 
sum(a.QtyGood) as DeltaStak
FROM [MntcEquipmentItemConvert] a,
  [MntcInventory] b
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and b.ItemStatus='READY TO USE'
  and a.ItemCodeDestination=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate,b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode, 
+sum(QtyGood) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
+sum(a.QtyGood) as DeltaStak
FROM [MntcEquipmentItemConvert] a,
  [MntcInventory] b
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and b.ItemStatus='READY TO USE'
  and a.ItemCodeDestination=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
+sum(a.QtyDisposal) as DeltaStockOut, 
-sum(a.QtyDisposal) as DeltaStak
  FROM [MntcEquipmentItemDisposal] a,
  [MntcInventory] b
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and b.ItemStatus='BAD STOCK'
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
-sum(a.QtyDisposal)as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
-sum(a.QtyDisposal) as DeltaStak
  FROM [MntcEquipmentItemDisposal] a,
  [MntcInventory] b
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and b.UnitCode='MTNC'
  and b.ItemStatus='BAD STOCK'
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStak,
0 DeltaStockIn,
sum(QtyTransfer) as DeltaStockOut, 
-sum(QtyTransfer) as DeltaStaw
  FROM [MntcEquipmentMovement] a,
  [MntcInventory] b    
  where a.TransferDate=b.InventoryDate
  and  a.LocationCodeSource=b.LocationCode
  and ((a.LocationCodeSource like 'REG%' and b.UnitCode='WHSE' and b.ItemStatus='IN TRANSIT')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode='MTNC' and b.ItemStatus='READY TO USE'))
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
-sum(QtyTransfer) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
-sum(QtyTransfer) as DeltaStaw
  FROM [MntcEquipmentMovement] a,
  [MntcInventory] b    
  where a.TransferDate<b.InventoryDate
  and  a.LocationCodeSource=b.LocationCode
  and ((a.LocationCodeSource like 'REG%' and b.UnitCode='WHSE' and b.ItemStatus='IN TRANSIT')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode='MTNC' and b.ItemStatus='READY TO USE'))
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyReceive) as DeltaStockIn,
0 DeltaStockOut, 
sum(QtyReceive) as DeltaStak
  FROM [MntcEquipmentMovement] a,
  [MntcInventory] b    
  where a.ReceiveDate=b.InventoryDate
  and  a.LocationCodeDestination=b.LocationCode
  and  a.UnitCodeDestination=b.UnitCode 
  and ((a.LocationCodeSource like 'REG%' and b.UnitCode='WHSE' and b.ItemStatus='IN TRANSIT')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode='MTNC' and b.ItemStatus='READY TO USE')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode<>'MTNC' and b.ItemStatus='ON USED'))
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyReceive) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyReceive) as DeltaStak
  FROM [MntcEquipmentMovement] a,
  [MntcInventory] b    
  where a.ReceiveDate<b.InventoryDate
  and  a.LocationCodeDestination=b.LocationCode
  and  a.UnitCodeDestination=b.UnitCode 
  and ((a.LocationCodeSource like 'REG%' and b.UnitCode='WHSE' and b.ItemStatus='IN TRANSIT')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode='MTNC' and b.ItemStatus='READY TO USE')
  or (a.LocationCodeSource like 'ID%' and b.UnitCode<>'MTNC' and b.ItemStatus='ON USED'))
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
sum(QtyReceiving) as DeltaStockOut, 
-sum(QtyReceiving) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='IN TRANSIT' 
  and a.ItemCode=b.ItemCode
  and b.BeginningStock>0
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
-sum(QtyReceiving) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
-sum(QtyReceiving) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='IN TRANSIT' 
  and a.ItemCode=b.ItemCode
  and b.BeginningStock>0
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyReceiving) as  DeltaStockIn,
sum(QtyPass+QtyReject+QtyReturn) as DeltaStockOut, 
sum(QtyReceiving-(QtyPass+QtyReject+QtyReturn)) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='ON QUALITY INSPECTION' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyReceiving-(QtyPass+QtyReject+QtyReturn)) DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyReceiving-(QtyPass+QtyReject+QtyReturn)) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='ON QUALITY INSPECTION' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyPass) as  DeltaStockIn,
0 DeltaStockOut, 
sum(QtyPass) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='READY TO USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyPass) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyPass) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='READY TO USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyReject) as  DeltaStockIn,
0 DeltaStockOut, 
sum(QtyReject) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='BAD STOCK' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyReject) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyReject) as DeltaStak
  FROM [MntcEquipmentQualityInspection] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  b.UnitCode='MTNC'
  and  b.ItemStatus='BAD STOCK' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyRepairRequest) as DeltaStockIn,
sum(QtyCompletion+QtyBadStock) as DeltaStockOut, 
sum(QtyRepairRequest-(QtyCompletion+QtyBadStock)) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='ON REPAIR' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyRepairRequest-(QtyCompletion+QtyBadStock)) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyRepairRequest-(QtyCompletion+QtyBadStock)) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='ON REPAIR' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyCompletion) as DeltaStockIn,
sum(QtyTakenByUnit) as DeltaStockOut, 
sum(QtyCompletion-QtyTakenByUnit) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='READY TO USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyCompletion-QtyTakenByUnit) as  DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyCompletion-QtyTakenByUnit) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='READY TO USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyTakenByUnit) as DeltaStockIn,
sum(QtyRepairRequest) as DeltaStockOut, 
sum(QtyTakenByUnit-QtyRepairRequest) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='ON USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyTakenByUnit-QtyRepairRequest) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyTakenByUnit-QtyRepairRequest) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='ON USE' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(QtyBadStock) as DeltaStockIn,
0 DeltaStockOut, 
sum(QtyBadStock) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='BAD STOCK' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(QtyBadStock) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(QtyBadStock) as DeltaStak
  FROM [MntcEquipmentRepair] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='BAD STOCK' 
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
sum(AdjustmentValue) as DeltaStockOut, 
-sum(AdjustmentValue) as DeltaStak
  FROM [MntcInventoryAdjustment] a,
  [MntcInventory] b        
  where a.AdjustmentDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  a.ItemStatusFrom=b.ItemStatus
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
sum(AdjustmentValue) as DeltaStockOut, 
-sum(AdjustmentValue) as DeltaStak
  FROM [MntcInventoryAdjustment] a,
  [MntcInventory] b        
  where a.AdjustmentDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  a.ItemStatusFrom=b.ItemStatus
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL  
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
-sum(AdjustmentValue) as  DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
-sum(AdjustmentValue) as DeltaStak
  FROM [MntcInventoryAdjustment] a,
  [MntcInventory] b        
  where a.AdjustmentDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  a.ItemStatusFrom=b.ItemStatus
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
sum(AdjustmentValue) as  DeltaStockIn,
0 DeltaStockOut, 
sum(AdjustmentValue) as DeltaStak
  FROM [MntcInventoryAdjustment] a,
  [MntcInventory] b        
  where a.AdjustmentDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  a.ItemStatusTo=b.ItemStatus
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL  
SELECT  b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
sum(AdjustmentValue) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
sum(AdjustmentValue) as DeltaStak
  FROM [MntcInventoryAdjustment] a,
  [MntcInventory] b        
  where a.AdjustmentDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  a.ItemStatusTo=b.ItemStatus
  and a.ItemCode=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
0 DeltaStaw,
0 DeltaStockIn,
sum(QtyUsage) as DeltaStockOut, 
-sum(QtyUsage) as DeltaStak
  FROM [MntcRepairItemUsage] a,
  [MntcInventory] b        
  where a.TransactionDate=b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='READY TO USE'
  and a.ItemCodeDestination=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
UNION ALL
SELECT b.InventoryDate, b.LocationCode,b.UnitCode,
b.ItemStatus,b.ItemCode,
-sum(QtyUsage) as DeltaStaw,
0 DeltaStockIn,
0 DeltaStockOut, 
-sum(QtyUsage) as DeltaStak
  FROM [MntcRepairItemUsage] a,
  [MntcInventory] b        
  where a.TransactionDate<b.InventoryDate
  and  a.LocationCode=b.LocationCode
  and  a.UnitCode=b.UnitCode
  and  b.ItemStatus='READY TO USE'
  and a.ItemCodeDestination=b.ItemCode
group by b.InventoryDate, b.LocationCode,b.UnitCode,b.ItemStatus, b.ItemCode  
) as x  group by InventoryDate, LocationCode, UnitCode, ItemStatus, ItemCode  

GO
/****** Object:  View [dbo].[MaintenanceExecutionInventoryView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MaintenanceExecutionInventoryView]
AS
--SELECT 'ID21' LocationCode,
--       'GLK-003' AS [Item Code],
--       0 AS StawIT,
--       0 AS InIT,
--       0 AS OutIT,
--       0 AS StackIT,

--       0 AS StawQI,
--       0 AS InQI,
--       0 AS OutQI,
--       0 AS StackQI,

--       0 AS StawReady,
--       0 AS InReady,
--       0 AS OutReady,
--       0 AS StackReady,

--       0 AS StawOU,
--       0 AS InOU,
--       0 AS OutOU,
--       0 AS StackOU,

--       0 AS StawOR,
--       0 AS InOR,
--       0 AS OutOR,
--       0 AS StackOR,

--       0 AS StawBS,
--       0 AS InBS,
--       0 AS OutBS,
--       0 AS StackBS;

SELECT ItemCode AS [Item Code], LocationCode,
       SUM(CASE WHEN midv.itemstatus = 'IN TRANSIT' THEN midv.DBeginningStock ELSE 0 END) AS StawIT,
       SUM(CASE WHEN midv.itemstatus = 'IN TRANSIT' THEN midv.DStockIn ELSE 0 END) AS InIT,
       SUM(CASE WHEN midv.itemstatus = 'IN TRANSIT' THEN midv.DStockOut ELSE 0 END) AS OutIT,
       SUM(CASE WHEN midv.itemstatus = 'IN TRANSIT' THEN midv.DEndingStock ELSE 0 END) AS StackIT,
	  SUM(CASE WHEN midv.itemstatus = 'ON QUALITY INSPECTION' THEN midv.DBeginningStock ELSE 0 END) AS StawQI,
       SUM(CASE WHEN midv.itemstatus = 'ON QUALITY INSPECTION' THEN midv.DStockIn ELSE 0 END) AS InQI,
       SUM(CASE WHEN midv.itemstatus = 'ON QUALITY INSPECTION' THEN midv.DStockOut ELSE 0 END) AS OutQI,
       SUM(CASE WHEN midv.itemstatus = 'ON QUALITY INSPECTION' THEN midv.DEndingStock ELSE 0 END) AS StackQI,
	  SUM(CASE WHEN midv.itemstatus = 'READY TO USE' THEN midv.DBeginningStock ELSE 0 END) AS StawReady,
       SUM(CASE WHEN midv.itemstatus = 'READY TO USE' THEN midv.DStockIn ELSE 0 END) AS InReady,
       SUM(CASE WHEN midv.itemstatus = 'READY TO USE' THEN midv.DStockOut ELSE 0 END) AS OutReady,
       SUM(CASE WHEN midv.itemstatus = 'READY TO USE' THEN midv.DEndingStock ELSE 0 END) AS StackReady,
	  SUM(CASE WHEN midv.itemstatus = 'ON USED' THEN midv.DBeginningStock ELSE 0 END) AS StawOU,
       SUM(CASE WHEN midv.itemstatus = 'ON USED' THEN midv.DStockIn ELSE 0 END) AS InOU,
       SUM(CASE WHEN midv.itemstatus = 'ON USED' THEN midv.DStockOut ELSE 0 END) AS OutOU,
       SUM(CASE WHEN midv.itemstatus = 'ON USED' THEN midv.DEndingStock ELSE 0 END) AS StackOU,
	  SUM(CASE WHEN midv.itemstatus = 'ON REPAIR' THEN midv.DBeginningStock ELSE 0 END) AS StawOR,
       SUM(CASE WHEN midv.itemstatus = 'ON REPAIR' THEN midv.DStockIn ELSE 0 END) AS InOR,
       SUM(CASE WHEN midv.itemstatus = 'ON REPAIR' THEN midv.DStockOut ELSE 0 END) AS OutOR,
       SUM(CASE WHEN midv.itemstatus = 'ON REPAIR' THEN midv.DEndingStock ELSE 0 END) AS StackOR,
	  SUM(CASE WHEN midv.itemstatus = 'BAD STOCK' THEN midv.DBeginningStock ELSE 0 END) AS StawBS,
       SUM(CASE WHEN midv.itemstatus = 'BAD STOCK' THEN midv.DStockIn ELSE 0 END) AS InBS,
       SUM(CASE WHEN midv.itemstatus = 'BAD STOCK' THEN midv.DStockOut ELSE 0 END) AS OutBS,
       SUM(CASE WHEN midv.itemstatus = 'BAD STOCK' THEN midv.DEndingStock ELSE 0 END) AS StackBS
FROM MntcInventoryDeltaView midv
GROUP BY midv.InventoryDate,
         midv.LocationCode,
         midv.UnitCode,
         midv.ItemCode;

GO
/****** Object:  View [dbo].[MntcInventoryAll]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
create view [dbo].[MntcInventoryAll] as
SELECT a.InventoryDate,a.ItemStatus,a.ItemCode,a.LocationCode,a.ItemType,
BeginningStock,StockIn,StockOut,EndingStock
FROM [MntcInventory] a
WHERE NOT EXISTS (SELECT 1 FROM [MntcInventoryDeltaView]
WHERE InventoryDate=a.InventoryDate
and LocationCode=a.LocationCode 
and UnitCode=a.UnitCode and ItemStatus=a.ItemStatus and ItemCode=a.ItemCode)
UNION ALL
SELECT a.InventoryDate,a.ItemStatus,a.ItemCode,a.LocationCode,a.ItemType,
BeginningStock+DBeginningStock as BeginningStock,
StockIn+DStockIn as StockIn,
StockOut+DStockOut as StockOut,
EndingStock+DEndingStock as  EndingStock 
  FROM [MntcInventory] a, [MntcInventoryDeltaView] b 
WHERE b.InventoryDate=a.InventoryDate
and b.LocationCode=a.LocationCode 
and b.UnitCode=a.UnitCode 
and b.ItemStatus=a.ItemStatus 
and b.ItemCode=a.ItemCode

GO
/****** Object:  UserDefinedFunction [dbo].[GetEquipmentRequirementReport]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* Alter function for extending the parameters (add @date parameter) */
CREATE FUNCTION [dbo].[GetEquipmentRequirementReport] 
(	
	@locationcode AS VARCHAR(20),
	@brandGroupCode AS VARCHAR(20),
	@userEntryPackage AS REAL,
	@date AS DATE
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		mgbpi.BrandGroupCode,
		mgbpi.ItemCode,
		mmi.ItemDescription, 
		SUM(COALESCE(mi.EndingStock,0)) AS realStockQty,
		mgbpi.Qty,
		mtp.Package * mgbpi.Qty AS currentQty,
		@userEntryPackage * mgbpi.Qty AS calculateQty,
		((mtp.Package * mgbpi.Qty)-(@userEntryPackage * mgbpi.Qty)) AS varianceQty
	FROM MstGenBrandPackageItem mgbpi 
	LEFT JOIN MntcInventory mi ON mi.ItemCode = mgbpi.ItemCode
	INNER JOIN MstMntcItem mmi ON mmi.ItemCode = mgbpi.ItemCode
	INNER JOIN MstTPOPackage mtp ON mtp.BrandGroupCode = mgbpi.BrandGroupCode
	WHERE 
		mgbpi.BrandGroupCode=@brandGroupCode AND
		mi.ItemStatus IN ('Ready To Use','On Used') AND
		mi.InventoryDate = CAST(@date as DATE) AND
		mi.LocationCode = @locationcode AND
		mtp.LocationCode = @locationcode
	GROUP BY
		mgbpi.BrandGroupCode,
		mgbpi.ItemCode,
		mmi.ItemDescription, 
		mgbpi.Qty,
		mtp.Package
)

GO
/****** Object:  UserDefinedFunction [dbo].[GetLocations]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetLocations]
(	
	@locationcode as varchar(20),
	@level as int
)
RETURNS TABLE 
AS
RETURN 
(
	WITH LocationDetail (LocationCode
					  ,LocationName
					  ,CostCenter
					  ,ABBR
					  ,Shift
					  ,ParentLocationCode
					  ,UMK
					  ,KPPBC
					  ,Address
					  ,City
					  ,Region
					  ,Phone
					  ,StatusActive
					  ,Remark
					  ,CreatedDate
					  ,CreatedBy
					  ,UpdatedDate
					  ,UpdatedBy,Level)
AS
(
	select LocationCode
		  ,LocationName
		  ,CostCenter
		  ,ABBR
		  ,Shift
		  ,ParentLocationCode
		  ,UMK
		  ,KPPBC
		  ,Address
		  ,City
		  ,Region
		  ,Phone
		  ,StatusActive
		  ,Remark
		  ,CreatedDate
		  ,CreatedBy
		  ,UpdatedDate
		  ,UpdatedBy
		  ,0 as Level from MstGenLocation where LocationCode = @locationcode and StatusActive = 1
	union all

	select l.LocationCode
		  ,l.LocationName
		  ,l.CostCenter
		  ,l.ABBR
		  ,l.Shift
		  ,l.ParentLocationCode
		  ,l.UMK
		  ,l.KPPBC
		  ,l.Address
		  ,l.City
		  ,l.Region
		  ,l.Phone
		  ,l.StatusActive
		  ,l.Remark
		  ,l.CreatedDate
		  ,l.CreatedBy
		  ,l.UpdatedDate
		  ,l.UpdatedBy, ld.Level + 1 from MstGenLocation l
		inner join LocationDetail as ld on l.ParentLocationCode = ld.LocationCode
		where l.StatusActive = 1
)

	select LocationCode
		  ,LocationName
		  ,CostCenter
		  ,ABBR
		  ,Shift
		  ,ParentLocationCode
		  ,UMK
		  ,KPPBC
		  ,Address
		  ,City
		  ,Region
		  ,Phone
		  ,StatusActive
		  ,Remark
		  ,CreatedDate
		  ,CreatedBy
		  ,UpdatedDate
		  ,UpdatedBy
	from  LocationDetail 
	where level <=  case 
						when @level = -1 then (select max(level) from LocationDetail)
						else @level
					end


)







GO
/****** Object:  UserDefinedFunction [dbo].[GetLocationsByLevel]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetLocationsByLevel]
(	
	@sourcelocationcode as varchar(20),
	@level as int
)
RETURNS TABLE 
AS
RETURN 
(
	WITH LocationDetail (LocationCode
					  ,LocationName
					  ,CostCenter
					  ,ABBR
					  ,Shift
					  ,ParentLocationCode
					  ,UMK
					  ,KPPBC
					  ,Address
					  ,City
					  ,Region
					  ,Phone
					  ,StatusActive
					  ,Remark
					  ,CreatedDate
					  ,CreatedBy
					  ,UpdatedDate
					  ,UpdatedBy,Level)
AS
(
	select LocationCode
		  ,LocationName
		  ,CostCenter
		  ,ABBR
		  ,Shift
		  ,ParentLocationCode
		  ,UMK
		  ,KPPBC
		  ,Address
		  ,City
		  ,Region
		  ,Phone
		  ,StatusActive
		  ,Remark
		  ,CreatedDate
		  ,CreatedBy
		  ,UpdatedDate
		  ,UpdatedBy
		  ,0 as Level from MstGenLocation where LocationCode = @sourcelocationcode and StatusActive = 1
	union all

	select l.LocationCode
		  ,l.LocationName
		  ,l.CostCenter
		  ,l.ABBR
		  ,l.Shift
		  ,l.ParentLocationCode
		  ,l.UMK
		  ,l.KPPBC
		  ,l.Address
		  ,l.City
		  ,l.Region
		  ,l.Phone
		  ,l.StatusActive
		  ,l.Remark
		  ,l.CreatedDate
		  ,l.CreatedBy
		  ,l.UpdatedDate
		  ,l.UpdatedBy, ld.Level + 1 from MstGenLocation l
		inner join LocationDetail as ld on l.ParentLocationCode = ld.LocationCode
		where l.StatusActive = 1
)

	select LocationCode
		  ,LocationName
		  ,CostCenter
		  ,ABBR
		  ,Shift
		  ,ParentLocationCode
		  ,UMK
		  ,KPPBC
		  ,Address
		  ,City
		  ,Region
		  ,Phone
		  ,StatusActive
		  ,Remark
		  ,CreatedDate
		  ,CreatedBy
		  ,UpdatedDate
		  ,UpdatedBy
	from  LocationDetail 
	where level =  @level


)




GO
/****** Object:  View [dbo].[BrandCodeByLocationView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BrandCodeByLocationView]
AS
SELECT        B.BrandCode, BG.BrandGroupCode, PSL.LocationCode
FROM            dbo.MstGenBrand AS B INNER JOIN
                         dbo.MstGenBrandGroup AS BG ON BG.BrandGroupCode = B.BrandGroupCode INNER JOIN
                         dbo.MstGenProcessSettings AS PS ON PS.BrandGroupCode = BG.BrandGroupCode INNER JOIN
                         dbo.MstGenProcessSettingsLocation AS PSL ON PSL.IDProcess = PS.IDProcess
GROUP BY B.BrandCode, BG.BrandGroupCode, PSL.LocationCode






GO
/****** Object:  View [dbo].[EquipmentRequestView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[EquipmentRequestView]
AS
     SELECT RequestDate,
            ItemCode,
            ItemDescription,
            [Ready to Use] AS ReadyToUse,
            [On Used] AS OnUse,
            [On Repair] AS OnRepair,
            TotalQty,
            ApprovedQty,
            LocationCode,
            RequestNumber,
            UpdatedBy
     FROM( 
           SELECT ER.RequestDate,
                  ER.ItemCode,
                  MI.ItemDescription,
                  I.EndingStock,
                  I.ItemStatus,
                  ER.Qty AS TotalQty,
                  ER.ApprovedQty,
                  ER.LocationCode,
                  ER.RequestNumber,
                  ER.UpdatedBy
           FROM MntcEquipmentRequest AS ER
                INNER JOIN MstMntcItem AS MI ON MI.ItemCode = ER.ItemCode
                LEFT JOIN(
                    SELECT s1.InventoryDate,
                           s1.ItemCode,
                           s1.LocationCode,
                           s1.ItemStatus,
                           s1.EndingStock
                    FROM MntcInventory s1
                         INNER JOIN(
                             SELECT MAX(InventoryDate) LAST_UPDATE_DATE_TIME,
                                    ItemCode,
                                    LocationCode,
                                    ItemStatus
                             FROM MntcInventory
                             GROUP BY ItemCode,
                                      LocationCode,
                                      ItemStatus ) s2 ON s1.ItemCode = s2.ItemCode
                                                     AND s1.LocationCode = s2.LocationCode
                                                     AND s1.InventoryDate = s2.LAST_UPDATE_DATE_TIME
                                                     AND s1.ItemStatus = s2.ItemStatus ) AS I ON ER.ItemCode = I.ItemCode
                                                                                             AND ER.LocationCode = I.LocationCode ) AS A PIVOT( MAX(EndingStock) FOR ItemStatus IN( [Ready to Use],
                                                                                                                                                                                    [On Used],
                                                                                                                                                                                    [On Repair] )) AS EquipmentRequestView;




GO
/****** Object:  View [dbo].[EquipmentRequirementView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[EquipmentRequirementView]
AS
SELECT        mgps.BrandGroupCode, mgpsl.LocationCode, mtp.Package
FROM            dbo.MstGenProcessSettingsLocation AS mgpsl INNER JOIN
                         dbo.MstGenProcessSettings AS mgps ON mgps.IDProcess = mgpsl.IDProcess INNER JOIN
                         dbo.MstGenLocation AS mgl ON mgl.LocationCode = mgpsl.LocationCode INNER JOIN
                         dbo.MstTPOPackage AS mtp ON mtp.BrandGroupCode = mgps.BrandGroupCode AND mtp.LocationCode = mgpsl.LocationCode
WHERE        (mgl.StatusActive = 1)
GROUP BY mgps.BrandGroupCode, mgpsl.LocationCode, mtp.Package


GO
/****** Object:  View [dbo].[InventoryByStatusView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[InventoryByStatusView]
AS
SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY ItemCode DESC), -1) AS RowID,
       ItemCode,
       LocationCode,
       [Ready to Use] AS ReadyToUse,
       [On Use] AS OnUse,
       [On Repair] AS OnRepair
FROM( 
      SELECT s1.ItemCode,
             s1.LocationCode,
             s1.ItemStatus,
             s1.EndingStock
      FROM MntcInventory s1
           INNER JOIN(
               SELECT MAX(InventoryDate) LAST_UPDATE_DATE_TIME,
                      ItemCode,
                      LocationCode,
                      ItemStatus
               FROM MntcInventory
               GROUP BY ItemCode,
                        LocationCode,
                        ItemStatus ) s2 ON s1.ItemCode = s2.ItemCode
                                       AND s1.LocationCode = s2.LocationCode
                                       AND s1.InventoryDate = s2.LAST_UPDATE_DATE_TIME
                                       AND s1.ItemStatus = s2.ItemStatus ) AS A PIVOT( MAX(EndingStock) FOR ItemStatus IN( [Ready to Use],
                                                                                                                           [On Use],
                                                                                                                           [On Repair] )) AS InventoryByStatusView;






GO
/****** Object:  View [dbo].[MaintenanceEquipmentFulfillmentDetailView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MaintenanceEquipmentFulfillmentDetailView]
AS
     --SELECT mi.ItemCode,
     --       mi.LocationCode,
     --       mi.EndingStock,
     --       mrtl.QtyFromLocation AS Quantity,
     --       mi.ItemStatus,
     --       mi.InventoryDate
     --FROM dbo.MntcInventory AS mi
     --     LEFT OUTER JOIN dbo.MntcRequestToLocation AS mrtl ON mrtl.LocationCode = mi.LocationCode;

     SELECT mmil.ItemCode,
            mmil.LocationCode,
            mi.EndingStock,
            mrtl.QtyFromLocation AS Quantity,
            mi.ItemStatus,
            mi.InventoryDate
     FROM dbo.MstMntcItemLocation mmil
          LEFT OUTER JOIN dbo.MntcInventory mi ON mi.ItemCode = mmil.ItemCode
                                              AND mi.LocationCode = mmil.LocationCode
          LEFT OUTER JOIN dbo.MntcRequestToLocation AS mrtl ON mrtl.LocationCode = mmil.LocationCode;

GO
/****** Object:  View [dbo].[MaintenanceEquipmentFulfillmentView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MaintenanceEquipmentFulfillmentView]

AS
SELECT        table1.LocationCode, table1.RequestDate, table1.RequestNumber, table1.CreatedBy, table1.FulFillmentDate, table1.ItemCode, table1.ItemDescription, 
                         tblInventoriReadyToUse.EndingStock AS ReadyToUse, tblInventoriOnUse.EndingStock AS OnUse, tblInventoriOnRepair.EndingStock AS OnRepair, 
                         table1.Qty AS RequestedQuantity, table1.ApprovedQty, table1.RequestToQty, table1.PurchaseNumber, table1.UpdatedDate
FROM            (SELECT        dbo.MntcEquipmentFulfillment.LocationCode, dbo.MntcEquipmentFulfillment.RequestDate, dbo.MntcEquipmentFulfillment.RequestNumber, 
                                                    dbo.MntcEquipmentFulfillment.CreatedBy, dbo.MntcEquipmentFulfillment.FulFillmentDate, dbo.MntcEquipmentFulfillment.ItemCode, 
                                                    dbo.MstMntcItem.ItemDescription, dbo.MntcEquipmentRequest.Qty, dbo.MntcEquipmentRequest.ApprovedQty, 
                                                    dbo.MntcEquipmentFulfillment.RequestToQty, dbo.MntcEquipmentFulfillment.PurchaseNumber, 
                                                    dbo.MntcEquipmentFulfillment.UpdatedDate
                          FROM            dbo.MntcEquipmentFulfillment INNER JOIN
                                                    dbo.MstMntcItem ON dbo.MntcEquipmentFulfillment.ItemCode = dbo.MstMntcItem.ItemCode INNER JOIN
                                                    dbo.MntcEquipmentRequest ON dbo.MntcEquipmentFulfillment.RequestDate = dbo.MntcEquipmentRequest.RequestDate AND 
                                                    dbo.MntcEquipmentFulfillment.ItemCode = dbo.MntcEquipmentRequest.ItemCode AND 
                                                    dbo.MntcEquipmentFulfillment.LocationCode = dbo.MntcEquipmentRequest.LocationCode AND 
                                                    dbo.MntcEquipmentFulfillment.RequestNumber = dbo.MntcEquipmentRequest.RequestNumber) AS table1 INNER JOIN
                             (SELECT        TOP (1) InventoryDate, ItemStatus, ItemCode, LocationCode, UnitCode, ItemType, UOM, BeginningStock, StockIn, StockOut, EndingStock
                               FROM            dbo.MntcInventory
                               WHERE        (ItemStatus = 'Ready to Use')
                               ORDER BY InventoryDate DESC) AS tblInventoriReadyToUse ON table1.ItemCode = tblInventoriReadyToUse.ItemCode AND 
                         table1.LocationCode = tblInventoriReadyToUse.LocationCode INNER JOIN
                             (SELECT        TOP (1) InventoryDate, ItemStatus, ItemCode, LocationCode, UnitCode, ItemType, UOM, BeginningStock, StockIn, StockOut, EndingStock
                               FROM            dbo.MntcInventory AS MntcInventory_2
                               WHERE        (ItemStatus = 'On Used')
                               ORDER BY InventoryDate DESC) AS tblInventoriOnUse ON table1.ItemCode = tblInventoriOnUse.ItemCode AND 
                         table1.LocationCode = tblInventoriOnUse.LocationCode INNER JOIN
                             (SELECT        TOP (1) InventoryDate, ItemStatus, ItemCode, LocationCode, UnitCode, ItemType, UOM, BeginningStock, StockIn, StockOut, EndingStock
                               FROM            dbo.MntcInventory AS MntcInventory_1
                               WHERE        (ItemStatus = 'On Repair')
                               ORDER BY InventoryDate DESC) AS tblInventoriOnRepair ON table1.ItemCode = tblInventoriOnRepair.ItemCode AND 
                         table1.LocationCode = tblInventoriOnRepair.LocationCode

GO
/****** Object:  View [dbo].[MaintenanceEquipmentStockView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* CREATE MAINTENANCE EQUIPMENT STOCK VIEW */
CREATE VIEW [dbo].[MaintenanceEquipmentStockView]
AS
SELECT 
	ISNULL(ROW_NUMBER() OVER (ORDER BY ItemCode DESC), - 1) AS RowID, 
	InventoryDate,
	ItemCode, 
	ItemDescription,
	LocationCode, 
	UnitCode,
	COALESCE([In Transit],0) InTransit, 
	COALESCE([QI],0) QI, 
	COALESCE([Ready To Use],0) ReadyToUse, 
	COALESCE([Bad Stock],0) BadStock, 
	(COALESCE([In Transit],0)+COALESCE([QI],0)+COALESCE([Ready To Use],0)+COALESCE([Bad Stock],0)) TotalStockMntc,
	COALESCE([On Used],0) Used,
	COALESCE([On Repair],0) Repair,
	(COALESCE([On Used],0)+COALESCE([On Repair],0)) TotalStockProd
FROM (
		select 
			mi.InventoryDate,
			mi.ItemCode,
			mmi.ItemDescription,
			mi.LocationCode,
			mi.UnitCode,
			mi.ItemStatus,
			mi.EndingStock 
		from MntcInventory mi
		INNER JOIN MstMntcItem mmi on mmi.ItemCode = mi.ItemCode
) as InventoryTable
PIVOT (MAX(EndingStock) FOR ItemStatus IN (
	[In Transit],
	[QI],
	[Ready To Use],
	[Bad Stock],
	[On Used],
	[On Repair]
))as PivotTable;




GO
/****** Object:  View [dbo].[MaintenanceItemConversionDestinationView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MaintenanceItemConversionDestinationView]
AS
SELECT        mmc.ItemCodeSource, mmc.ItemCodeDestination, mmi.ItemDescription AS ItemCodeDestinationDescription, mmc.QtyConvert, C.SourceStock, 
                         C.DestinationStock, C.QtyGood, C.QtyDisposal
FROM            dbo.MstMntcConvert AS mmc INNER JOIN
                         dbo.MstMntcItem AS mmi ON mmi.ItemCode = mmc.ItemCodeDestination LEFT OUTER JOIN
                             (SELECT        ItemCodeSource, ItemCodeDestination, SourceStock, DestinationStock, QtyGood, QtyDisposal
                               FROM            dbo.MntcEquipmentItemConvert AS meic) AS C ON C.ItemCodeSource = mmc.ItemCodeSource AND 
                         C.ItemCodeDestination = mmc.ItemCodeDestination
WHERE        (mmc.ConversionType = 1)






GO
/****** Object:  View [dbo].[MntcRepairItemUsageView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* CREATE Maintenance Repair Item Usage View */
CREATE VIEW [dbo].[MntcRepairItemUsageView]
AS
SELECT        mriu.TransactionDate, mriu.LocationCode, mriu.ItemCodeSource, mriu.ItemCodeDestination, mmi.ItemDescription, mmi.UOM, mmc.QtyConvert, SUM(mriu.QtyUsage) AS Quantity
FROM            dbo.MntcRepairItemUsage AS mriu INNER JOIN
                         dbo.MstMntcItem AS mmi ON mmi.ItemCode = mriu.ItemCodeDestination INNER JOIN
                         dbo.MstMntcConvert AS mmc ON mmc.ItemCodeDestination = mriu.ItemCodeDestination AND mmc.ItemCodeSource = mriu.ItemCodeSource
GROUP BY mriu.TransactionDate, mriu.LocationCode, mriu.ItemCodeSource, mriu.ItemCodeDestination, mmi.ItemDescription, mmi.UOM, mmc.QtyConvert


GO
/****** Object:  View [dbo].[MstMntcConvertGetItemDestinationView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MstMntcConvertGetItemDestinationView]
AS
SELECT        A.ItemCode, B.ItemCode AS ItemCodeDest, B.ItemDescription, C.QtyConvert AS Qty
FROM            (SELECT        ItemCode, ItemDescription
                          FROM            dbo.MstMntcItem
                          WHERE        (ItemType <> 'SPARE PART')) AS A CROSS JOIN
                             (SELECT        ItemCode, ItemDescription
                               FROM            dbo.MstMntcItem AS MstMntcItem_1
                               WHERE        (ItemType = 'SPARE PART')) AS B LEFT OUTER JOIN
                             (SELECT        ItemCodeSource, ItemCodeDestination, QtyConvert
                               FROM            dbo.MstMntcConvert AS MstMntcConvert_1
                               WHERE        (ConversionType = 1)) AS C ON C.ItemCodeSource = A.ItemCode AND C.ItemCodeDestination = B.ItemCode






GO
/****** Object:  View [dbo].[MstPlantProductionGroupView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MstPlantProductionGroupView]
AS
SELECT        ISNULL(ROW_NUMBER() OVER (ORDER BY MstPlantProductionGroup.CreatedDate DESC), - 1) AS RowID, EmpJobsData.GroupCode, EmpJobsData.LocationCode, EmpJobsData.UnitCode, 
EmpJobsData.ProcessSettingsCode, EmpJobsData.WorkerCount, dbo.MstPlantProductionGroup.Leader1, dbo.MstPlantProductionGroup.Leader2, dbo.MstPlantProductionGroup.InspectionLeader, 
dbo.MstPlantProductionGroup.NextGroupCode, CAST(CASE WHEN dbo.MstPlantProductionGroup.StatusActive IS NULL THEN 1 ELSE dbo.MstPlantProductionGroup.StatusActive END AS BIT) AS StatusActive, 
dbo.MstPlantProductionGroup.Remark, dbo.MstPlantProductionGroup.CreatedDate, dbo.MstPlantProductionGroup.CreatedBy, dbo.MstPlantProductionGroup.UpdatedDate, dbo.MstPlantProductionGroup.UpdatedBy, 
InspectionLeader.EmployeeName AS LeaderInspectionName, Leader1.EmployeeName AS Leader1Name, Leader2.EmployeeName AS Leader2Name
FROM            (SELECT        GroupCode, LocationCode, UnitCode, ProcessSettingsCode, COUNT(EmployeeID) AS WorkerCount
                          FROM            dbo.MstPlantEmpJobsDataAcv
                          GROUP BY GroupCode, LocationCode, UnitCode, ProcessSettingsCode) AS EmpJobsData LEFT OUTER JOIN
                         dbo.MstPlantProductionGroup ON EmpJobsData.GroupCode = dbo.MstPlantProductionGroup.GroupCode LEFT JOIN
                         MstPlantEmpJobsDataAcv InspectionLeader ON MstPlantProductionGroup.InspectionLeader = InspectionLeader.EmployeeID LEFT JOIN
                         MstPlantEmpJobsDataAcv Leader1 ON MstPlantProductionGroup.Leader1 = Leader1.EmployeeID LEFT JOIN
                         MstPlantEmpJobsDataAcv Leader2 ON MstPlantProductionGroup.Leader2 = Leader2.EmployeeID;






GO
/****** Object:  View [dbo].[MstPlantUnitView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MstPlantUnitView]
AS
SELECT        dbo.MstPlantUnit.UnitCode, dbo.MstPlantUnit.LocationCode, dbo.MstPlantUnit.UnitName, dbo.MstPlantUnit.StatusActive, dbo.MstPlantUnit.Remark, dbo.MstPlantUnit.CreatedDate, dbo.MstPlantUnit.CreatedBy, 
                         dbo.MstPlantUnit.UpdatedDate, dbo.MstPlantUnit.UpdatedBy, MAX(CASE WHEN UtilRoles.RolesCode = 'PPC' THEN MstADTemp.Name END) AS PPC, 
                         MAX(CASE WHEN UtilRoles.RolesCode = 'HRA' THEN MstADTemp.Name END) AS HRA
FROM            dbo.MstPlantUnit LEFT OUTER JOIN
                         dbo.UtilRules ON dbo.MstPlantUnit.LocationCode = dbo.UtilRules.Location AND dbo.MstPlantUnit.UnitCode = dbo.UtilRules.Unit LEFT OUTER JOIN
                         dbo.UtilResponsibilityRules ON dbo.UtilRules.IDRule = dbo.UtilResponsibilityRules.IDRule LEFT OUTER JOIN
                         dbo.UtilResponsibility ON dbo.UtilResponsibilityRules.IDResponsibility = dbo.UtilResponsibility.IDResponsibility LEFT OUTER JOIN
                         dbo.UtilRoles ON dbo.UtilResponsibility.IDRole = dbo.UtilRoles.IDRole LEFT OUTER JOIN
                         dbo.UtilUsersResponsibility ON dbo.UtilResponsibility.IDResponsibility = dbo.UtilUsersResponsibility.IDResponsibility LEFT OUTER JOIN
                         dbo.MstADTemp ON dbo.UtilUsersResponsibility.UserAD = dbo.MstADTemp.UserAD
GROUP BY dbo.MstPlantUnit.UnitCode, dbo.MstPlantUnit.LocationCode, dbo.MstPlantUnit.UnitName, dbo.MstPlantUnit.StatusActive, dbo.MstPlantUnit.Remark, dbo.MstPlantUnit.CreatedDate, dbo.MstPlantUnit.CreatedBy, 
                         dbo.MstPlantUnit.UpdatedBy, dbo.MstPlantUnit.UpdatedDate











GO
/****** Object:  View [dbo].[PlanPlantIndividualCapacityByReference]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PlanPlantIndividualCapacityByReference]
AS
SELECT        dbo.ExePlantProductionEntryVerification.ProductionEntryDate, dbo.ExePlantProductionEntryVerification.GroupCode, 
                         dbo.ExePlantProductionEntryVerification.UnitCode, dbo.ExePlantProductionEntryVerification.LocationCode, 
                         dbo.ExePlantProductionEntryVerification.ProcessGroup, dbo.ExePlantProductionEntryVerification.BrandCode, 
                         dbo.ExePlantProductionEntryVerification.WorkHours, 
MIN(ProdActual) OVER(PARTITION BY dbo.ExePlantProductionEntry.EmployeeID) AS MinimumValue,
MAX(ProdActual) OVER(PARTITION BY dbo.ExePlantProductionEntry.EmployeeID) AS MaximumValue,  
AVG(ProdActual) OVER(PARTITION BY dbo.ExePlantProductionEntry.EmployeeID) AS AverageValue,
PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY ProdActual) OVER (PARTITION BY dbo.ExePlantProductionEntry.EmployeeID) AS MedianValue,
ProdActual AS LatestValue,
dbo.MstPlantEmpJobsDataAcv.EmployeeID as EmployeeID,
dbo.MstPlantEmpJobsDataAcv.EmployeeNumber as EmployeeNumber,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity3,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity5,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity6,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity7,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity8,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity9,
dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity10
FROM            dbo.ExePlantProductionEntryVerification 
INNER JOIN dbo.ExePlantProductionEntry ON dbo.ExePlantProductionEntryVerification.ProductionEntryDate = dbo.ExePlantProductionEntry.ProductionEntryDate
INNER JOIN dbo.MstPlantEmpJobsDataAcv ON dbo.MstPlantEmpJobsDataAcv.EmployeeID = dbo.ExePlantProductionEntry.EmployeeID
INNER JOIN dbo.PlanPlantIndividualCapacityWorkHours ON dbo.PlanPlantIndividualCapacityWorkHours.EmployeeID = dbo.ExePlantProductionEntry.EmployeeID
AND dbo.PlanPlantIndividualCapacityWorkHours.GroupCode = dbo.ExePlantProductionEntry.GroupCode
AND dbo.PlanPlantIndividualCapacityWorkHours.UnitCode = dbo.ExePlantProductionEntry.UnitCode
AND dbo.PlanPlantIndividualCapacityWorkHours.LocationCode = dbo.ExePlantProductionEntry.LocationCode
AND dbo.PlanPlantIndividualCapacityWorkHours.ProcessGroup= dbo.ExePlantProductionEntry.ProcessGroup

GO
/****** Object:  View [dbo].[PlanPlantTargetProductionKelompokView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PlanPlantTargetProductionKelompokView]
AS
SELECT        BrandCode, LocationCode, SUM(TargetSystem1) AS TargetSystem1, SUM(TargetSystem2) AS TargetSystem2, SUM(TargetSystem3) AS TargetSystem3, 
                         SUM(TargetSystem4) AS TargetSystem4, SUM(TargetSystem5) AS TargetSystem5, SUM(TargetSystem6) AS TargetSystem6, SUM(TargetSystem7) 
                         AS TargetSystem7
FROM            dbo.PlanPlantTargetProductionKelompok AS pptpk
GROUP BY BrandCode, LocationCode






GO
/****** Object:  View [dbo].[TargetProductionUnitView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TargetProductionUnitView]
AS
--SELECT tpu.*, wpp.Value1, sh.*
--FROM PlanTargetProductionUnit AS tpu
--     INNER JOIN PlanWeeklyProductionPlanning AS wpp ON 
--	tpu.KPSYear = wpp.KPSYear
--	AND tpu.KPSWeek = wpp.KPSWeek
--	AND tpu.BrandCode = wpp.BrandCode
--	AND tpu.LocationCode = wpp.LocationCode
--CROSS JOIN
--(SELECT * FROM (
--SELECT DayName, JknHour FROM MstGenStandardHours WHERE DayType = 'NON-HOLIDAY'
--) AS S
--PIVOT (
--MAX(JknHour) FOR DayName IN (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)
--) AS pv) AS sh

SELECT tpu.*, wpp.Value1
FROM PlanTargetProductionUnit AS tpu
     INNER JOIN PlanWeeklyProductionPlanning AS wpp ON 
	tpu.KPSYear = wpp.KPSYear
	AND tpu.KPSWeek = wpp.KPSWeek
	AND tpu.BrandCode = wpp.BrandCode
	AND tpu.LocationCode = wpp.LocationCode






GO
/****** Object:  View [dbo].[UtilRolesFunctionView]    Script Date: 10/16/2015 2:56:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UtilRolesFunctionView]
AS
SELECT        dbo.UtilFunctions.*, dbo.UtilRolesFunction.IDRole
FROM            dbo.UtilFunctions INNER JOIN
                         dbo.UtilRolesFunction ON dbo.UtilFunctions.IDFunction = dbo.UtilRolesFunction.IDFunction









GO
ALTER TABLE [dbo].[VT_SchemaChangeLog] ADD  DEFAULT (getdate()) FOR [ScriptDateApplied]
GO
ALTER TABLE [dbo].[ExeActualWorkHours]  WITH CHECK ADD  CONSTRAINT [FK_EXEACTUALWORKHOURS_REFERENCE_131_MSTGENEMPSTATUS] FOREIGN KEY([StatusEmp])
REFERENCES [dbo].[MstGenEmpStatus] ([StatusEmp])
GO
ALTER TABLE [dbo].[ExeActualWorkHours] CHECK CONSTRAINT [FK_EXEACTUALWORKHOURS_REFERENCE_131_MSTGENEMPSTATUS]
GO
ALTER TABLE [dbo].[ExeActualWorkHours]  WITH CHECK ADD  CONSTRAINT [FK_EXEACTUALWORKHOURS_RELATIONSHIP_123_MSTGENPROCESSSETTINGSLOC] FOREIGN KEY([IDProcess], [ProcessIdentifier], [LocationCode])
REFERENCES [dbo].[MstGenProcessSettingsLocation] ([IDProcess], [ProcessIdentifier], [LocationCode])
GO
ALTER TABLE [dbo].[ExeActualWorkHours] CHECK CONSTRAINT [FK_EXEACTUALWORKHOURS_RELATIONSHIP_123_MSTGENPROCESSSETTINGSLOC]
GO
ALTER TABLE [dbo].[ExeMaterialUsage]  WITH CHECK ADD  CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_125_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[ExeMaterialUsage] CHECK CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_125_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[ExeMaterialUsage]  WITH CHECK ADD  CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_38_MSTGENMATERIAL] FOREIGN KEY([MaterialCode])
REFERENCES [dbo].[MstGenMaterial] ([MaterialCode])
GO
ALTER TABLE [dbo].[ExeMaterialUsage] CHECK CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_38_MSTGENMATERIAL]
GO
ALTER TABLE [dbo].[ExeMaterialUsage]  WITH CHECK ADD  CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_44_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[ExeMaterialUsage] CHECK CONSTRAINT [FK_EXEMATERIALUSAGE_RELATIONSHIP_44_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[ExePlantCKSubmit]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTCKSUBMIT_RELATIONSHIP_62_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[ExePlantCKSubmit] CHECK CONSTRAINT [FK_EXEPLANTCKSUBMIT_RELATIONSHIP_62_MSTGENBRAND]
GO
ALTER TABLE [dbo].[ExePlantCKSubmit]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTCKSUBMIT_RELATIONSHIP_63_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[ExePlantCKSubmit] CHECK CONSTRAINT [FK_EXEPLANTCKSUBMIT_RELATIONSHIP_63_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[ExePlantProductionEntry]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_REFERENCE_121_EXEPLANTPRODUCTIONENTRYV] FOREIGN KEY([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
REFERENCES [dbo].[ExePlantProductionEntryVerification] ([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
GO
ALTER TABLE [dbo].[ExePlantProductionEntry] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_REFERENCE_121_EXEPLANTPRODUCTIONENTRYV]
GO
ALTER TABLE [dbo].[ExePlantProductionEntry]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS] FOREIGN KEY([StartDateAbsent], [EmployeeID], [AbsentType])
REFERENCES [dbo].[ExePlantWorkerAbsenteeism] ([StartDateAbsent], [EmployeeID], [AbsentType])
GO
ALTER TABLE [dbo].[ExePlantProductionEntry] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS]
GO
ALTER TABLE [dbo].[ExePlantWorkerAbsenteeism]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTWORKERABSENTEEIS_REFERENCE_133_MSTPLANTABSENTTYPE] FOREIGN KEY([AbsentType])
REFERENCES [dbo].[MstPlantAbsentType] ([AbsentType])
GO
ALTER TABLE [dbo].[ExePlantWorkerAbsenteeism] CHECK CONSTRAINT [FK_EXEPLANTWORKERABSENTEEIS_REFERENCE_133_MSTPLANTABSENTTYPE]
GO
ALTER TABLE [dbo].[ExePlantWorkerAbsenteeism]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTWORKERABSENTEEIS_RELATIONSHIP_69_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[ExePlantWorkerAbsenteeism] CHECK CONSTRAINT [FK_EXEPLANTWORKERABSENTEEIS_RELATIONSHIP_69_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[ExePlantWorkerAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_REFERENCE_128_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([DestinationGroupCode], [DestinationUnitCode], [DestinationLocationCode], [DestinationProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[ExePlantWorkerAssignment] CHECK CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_REFERENCE_128_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[ExePlantWorkerAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_RELATIONSHIP_45_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([SourceGroupCode], [SourceUnitCode], [SourceLocationCode], [SourceProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[ExePlantWorkerAssignment] CHECK CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_RELATIONSHIP_45_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[ExeProductionEntryRelease]  WITH CHECK ADD  CONSTRAINT [FK_EXEPRODUCTIONENTRYRELEAS_RELATIONSHIP_129_EXEPLANTPRODUCTIONENTRYV] FOREIGN KEY([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
REFERENCES [dbo].[ExePlantProductionEntryVerification] ([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
GO
ALTER TABLE [dbo].[ExeProductionEntryRelease] CHECK CONSTRAINT [FK_EXEPRODUCTIONENTRYRELEAS_RELATIONSHIP_129_EXEPLANTPRODUCTIONENTRYV]
GO
ALTER TABLE [dbo].[MntcEquipmentFulfillment]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTFULFILLMENT_RELATIONSHIP_81_MNTCEQUIPMENTREQUEST] FOREIGN KEY([RequestDate], [ItemCode], [LocationCode], [RequestNumber])
REFERENCES [dbo].[MntcEquipmentRequest] ([RequestDate], [ItemCode], [LocationCode], [RequestNumber])
GO
ALTER TABLE [dbo].[MntcEquipmentFulfillment] CHECK CONSTRAINT [FK_MNTCEQUIPMENTFULFILLMENT_RELATIONSHIP_81_MNTCEQUIPMENTREQUEST]
GO
ALTER TABLE [dbo].[MntcEquipmentItemConvert]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTITEMCONVERT_REFERENCE_134_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentItemConvert] CHECK CONSTRAINT [FK_MNTCEQUIPMENTITEMCONVERT_REFERENCE_134_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MntcEquipmentItemConvert]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTITEMCONVERT_RELATIONSHIP_134_MSTMNTCCONVERT] FOREIGN KEY([ItemCodeSource], [ItemCodeDestination])
REFERENCES [dbo].[MstMntcConvert] ([ItemCodeSource], [ItemCodeDestination])
GO
ALTER TABLE [dbo].[MntcEquipmentItemConvert] CHECK CONSTRAINT [FK_MNTCEQUIPMENTITEMCONVERT_RELATIONSHIP_134_MSTMNTCCONVERT]
GO
ALTER TABLE [dbo].[MntcEquipmentItemDisposal]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTITEMDISPOSA_REFERENCE_129_MSTMNTCITEMLOCATION] FOREIGN KEY([ItemCode], [LocationCode])
REFERENCES [dbo].[MstMntcItemLocation] ([ItemCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentItemDisposal] CHECK CONSTRAINT [FK_MNTCEQUIPMENTITEMDISPOSA_REFERENCE_129_MSTMNTCITEMLOCATION]
GO
ALTER TABLE [dbo].[MntcEquipmentMovement]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_REFERENCE_132_MSTPLANTUNIT] FOREIGN KEY([UnitCodeDestination], [LocationCodeDestination])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentMovement] CHECK CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_REFERENCE_132_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[MntcEquipmentMovement]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_RELATIONSHIP_13_MSTGENLOCATION] FOREIGN KEY([LocationCodeSource])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentMovement] CHECK CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_RELATIONSHIP_13_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MntcEquipmentMovement]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_RELATIONSHIP_26_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MntcEquipmentMovement] CHECK CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_RELATIONSHIP_26_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MntcEquipmentQualityInspection]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTQUALITYINSP_RELATIONSHIP_12_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentQualityInspection] CHECK CONSTRAINT [FK_MNTCEQUIPMENTQUALITYINSP_RELATIONSHIP_12_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MntcEquipmentQualityInspection]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTQUALITYINSP_RELATIONSHIP_25_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MntcEquipmentQualityInspection] CHECK CONSTRAINT [FK_MNTCEQUIPMENTQUALITYINSP_RELATIONSHIP_25_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MntcEquipmentRepair]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTREPAIR_REFERENCE_127_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MntcEquipmentRepair] CHECK CONSTRAINT [FK_MNTCEQUIPMENTREPAIR_REFERENCE_127_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MntcEquipmentRepair]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTREPAIR_RELATIONSHIP_20_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentRepair] CHECK CONSTRAINT [FK_MNTCEQUIPMENTREPAIR_RELATIONSHIP_20_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[MntcEquipmentRequest]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTREQUEST_RELATIONSHIP_91_MSTMNTCITEMLOCATION] FOREIGN KEY([ItemCode], [LocationCode])
REFERENCES [dbo].[MstMntcItemLocation] ([ItemCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentRequest] CHECK CONSTRAINT [FK_MNTCEQUIPMENTREQUEST_RELATIONSHIP_91_MSTMNTCITEMLOCATION]
GO
ALTER TABLE [dbo].[MntcInventory]  WITH CHECK ADD  CONSTRAINT [FK_MNTCINVENTORY_REFERENCE_135_MSTMNTCITEMLOCATION] FOREIGN KEY([ItemCode], [LocationCode])
REFERENCES [dbo].[MstMntcItemLocation] ([ItemCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcInventory] CHECK CONSTRAINT [FK_MNTCINVENTORY_REFERENCE_135_MSTMNTCITEMLOCATION]
GO
ALTER TABLE [dbo].[MntcInventoryAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[MntcInventoryAdjustment] CHECK CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[MntcInventoryAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MntcInventoryAdjustment] CHECK CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MntcRepairItemUsage]  WITH CHECK ADD  CONSTRAINT [FK_MNTCREPAIRITEMUSAGE_REFERENCE_136_MSTMNTCITEM] FOREIGN KEY([ItemCodeDestination])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MntcRepairItemUsage] CHECK CONSTRAINT [FK_MNTCREPAIRITEMUSAGE_REFERENCE_136_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MntcRepairItemUsage]  WITH CHECK ADD  CONSTRAINT [FK_MNTCREPAIRITEMUSAGE_RELATIONSHIP_131_MNTCEQUIPMENTREPAIR] FOREIGN KEY([TransactionDate], [UnitCode], [LocationCode], [ItemCodeSource])
REFERENCES [dbo].[MntcEquipmentRepair] ([TransactionDate], [UnitCode], [LocationCode], [ItemCode])
GO
ALTER TABLE [dbo].[MntcRepairItemUsage] CHECK CONSTRAINT [FK_MNTCREPAIRITEMUSAGE_RELATIONSHIP_131_MNTCEQUIPMENTREPAIR]
GO
ALTER TABLE [dbo].[MntcRequestToLocation]  WITH CHECK ADD  CONSTRAINT [FK_MNTCREQUESTTOLOCATION_RELATIONSHIP_80_MNTCEQUIPMENTFULFILLMENT] FOREIGN KEY([FulFillmentDate], [RequestNumber])
REFERENCES [dbo].[MntcEquipmentFulfillment] ([FulFillmentDate], [RequestNumber])
GO
ALTER TABLE [dbo].[MntcRequestToLocation] CHECK CONSTRAINT [FK_MNTCREQUESTTOLOCATION_RELATIONSHIP_80_MNTCEQUIPMENTFULFILLMENT]
GO
ALTER TABLE [dbo].[MntcRequestToLocation]  WITH CHECK ADD  CONSTRAINT [FK_MNTCREQUESTTOLOCATION_RELATIONSHIP_92_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MntcRequestToLocation] CHECK CONSTRAINT [FK_MNTCREQUESTTOLOCATION_RELATIONSHIP_92_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstGenBrand]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENBRAND_RELATIONSHIP_48_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenBrand] CHECK CONSTRAINT [FK_MSTGENBRAND_RELATIONSHIP_48_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenBrandPackageItem]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENBRANDPACKAGEITEM_RELATIONSHIP_30_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenBrandPackageItem] CHECK CONSTRAINT [FK_MSTGENBRANDPACKAGEITEM_RELATIONSHIP_30_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenBrandPackageItem]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENBRANDPACKAGEITEM_RELATIONSHIP_31_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MstGenBrandPackageItem] CHECK CONSTRAINT [FK_MSTGENBRANDPACKAGEITEM_RELATIONSHIP_31_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MstGenBrandPkgMapping]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENBRANDPKGMAPPING_RELATIONSHIP_36_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCodeSource])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenBrandPkgMapping] CHECK CONSTRAINT [FK_MSTGENBRANDPKGMAPPING_RELATIONSHIP_36_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenBrandPkgMapping]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENBRANDPKGMAPPING_RELATIONSHIP_95_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCodeDestination])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenBrandPkgMapping] CHECK CONSTRAINT [FK_MSTGENBRANDPKGMAPPING_RELATIONSHIP_95_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenHoliday]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENHOLIDAY_RELATIONSHIP_7_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstGenHoliday] CHECK CONSTRAINT [FK_MSTGENHOLIDAY_RELATIONSHIP_7_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstGenLocation]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENLOCATION_RELATIONSHIP_8_MSTGENLOCATION] FOREIGN KEY([ParentLocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstGenLocation] CHECK CONSTRAINT [FK_MSTGENLOCATION_RELATIONSHIP_8_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstGenLocStatus]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENLOCSTATUS_RELATIONSHIP_93_MSTGENEMPSTATUS] FOREIGN KEY([StatusEmp])
REFERENCES [dbo].[MstGenEmpStatus] ([StatusEmp])
GO
ALTER TABLE [dbo].[MstGenLocStatus] CHECK CONSTRAINT [FK_MSTGENLOCSTATUS_RELATIONSHIP_93_MSTGENEMPSTATUS]
GO
ALTER TABLE [dbo].[MstGenLocStatus]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENLOCSTATUS_RELATIONSHIP_94_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstGenLocStatus] CHECK CONSTRAINT [FK_MSTGENLOCSTATUS_RELATIONSHIP_94_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstGenMaterial]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENMATERIAL_REFERENCE_130_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstGenMaterial] CHECK CONSTRAINT [FK_MSTGENMATERIAL_REFERENCE_130_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstGenMaterial]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENMATERIAL_RELATIONSHIP_37_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenMaterial] CHECK CONSTRAINT [FK_MSTGENMATERIAL_RELATIONSHIP_37_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenProcessSettings]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENPROCESSSETTINGS_RELATIONSHIP_18_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstGenProcessSettings] CHECK CONSTRAINT [FK_MSTGENPROCESSSETTINGS_RELATIONSHIP_18_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstGenProcessSettings]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENPROCESSSETTINGS_RELATIONSHIP_40_MSTGENPROCESS] FOREIGN KEY([ProcessGroup])
REFERENCES [dbo].[MstGenProcess] ([ProcessGroup])
GO
ALTER TABLE [dbo].[MstGenProcessSettings] CHECK CONSTRAINT [FK_MSTGENPROCESSSETTINGS_RELATIONSHIP_40_MSTGENPROCESS]
GO
ALTER TABLE [dbo].[MstGenProcessSettingsLocation]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENPROCESSSETTINGSLOC_RELATIONSHIP_16_MSTGENPROCESSSETTINGS] FOREIGN KEY([IDProcess], [ProcessIdentifier])
REFERENCES [dbo].[MstGenProcessSettings] ([IDProcess], [ProcessIdentifier])
GO
ALTER TABLE [dbo].[MstGenProcessSettingsLocation] CHECK CONSTRAINT [FK_MSTGENPROCESSSETTINGSLOC_RELATIONSHIP_16_MSTGENPROCESSSETTINGS]
GO
ALTER TABLE [dbo].[MstGenProcessSettingsLocation]  WITH CHECK ADD  CONSTRAINT [FK_MSTGENPROCESSSETTINGSLOC_RELATIONSHIP_17_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstGenProcessSettingsLocation] CHECK CONSTRAINT [FK_MSTGENPROCESSSETTINGSLOC_RELATIONSHIP_17_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstMntcConvert]  WITH CHECK ADD  CONSTRAINT [FK_MSTMNTCCONVERT_RELATIONSHIP_132_MSTMNTCITEM] FOREIGN KEY([ItemCodeSource])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MstMntcConvert] CHECK CONSTRAINT [FK_MSTMNTCCONVERT_RELATIONSHIP_132_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MstMntcConvert]  WITH CHECK ADD  CONSTRAINT [FK_MSTMNTCCONVERT_RELATIONSHIP_32_MSTMNTCITEM] FOREIGN KEY([ItemCodeDestination])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MstMntcConvert] CHECK CONSTRAINT [FK_MSTMNTCCONVERT_RELATIONSHIP_32_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MstMntcItemLocation]  WITH CHECK ADD  CONSTRAINT [FK_MSTMNTCITEMLOCATION_RELATIONSHIP_23_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO
ALTER TABLE [dbo].[MstMntcItemLocation] CHECK CONSTRAINT [FK_MSTMNTCITEMLOCATION_RELATIONSHIP_23_MSTMNTCITEM]
GO
ALTER TABLE [dbo].[MstMntcItemLocation]  WITH CHECK ADD  CONSTRAINT [FK_MSTMNTCITEMLOCATION_RELATIONSHIP_29_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstMntcItemLocation] CHECK CONSTRAINT [FK_MSTMNTCITEMLOCATION_RELATIONSHIP_29_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstPlantEmpJobsDataAcv]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTEMPJOBSDATAACV_RELATIONSHIP_68_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantEmpJobsDataAcv] CHECK CONSTRAINT [FK_MSTPLANTEMPJOBSDATAACV_RELATIONSHIP_68_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[MstPlantEmpUpd]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTEMPUPD_RELATIONSHIP_67_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantEmpUpd] CHECK CONSTRAINT [FK_MSTPLANTEMPUPD_RELATIONSHIP_67_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[MstPlantEmpUpdTmp]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTEMPUPDTMP_RELATIONSHIP_66_MSTPLANTEMPUPD] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpUpd] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantEmpUpdTmp] CHECK CONSTRAINT [FK_MSTPLANTEMPUPDTMP_RELATIONSHIP_66_MSTPLANTEMPUPD]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_100_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([NextGroupCode], [Mst_UnitCode], [Mst_LocationCode], [Mst_ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_100_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_147_MSTGENPROCESS] FOREIGN KEY([ProcessGroup])
REFERENCES [dbo].[MstGenProcess] ([ProcessGroup])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_147_MSTGENPROCESS]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_46_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_46_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_97_MSTPLANTEMPJOBSDATAACV] FOREIGN KEY([InspectionLeader])
REFERENCES [dbo].[MstPlantEmpJobsDataAcv] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_97_MSTPLANTEMPJOBSDATAACV]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_98_MSTPLANTEMPJOBSDATAACV] FOREIGN KEY([Leader1])
REFERENCES [dbo].[MstPlantEmpJobsDataAcv] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_98_MSTPLANTEMPJOBSDATAACV]
GO
ALTER TABLE [dbo].[MstPlantProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_99_MSTPLANTEMPJOBSDATAACV] FOREIGN KEY([Leader2])
REFERENCES [dbo].[MstPlantEmpJobsDataAcv] ([EmployeeID])
GO
ALTER TABLE [dbo].[MstPlantProductionGroup] CHECK CONSTRAINT [FK_MSTPLANTPRODUCTIONGROUP_RELATIONSHIP_99_MSTPLANTEMPJOBSDATAACV]
GO
ALTER TABLE [dbo].[MstPlantUnit]  WITH CHECK ADD  CONSTRAINT [FK_MSTPLANTUNIT_RELATIONSHIP_19_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstPlantUnit] CHECK CONSTRAINT [FK_MSTPLANTUNIT_RELATIONSHIP_19_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstTPOFeeRate]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOFEERATE_RELATIONSHIP_4_MSTTPOINFO] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstTPOInfo] ([LocationCode])
GO
ALTER TABLE [dbo].[MstTPOFeeRate] CHECK CONSTRAINT [FK_MSTTPOFEERATE_RELATIONSHIP_4_MSTTPOINFO]
GO
ALTER TABLE [dbo].[MstTPOFeeRate]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOFEERATE_RELATIONSHIP_6_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstTPOFeeRate] CHECK CONSTRAINT [FK_MSTTPOFEERATE_RELATIONSHIP_6_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstTPOInfo]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOINFO_RELATIONSHIP_9_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MstTPOInfo] CHECK CONSTRAINT [FK_MSTTPOINFO_RELATIONSHIP_9_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[MstTPOPackage]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOPACKAGE_RELATIONSHIP_3_MSTTPOINFO] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstTPOInfo] ([LocationCode])
GO
ALTER TABLE [dbo].[MstTPOPackage] CHECK CONSTRAINT [FK_MSTTPOPACKAGE_RELATIONSHIP_3_MSTTPOINFO]
GO
ALTER TABLE [dbo].[MstTPOPackage]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOPACKAGE_RELATIONSHIP_5_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[MstTPOPackage] CHECK CONSTRAINT [FK_MSTTPOPACKAGE_RELATIONSHIP_5_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[MstTPOProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOPRODUCTIONGROUP_RELATIONSHIP_126_MSTGENPROCESS] FOREIGN KEY([ProcessGroup])
REFERENCES [dbo].[MstGenProcess] ([ProcessGroup])
GO
ALTER TABLE [dbo].[MstTPOProductionGroup] CHECK CONSTRAINT [FK_MSTTPOPRODUCTIONGROUP_RELATIONSHIP_126_MSTGENPROCESS]
GO
ALTER TABLE [dbo].[MstTPOProductionGroup]  WITH CHECK ADD  CONSTRAINT [FK_MSTTPOPRODUCTIONGROUP_RELATIONSHIP_128_MSTGENLOCSTATUS] FOREIGN KEY([LocationCode], [StatusEmp])
REFERENCES [dbo].[MstGenLocStatus] ([LocationCode], [StatusEmp])
GO
ALTER TABLE [dbo].[MstTPOProductionGroup] CHECK CONSTRAINT [FK_MSTTPOPRODUCTIONGROUP_RELATIONSHIP_128_MSTGENLOCSTATUS]
GO
ALTER TABLE [dbo].[NewsInfo]  WITH CHECK ADD  CONSTRAINT [FK_NewsInfo_MstGenLocation] FOREIGN KEY([Location])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[NewsInfo] CHECK CONSTRAINT [FK_NewsInfo_MstGenLocation]
GO
ALTER TABLE [dbo].[PlanPlantAllocation]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTALLOCATION_REFERENCE_140_PLANPLANTTARGETPRODUCTIONKELOMPO] FOREIGN KEY([TPKPlantStartProductionDate], [KPSYear], [KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode], [Shift])
REFERENCES [dbo].[PlanPlantTargetProductionKelompok] ([TPKPlantStartProductionDate], [KPSYear], [KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode], [Shift])
GO
ALTER TABLE [dbo].[PlanPlantAllocation] CHECK CONSTRAINT [FK_PLANPLANTALLOCATION_REFERENCE_140_PLANPLANTTARGETPRODUCTIONKELOMPO]
GO
ALTER TABLE [dbo].[PlanPlantAllocation]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTALLOCATION_REFERENCE_141_MSTPLANTEMPJOBSDATAACV] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAcv] ([EmployeeID])
GO
ALTER TABLE [dbo].[PlanPlantAllocation] CHECK CONSTRAINT [FK_PLANPLANTALLOCATION_REFERENCE_141_MSTPLANTEMPJOBSDATAACV]
GO
ALTER TABLE [dbo].[PlanPlantGroupShift]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTGROUPSHIFT_RELATIONSHIP_56_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantGroupShift] CHECK CONSTRAINT [FK_PLANPLANTGROUPSHIFT_RELATIONSHIP_56_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLAN_REFERENCE_MSTGENBR] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLAN_REFERENCE_MSTGENBR]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLAN_REFERENCE_MSTPLANT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLAN_REFERENCE_MSTPLANT]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLAN_RELATIONS_MSTPLANT] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLAN_RELATIONS_MSTPLANT]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_112_MSTPLANTEMPJOBSDATAACV] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAcv] ([EmployeeID])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_112_MSTPLANTEMPJOBSDATAACV]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_51_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_51_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_53_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityWorkHours] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_53_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantLineBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTLINEBALANCING_RELATIONSHIP_118_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantLineBalancing] CHECK CONSTRAINT [FK_PLANPLANTLINEBALANCING_RELATIONSHIP_118_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantLineBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTLINEBALANCING_RELATIONSHIP_119_MSTGENPROCESSSETTINGSLOC] FOREIGN KEY([IDProcess], [ProcessIdentifier], [Mst_LocationCode])
REFERENCES [dbo].[MstGenProcessSettingsLocation] ([IDProcess], [ProcessIdentifier], [LocationCode])
GO
ALTER TABLE [dbo].[PlanPlantLineBalancing] CHECK CONSTRAINT [FK_PLANPLANTLINEBALANCING_RELATIONSHIP_119_MSTGENPROCESSSETTINGSLOC]
GO
ALTER TABLE [dbo].[PlanPlantTargetProductionKelompok]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTTARGETPRODUCTIO_RELATIONSHIP_59_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantTargetProductionKelompok] CHECK CONSTRAINT [FK_PLANPLANTTARGETPRODUCTIO_RELATIONSHIP_59_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantTargetProductionKelompok]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTTARGETPRODUCTIO_RELATIONSHIP_61_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[PlanPlantTargetProductionKelompok] CHECK CONSTRAINT [FK_PLANPLANTTARGETPRODUCTIO_RELATIONSHIP_61_MSTGENBRAND]
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_101_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCodeDestination], [UnitCodeDestination], [LocationCodeDestination], [ProcessGroupDestination])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing] CHECK CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_101_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_120_PLANPLANTLINEBALANCING] FOREIGN KEY([ProductionDate])
REFERENCES [dbo].[PlanPlantLineBalancing] ([ProductionDate])
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing] CHECK CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_120_PLANPLANTLINEBALANCING]
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_57_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCodeSource], [UnitCodeSource], [LocationCodeSource], [ProcessGroupSource])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing] CHECK CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_57_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_85_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[PlanPlantWorkerBalancing] CHECK CONSTRAINT [FK_PLANPLANTWORKERBALANCING_RELATIONSHIP_85_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[PlanTargetProductionUnit]  WITH CHECK ADD  CONSTRAINT [FK_PLANTARGETPRODUCTIONUNIT_REFERENCE_125_PLANTMPWEEKLYPRODUCTIONP] FOREIGN KEY([KPSYear], [KPSWeek], [BrandCode], [LocationCode])
REFERENCES [dbo].[PlanWeeklyProductionPlanning] ([KPSYear], [KPSWeek], [BrandCode], [LocationCode])
GO
ALTER TABLE [dbo].[PlanTargetProductionUnit] CHECK CONSTRAINT [FK_PLANTARGETPRODUCTIONUNIT_REFERENCE_125_PLANTMPWEEKLYPRODUCTIONP]
GO
ALTER TABLE [dbo].[PlanTPOTargetProductionKelompok]  WITH CHECK ADD  CONSTRAINT [FK_PLANTPOTARGETPRODUCTIONK_REFERENCE_120_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[PlanTPOTargetProductionKelompok] CHECK CONSTRAINT [FK_PLANTPOTARGETPRODUCTIONK_REFERENCE_120_MSTGENBRAND]
GO
ALTER TABLE [dbo].[PlanTPOTargetProductionKelompok]  WITH CHECK ADD  CONSTRAINT [FK_PLANTPOTARGETPRODUCTIONK_RELATIONSHIP_110_MSTTPOPRODUCTIONGROUP] FOREIGN KEY([ProdGroup], [ProcessGroup], [LocationCode], [StatusEmp])
REFERENCES [dbo].[MstTPOProductionGroup] ([ProdGroup], [ProcessGroup], [LocationCode], [StatusEmp])
GO
ALTER TABLE [dbo].[PlanTPOTargetProductionKelompok] CHECK CONSTRAINT [FK_PLANTPOTARGETPRODUCTIONK_RELATIONSHIP_110_MSTTPOPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanWeeklyProductionPlanning]  WITH CHECK ADD  CONSTRAINT [FK_PLANWEEKLYPRODUCTIONPLAN_RELATIONSHIP_109_MSTGENLOCATION] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[PlanWeeklyProductionPlanning] CHECK CONSTRAINT [FK_PLANWEEKLYPRODUCTIONPLAN_RELATIONSHIP_109_MSTGENLOCATION]
GO
ALTER TABLE [dbo].[PlanWeeklyProductionPlanning]  WITH CHECK ADD  CONSTRAINT [FK_PLANWEEKLYPRODUCTIONPLAN_RELATIONSHIP_35_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[PlanWeeklyProductionPlanning] CHECK CONSTRAINT [FK_PLANWEEKLYPRODUCTIONPLAN_RELATIONSHIP_35_MSTGENBRAND]
GO
ALTER TABLE [dbo].[ProductAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTADJUSTMENT_RELATIONSHIP_145_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[ProductAdjustment] CHECK CONSTRAINT [FK_PRODUCTADJUSTMENT_RELATIONSHIP_145_MSTGENBRAND]
GO
ALTER TABLE [dbo].[ProductAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTADJUSTMENT_RELATIONSHIP_146_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[ProductAdjustment] CHECK CONSTRAINT [FK_PRODUCTADJUSTMENT_RELATIONSHIP_146_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[ProductionCard]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_122_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[ProductionCard] CHECK CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_122_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[ProductionCard]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_49_MSTGENBRANDGROUP] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[ProductionCard] CHECK CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_49_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[ProductionCard]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_55_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[ProductionCard] CHECK CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_55_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[TPOFeeCalculation]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEECALCULATION_RELATIONSHIP_139_TPOFEEHDR] FOREIGN KEY([KPSYear], [KPSWeek])
REFERENCES [dbo].[TPOFeeHdr] ([KPSYear], [KPSWeek])
GO
ALTER TABLE [dbo].[TPOFeeCalculation] CHECK CONSTRAINT [FK_TPOFEECALCULATION_RELATIONSHIP_139_TPOFEEHDR]
GO
ALTER TABLE [dbo].[TPOFeeCalculationPlan]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEECALCULATIONPLAN_RELATIONSHIP_141_TPOFEEHDRPLAN] FOREIGN KEY([KPSYear], [KPSWeek])
REFERENCES [dbo].[TPOFeeHdrPlan] ([KPSYear], [KPSWeek])
GO
ALTER TABLE [dbo].[TPOFeeCalculationPlan] CHECK CONSTRAINT [FK_TPOFEECALCULATIONPLAN_RELATIONSHIP_141_TPOFEEHDRPLAN]
GO
ALTER TABLE [dbo].[TPOFeeHdr]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_137_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[TPOFeeHdr] CHECK CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_137_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[TPOFeeHdr]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_86_MSTTPOINFO] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstTPOInfo] ([LocationCode])
GO
ALTER TABLE [dbo].[TPOFeeHdr] CHECK CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_86_MSTTPOINFO]
GO
ALTER TABLE [dbo].[TPOFeeHdr]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_87_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[TPOFeeHdr] CHECK CONSTRAINT [FK_TPOFEEHDR_RELATIONSHIP_87_MSTGENBRAND]
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_142_MSTTPOINFO] FOREIGN KEY([LocationCode])
REFERENCES [dbo].[MstTPOInfo] ([LocationCode])
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan] CHECK CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_142_MSTTPOINFO]
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_143_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan] CHECK CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_143_MSTGENBRANDGROUP]
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_144_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[TPOFeeHdrPlan] CHECK CONSTRAINT [FK_TPOFEEHDRPLAN_RELATIONSHIP_144_MSTGENBRAND]
GO
ALTER TABLE [dbo].[TPOFeeProductionDaily]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEPRODUCTIONDAILY_RELATIONSHIP_138_TPOFEEHDR] FOREIGN KEY([KPSYear], [KPSWeek])
REFERENCES [dbo].[TPOFeeHdr] ([KPSYear], [KPSWeek])
GO
ALTER TABLE [dbo].[TPOFeeProductionDaily] CHECK CONSTRAINT [FK_TPOFEEPRODUCTIONDAILY_RELATIONSHIP_138_TPOFEEHDR]
GO
ALTER TABLE [dbo].[TPOFeeProductionDailyPlan]  WITH CHECK ADD  CONSTRAINT [FK_TPOFEEPRODUCTIONDAILYPLA_RELATIONSHIP_140_TPOFEEHDRPLAN] FOREIGN KEY([KPSYear], [KPSWeek])
REFERENCES [dbo].[TPOFeeHdrPlan] ([KPSYear], [KPSWeek])
GO
ALTER TABLE [dbo].[TPOFeeProductionDailyPlan] CHECK CONSTRAINT [FK_TPOFEEPRODUCTIONDAILYPLA_RELATIONSHIP_140_TPOFEEHDRPLAN]
GO
ALTER TABLE [dbo].[UtilDelegation]  WITH CHECK ADD  CONSTRAINT [FK_UTILDELEGATION_RELATIONSHIP_105_MSTADTEMP] FOREIGN KEY([UserADTo])
REFERENCES [dbo].[MstADTemp] ([UserAD])
GO
ALTER TABLE [dbo].[UtilDelegation] CHECK CONSTRAINT [FK_UTILDELEGATION_RELATIONSHIP_105_MSTADTEMP]
GO
ALTER TABLE [dbo].[UtilDelegation]  WITH CHECK ADD  CONSTRAINT [FK_UTILDELEGATION_RELATIONSHIP_71_UTILUSERSRESPONSIBILITY] FOREIGN KEY([IDResponsibility], [UserADFrom])
REFERENCES [dbo].[UtilUsersResponsibility] ([IDResponsibility], [UserAD])
GO
ALTER TABLE [dbo].[UtilDelegation] CHECK CONSTRAINT [FK_UTILDELEGATION_RELATIONSHIP_71_UTILUSERSRESPONSIBILITY]
GO
ALTER TABLE [dbo].[UtilFlows]  WITH CHECK ADD  CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_102_UTILROLESFUNCTION] FOREIGN KEY([DestinationForm], [DestinationRole])
REFERENCES [dbo].[UtilRolesFunction] ([IDFunction], [IDRole])
GO
ALTER TABLE [dbo].[UtilFlows] CHECK CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_102_UTILROLESFUNCTION]
GO
ALTER TABLE [dbo].[UtilFlows]  WITH CHECK ADD  CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_103_UTILFUNCTIONS] FOREIGN KEY([ActionButton])
REFERENCES [dbo].[UtilFunctions] ([IDFunction])
GO
ALTER TABLE [dbo].[UtilFlows] CHECK CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_103_UTILFUNCTIONS]
GO
ALTER TABLE [dbo].[UtilFlows]  WITH CHECK ADD  CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_78_UTILFUNCTIONS] FOREIGN KEY([FormSource])
REFERENCES [dbo].[UtilFunctions] ([IDFunction])
GO
ALTER TABLE [dbo].[UtilFlows] CHECK CONSTRAINT [FK_UTILFLOWS_RELATIONSHIP_78_UTILFUNCTIONS]
GO
ALTER TABLE [dbo].[UtilFunctions]  WITH CHECK ADD  CONSTRAINT [FK_UTILFUNCTIONS_RELATIONSHIP_77_UTILFUNCTIONS] FOREIGN KEY([ParentIDFunction])
REFERENCES [dbo].[UtilFunctions] ([IDFunction])
GO
ALTER TABLE [dbo].[UtilFunctions] CHECK CONSTRAINT [FK_UTILFUNCTIONS_RELATIONSHIP_77_UTILFUNCTIONS]
GO
ALTER TABLE [dbo].[UtilResponsibility]  WITH CHECK ADD  CONSTRAINT [FK_UTILRESPONSIBILITY_RELATIONSHIP_74_UTILROLES] FOREIGN KEY([IDRole])
REFERENCES [dbo].[UtilRoles] ([IDRole])
GO
ALTER TABLE [dbo].[UtilResponsibility] CHECK CONSTRAINT [FK_UTILRESPONSIBILITY_RELATIONSHIP_74_UTILROLES]
GO
ALTER TABLE [dbo].[UtilResponsibilityRules]  WITH CHECK ADD  CONSTRAINT [FK_UTILRESPONSIBILITYRULES_RELATIONSHIP_106_UTILRULES] FOREIGN KEY([IDRule])
REFERENCES [dbo].[UtilRules] ([IDRule])
GO
ALTER TABLE [dbo].[UtilResponsibilityRules] CHECK CONSTRAINT [FK_UTILRESPONSIBILITYRULES_RELATIONSHIP_106_UTILRULES]
GO
ALTER TABLE [dbo].[UtilResponsibilityRules]  WITH CHECK ADD  CONSTRAINT [FK_UTILRESPONSIBILITYRULES_RELATIONSHIP_72_UTILRESPONSIBILITY] FOREIGN KEY([IDResponsibility])
REFERENCES [dbo].[UtilResponsibility] ([IDResponsibility])
GO
ALTER TABLE [dbo].[UtilResponsibilityRules] CHECK CONSTRAINT [FK_UTILRESPONSIBILITYRULES_RELATIONSHIP_72_UTILRESPONSIBILITY]
GO
ALTER TABLE [dbo].[UtilRolesFunction]  WITH CHECK ADD  CONSTRAINT [FK_UTILROLESFUNCTION_RELATIONSHIP_75_UTILROLES] FOREIGN KEY([IDRole])
REFERENCES [dbo].[UtilRoles] ([IDRole])
GO
ALTER TABLE [dbo].[UtilRolesFunction] CHECK CONSTRAINT [FK_UTILROLESFUNCTION_RELATIONSHIP_75_UTILROLES]
GO
ALTER TABLE [dbo].[UtilRolesFunction]  WITH CHECK ADD  CONSTRAINT [FK_UTILROLESFUNCTION_RELATIONSHIP_88_UTILFUNCTIONS] FOREIGN KEY([IDFunction])
REFERENCES [dbo].[UtilFunctions] ([IDFunction])
GO
ALTER TABLE [dbo].[UtilRolesFunction] CHECK CONSTRAINT [FK_UTILROLESFUNCTION_RELATIONSHIP_88_UTILFUNCTIONS]
GO
ALTER TABLE [dbo].[UtilTransactionLogs]  WITH CHECK ADD  CONSTRAINT [FK_UTILTRANSACTIONLOGS_RELATIONSHIP_79_UTILFLOWS] FOREIGN KEY([IDFlow])
REFERENCES [dbo].[UtilFlows] ([IDFlow])
GO
ALTER TABLE [dbo].[UtilTransactionLogs] CHECK CONSTRAINT [FK_UTILTRANSACTIONLOGS_RELATIONSHIP_79_UTILFLOWS]
GO
ALTER TABLE [dbo].[UtilUsersResponsibility]  WITH CHECK ADD  CONSTRAINT [FK_UTILUSERSRESPONSIBILITY_RELATIONSHIP_104_MSTADTEMP] FOREIGN KEY([UserAD])
REFERENCES [dbo].[MstADTemp] ([UserAD])
GO
ALTER TABLE [dbo].[UtilUsersResponsibility] CHECK CONSTRAINT [FK_UTILUSERSRESPONSIBILITY_RELATIONSHIP_104_MSTADTEMP]
GO
ALTER TABLE [dbo].[UtilUsersResponsibility]  WITH CHECK ADD  CONSTRAINT [FK_UTILUSERSRESPONSIBILITY_RELATIONSHIP_90_UTILRESPONSIBILITY] FOREIGN KEY([IDResponsibility])
REFERENCES [dbo].[UtilResponsibility] ([IDResponsibility])
GO
ALTER TABLE [dbo].[UtilUsersResponsibility] CHECK CONSTRAINT [FK_UTILUSERSRESPONSIBILITY_RELATIONSHIP_90_UTILRESPONSIBILITY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 235
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "BG"
            Begin Extent = 
               Top = 6
               Left = 273
               Bottom = 136
               Right = 470
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PS"
            Begin Extent = 
               Top = 6
               Left = 508
               Bottom = 136
               Right = 705
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PSL"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BrandCodeByLocationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BrandCodeByLocationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EquipmentRequestView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EquipmentRequestView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InventoryByStatusView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'InventoryByStatusView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[16] 4[45] 2[11] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaintenanceEquipmentFulfillmentView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaintenanceEquipmentFulfillmentView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "mmc"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mmi"
            Begin Extent = 
               Top = 6
               Left = 277
               Bottom = 136
               Right = 450
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 6
               Left = 488
               Bottom = 136
               Right = 689
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaintenanceItemConversionDestinationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MaintenanceItemConversionDestinationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 101
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 249
               Bottom = 101
               Right = 422
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 102
               Left = 38
               Bottom = 214
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MstMntcConvertGetItemDestinationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MstMntcConvertGetItemDestinationView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[27] 4[34] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MstPlantProductionGroupView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'MstPlantProductionGroupView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlanPlantIndividualCapacityByReference'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlanPlantIndividualCapacityByReference'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[28] 4[33] 2[14] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "pptpk"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 298
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlanPlantTargetProductionKelompokView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlanPlantTargetProductionKelompokView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TargetProductionUnitView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TargetProductionUnitView'
GO
USE [master]
GO
ALTER DATABASE [SKT-IS-V1.1.7] SET  READ_WRITE 
GO
