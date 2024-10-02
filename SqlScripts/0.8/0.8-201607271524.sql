--Description   : Util User Responsibility Role
--Author		: Indra Permana
--Ticket		: http://tp.voxteneo.co.id/entity/8511
--Date			: 2016-07-27

CREATE VIEW dbo.UtilUserResponsibilitiesRoleView
AS
SELECT uur.IDResponsibility
	  ,ur.ResponsibilityName
      ,uur.[UserAD]
	  ,uro.RolesCode
	  ,uro.RolesName
      ,uur.[EffectiveDate]
      ,uur.[ExpiredDate]
      ,uur.[CreatedDate]
      ,uur.[CreatedBy]
      ,uur.[UpdatedDate]
      ,uur.[UpdatedBy]
  FROM [dbo].[UtilUsersResponsibility] uur
  INNER JOIN dbo.UtilResponsibility ur on uur.IDResponsibility = ur.IDResponsibility
  INNER JOIN dbo.UtilRoles uro on ur.IDRole = uro.IDRole

GO

