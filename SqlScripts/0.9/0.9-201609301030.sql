-- =============================================

-- Create date: 18-01-2016
-- Description:	EMS Source Data
-- Edited by: TIM HM Sampoerna

-- =============================================

-- =============================================
-- Author:		Azka
-- Create date: 5/18/2016
-- Description:	remove [SKT_DEV]
-- =============================================

-- =============================================
-- Author:		Wahyu
-- Create date: 30/09/2016
-- Description:	STAMPING -> PACKING
-- =============================================

ALTER PROCEDURE [dbo].[GetEMSSourceData]
	-- Add the parameters for the stored procedure here
	(
	@LocationCode varchar(20),
	@BrandCode varchar(20),
	@start Date,
    @enddate Date
    )
AS
BEGIN
	IF @LocationCode = 'SKT' and @BrandCode = 'All'
	Begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate >= @start and ProductionDate <= @enddate
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate >= @start and ProductionDate <= @enddate
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode 
	End
	ELSE IF @LocationCode = 'SKT' and @BrandCode != 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode 
	end
	ELSE If @LocationCode = 'PLNT' and @BrandCode = 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join[dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode = @LocationCode
	end	
	ELSE If @LocationCode = 'PLNT' and @BrandCode != 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and   ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode = @LocationCode
	end	
		ELSE If @LocationCode = 'TPO' and @BrandCode = 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode like '%REG%'
	end	
	ELSE If @LocationCode = 'TPO' and @BrandCode != 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode like '%REG%'
	end	
	ELSE If @LocationCode = 'REG1' or @LocationCode = 'REG2' or @LocationCode = 'REG3' or @LocationCode = 'REG4' and @BrandCode = 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode =  @LocationCode
	end	
	ELSE If @LocationCode = 'REG1' or @LocationCode = 'REG2' or @LocationCode = 'REG3' or @LocationCode = 'REG4' and @BrandCode != 'All'
	begin
	Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode						
			 where PLNT.ParentLocationCode =  @LocationCode
	end	
	ELSE IF @LocationCode != 'SKT' and @BrandCode = 'All'
	 Begin
			  Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate and LocationCode = @LocationCode 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate and LocationCode = @LocationCode 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode 
	End
	Else IF @LocationCode != 'SKT' and @BrandCode != 'All'
	Begin
			  Select Company,EMS.LocationCode,EMS.BrandGroupCode,EMS.BrandCode,DIS.Description,PackageQTY,ProducedQTY,UOM,ProductionDate From   (
			  SELECT Cast('3066' as varchar(10)) As Company ,Package.LocationCode,Package.BrandGroupCode ,Produced.BrandCode,PackageQTY,ProducedQTY,('STICK') As UOM,Package.ProductionDate
			  FROM ( (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as ProducedQTY ,ProductionDate   
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'CUTTING' and ProductionDate between @start and @enddate and LocationCode = @LocationCode And BrandCode = @BrandCode 
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate ) AS Produced  inner join
			  (SELECT LocationCode,BrandCode,BrandGroupCode,SUM( Production ) as PackageQTY ,ProductionDate  
			  FROM [dbo].[ExeReportByGroups] where ProcessGroup = 'PACKING' or ProcessGroup = 'WRAPPING' and ProductionDate between @start and @enddate and LocationCode = @LocationCode And BrandCode = @BrandCode
			  group by LocationCode,BrandCode,BrandGroupCode,ProductionDate) AS Package on Produced.LocationCode = Package.LocationCode and Produced.BrandCode = Package.BrandCode and Produced.ProductionDate = Package.ProductionDate
			 ) ) AS EMS inner join [dbo].[MstGenBrand] AS DIS on EMS.BrandCode = DIS.BrandCode AND EMS.BrandGroupCode = DIS.BrandGroupCode inner join [dbo].[MstGenLocation] PLNT on EMS.LocationCode = PLNT.LocationCode 
	End
  End



GO


