SELECT     MAX(genWeek.StartDate) AS ProductionStartDate, MAX(planwpp.KPSYear) AS KPSYear, MAX(planwpp.KPSWeek) AS KPSWeek, planwpp.LocationCode, 
                      MstGenBrand.BrandGroupCode,planwpp.BrandCode,( cast(MAX(planwpp.Value1) * " + (DT_STR,50,1252) @[User::MilionConvert] +" as decimal) /MstGenBrandGroup.StickPerBox) as WPP 
FROM         MstGenBrandGroup INNER JOIN
                      MstGenBrand ON MstGenBrandGroup.BrandGroupCode = MstGenBrand.BrandGroupCode RIGHT OUTER JOIN
                      PlanTmpWeeklyProductionPlanning AS planwpp INNER JOIN
                      MstGenWeek AS genWeek ON genWeek.Week = planwpp.KPSWeek AND genWeek.Year = planwpp.KPSYear ON 
                      MstGenBrand.BrandCode = planwpp.BrandCode
                      inner join
                      (SELECT    WPP.LocationCode, case when COUNT(1) > 1 then 1 else 0 end as SwitchBrand
						FROM         PlanTmpWeeklyProductionPlanning  as wpp INNER JOIN
											  MstGenBrand ON WPP.BrandCode = MstGenBrand.BrandCode                      
						group by WPP.KPSYear, WPP.KPSWeek, WPP.LocationCode, MstGenBrand.BrandGroupCode) as switchbrand on
                      planwpp.LocationCode = switchbrand.LocationCode
where switchbrand = 0                        
GROUP BY planwpp.LocationCode, planwpp.BrandCode, MstGenBrand.BrandGroupCode,MstGenBrandGroup.StickPerBox