using System.ComponentModel;

namespace HMS.SKTIS.Core
{
    public abstract partial class ExceptionCodes
    {
        /// <summary>
        /// Enum ExceptionCodes for user defined error codes
        /// </summary>
        public enum BaseExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            [Description("Delegation Responsibility Violation")]
            delegation_exception,
        }

        public enum BLLExceptions
        {
            [Description("An unknown error occured")]
            UnhandledException,

            [Description("Operation not allowed")]
            OperationNotAllowed,

            [Description("The data received is not valid")]
            InvalidData,

            [Description("The data received is not found")]
            DataNotFound,

            [Description("Data has been deleted or not exist on database")]
            DataNotFoundOrDeleted,

            [Description("The data is duplicate")]
            DuplicateEntity,

            [Description("Code/Key is already exist")]
            KeyExist,

            [Description("Cannot add with this effective and expired date")]
            ExpiredAndEffectiveExist,

            [Description("This data already exists")]
            DataAlreadyExist,

            [Description("Some field is mandatory")]
            Mandatory,

            [Description("The same Process Order Code/Key already exist.")]
            ProcessOrderExist,

            [Description("The same Process Group Code/Key already exist.")]
            ProcessGroupExist,

            [Description("The same Process Identifier Code/Key already exist.")]
            ProcessIdentifierExist,

            [Description("The same Location Code has been found. Please check again your entry")]
            LocationCodeAndParentSame,

            [Description("Location with Parent Location is TPO cannot be changed into another Parent Location.")]
            ParentTPOCanNotBeChanged,

            [Description("Day cannot be empty. Please check again your entry")]
            EmptyDay,

            [Description("Location and Status is not found. Please check again your enty")]
            LocationAndStatusEmpNotFound,

            [Description("Location already have process ID")]
            LocationSettingProcessExist,

            [Description("Transfer quantity larger than available stock")]
            TransferQtyLargerThanAvailableStock,

            [Description("Receive quantity larger than transfer quantity")]
            QtyReceiveLargerThanQtyTransfer,

            [Description("Cannot proceed before unit and process filled")]
            UnitAndProcessNotFound,

            [Description("There are already group shift data on this date range")]
            DateInRange,

            [Description("End date cannot be less than start date")]
            EndDateLessThanStartDate,

            [Description("Expiration date cannot be less than Effective date")]
            ExpirationDateLessThanEffectiveDate,

            [Description("Total maximum period for absenteeism {0} is {1}.")]
            TotalMaxDayAbsent,

            [Description("Date cannot less than current date")]
            LessThanCurrentDate,

            [Description("Absent Type not allowed")]
            AbsentTypeNotAllowed,

            [Description("Production Entry not found")]
            ProductionEntryNotFound,

            [Description("Error range date: There is already existing data in {0} to {1}")]
            RangeDateOverlapOrUnion,

            [Description("Absenteeism {0} is already reached total maximum period.")]
            MaxDayAbsentReached,

            [Description("The same transaction date already exist")]
            TransactionDateExist,

            [Description("Unit for selected Location data is not found in database.")]
            UnitForSelectedLocationNotExist,

            [Description("Start Date cannot be earlier than current existing Start Date.")]
            StartDateLessThanCurrentStartDate,

            [Description("End Date cannot be earlier than current existing End Date.")]
            EndDateLessThanCurrentEndDate,

            [Description("Material Code and Brand Group Code for selected is not found in Master Gen Material.")]
            MaterialCodeAndBrandGroupCodeNotExistinMstGenMaterial,

            [Description("Approved Quantity cannot larger than Requested Quantity")]
            RequestLessThanFulfillment,
            
            [Description("Approved Quantity already exist, cannot modify Requested Quantity")]
            ApprovedQtyAlreadyExist,

            [Description("Start Date Absent cannot be null")]
            StartDateAbsentNull,

            [Description("Entry is Submitted")]
            EblekSubmitted,

            [Description("Employee ID cannot be null")]
            EmployeeIDNull,

            [Description("Created By cannot be null")]
            CreatedByNull,

            [Description("Updated By cannot be null")]
            UpdatedByNull,

            [Description("Location Code cannot be null")]
            LocationCodeNull,

            [Description("Unit Code cannot be null")]
            UnitCodeNull,

            [Description("Group Code cannot be null")]
            GroupCodeNull,

            [Description("Shift cannot be null")]
            ShiftNull,

            [Description("Quantity Receive  value is not same with Quantity Transfer")]
            QtyRecieveIsNotSameWithQtyTransfer,

            [Description("Quantity Transit + Quantity Recieving large than Total Approved Quantity")]
            QtyTransitPlusReceiving,

            [Description("Absent Type cannot be null")]
            AbsentTypeNull,

            [Description("There are Multiskill Out Absents in Production Entry which Actual >= Min Stick/Hour")]
            ActualGreaterOrIsSameThanMinStickHour,

            [Description("Error range date: There is already existing data in Worker Assignment {0} to {1}")]
            RangeDateOverlapOrUnionWorkerAssignment,

            [Description("Error range date: There is already existing data in Production Entry {0} to {1}")]
            RangeDateOverlapOrUnionProductionEntry,

            [Description("Generate Data H-1 First !!!")]
            PreviousDateExeByProcessNull,

            [Description("Production Entry Source does not exist, Please Check Again.")]
            SourceProductionEntryVerificationNULL,

            [Description("Not exist in Master Item Location, please check again.")]
            ForeignKeyErrorItemDisposal,

            [Description("Brand Group with the same Material Code is already exist, please check again your entry.")]
            BrandGroupMaterialCodeExistBrandGroupMaterial,

            [Description("Last Payroll Date is Undefined.")]
            UndefinedLastPayrollDate,

            [Description("Cancel all Submitted TPO Production entry in selected week to continue!!!")]
            SubmittedTPOEntry,

            [Description("Cancel all Submitted Plant Production entry in selected week to continue!!!")]
            SubmittedPlantEntry
        }

        /// <summary>
        /// Security Exceptions for wcf responses
        /// </summary>
        public enum SecurityExceptions
        {
            AccessDenied,
            AuthorizationDenied,
            AuthenticationFailure
        }
    }
}