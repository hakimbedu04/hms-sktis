/****** Object:  StoredProcedure [dbo].[EMSSourceDataBrandCodeView]    Script Date: 5/18/2016 12:07:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Create date: 20-04-2016
-- Description:	EMS Source Data
-- Edited by: TIM HM Sampoerna

-- =============================================	

-- =============================================
-- Author:		Azka
-- Create date: 5/18/2016
-- Description:	remove [SKT_DEV]
-- =============================================

ALTER Procedure [dbo].[EMSSourceDataBrandCodeView]	
(
	@LocationCode varchar(20),	
	@start Date,
    @enddate Date
    )
AS
Begin
  IF @LocationCode = 'SKT'
  Begin
  SELECT Distinct BrandCode
  FROM [dbo].[EMSSourceDataBrandView]
  where IDMstWeek between  (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @start >= StartDate  and @start <= EndDate)  and (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @enddate >= StartDate  and @enddate <= EndDate) 
  order by BrandCode
  End
  Else IF @LocationCode = 'PLNT'
  Begin
  SELECT Distinct BrandCode
  FROM [dbo].[EMSSourceDataBrandView]
  where IDMstWeek between  (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @start >= StartDate  and @start <= EndDate)  and (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @enddate >= StartDate  and @enddate <= EndDate) and ParentLocationCode = @LocationCode
  order by BrandCode
  End
  Else IF @LocationCode = 'TPO'
  Begin
  SELECT Distinct BrandCode 
  FROM [dbo].[EMSSourceDataBrandView]
  where IDMstWeek between  (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @start >= StartDate  and @start <= EndDate)  and (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @enddate >= StartDate  and @enddate <= EndDate) and ParentLocationCode like '%REG%'
  order by BrandCode
  End
  Else IF @LocationCode = 'REG1' OR @LocationCode = 'REG2' OR @LocationCode = 'REG3' OR @LocationCode = 'REG4'
  Begin
  SELECT Distinct BrandCode 
  FROM [dbo].[EMSSourceDataBrandView]
  where IDMstWeek between  (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @start >= StartDate  and @start <= EndDate)  and (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @enddate >= StartDate  and @enddate <= EndDate) and ParentLocationCode = @LocationCode
  order by BrandCode
  End
  Else 
  Begin
  SELECT Distinct BrandCode 
  FROM [dbo].[EMSSourceDataBrandView]
  where IDMstWeek between  (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @start >= StartDate  and @start <= EndDate)  and (select IDMstWeek
  FROM [dbo].[Mstgenweek] 
  where @enddate >= StartDate  and @enddate <= EndDate) and LocationCode = @LocationCode
  order by BrandCode 
  End
  
 End
 
 
 
  

