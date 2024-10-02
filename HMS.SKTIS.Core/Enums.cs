using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Core
{
    public class Enums
    {
        /// <summary>
        /// MstGeneralList properties
        /// </summary>
        public enum MasterGeneralList
        {
            [Description("Brand Family")]
            BrandFamily,
            [Description("Process Class")]
            ProcessClass,
            [Description("Holiday Type")]
            HolidayType,
            [Description("CigUOM")]
            CigUOM,
            [Description("Shift")]
            Shift,
            [Description("Maintenance Item Type")]
            MtncItemType,
            [Description("MtrlUOM")]
            MtrlUOM,
            [Description("Mainteance Price Type")]
            MtncPrice,
            [Description("TPORank")]
            TPORank,
            [Description("Pack")]
            Pack,
            [Description("Class")]
            Class,
            [Description("MtncInvSts")]
            MtncInvSts,
            [Description("FunctionType")]
            FunctionType,
            [Description("Day")]
            Day,
            [Description("Day Type")]
            DayType,
            [Description("Adjustment Type")]
            MtncAdjType,
            [Description("Product Adjustment Type")]
            ProdAdjType
            //AdjType
        }

        /// <summary>
        /// enum for UOM
        /// </summary>
        public enum Uom
        {
            EA,
            KG
        }

        /// <summary>
        /// enum for dropdown value and dropdown text
        /// </summary>
        public enum DropdownOption
        {
            ListGroup,
            ListDetail,
            BrandGroupCode,
            LocationCode,
            LocationName
        }

        public enum ResponseType
        {
            Success,
            Error
        }

        public enum ExcelTemplate
        {
            MstGeneralList,
            MstMaintenanceItem,
            MstProcess,
            MstItemLocation,
            MstBrand,
            MstBrandGroup,
            MstStandardHour,
            MstLocation,
            MstTPO,
            MstTPOPackage,
            MstPackageEquipment,
            MstTPOFeeRate,
            MstBrandGroupMaterial,
            MstPlantUnit,
            MstEmployeeJobsData,
            MstPlantProductionGroup,
            MstBrandPackageMapping,
            MstTPOProductionGroup,
            MstPlantAbsentType,
            EquipmentRequest,
            MstGenProcessSettingLocation,
            MstMntcConvertSts,
            MstGenProcessSettings,
            MstGenHoliday,
            MaintenanceItemDisposal,
            EquipmentTransfer,
            EquipmentReceive,
            MaintenanceItemConversion,
            MaintenanceItemConversionES,
            MaintenanceEquipmentQualityInspection,
            PlantEquipmentRepair,
            MaintenanceEquipmentStockReport,
            PlantEquipmentRepairTPO,
            MaintenanceExecutionInventory,
            MaintenanceExecutionInventoryAdjustment,
            TPOFeeExeActual,
            TPOFeeExeActualDetail,
            TpoFeeExePlan,
            TpoFeeExePlanDetail,
            TPOFeeExeApproval,
            TPOFeeAPOpen,
            TPOFeeAPClose,
            TPOFeeExeGLAccruedDetail,
            TPOFeeExeGLAccrued,
            TPOFeeReportsSummary,
            TPOFeeReportsProduction
        }

        public enum PlanningExcelTemplate
        {
            PlanWeeklyProductionPlanning,
            PlanningPlantIndividualCapacity,
            PlanningPlantIndividualCapacityByRefence,
            ProductionPlanTPU,
            PlantGroupShift,
            ReportProductionTarget,
            ReportSummaryProcessTargets,
            PlanningTPOTPK,
            EquipmentFulfillment,
            ProductionPlantTPK
        }

        public enum ExecuteExcelTemplate
        {
            ExecutePlantProductionEntry,
            ExecuteTPOProductionEntry,
            ExecuteTPOProductionEntryMultiskill,
            ExecuteTPOProductionEntryVerification,
            ExecutePlantMaterialUsageTobacco,
            ExecutePlantMaterialUsageNonTobacco,
            ExecutePlantActualWorkHours,
            ExecuteTPOActualWorkHours,
            ExecutePlantWorkerAssignment,
            ExecutePlantWorkerAbsenteeismPieceRate,
            ExecutePlantWorkerAbsenteeismDaily,
            ExecuteProductAdjustment,
            ExecutePlantProductionEntryVerification,
            ExeReportByGroup,
            ExeReportByStatus,
            ExeReportProductionStockByProcess,
            ExeEMSSourceData,
            ExeReportDailyProductionAchievement,
            ExeReportByProcess,
            UtilSecurityFunctions,
            UtilSecurityRules,
            UtilSecurityResponsibilities
        }

        public enum PlantWagesExcelTemplate
        {
            PlantWagesExecutionProductionCard,
            PlantWagesExecutionEblekRelease,
            PlantWagesEblekReleaseApproval,
            WagesReportAbsentsDetail,
            PlantWagesProductionCardCorrection,
            PlantWagesProductionCardApprovalDetail,
            WagesReportSummary,
            WagesReportAvailablePositionNumber,
            WagesReportAbsentsEmployee,
            WagesReportAbsentsEmployeeDetail
        }

        public enum TpoFeeExcelTemplate
        {
            TPOFeeReportsPackage
        }

        public enum Day
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        public enum Years
        {
            Start = 2012,
            End = 5
        }

        public enum LocationCode
        {
            SKT,
            PLNT,
            TPO,
            REG,
            REG1,
            REG2,
            REG3,
            REG4
        }

        public enum ProcessSettings
        {
            Mandor
        }

        public enum All
        {
            All
        }

        public enum SortOrder
        {
            Asc,
            Desc
        }

        public enum Languages
        {
            id = 1,
            en = 2
        }

        public enum ItemStatus
        {
            [Description("Ready To Use")]
            ReadyToUse,
            [Description("In Transit")]
            InTransit,
            [Description("Bad Stock")]
            BadStock,
            [Description("On Used")]
            OnUsed,
            [Description("On Repair")]
            OnRepair,
            [Description("Quality Inspection")]
            QI,
            OnUse
        }

        public enum Shift
        {
            Shift1 = 1,
            Shift2 = 2
        }

        public enum ItemType
        {
            Equipment,
            Sparepart
        }
        public enum UnitCodeDefault
        {
            [Description("MTNC Maintenance")]
            MTNC,
            [Description("PROD Maintenance")]
            PROD,
            [Description("WHSE Maintenance")]
            WHSE,
            [Description("MNTC Maintenance")]
            MNTC,
        }

        public enum Conversion
        {
            Box,
            Stick,
            Pack
        }

        public enum CombineCode
        {
            WPP,
            TPU,
            TPK,
            FUL,
            EBL,
            WPC,
            FEE
        }

        public enum SKTAbsentCode
        {
            [Description("Pulang Pagi (Sakit)")]
            SLS,
            [Description("Pulang Pagi (Mendadak) > 4 jam")]
            SL4,
            [Description("Pulang Pagi Ijin")]
            SLP,
            [Description("Multiskill Out")]
            MO,
            [Description("Tugas Lain Perusahaan")]
            LP,
            [Description("Tugas Lain Organisasi")]
            LO,
            [Description("Sakit")]
            S,
            A,
            I,
            [Description("Tugas Luar")]
            TL,
            SB,
            SKR,
            PG,
            LP1,
            LL,
            C,
            CT,
            CH,
            T
            



        }

        public enum StatusEmployeeJobsData
        {
            Mandor = 4,
            PieceRate = 5
        }

        public enum ExcelFormat
        {
            [Description(".xls")]
            Xls,
            [Description(".xlsx")]
            Xlsx
        }

        public enum Process
        {
            Rolling,
            [Description("DAILY")]
            Daily,
            Wrapping,
            Stamping,
            Packing,
            Stickwrapping,
            Cutting
        }
        public enum PageName
        {
            [Description("Plant Production Entry")]
            PlantProductionEntry,
            ProductionEntryVerification,
            [Description("Production Entry Release")]
            EblekRelease,
            ProductionCardApprovalDetail,
            [Description("Production Card")]
            ProductionCard,
            [Description("Production Entry Release Approval")]
            EblekReleaseApproval,
            [Description("TPO Fee Actual")]
            TPOFeeActual,
            [Description("TPO Fee Actual Detail")]
            TPOFeeActualDetail,
            [Description("TPO Fee Approval")]
            ApprovalPage,
            [Description("Production Execution - TPO Production Entry")]
            TPOProductionEntry,
            TPOProductionEntryVerification,
            TPOFeeAP,
            TargetProductionUnit,
            TPOFeeGL,
            TPOTargetProductionGroup,
            [Description("Production Planning - Plant Target Production of Groups (TPK)")]
            PlantTargetProductionGroup,
            [Description("Mainenance")]
            EquipmentRequest,
            EquipmentFulfillment,
            MaintenanceEquipmentQualityInspection,
            [Description("Production Report by Process")]
            ProductionReportbyProcess,
            [Description("Daily Production Achievement")]
            DailyProductionAchievement,
            [Description("Exe Plant Worker Balancing")]
            LoadBalancing,
            [Description("P1Template")]
            APOPEN,
            APCLOSE

        }

        public enum PageNameEmail
        {
            [Description("Maintenance")]
            //spelling according to db and util flow excel
            EquipmentRequest,
            EquipmentFullfillment, 
            QualityInspection,
            [Description("EquipmentTransfer")]
            EquipmentTransfer,
            [Description("EquipmentReceive")]
            EquipmentReceive
        }

        public enum IdFlow
        {
            ProductionCardSubmitComplete = 23,
            ProductionCardApprovalComplete = 26,
            ProdEntryVerificationSubmitProdCard = 17,
            PlanningPlantTPUSubmit = 4,
            PlanningPlantTPUSave = 3,
            PlanningPlantTPKSubmit = 6,
            WPPTPKTPOSubmit = 31,
            TPOTPKSubmit = 33,
            ProductionEntryReleaseApprove = 29,
            EblekReleaseApprovalFinal = 30,
            PlantProductionEntryVerificationSubmit = 19,
            EblekReleaseApproval = 29,
            EblekReleaseSendApproval = 27

        }

        public enum ButtonName
        {
            Add,
            Allocation,
            Approve,
            Authorize,
            BackToList,
            Calculate,
            [Description("Cancel Submit")]
            CancelSubmit,
            Complete,
            Delete,
            Detail,
            Excel,
            JKProses,
            P1Template,
            Print,
            Return,
            Revise,
            Save,
            SendApproval,
            SendNotification,
            Submit,
            UnitWorkHours,
            Upload,
            Usage,
            Verify,
            View,
            WIPStock,
            ApprovalPage
        }

        public enum StatusEmp
        {
            Resmi,
            Multiskill
        }

        public enum Role
        {
            SLT,
            RM,
            SKTHD
        }

        public enum DayType
        {
            [Description("Holiday")]
            Holiday,
            [Description("Non-Holiday")]
            NonHoliday
        }

        public enum ProductionFeeType
        {
            [Description("JKN")]
            JknBox,
            [Description("JL1")]
            Jl1Box,
            [Description("JL2")]
            Jl2Box,
            [Description("JL3")]
            Jl3Box,
            [Description("JL4")]
            Jl4Box,
            [Description("Biaya Produksi")]
            ProductionCost,
            [Description("Jasa Manajemen")]
            MaklonFee,
            [Description("Pajak Jasa Manajemen Sebesar 2%")]
            MaklonFeeTwoPercent,
            //[Description("Total Biaya Produksi & Jasa Manajemen")]
            [Description("Productivity Incentives")]
            ProductivityIncentives,
            [Description("Total Biaya Yang Harus Dibayarkan Ke MPS")]
            TotalCostMps,
            [Description("Pembayaran")]
            Cost,
            [Description("Sisa yang harus dibayar")]
            SisaYangHarusDibayar,
            [Description("PPN Biaya Produksi 10%")]
            PpnBiayaProduksiSepuluhPersen,
            [Description("PPN Jasa Manajemen 10%")]
            PpnJasaManajemenSepuluhPersen,
            [Description("Total Bayar")]
            TotalCost
        }

        public enum ReportTableau
        {
            ReportPlan,
            ReportTpo
        }

        public enum EmailSubject
        {
            [Description("Production Planning - Plant Target Production Units (TPU)")]
            PlantTPU,
            [Description("Production Planning - TPO Target Production of Groups (TPK)")]
            PlanTPOTPK,
            [Description("Production Planning - Plant Target Production of Groups (TPK)")]
            PlanTPK,
            [Description("TPO Fee Actual Detail")]
            TPOFeeActualDetail,
            [Description("TPO Fee Approval")]
            TPOFeeApproval,
            [Description("EquipmentRequest")]
            EquipmentRequest,
            [Description("EquipmentFulfillment")]
            EquipmentFulfillment,
            [Description("Production Card")]
            ProductionCard,
            [Description("Plant Production Entry")]
            PlantProductionEntry,
            [Description("TPO - Production Entry Verification")]
            TPOProductionEntryVerification,
            [Description("Wages Eblek Release (Production Entry Release)")]
            EblekRelease,
            [Description("Production Card Approval")]
            ProductionCardApprovalDetail,
            [Description("EquipmentTransfer")]
            EquipmentTransfer,
            [Description("Plant Production Entry Verification")]
            PlantProductionEntryVerification,
            
        }

        public enum MinValueAbsenType
        {
            [Description("Pulang Pagi (Ijin)")]
            SLPIjin,
            [Description("Pulang Pagi (Mendadak) > 4 Jam")]
            SLPMendadak,
            [Description("Pulang Pagi (Sakit)")]
            SLS,
            [Description("Pengurangan Garapan")]
            PG,

        }

        public enum AbsenTypeHoliday
        {
            [Description("Holiday Sakit")]
            HS,
            [Description("Holiday Ijin")]
            HI,
            [Description("Holiday Alpa")]
            HA,
            [Description("Holiday Cuti")]
            HC,

        }
        public enum ProdAdjType
        {
            [Description("0/Panel Cigarettes")]
            PanelCig,
            [Description("0/External Move")]
            ExtMove,
            [Description("2/Cut Cigarettes")]
            CutCig,
            [Description("3/Pack Cigarettes")]
            PackCig,
            [Description("4/Stamppack Cigarettes")]
            StampCig4,
            [Description("7/Wrap Cigarettes")]
            WrapCig,
            [Description("8/Stamppack Cigarettes")]
            StampCig8

        }

        public enum ProcessGroup
        {
            ROLLING,
            CUTTING,
            PACKING,
            FOILROLL,
            STICKWRAPPING,
            STAMPING,
            WRAPPING
        }

        public enum TpoFeeReportsSummaryProductionFeeType
        {
            [Description("Biaya Produksi")]
            BiayaProduksi,
            [Description("Jasa Manajemen")]
            JasaManajemen,
            [Description("Productivity Incentives")]
            ProductivityIncentives,
            [Description("Total Bayar")]
            TotalBayar
        }

        public enum FilterType
        {
            All,
            Daily,
            YearWeek,
            YearMonth,
            Yearly,
            Annualy,
            Period
        }

        public enum SaveType
        {
            New,
            Edit
        }

        public enum TabName
        {
            PieceRate,
            Daily,
            AllWorkHours,
            ByReference,
            EquipmenttoSparePart,
            SpareParttoSparePart,
            Process,
            Location
        }

        public enum StatusTPOFee
        { 
            DRAFT,
            OPEN,
            REVISED
        }
    }
}
