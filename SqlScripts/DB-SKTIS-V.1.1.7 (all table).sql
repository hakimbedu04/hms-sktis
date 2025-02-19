/****** Object:  StoredProcedure [dbo].[GetMstGenLocationsByParentCode]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  StoredProcedure [dbo].[VT_DBUpdatesExecScript]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[VT_DBUpdatesExecScript] (	@ScriptText VARCHAR(MAX)) AS	BEGIN		DECLARE @AddingScript VARCHAR(20) = 'AddingScriptTransaction'			EXEC (@ScriptText) END
GO
/****** Object:  StoredProcedure [dbo].[VT_DBUpdatesSaveScriptExecution]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[VT_DBUpdatesSaveScriptExecution] (	@ScriptName VARCHAR(100),	@MajorReleaseNumber VARCHAR(2),	@MinorReleaseNumber VARCHAR(2),	@ReturnValue VARCHAR(200) OUTPUT) AS	BEGIN		BEGIN TRY		    INSERT INTO VT_SchemaChangeLog (MajorReleaseNumber, MinorReleaseNumber, ScriptName) 		    VALUES (@MajorReleaseNumber, @MinorReleaseNumber, @ScriptName)			IF @@ERROR = 0				        SET @ReturnValue = 'OK: Script execution successfull'		END TRY		BEGIN CATCH			SET @ReturnValue = 'ERROR: ' + ERROR_MESSAGE()		END CATCH		 END
GO
/****** Object:  Table [dbo].[ExeActualWorkHours]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[ExeMaterialUsage]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeMaterialUsage](
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[ExePlantCKSubmit]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[ExePlantProductionEntry]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantProductionEntry](
	[ProductionEntryDate] [datetime] NOT NULL,
	[StartDateAbsent] [datetime] NULL,
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
	[ProductionEntryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantProductionEntryVerification]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[ExePlantWorkerAbsenteeism]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantWorkerAbsenteeism](
	[StartDateAbsent] [datetime] NOT NULL,
	[EmployeeID] [varchar](64) NULL,
	[AbsentType] [varchar](128) NULL,
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
 CONSTRAINT [PK_EXEPLANTWORKERABSENTEEISM] PRIMARY KEY NONCLUSTERED 
(
	[StartDateAbsent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantWorkerAssignment]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantWorkerAssignment](
	[SourceGroupCode] [varchar](4) NULL,
	[SourceUnitCode] [varchar](4) NULL,
	[SourceLocationCode] [varchar](8) NULL,
	[SourceProcessGroup] [varchar](16) NULL,
	[DestinationGroupCode] [varchar](4) NULL,
	[DestinationUnitCode] [varchar](4) NULL,
	[DestinationLocationCode] [varchar](8) NULL,
	[DestinationProcessGroup] [varchar](16) NULL,
	[BrandCode] [varchar](11) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExeProductionEntryRelease]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[ExeTPOProduction]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeTPOProduction](
	[TPKTPOStartProductionDate] [date] NULL,
	[TPOProductionEntryCode] [varchar](32) NULL,
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
	[UpdatedBy] [varchar](64) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentFulfillment]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcEquipmentItemConvert]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcEquipmentItemDisposal]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcEquipmentMovement]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[LocationCodeDestination] [varchar](8) NULL,
	[ItemCode] [varchar](20) NOT NULL,
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
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcEquipmentQualityInspection]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcEquipmentRepair]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcEquipmentRequest]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcInventory]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcInventory](
	[InventoryDate] [datetime] NOT NULL,
	[ItemStatus] [varchar](16) NOT NULL,
	[ItemCode] [varchar](20) NULL,
	[LocationCode] [varchar](8) NULL,
	[ItemType] [varchar](16) NULL,
	[UOM] [varchar](8) NULL,
	[BeginningStock] [int] NULL,
	[StockIn] [int] NULL,
	[StockOut] [int] NULL,
	[EndingStock] [int] NULL,
 CONSTRAINT [PK_MNTCINVENTORY] PRIMARY KEY NONCLUSTERED 
(
	[InventoryDate] ASC,
	[ItemStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcInventrotyAdjustment]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MntcInventrotyAdjustment](
	[ItemStatusTo] [varchar](16) NULL,
	[AdjustmentDate] [datetime] NOT NULL,
	[AdjustmentValue] [int] NULL,
	[AdjustmentType] [varchar](32) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBY] [varchar](64) NULL,
 CONSTRAINT [PK_MNTCINVENTROTYADJUSTMENT] PRIMARY KEY CLUSTERED 
(
	[AdjustmentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MntcRepairItemUsage]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MntcRequestToLocation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
 CONSTRAINT [PK_MNTCREQUESTTOLOCATION] PRIMARY KEY CLUSTERED 
(
	[FulFillmentDate] ASC,
	[RequestNumber] ASC,
	[LocationCode] ASC,
	[QtyFromLocation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MstADTemp]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenBrand]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrand](
	[BrandCode] [varchar](11) NOT NULL,
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[MstGenBrandGroup]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandGroup](
	[BrandGroupCode] [char](20) NOT NULL,
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
/****** Object:  Table [dbo].[MstGenBrandPackageItem]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandPackageItem](
	[ItemCode] [varchar](20) NOT NULL,
	[BrandGroupCode] [char](20) NOT NULL,
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
/****** Object:  Table [dbo].[MstGenBrandPkgMapping]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenBrandPkgMapping](
	[BrandGroupCodeSource] [char](20) NOT NULL,
	[BrandGroupCodeDestination] [char](20) NOT NULL,
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
/****** Object:  Table [dbo].[MstGenEmpStatus]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenHoliday]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenHoliday](
	[HolidayDate] [datetime] NOT NULL,
	[HolidayType] [varchar](11) NOT NULL,
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
/****** Object:  Table [dbo].[MstGenList]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenLocation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[Address] [varchar](32) NULL,
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
/****** Object:  Table [dbo].[MstGenLocStatus]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenMaterial]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenMaterial](
	[MaterialCode] [varchar](11) NOT NULL,
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[MstGenProcess]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenProcessSettings]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstGenProcessSettings](
	[IDProcess] [int] NOT NULL,
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[MstGenProcessSettingsLocation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenStandardHours]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstGenWeek]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstMntcConvert]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstMntcItem]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstMntcItemLocation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantAbsentType]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantEmpJobsDataAcv]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantEmpJobsDataAll]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantEmpUpd]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantEmpUpdTmp]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantProductionGroup]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstPlantUnit]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[MstTPOFeeRate]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOFeeRate](
	[EffectiveDate] [date] NOT NULL,
	[BrandGroupCode] [char](20) NOT NULL,
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
	[MemoPath] [varchar](32) NULL,
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
/****** Object:  Table [dbo].[MstTPOInfo]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[BankAccountNumber] [numeric](18, 0) NULL,
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
/****** Object:  Table [dbo].[MstTPOPackage]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MstTPOPackage](
	[LocationCode] [varchar](8) NOT NULL,
	[BrandGroupCode] [char](20) NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[ExpiredDate] [date] NOT NULL,
	[Package] [real] NOT NULL,
	[MemoRef] [varchar](32) NULL,
	[MemoFile] [varchar](32) NULL,
	[MemoPath] [varchar](32) NULL,
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
/****** Object:  Table [dbo].[MstTPOProductionGroup]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[PlanPlantGroupShift]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[EndDate] [datetime] NOT NULL,
	[Shift] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_PLANPLANTGROUPSHIFT] PRIMARY KEY NONCLUSTERED 
(
	[StartDate] ASC,
	[GroupCode] ASC,
	[ProcessGroup] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[Shift] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantIndividualCapacityReference]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantIndividualCapacityReference](
	[BrandCode] [varchar](11) NOT NULL,
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
 CONSTRAINT [PK_PLANPLANTINDIVIDUALCAPACITYREFERENCE] PRIMARY KEY CLUSTERED 
(
	[BrandCode] ASC,
	[EmployeeID] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantIndividualCapacityWorkHours]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantIndividualCapacityWorkHours](
	[BrandGroupCode] [char](20) NOT NULL,
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
/****** Object:  Table [dbo].[PlanPlantLineBalancing]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[PlanPlantTargetProductionKelompok]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantTargetProductionKelompok](
	[KPSWeek] [int] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[TPKCode] [varchar](32) NULL,
	[Shift] [int] NULL,
	[WorkerRegister] [int] NULL,
	[WorkerAvailable] [int] NULL,
	[WorkerAllocation] [int] NULL,
	[WIPStock] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[PercentAttendance1] [int] NULL,
	[PercentAttendance2] [int] NULL,
	[PercentAttendance3] [int] NULL,
	[PercentAttendance4] [int] NULL,
	[PercentAttendance5] [int] NULL,
	[PercentAttendance6] [int] NULL,
	[PercentAttendance7] [int] NULL,
	[HistoricalCapacityWorker1] [int] NULL,
	[HistoricalCapacityWorker2] [int] NULL,
	[HistoricalCapacityWorker3] [int] NULL,
	[HistoricalCapacityWorker4] [int] NULL,
	[HistoricalCapacityWorker5] [int] NULL,
	[HistoricalCapacityWorker6] [int] NULL,
	[HistoricalCapacityWorker7] [int] NULL,
	[HistoricalCapacityGroup1] [int] NULL,
	[HistoricalCapacityGroup2] [int] NULL,
	[HistoricalCapacityGroup3] [int] NULL,
	[HistoricalCapacityGroup4] [int] NULL,
	[HistoricalCapacityGroup5] [int] NULL,
	[HistoricalCapacityGroup6] [int] NULL,
	[HistoricalCapacityGroup7] [int] NULL,
	[TargetSystem1] [int] NULL,
	[TargetSystem2] [int] NULL,
	[TargetSystem3] [int] NULL,
	[TargetSystem4] [int] NULL,
	[TargetSystem5] [int] NULL,
	[TargetSystem6] [int] NULL,
	[TargetSystem7] [int] NULL,
	[TargetManual1] [int] NULL,
	[TargetManual2] [int] NULL,
	[TargetManual3] [int] NULL,
	[TargetManual4] [int] NULL,
	[TargetManual5] [int] NULL,
	[TargetManual6] [int] NULL,
	[TargetManual7] [int] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcesstWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [int] NULL,
	[TotalTargetManual] [int] NULL,
 CONSTRAINT [PK_PLANPLANTTARGETPRODUCTIONKELOMPOK] PRIMARY KEY NONCLUSTERED 
(
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
/****** Object:  Table [dbo].[PlanPlantWIPDetail]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanPlantWIPDetail](
	[KPSWeek] [int] NULL,
	[GroupCode] [varchar](4) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[UnitCode] [varchar](4) NULL,
	[LocationCode] [varchar](8) NULL,
	[BrandCode] [varchar](11) NULL,
	[WIPCurrentValue] [int] NULL,
	[WIPPreviousValue] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanPlantWorkerBalancing]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[PlanTargetProductionUnit]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[TPUCode] [varchar](256) NULL,
	[WorkerRegister] [int] NULL,
	[WorkerAvailable] [int] NULL,
	[WorkerAlocation] [int] NULL,
	[PercentAttendance1] [int] NULL,
	[PercentAttendance2] [int] NULL,
	[PercentAttendance3] [int] NULL,
	[PercentAttendance4] [int] NULL,
	[PercentAttendance5] [int] NULL,
	[PercentAttendance6] [int] NULL,
	[PercentAttendance7] [int] NULL,
	[HistoricalCapacityWorker1] [int] NULL,
	[HistoricalCapacityWorker2] [int] NULL,
	[HistoricalCapacityWorker3] [int] NULL,
	[HistoricalCapacityWorker4] [int] NULL,
	[HistoricalCapacityWorker5] [int] NULL,
	[HistoricalCapacityWorker6] [int] NULL,
	[HistoricalCapacityWorker7] [int] NULL,
	[HistoricalCapacityGroup1] [int] NULL,
	[HistoricalCapacityGroup2] [int] NULL,
	[HistoricalCapacityGroup3] [int] NULL,
	[HistoricalCapacityGroup4] [int] NULL,
	[HistoricalCapacityGroup5] [int] NULL,
	[HistoricalCapacityGroup6] [int] NULL,
	[HistoricalCapacityGroup7] [int] NULL,
	[TargetSystem1] [int] NULL,
	[TargetSystem2] [int] NULL,
	[TargetSystem3] [int] NULL,
	[TargetSystem4] [int] NULL,
	[TargetSystem5] [int] NULL,
	[TargetSystem6] [int] NULL,
	[TargetSystem7] [int] NULL,
	[TargetManual1] [int] NULL,
	[TargetManual2] [int] NULL,
	[TargetManual3] [int] NULL,
	[TargetManual4] [int] NULL,
	[TargetManual5] [int] NULL,
	[TargetManual6] [int] NULL,
	[TargetManual7] [int] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcessWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [int] NULL,
	[TotalTargetManual] [int] NULL,
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
	[UnitCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanTmpWeeklyProductionPlanning]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[ValueWPP] [real] NULL,
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
/****** Object:  Table [dbo].[PlanTPOTargetProductionKelompok]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlanTPOTargetProductionKelompok](
	[TPKTPOStartProductionDate] [date] NOT NULL,
	[ProdGroup] [char](4) NULL,
	[ProcessGroup] [varchar](16) NULL,
	[LocationCode] [varchar](8) NULL,
	[TPKCode] [varchar](256) NULL,
	[StatusEmp] [varchar](16) NULL,
	[WorkerRegister] [int] NULL,
	[WIPStock] [int] NULL,
	[KPSWeek] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
	[UploadedBy] [varchar](64) NOT NULL,
	[PercentAttendance1] [int] NULL,
	[PercentAttendance2] [int] NULL,
	[PercentAttendance3] [int] NULL,
	[PercentAttendance4] [int] NULL,
	[PercentAttendance5] [int] NULL,
	[PercentAttendance6] [int] NULL,
	[PercentAttendance7] [int] NULL,
	[HistoricalCapacityWorker1] [int] NULL,
	[HistoricalCapacityWorker2] [int] NULL,
	[HistoricalCapacityWorker3] [int] NULL,
	[HistoricalCapacityWorker4] [int] NULL,
	[HistoricalCapacityWorker5] [int] NULL,
	[HistoricalCapacityWorker6] [int] NULL,
	[HistoricalCapacityWorker7] [int] NULL,
	[HistoricalCapacityGroup1] [int] NULL,
	[HistoricalCapacityGroup2] [int] NULL,
	[HistoricalCapacityGroup3] [int] NULL,
	[HistoricalCapacityGroup4] [int] NULL,
	[HistoricalCapacityGroup5] [int] NULL,
	[HistoricalCapacityGroup6] [int] NULL,
	[HistoricalCapacityGroup7] [int] NULL,
	[TargetSystem1] [int] NULL,
	[TargetSystem2] [int] NULL,
	[TargetSystem3] [int] NULL,
	[TargetSystem4] [int] NULL,
	[TargetSystem5] [int] NULL,
	[TargetSystem6] [int] NULL,
	[TargetSystem7] [int] NULL,
	[TargetManual1] [int] NULL,
	[TargetManual2] [int] NULL,
	[TargetManual3] [int] NULL,
	[TargetManual4] [int] NULL,
	[TargetManual5] [int] NULL,
	[TargetManual6] [int] NULL,
	[TargetManual7] [int] NULL,
	[ProcessWorkHours1] [int] NULL,
	[ProcessWorkHours2] [int] NULL,
	[ProcessWorkHours3] [int] NULL,
	[ProcesstWorkHours4] [int] NULL,
	[ProcessWorkHours5] [int] NULL,
	[ProcessWorkHours6] [int] NULL,
	[ProcessWorkHours7] [int] NULL,
	[TotalWorkhours] [int] NULL,
	[TotalTargetSystem] [int] NULL,
	[TotalTargetManual] [int] NULL,
 CONSTRAINT [PK_PLANTPOTARGETPRODUCTIONKELOMPOK] PRIMARY KEY NONCLUSTERED 
(
	[TPKTPOStartProductionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlanWeeklyProductionPlanning]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[WPPCode] [varchar](256) NULL,
	[ValueWPP] [real] NULL,
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
/****** Object:  Table [dbo].[ProductAdjustment]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[ProductionCard]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductionCard](
	[RevisionType] [int] NOT NULL,
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[TPOFeeCalculation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[TPOFeeCalculationPlan]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[TPOFeeGLAccount]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[TPOFeeHdr]    Script Date: 8/27/2015 10:27:35 AM ******/
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
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[TPOFeeHdrPlan]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TPOFeeHdrPlan](
	[KPSYear] [int] NOT NULL,
	[KPSWeek] [int] NOT NULL,
	[BrandGroupCode] [char](20) NULL,
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
/****** Object:  Table [dbo].[TPOFeeProductionDaily]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[TPOFeeProductionDailyPlan]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilDelegation]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilFlows]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilFunctions]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilResponsibility]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilResponsibilityRules]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilRoles]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilRolesFunction]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilRules]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilTransactionLogs]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[UtilUsersResponsibility]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  Table [dbo].[VT_SchemaChangeLog]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  UserDefinedFunction [dbo].[GetLocations]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  View [dbo].[MstMntcConvertGetItemDestinationView]    Script Date: 8/27/2015 10:27:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MstMntcConvertGetItemDestinationView]
AS
SELECT        A.ItemCode, B.ItemCode AS ItemCodeDest, B.ItemDescription, C.QtyConvert
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
/****** Object:  View [dbo].[MstPlantUnitView]    Script Date: 8/27/2015 10:27:35 AM ******/
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
/****** Object:  View [dbo].[UtilRolesFunctionView]    Script Date: 8/27/2015 10:27:35 AM ******/
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
ALTER TABLE [dbo].[ExePlantProductionEntry]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS] FOREIGN KEY([StartDateAbsent])
REFERENCES [dbo].[ExePlantWorkerAbsenteeism] ([StartDateAbsent])
GO
ALTER TABLE [dbo].[ExePlantProductionEntry] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS]
GO
ALTER TABLE [dbo].[ExePlantProductionEntryVerification]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRYV_RELATIONSHIP_60_PLANPLANTTARGETPRODUCTIO] FOREIGN KEY([KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode])
REFERENCES [dbo].[PlanPlantTargetProductionKelompok] ([KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode])
GO
ALTER TABLE [dbo].[ExePlantProductionEntryVerification] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRYV_RELATIONSHIP_60_PLANPLANTTARGETPRODUCTIO]
GO
ALTER TABLE [dbo].[ExePlantProductionEntryVerification]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRYV_RELATIONSHIP_65_EXEPLANTPRODUCTIONENTRY] FOREIGN KEY([ProductionEntryDate])
REFERENCES [dbo].[ExePlantProductionEntry] ([ProductionEntryDate])
GO
ALTER TABLE [dbo].[ExePlantProductionEntryVerification] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRYV_RELATIONSHIP_65_EXEPLANTPRODUCTIONENTRY]
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
ALTER TABLE [dbo].[ExePlantWorkerAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_RELATIONSHIP_50_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[ExePlantWorkerAssignment] CHECK CONSTRAINT [FK_EXEPLANTWORKERASSIGNMENT_RELATIONSHIP_50_MSTGENBRAND]
GO
ALTER TABLE [dbo].[ExeProductionEntryRelease]  WITH CHECK ADD  CONSTRAINT [FK_EXEPRODUCTIONENTRYRELEAS_RELATIONSHIP_129_EXEPLANTPRODUCTIONENTRYV] FOREIGN KEY([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
REFERENCES [dbo].[ExePlantProductionEntryVerification] ([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
GO
ALTER TABLE [dbo].[ExeProductionEntryRelease] CHECK CONSTRAINT [FK_EXEPRODUCTIONENTRYRELEAS_RELATIONSHIP_129_EXEPLANTPRODUCTIONENTRYV]
GO
ALTER TABLE [dbo].[ExeTPOProduction]  WITH CHECK ADD  CONSTRAINT [FK_EXETPOPRODUCTION_REFERENCE_137_PLANTPOTARGETPRODUCTIONK] FOREIGN KEY([TPKTPOStartProductionDate])
REFERENCES [dbo].[PlanTPOTargetProductionKelompok] ([TPKTPOStartProductionDate])
GO
ALTER TABLE [dbo].[ExeTPOProduction] CHECK CONSTRAINT [FK_EXETPOPRODUCTION_REFERENCE_137_PLANTPOTARGETPRODUCTIONK]
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
ALTER TABLE [dbo].[MntcEquipmentMovement]  WITH CHECK ADD  CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_REFERENCE_132_MSTGENLOCATION] FOREIGN KEY([LocationCodeDestination])
REFERENCES [dbo].[MstGenLocation] ([LocationCode])
GO
ALTER TABLE [dbo].[MntcEquipmentMovement] CHECK CONSTRAINT [FK_MNTCEQUIPMENTMOVEMENT_REFERENCE_132_MSTGENLOCATION]
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
ALTER TABLE [dbo].[PlanPlantGroupShift]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTGROUPSHIFT_RELATIONSHIP_56_MSTPLANTPRODUCTIONGROUP] FOREIGN KEY([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
REFERENCES [dbo].[MstPlantProductionGroup] ([GroupCode], [UnitCode], [LocationCode], [ProcessGroup])
GO
ALTER TABLE [dbo].[PlanPlantGroupShift] CHECK CONSTRAINT [FK_PLANPLANTGROUPSHIFT_RELATIONSHIP_56_MSTPLANTPRODUCTIONGROUP]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_REFERENCE_124_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_REFERENCE_124_MSTPLANTUNIT]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_82_MSTPLANTEMPJOBSDATAALL] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MstPlantEmpJobsDataAll] ([EmployeeID])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_82_MSTPLANTEMPJOBSDATAALL]
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_84_MSTGENBRAND] FOREIGN KEY([BrandCode])
REFERENCES [dbo].[MstGenBrand] ([BrandCode])
GO
ALTER TABLE [dbo].[PlanPlantIndividualCapacityReference] CHECK CONSTRAINT [FK_PLANPLANTINDIVIDUALCAPAC_RELATIONSHIP_84_MSTGENBRAND]
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
ALTER TABLE [dbo].[PlanPlantWIPDetail]  WITH CHECK ADD  CONSTRAINT [FK_PLANPLANTWIPDETAIL_REFERENCE_126_PLANPLANTTARGETPRODUCTIO] FOREIGN KEY([KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode])
REFERENCES [dbo].[PlanPlantTargetProductionKelompok] ([KPSWeek], [GroupCode], [ProcessGroup], [UnitCode], [LocationCode], [BrandCode])
GO
ALTER TABLE [dbo].[PlanPlantWIPDetail] CHECK CONSTRAINT [FK_PLANPLANTWIPDETAIL_REFERENCE_126_PLANPLANTTARGETPRODUCTIO]
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
REFERENCES [dbo].[PlanTmpWeeklyProductionPlanning] ([KPSYear], [KPSWeek], [BrandCode], [LocationCode])
GO
ALTER TABLE [dbo].[PlanTargetProductionUnit] CHECK CONSTRAINT [FK_PLANTARGETPRODUCTIONUNIT_REFERENCE_125_PLANTMPWEEKLYPRODUCTIONP]
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
ALTER TABLE [dbo].[ProductionCard]  WITH CHECK ADD  CONSTRAINT [FK_PRODUCTIONCARD_RELATIONSHIP_49_MSTGENBRANDGROUP] FOREIGN KEY([BrandGroupCode])
REFERENCES [dbo].[MstGenBrandGroup] ([BrandGroupCode])
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
