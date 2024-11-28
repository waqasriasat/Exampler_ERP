using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Models
{
  public class AppDBContext : DbContext
  {
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
    public DbSet<Settings_RoleType> Settings_RoleTypes { get; set; }
    public DbSet<Settings_BranchType> Settings_BranchTypes { get; set; }
    public DbSet<Settings_DepartmentType> Settings_DepartmentTypes { get; set; }
    public DbSet<Settings_SectionType> Settings_SectionTypes { get; set; }
    public DbSet<Settings_QualificationType> Settings_QualificationTypes { get; set; }
    public DbSet<Settings_SubQualificationType> Settings_SubQualificationTypes { get; set; }
    public DbSet<Settings_DesignationType> Settings_DesignationTypes { get; set; }
    public DbSet<Settings_SalaryType> Settings_SalaryTypes { get; set; }
    public DbSet<Settings_DeductionType> Settings_DeductionTypes { get; set; }
    public DbSet<Settings_VacationType> Settings_VacationTypes { get; set; }
    public DbSet<Settings_ProcessType> Settings_ProcessTypes { get; set; }
    public DbSet<Settings_EmployeeStatusType> Settings_EmployeeStatusTypes { get; set; }
    public DbSet<Settings_ActiveYNIDType> Settings_ActiveYNIDTypes { get; set; }
    public DbSet<Settings_DeleteYNIDType> Settings_DeleteYNIDTypes { get; set; }
    public DbSet<Settings_GenderType> Settings_GenderTypes { get; set; }
    public DbSet<Settings_MaritalStatusType> Settings_MaritalStatusTypes { get; set; }
    public DbSet<Settings_ReligionType> Settings_ReligionTypes { get; set; }
    public DbSet<Settings_CountryType> Settings_CountryTypes { get; set; }
    public DbSet<Settings_SalaryOption> Settings_SalaryOptions { get; set; }
    public DbSet<Settings_ContractType> Settings_ContractTypes { get; set; }
    public DbSet<Settings_DeductionValue> Settings_DeductionValues { get; set; }
    public DbSet<Settings_DeductionClass> Settings_DeductionClasses { get; set; }
    public DbSet<Settings_EndOfServiceReasonType> Settings_EndOfServiceReasonTypes { get; set; }
    public DbSet<Settings_EmployeeRequestType> Settings_EmployeeRequestTypes { get; set; }
    public DbSet<Settings_OverTimeRate> Settings_OverTimeRates { get; set; }
    public DbSet<Settings_OverTimeType> Settings_OverTimeTypes { get; set; }
    public DbSet<Settings_MonthType> Settings_MonthTypes { get; set; }
    public DbSet<Settings_AddionalAllowanceType> Settings_AddionalAllowanceTypes { get; set; }
    public DbSet<Settings_FixedDeductionType> Settings_FixedDeductionTypes { get; set; }
    public DbSet<Settings_HolidayType> Settings_HolidayTypes { get; set; }
    public DbSet<CR_ProcessTypeApproval> CR_ProcessTypeApprovals { get; set; }
    public DbSet<CR_ProcessTypeApprovalDetail> CR_ProcessTypeApprovalDetails { get; set; }
    public DbSet<CR_ProcessTypeApprovalDetailDoc> CR_ProcessTypeApprovalDetailDocs { get; set; }
    public DbSet<CR_ProcessTypeApprovalSetup> CR_ProcessTypeApprovalSetups { get; set; }
    public DbSet<CR_ProcessTypeForward> CR_ProcessTypeForwards { get; set; }
    public DbSet<CR_FaceAttendance> CR_FaceAttendances { get; set; }
    public DbSet<CR_ThumbAttendance> CR_ThumbAttendances { get; set; }
    public DbSet<CR_FaceAttendancePosted> CR_FaceAttendancePosteds { get; set; }
    public DbSet<CR_AccessRightsByRole> CR_AccessRightsByRoles { get; set; }
    public DbSet<HR_ThumbEnrollment> HR_ThumbEnrollments { get; set; }
    public DbSet<CR_User> CR_Users { get; set; }
    public DbSet<HR_DeductionSetup> HR_DeductionSetups { get; set; }
    public DbSet<HR_Employee> HR_Employees { get; set; }
    public DbSet<HR_Contract> HR_Contracts { get; set; }
    public DbSet<HR_Salary> HR_Salarys { get; set; }
    public DbSet<HR_SalaryDetail> HR_SalaryDetails { get; set; }
    public DbSet<HR_Joining> HR_Joinings { get; set; }
    public DbSet<HR_Deduction> HR_Deductions { get; set; }
    public DbSet<HR_BankAccount> HR_BankAccounts { get; set; }
    public DbSet<HR_ContractRenewal> HR_ContractRenewals { get; set; }
    public DbSet<HR_Applicant> HR_Applicants { get; set; }
    public DbSet<HR_WorkDay> HR_WorkDays { get; set; }
    public DbSet<HR_OverTime> HR_OverTimes { get; set; }
    public DbSet<HR_EndOfService> HR_EndOfServices { get; set; }
    public DbSet<HR_Vacation> HR_Vacations { get; set; }
    public DbSet<HR_EmployeeRequestTypeApproval> HR_EmployeeRequestTypeApprovals { get; set; }
    public DbSet<HR_EmployeeRequestTypeApprovalDetail> HR_EmployeeRequestTypeApprovalDetails { get; set; }
    public DbSet<HR_EmployeeRequestTypeApprovalDetailDoc> HR_EmployeeRequestTypeApprovalDetailDocs { get; set; }
    public DbSet<HR_EmployeeRequestTypeApprovalSetup> HR_EmployeeRequestTypeApprovalSetups { get; set; }
    public DbSet<HR_EmployeeRequestTypeForward> HR_EmployeeRequestTypeForwards { get; set; }
    public DbSet<HR_VacationSettle> HR_VacationSettles { get; set; }
    public DbSet<HR_DocumentUpload> HR_DocumentUploads { get; set; }
    public DbSet<HR_AddionalAllowance> HR_AddionalAllowances { get; set; }
    public DbSet<HR_AddionalAllowanceDetail> HR_AddionalAllowanceDetails { get; set; }
    public DbSet<HR_FixedDeduction> HR_FixedDeductions { get; set; }
    public DbSet<HR_FixedDeductionDetail> HR_FixedDeductionDetails { get; set; }
    public DbSet<HR_EmployeeEducation> HR_EmployeeEducations { get; set; }
    public DbSet<HR_EmployeeExperience> HR_EmployeeExperiences { get; set; }
    public DbSet<HR_MonthlyPayroll_FixedDeduction> HR_MonthlyPayroll_FixedDeductions { get; set; }
    public DbSet<HR_MonthlyPayroll_FixedDeductionDetail> HR_MonthlyPayroll_FixedDeductionDetails { get; set; }
    public DbSet<HR_MonthlyPayroll_Salary> HR_MonthlyPayroll_Salarys { get; set; }
    public DbSet<HR_MonthlyPayroll_SalaryDetail> HR_MonthlyPayroll_SalaryDetails { get; set; }
    public DbSet<HR_MonthlyPayroll> HR_MonthlyPayrolls { get; set; }
    public DbSet<HR_MonthlyPayrollPosted> HR_MonthlyPayrollPosteds { get; set; }
    public DbSet<HR_Holiday> HR_Holidays { get; set; }
    public DbSet<HR_GlobalSetting> HR_GlobalSettings { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<CR_AccessRightsByRole>().HasData(
         new CR_AccessRightsByRole() { AccessRightID = 1, ActionSOR = 1, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Humain Resources", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 2, ActionSOR = 2, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Finance", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 3, ActionSOR = 3, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Store Management", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 4, ActionSOR = 4, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Purchase", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 5, ActionSOR = 5, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Setup", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 6, ActionSOR = 6, ActionType = 1, ModuleID = 0, MenuID = 0, ActionName = "Setting", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },


        new CR_AccessRightsByRole() { AccessRightID = 7, ActionSOR = 51, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Branch", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 8, ActionSOR = 52, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Department", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 9, ActionSOR = 53, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Section", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 10, ActionSOR = 54, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Qualification", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 11, ActionSOR = 55, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "SubQualification", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 12, ActionSOR = 56, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Designation", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 13, ActionSOR = 57, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Salary Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 14, ActionSOR = 58, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Addional Allowance", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 15, ActionSOR = 59, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Deduction Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 16, ActionSOR = 60, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Deduction Setup", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 17, ActionSOR = 61, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Employee Status Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 18, ActionSOR = 62, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Over Time Rate", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 19, ActionSOR = 63, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Holiday Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 20, ActionSOR = 64, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Vacation Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 21, ActionSOR = 65, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Employee Request Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 22, ActionSOR = 66, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Employee Request Approval Setup", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 23, ActionSOR = 67, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Employee Request Forword", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 24, ActionSOR = 68, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Process Type", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 25, ActionSOR = 69, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Process Type Approval Setup", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 26, ActionSOR = 70, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Process Type Forword", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 27, ActionSOR = 71, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Roles", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 28, ActionSOR = 72, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Users", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 29, ActionSOR = 73, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Access Rights", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 30, ActionSOR = 74, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Copy Access Rights", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 31, ActionSOR = 75, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Evaluation Question", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 32, ActionSOR = 76, ActionType = 1, ModuleID = 1, MenuID = 1, ActionName = "Evaluation Templete", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },


        new CR_AccessRightsByRole() { AccessRightID = 33, ActionSOR = 101, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Applicant", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 34, ActionSOR = 102, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Employee", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 35, ActionSOR = 103, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Contract", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 36, ActionSOR = 104, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Salary", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 37, ActionSOR = 105, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Joining", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 38, ActionSOR = 106, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Bank Account", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 39, ActionSOR = 107, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Contract Renewal", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 40, ActionSOR = 108, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Card Print", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 41, ActionSOR = 109, ActionType = 1, ModuleID = 1, MenuID = 2, ActionName = "Thumb Enrollment", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },



        new CR_AccessRightsByRole() { AccessRightID = 42, ActionSOR = 151, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Deduction", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 43, ActionSOR = 152, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Over Time", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 44, ActionSOR = 153, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Addional Allowance", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 45, ActionSOR = 154, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Holiday", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 46, ActionSOR = 155, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Vacation Settle", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 47, ActionSOR = 156, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "End of Service", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 48, ActionSOR = 157, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Evaluation Assign", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 49, ActionSOR = 158, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Evaluation Approval", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 50, ActionSOR = 159, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Evaluation List", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 51, ActionSOR = 160, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Face Attendance Time Adjust", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 52, ActionSOR = 161, ActionType = 1, ModuleID = 1, MenuID = 3, ActionName = "Face Attendance Forwarding", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },



        new CR_AccessRightsByRole() { AccessRightID = 53, ActionSOR = 201, ActionType = 1, ModuleID = 1, MenuID = 4, ActionName = "Work Days", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 54, ActionSOR = 202, ActionType = 1, ModuleID = 1, MenuID = 4, ActionName = "Fixed Deduction", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 55, ActionSOR = 203, ActionType = 1, ModuleID = 1, MenuID = 4, ActionName = "Hously Salary Sheet", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 56, ActionSOR = 204, ActionType = 1, ModuleID = 1, MenuID = 4, ActionName = "Monthly Salary Sheet", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },


         new CR_AccessRightsByRole() { AccessRightID = 57, ActionSOR = 251, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Employee BioData", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 58, ActionSOR = 252, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Vacation Report", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 59, ActionSOR = 253, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Vacation Balance", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 60, ActionSOR = 254, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Contract Expiry", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 61, ActionSOR = 255, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "User Monitor", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 62, ActionSOR = 256, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Experince Certificate", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 63, ActionSOR = 257, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Education Document", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 64, ActionSOR = 258, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Evalution", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 65, ActionSOR = 259, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Process Approval Detail", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 66, ActionSOR = 260, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Employee Request Approval Detail", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 67, ActionSOR = 261, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Approved Monthly Salary Sheet", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 68, ActionSOR = 262, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Monthly PaySlip", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 },
        new CR_AccessRightsByRole() { AccessRightID = 69, ActionSOR = 263, ActionType = 1, ModuleID = 1, MenuID = 5, ActionName = "Face Attendance Report", _1 = 1, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0, _12 = 0, _13 = 0, _14 = 0, _15 = 0, _16 = 0, _17 = 0, _18 = 0, _19 = 0, _20 = 0, _21 = 0, _22 = 0, _23 = 0, _24 = 0, _25 = 0, _26 = 0, _27 = 0, _28 = 0, _29 = 0, _30 = 0, _31 = 0, _32 = 0, _33 = 0, _34 = 0, _35 = 0, _36 = 0, _37 = 0, _38 = 0, _39 = 0, _40 = 0, _41 = 0, _42 = 0, _43 = 0, _44 = 0, _45 = 0, _46 = 0, _47 = 0, _48 = 0, _49 = 0, _50 = 0 }
      );

      modelBuilder.Entity<HR_GlobalSetting>().HasData(

     new HR_GlobalSetting()
     {
       HR_GlobalSettingID= 1,
       LateAppID=1,
       LateTypeID=1,
       LateGraceMinute=15,
       LateValueofHours=12,
       EarlyGoingAppID=1,
       EarlyGoingTypeID=2,
       EarlyGoingGraceMinute=15,
       EarlyGoingValueofHours=12,
       EarlyComingAppID=1,
       EarlyComingGraceMinute=15,
       EarlyComingValueofHours=12,
       LateSeatingAppID=1,
       LateSeatingGraceMinute=15,
       LateSeatingValueofHours=12,
       AbsentAppID=1,
       AbsentTypeID=3,
       FlexibleDutyHourID=0,
       WorkingDayInWeek=5
     });

      modelBuilder.Entity<Settings_HolidayType>().HasData(
   new Settings_HolidayType() { HolidayTypeID = 1, HolidayTypeName = "New Year’s Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 2, HolidayTypeName = "Kashmir Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 3, HolidayTypeName = "Pakistan Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 4, HolidayTypeName = "Labour Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 5, HolidayTypeName = "Independence Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 6, HolidayTypeName = "Iqbal Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 7, HolidayTypeName = "Quaid-e-Azam Day / Christmas", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 8, HolidayTypeName = "Day After Christmas", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 9, HolidayTypeName = "Eid-ul-Fitr", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 10, HolidayTypeName = "Eid-ul-Adha", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 11, HolidayTypeName = "Ashura", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 12, HolidayTypeName = "Eid Milad-un-Nabi", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 13, HolidayTypeName = "Shab-e-Miraj", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 14, HolidayTypeName = "Shab-e-Barat", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 15, HolidayTypeName = "Basant", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 16, HolidayTypeName = "Sindhi Culture Day", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_HolidayType() { HolidayTypeID = 17, HolidayTypeName = "Baloch Culture Day", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_FixedDeductionType>().HasData(
   new Settings_FixedDeductionType() { FixedDeductionTypeID = 1, FixedDeductionTypeName = "Social Insurance", ActiveYNID = 1, DeleteYNID = 0 },
   new Settings_FixedDeductionType() { FixedDeductionTypeID = 2, FixedDeductionTypeName = "Income Tax", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_AddionalAllowanceType>().HasData(
    new Settings_AddionalAllowanceType() { AddionalAllowanceTypeID = 1, AddionalAllowanceTypeName = "Bonus", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_AddionalAllowanceType() { AddionalAllowanceTypeID = 2, AddionalAllowanceTypeName = "Month Perfection Bonus", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_AddionalAllowanceType() { AddionalAllowanceTypeID = 3, AddionalAllowanceTypeName = "Extra Duty", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_MonthType>().HasData(
    new Settings_MonthType() { MonthTypeID = 1, MonthTypeName = "January" },
    new Settings_MonthType() { MonthTypeID = 2, MonthTypeName = "February" },
    new Settings_MonthType() { MonthTypeID = 3, MonthTypeName = "March" },
    new Settings_MonthType() { MonthTypeID = 4, MonthTypeName = "April" },
    new Settings_MonthType() { MonthTypeID = 5, MonthTypeName = "May" },
    new Settings_MonthType() { MonthTypeID = 6, MonthTypeName = "June" },
    new Settings_MonthType() { MonthTypeID = 7, MonthTypeName = "July" },
    new Settings_MonthType() { MonthTypeID = 8, MonthTypeName = "August" },
    new Settings_MonthType() { MonthTypeID = 9, MonthTypeName = "September" },
    new Settings_MonthType() { MonthTypeID = 10, MonthTypeName = "October" },
    new Settings_MonthType() { MonthTypeID = 11, MonthTypeName = "November" },
    new Settings_MonthType() { MonthTypeID = 12, MonthTypeName = "December" }
);


      modelBuilder.Entity<Settings_OverTimeType>().HasData(
        new Settings_OverTimeType() { OverTimeTypeID = 1, OverTimeTypeName = "Basic Salary", ActiveYNID = 1, DeleteYNID = 0 },
        new Settings_OverTimeType() { OverTimeTypeID = 2, OverTimeTypeName = "Full Salary", ActiveYNID = 1, DeleteYNID = 0 }
        );

      modelBuilder.Entity<Settings_OverTimeRate>().HasData(
        new Settings_OverTimeRate() { OverTimeRateTypeID = 1, OverTimeRateValue = 1, ActiveYNID = 1, DeleteYNID = 0 },
        new Settings_OverTimeRate() { OverTimeRateTypeID = 2, OverTimeRateValue = 1.5, ActiveYNID = 1, DeleteYNID = 0 },
        new Settings_OverTimeRate() { OverTimeRateTypeID = 3, OverTimeRateValue = 2, ActiveYNID = 1, DeleteYNID = 0 }
        );

      modelBuilder.Entity<Settings_EmployeeRequestType>().HasData(
           // Attendance Requests
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 1, EmployeeRequestTypeName = "Overtime Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 2, EmployeeRequestTypeName = "Early Going Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 3, EmployeeRequestTypeName = "Late Coming Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 4, EmployeeRequestTypeName = "Timesheet Correction Request", ActiveYNID = 1, DeleteYNID = 0 },

           // Expense Requests
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 5, EmployeeRequestTypeName = "Travel Expenses Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 6, EmployeeRequestTypeName = "Accommodation Expense Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 7, EmployeeRequestTypeName = "Reimbursement Request", ActiveYNID = 1, DeleteYNID = 0 },

           // Shift & Work Requests
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 8, EmployeeRequestTypeName = "Shift Timing Change Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 9, EmployeeRequestTypeName = "Rotational Shift/Shift Swapping Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 10, EmployeeRequestTypeName = "Work from Home Request", ActiveYNID = 1, DeleteYNID = 0 },

           // Office Equipment Requests
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 11, EmployeeRequestTypeName = "Office Desk Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 12, EmployeeRequestTypeName = "Office Chair Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 13, EmployeeRequestTypeName = "Filing Cabinets Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 14, EmployeeRequestTypeName = "Storage Shelves Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 15, EmployeeRequestTypeName = "Bookshelves Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 16, EmployeeRequestTypeName = "Meeting Tables Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 17, EmployeeRequestTypeName = "Reception Desk Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 18, EmployeeRequestTypeName = "Side Tables Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 19, EmployeeRequestTypeName = "Footrest Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 20, EmployeeRequestTypeName = "Laptops Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 21, EmployeeRequestTypeName = "Desktop Computers Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 22, EmployeeRequestTypeName = "Monitors Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 23, EmployeeRequestTypeName = "LED Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 24, EmployeeRequestTypeName = "Keyboard Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 25, EmployeeRequestTypeName = "Mouse Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 26, EmployeeRequestTypeName = "External Hard Drives Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 27, EmployeeRequestTypeName = "USB Flash Drives Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 28, EmployeeRequestTypeName = "Webcam Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 29, EmployeeRequestTypeName = "Headset with Microphone Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 30, EmployeeRequestTypeName = "Printers Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 31, EmployeeRequestTypeName = "Scanners Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 32, EmployeeRequestTypeName = "Projector Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 33, EmployeeRequestTypeName = "External Speakers Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 34, EmployeeRequestTypeName = "External Microphone Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 35, EmployeeRequestTypeName = "UPS Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 36, EmployeeRequestTypeName = "Wi-Fi Router Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 37, EmployeeRequestTypeName = "Ethernet Cables Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 38, EmployeeRequestTypeName = "Switches and Hubs Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 39, EmployeeRequestTypeName = "LAN Cards Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 40, EmployeeRequestTypeName = "Modems Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 41, EmployeeRequestTypeName = "VPN Devices Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 42, EmployeeRequestTypeName = "Mouse Pads Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 43, EmployeeRequestTypeName = "Desk Lamps Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 44, EmployeeRequestTypeName = "Laptop Cooling Pads Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 45, EmployeeRequestTypeName = "Phone Stands Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 46, EmployeeRequestTypeName = "Pen Holders & Desk Organizers Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 47, EmployeeRequestTypeName = "Whiteboards/Notice Boards Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 48, EmployeeRequestTypeName = "Presentation Pointer/Clicker Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 49, EmployeeRequestTypeName = "Pens/Pencils Request", ActiveYNID = 1, DeleteYNID = 0 },
           new Settings_EmployeeRequestType { EmployeeRequestTypeID = 50, EmployeeRequestTypeName = "Notebooks/Sticky Notes Request", ActiveYNID = 1, DeleteYNID = 0 }
       );

      modelBuilder.Entity<Settings_EmployeeStatusType>().HasData(
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 1, EmployeeStatusTypeName = "Working", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 2, EmployeeStatusTypeName = "On Probation", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 3, EmployeeStatusTypeName = "On Leave", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 4, EmployeeStatusTypeName = "Resigned", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 5, EmployeeStatusTypeName = "Terminated", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 6, EmployeeStatusTypeName = "Retired", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 7, EmployeeStatusTypeName = "Contract Ended", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 8, EmployeeStatusTypeName = "Suspended", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 9, EmployeeStatusTypeName = "Deceased", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 10, EmployeeStatusTypeName = "Furloughed", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_EmployeeStatusType() { EmployeeStatusTypeID = 11, EmployeeStatusTypeName = "Transferred", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_ProcessType>().HasData(
     new Settings_ProcessType() { ProcessTypeID = 1, ProcessTypeName = "New User", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 2, ProcessTypeName = "New Employee", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 3, ProcessTypeName = "New Contract", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 4, ProcessTypeName = "Contract Renewal", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 5, ProcessTypeName = "End of Service", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 6, ProcessTypeName = "New Salary", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 7, ProcessTypeName = "Overtime", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 8, ProcessTypeName = "Addional Allowance", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 9, ProcessTypeName = "Deduction", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 10, ProcessTypeName = "Fixed Deduction", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 11, ProcessTypeName = "Vacation", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 12, ProcessTypeName = "Vacation Settle", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 13, ProcessTypeName = "PayRoll", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 14, ProcessTypeName = "Employee Request", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 15, ProcessTypeName = "Holiday Approval", ActiveYNID = 1, DeleteYNID = 0 }
     );

      modelBuilder.Entity<Settings_RoleType>().HasData(
     new Settings_RoleType() { RoleTypeID = 1, RoleTypeName = "Admin", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 2, RoleTypeName = "General Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 3, RoleTypeName = "ERP Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 4, RoleTypeName = "Finance Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 5, RoleTypeName = "Human Resources Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 6, RoleTypeName = "Procurement Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 7, RoleTypeName = "Inventory Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 8, RoleTypeName = "Sales Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 9, RoleTypeName = "Production Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 10, RoleTypeName = "Project Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 11, RoleTypeName = "Customer Service Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 12, RoleTypeName = "Quality Control Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 13, RoleTypeName = "IT Support Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 14, RoleTypeName = "Data Analyst", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 15, RoleTypeName = "Compliance Officer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 16, RoleTypeName = "Logistics Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 17, RoleTypeName = "Security Officer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 18, RoleTypeName = "Training Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 19, RoleTypeName = "Product Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 20, RoleTypeName = "Marketing Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 21, RoleTypeName = "Vendor/Supplier Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 22, RoleTypeName = "Change Management Lead", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 23, RoleTypeName = "Audit Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 24, RoleTypeName = "Legal Advisor", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 25, RoleTypeName = "Product Customization Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 26, RoleTypeName = "Vendor Relationship Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 27, RoleTypeName = "Business Analyst", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 28, RoleTypeName = "Data Migration Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 29, RoleTypeName = "Integration Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 30, RoleTypeName = "Support Desk Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 31, RoleTypeName = "Process Improvement Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 32, RoleTypeName = "Risk Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 33, RoleTypeName = "User Experience (UX) Designer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 34, RoleTypeName = "Master Data Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 35, RoleTypeName = "Budget Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_RoleType() { RoleTypeID = 36, RoleTypeName = "Product Development Manager", ActiveYNID = 1, DeleteYNID = 0 }
 );

      modelBuilder.Entity<CR_User>().HasData(

      new CR_User()
      {
        UserID = 1,
        UserName = "v0tnPv0i6sOwp4eLjWF4rA==", //ibs
        Password = "LiGbnDcHpE6eeqlmOScrOA==", //A@123456
        ActiveYNID = 1,
        DeleteYNID = 0,
        RoleTypeID = 1,
        EmployeeID = 0
      });
      modelBuilder.Entity<Settings_BranchType>().HasData(
      new Settings_BranchType() { BranchTypeID = 1, BranchTypeName = "Head Office", ActiveYNID = 1, DeleteYNID = 0 }
      );

      modelBuilder.Entity<Settings_DepartmentType>().HasData(
    new Settings_DepartmentType() { DepartmentTypeID = 1, DepartmentTypeName = "Human Resources (HR)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 2, DepartmentTypeName = "Finance", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 3, DepartmentTypeName = "Procurement", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 4, DepartmentTypeName = "Sales", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 5, DepartmentTypeName = "Marketing", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 6, DepartmentTypeName = "Operations", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 7, DepartmentTypeName = "Production/Manufacturing", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 8, DepartmentTypeName = "Logistics", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 9, DepartmentTypeName = "Information Technology (IT)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 10, DepartmentTypeName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 11, DepartmentTypeName = "Research and Development (R&D)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 12, DepartmentTypeName = "Quality Assurance", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 13, DepartmentTypeName = "Legal", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 14, DepartmentTypeName = "Compliance", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 15, DepartmentTypeName = "Public Relations (PR)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 16, DepartmentTypeName = "Administration", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 17, DepartmentTypeName = "Facilities Management", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 18, DepartmentTypeName = "Supply Chain Management", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 19, DepartmentTypeName = "Risk Management", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 20, DepartmentTypeName = "Project Management Office (PMO)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 21, DepartmentTypeName = "Business Development", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 22, DepartmentTypeName = "Strategy and Planning", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 23, DepartmentTypeName = "Product Development", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 24, DepartmentTypeName = "Training and Development", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 25, DepartmentTypeName = "Corporate Communications", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 26, DepartmentTypeName = "Internal Audit", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 27, DepartmentTypeName = "Data Analytics/Business Intelligence", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 28, DepartmentTypeName = "Security (including Physical and Cybersecurity)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 29, DepartmentTypeName = "Health and Safety", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 30, DepartmentTypeName = "Customer Relations Management (CRM)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 31, DepartmentTypeName = "Legal and Compliance", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 32, DepartmentTypeName = "Vendor Management", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 33, DepartmentTypeName = "Environment, Health, and Safety (EHS)", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 34, DepartmentTypeName = "Sustainability", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 },
    new Settings_DepartmentType() { DepartmentTypeID = 35, DepartmentTypeName = "Investor Relations", ActiveYNID = 1, DeleteYNID = 0, BranchTypeID = 1 }
);

      modelBuilder.Entity<Settings_SectionType>().HasData(
      new Settings_SectionType() { SectionTypeID = 1, SectionTypeName = "Recruitment", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 2, SectionTypeName = "Payroll", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 3, SectionTypeName = "Employee Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 4, SectionTypeName = "Training and Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 5, SectionTypeName = "Compensation and Benefits", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 6, SectionTypeName = "Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 7, SectionTypeName = "Performance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 1 },
      new Settings_SectionType() { SectionTypeID = 8, SectionTypeName = "Accounts Payable", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 9, SectionTypeName = "Accounts Receivable", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 10, SectionTypeName = "General Ledger", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 11, SectionTypeName = "Budgeting and Forecasting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 12, SectionTypeName = "Tax Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 13, SectionTypeName = "Financial Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 14, SectionTypeName = "Internal Audit", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 15, SectionTypeName = "Treasury", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 2 },
      new Settings_SectionType() { SectionTypeID = 16, SectionTypeName = "Vendor Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 17, SectionTypeName = "Contract Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 18, SectionTypeName = "Purchase Order Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 19, SectionTypeName = "Inventory Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 20, SectionTypeName = "Supplier Relationship Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 21, SectionTypeName = "Strategic Sourcing", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 3 },
      new Settings_SectionType() { SectionTypeID = 22, SectionTypeName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 23, SectionTypeName = "Sales Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 24, SectionTypeName = "Sales Analytics", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 25, SectionTypeName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 26, SectionTypeName = "Lead Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 27, SectionTypeName = "Territory Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 4 },
      new Settings_SectionType() { SectionTypeID = 28, SectionTypeName = "Digital Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 29, SectionTypeName = "Content Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 30, SectionTypeName = "Brand Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 31, SectionTypeName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 32, SectionTypeName = "Public Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 33, SectionTypeName = "Advertising", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 34, SectionTypeName = "Social Media Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 5 },
      new Settings_SectionType() { SectionTypeID = 35, SectionTypeName = "Process Optimization", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 36, SectionTypeName = "Operations Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 37, SectionTypeName = "Facility Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 38, SectionTypeName = "Workflow Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 39, SectionTypeName = "Resource Allocation", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 40, SectionTypeName = "Vendor Coordination", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 6 },
      new Settings_SectionType() { SectionTypeID = 41, SectionTypeName = "Production Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 42, SectionTypeName = "Quality Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 43, SectionTypeName = "Maintenance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 44, SectionTypeName = "Inventory Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 45, SectionTypeName = "Supply Chain Coordination", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 46, SectionTypeName = "Process Engineering", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 7 },
      new Settings_SectionType() { SectionTypeID = 47, SectionTypeName = "Transportation Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 48, SectionTypeName = "Warehouse Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 49, SectionTypeName = "Inventory Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 50, SectionTypeName = "Distribution", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 51, SectionTypeName = "Logistics Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 52, SectionTypeName = "Supplier Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 53, SectionTypeName = "Fleet Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 8 },
      new Settings_SectionType() { SectionTypeID = 54, SectionTypeName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 55, SectionTypeName = "Client Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 56, SectionTypeName = "Support Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 57, SectionTypeName = "Service Quality", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 58, SectionTypeName = "Customer Feedback", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 59, SectionTypeName = "Service Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 60, SectionTypeName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 61, SectionTypeName = "Client Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 62, SectionTypeName = "Service Innovations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 63, SectionTypeName = "Customer Experience", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 9 },
      new Settings_SectionType() { SectionTypeID = 64, SectionTypeName = "Technical Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 65, SectionTypeName = "IT Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 66, SectionTypeName = "Infrastructure", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 67, SectionTypeName = "Systems Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 68, SectionTypeName = "Network Administration", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 69, SectionTypeName = "Software Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 70, SectionTypeName = "Security Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 10 },
      new Settings_SectionType() { SectionTypeID = 71, SectionTypeName = "Product Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 72, SectionTypeName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 73, SectionTypeName = "Development Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 74, SectionTypeName = "Product Design", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 75, SectionTypeName = "Product Lifecycle", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 76, SectionTypeName = "Innovation Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 77, SectionTypeName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 78, SectionTypeName = "Market Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 79, SectionTypeName = "Business Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 11 },
      new Settings_SectionType() { SectionTypeID = 80, SectionTypeName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 81, SectionTypeName = "Sales Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 82, SectionTypeName = "Lead Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 83, SectionTypeName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 84, SectionTypeName = "Sales Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 85, SectionTypeName = "Market Penetration", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 86, SectionTypeName = "Revenue Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 87, SectionTypeName = "Contract Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 88, SectionTypeName = "Customer Contracts", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 89, SectionTypeName = "Customer Acquisition", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 12 },
      new Settings_SectionType() { SectionTypeID = 90, SectionTypeName = "Corporate Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 91, SectionTypeName = "Strategic Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 92, SectionTypeName = "Business Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 93, SectionTypeName = "Risk Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 94, SectionTypeName = "Corporate Governance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 95, SectionTypeName = "Policy Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 96, SectionTypeName = "Operational Excellence", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 97, SectionTypeName = "Performance Metrics", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 98, SectionTypeName = "Change Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 13 },
      new Settings_SectionType() { SectionTypeID = 99, SectionTypeName = "Compliance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 100, SectionTypeName = "Regulatory Affairs", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 101, SectionTypeName = "Audit & Controls", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 102, SectionTypeName = "Legal Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 103, SectionTypeName = "Ethics & Standards", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 104, SectionTypeName = "Compliance Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 105, SectionTypeName = "Data Protection", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 106, SectionTypeName = "Privacy Policies", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 107, SectionTypeName = "Insurance & Risk", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 14 },
      new Settings_SectionType() { SectionTypeID = 108, SectionTypeName = "Legal Affairs", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 109, SectionTypeName = "Contract Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 110, SectionTypeName = "Litigation Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 111, SectionTypeName = "Intellectual Property", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 112, SectionTypeName = "Employment Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 113, SectionTypeName = "Legal Advice", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 114, SectionTypeName = "Regulatory Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 115, SectionTypeName = "Corporate Legal", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 116, SectionTypeName = "Legal Documentation", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 117, SectionTypeName = "Environmental Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 15 },
      new Settings_SectionType() { SectionTypeID = 118, SectionTypeName = "Human Resources", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 119, SectionTypeName = "Talent Acquisition", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 120, SectionTypeName = "Employee Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 121, SectionTypeName = "Compensation & Benefits", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 122, SectionTypeName = "Training & Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 123, SectionTypeName = "Performance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 124, SectionTypeName = "HR Policies", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 125, SectionTypeName = "Employee Engagement", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 126, SectionTypeName = "Workplace Safety", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 127, SectionTypeName = "Organizational Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 16 },
      new Settings_SectionType() { SectionTypeID = 128, SectionTypeName = "Finance & Accounting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 129, SectionTypeName = "Financial Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 130, SectionTypeName = "Budgeting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 131, SectionTypeName = "Accounting Standards", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 132, SectionTypeName = "Tax Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 133, SectionTypeName = "Financial Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 134, SectionTypeName = "Audit & Assurance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 135, SectionTypeName = "Cash Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 136, SectionTypeName = "Investment Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 137, SectionTypeName = "Payroll Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 17 },
      new Settings_SectionType() { SectionTypeID = 138, SectionTypeName = "Marketing Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 139, SectionTypeName = "Brand Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 140, SectionTypeName = "Digital Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 141, SectionTypeName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 142, SectionTypeName = "Advertising", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 143, SectionTypeName = "Public Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 144, SectionTypeName = "Content Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 145, SectionTypeName = "Social Media", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 146, SectionTypeName = "Event Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 147, SectionTypeName = "Customer Outreach", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 18 },
      new Settings_SectionType() { SectionTypeID = 148, SectionTypeName = "Sales Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 149, SectionTypeName = "Sales Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 150, SectionTypeName = "Lead Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 151, SectionTypeName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 152, SectionTypeName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 153, SectionTypeName = "Revenue Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 154, SectionTypeName = "Client Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 155, SectionTypeName = "Sales Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 156, SectionTypeName = "Sales Performance", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 157, SectionTypeName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 158, SectionTypeName = "Pricing Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 159, SectionTypeName = "Competitive Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 160, SectionTypeName = "Sales Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 19 },
      new Settings_SectionType() { SectionTypeID = 161, SectionTypeName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 162, SectionTypeName = "Support Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 163, SectionTypeName = "Client Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 164, SectionTypeName = "Help Desk", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 165, SectionTypeName = "Customer Care", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 166, SectionTypeName = "Support Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 },
      new Settings_SectionType() { SectionTypeID = 167, SectionTypeName = "Service Improvement", ActiveYNID = 1, DeleteYNID = 0, DepartmentTypeID = 20 }
  );

      modelBuilder.Entity<Settings_QualificationType>().HasData(
          new Settings_QualificationType() { QualificationTypeID = 1, QualificationTypeName = "Matriculation", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_QualificationType() { QualificationTypeID = 2, QualificationTypeName = "Intermediate", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_QualificationType() { QualificationTypeID = 3, QualificationTypeName = "Bachelor’s Degree", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_QualificationType() { QualificationTypeID = 4, QualificationTypeName = "Master’s Degree", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_QualificationType() { QualificationTypeID = 5, QualificationTypeName = "MPhil", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_QualificationType() { QualificationTypeID = 6, QualificationTypeName = "Ph.D.", ActiveYNID = 1, DeleteYNID = 0 }
      );


      modelBuilder.Entity<Settings_SubQualificationType>().HasData(
    new Settings_SubQualificationType { SubQualificationTypeID = 1, SubQualificationTypeName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 1 },
    new Settings_SubQualificationType { SubQualificationTypeID = 2, SubQualificationTypeName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 1 },
    new Settings_SubQualificationType { SubQualificationTypeID = 3, SubQualificationTypeName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 1 },
    new Settings_SubQualificationType { SubQualificationTypeID = 4, SubQualificationTypeName = "Pre-Medical", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 2 },
    new Settings_SubQualificationType { SubQualificationTypeID = 5, SubQualificationTypeName = "Pre-Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 2 },
    new Settings_SubQualificationType { SubQualificationTypeID = 6, SubQualificationTypeName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 2 },
    new Settings_SubQualificationType { SubQualificationTypeID = 7, SubQualificationTypeName = "Humanities", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 2 },
    new Settings_SubQualificationType { SubQualificationTypeID = 8, SubQualificationTypeName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 3 },
    new Settings_SubQualificationType { SubQualificationTypeID = 9, SubQualificationTypeName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 3 },
    new Settings_SubQualificationType { SubQualificationTypeID = 10, SubQualificationTypeName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 3 },
    new Settings_SubQualificationType { SubQualificationTypeID = 11, SubQualificationTypeName = "Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 4 },
    new Settings_SubQualificationType { SubQualificationTypeID = 12, SubQualificationTypeName = "Business", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 4 },
    new Settings_SubQualificationType { SubQualificationTypeID = 13, SubQualificationTypeName = "Information Technology", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 4 },
    new Settings_SubQualificationType { SubQualificationTypeID = 14, SubQualificationTypeName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 5 },
    new Settings_SubQualificationType { SubQualificationTypeID = 15, SubQualificationTypeName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 5 },
    new Settings_SubQualificationType { SubQualificationTypeID = 16, SubQualificationTypeName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 5 },
    new Settings_SubQualificationType { SubQualificationTypeID = 17, SubQualificationTypeName = "Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 6 },
    new Settings_SubQualificationType { SubQualificationTypeID = 18, SubQualificationTypeName = "Business", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 6 },
    new Settings_SubQualificationType { SubQualificationTypeID = 19, SubQualificationTypeName = "Information Technology", ActiveYNID = 1, DeleteYNID = 0, QualificationTypeID = 6 }
);

      modelBuilder.Entity<Settings_DesignationType>().HasData(
    new Settings_DesignationType() { DesignationTypeID = 1, DesignationTypeName = "CEO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 2, DesignationTypeName = "COO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 3, DesignationTypeName = "CFO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 4, DesignationTypeName = "CIO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 5, DesignationTypeName = "CTO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 6, DesignationTypeName = "General Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 7, DesignationTypeName = "Regional Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 8, DesignationTypeName = "Department Head", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 9, DesignationTypeName = "Director", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 10, DesignationTypeName = "Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 11, DesignationTypeName = "Assistant Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 12, DesignationTypeName = "Team Lead", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 13, DesignationTypeName = "Supervisor", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 14, DesignationTypeName = "Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 15, DesignationTypeName = "Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 16, DesignationTypeName = "Officer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 17, DesignationTypeName = "IT Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 18, DesignationTypeName = "Systems Analyst", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 19, DesignationTypeName = "Database Administrator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 20, DesignationTypeName = "Network Administrator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 21, DesignationTypeName = "Software Developer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 22, DesignationTypeName = "Customer Service Representative", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 23, DesignationTypeName = "Administrative Assistant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 24, DesignationTypeName = "HR Assistant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 25, DesignationTypeName = "Accountant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 26, DesignationTypeName = "Sales Executive", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 27, DesignationTypeName = "Marketing Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 28, DesignationTypeName = "Business Development Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 29, DesignationTypeName = "Production Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 30, DesignationTypeName = "Supply Chain Analyst", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 31, DesignationTypeName = "Logistics Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 32, DesignationTypeName = "Quality Assurance Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 33, DesignationTypeName = "Compliance Officer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 34, DesignationTypeName = "Project Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_DesignationType() { DesignationTypeID = 35, DesignationTypeName = "Project Coordinator", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_SalaryType>().HasData(
 new Settings_SalaryType() { SalaryTypeID = 1, SalaryTypeName = "Basic Salary", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 2, SalaryTypeName = "House Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 3, SalaryTypeName = "Transport Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 4, SalaryTypeName = "Medical Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 5, SalaryTypeName = "Food Allowance", ActiveYNID = 1, DeleteYNID = 0 }
 );

      modelBuilder.Entity<Settings_DeductionType>().HasData(
        new Settings_DeductionType() { DeductionTypeID = 1, DeductionTypeName = "Late Coming", ActiveYNID = 1, DeleteYNID = 0 },
        new Settings_DeductionType() { DeductionTypeID = 2, DeductionTypeName = "Early Going", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 3, DeductionTypeName = "Absent", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 4, DeductionTypeName = "Social Security Contributions", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 5, DeductionTypeName = "Health Insurance Premiums", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 6, DeductionTypeName = "Pension Contributions", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 7, DeductionTypeName = "Loan Repayments", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 8, DeductionTypeName = "Union Dues", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 9, DeductionTypeName = "Garnishments", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 10, DeductionTypeName = "Absent Days or Leave Without Pay", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 11, DeductionTypeName = "Wage Advances", ActiveYNID = 1, DeleteYNID = 0 },
         new Settings_DeductionType() { DeductionTypeID = 12, DeductionTypeName = "Miscellaneous Deductions", ActiveYNID = 1, DeleteYNID = 0 }
     );
      modelBuilder.Entity<HR_DeductionSetup>().HasData(
        new HR_DeductionSetup() { DeductionSetupID = 1, DeductionTypeID = 1, ClassID = 2, SalaryTypeID = 1, DeductionValueID = 4 },
        new HR_DeductionSetup() { DeductionSetupID = 2, DeductionTypeID = 2, ClassID = 2, SalaryTypeID = 1, DeductionValueID = 4 }
     );

      modelBuilder.Entity<Settings_ActiveYNIDType>().HasData(
         new Settings_ActiveYNIDType() { ActiveYNID = 1, ActiveName = "Yes" },
         new Settings_ActiveYNIDType() { ActiveYNID = 2, ActiveName = "No" }
      );

            modelBuilder.Entity<Settings_DeleteYNIDType>().HasData(
         new Settings_DeleteYNIDType() { DeleteYNID = 1, DeleteName = "Yes" },
         new Settings_DeleteYNIDType() { DeleteYNID = 2, DeleteName = "No" }
      );

      modelBuilder.Entity<Settings_VacationType>().HasData(
          new Settings_VacationType() { VacationTypeID = 1, VacationTypeName = "Annual Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 2, VacationTypeName = "Sick Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 3, VacationTypeName = "Casual Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 4, VacationTypeName = "Public Holidays", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 5, VacationTypeName = "Maternity Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 6, VacationTypeName = "Paternity Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 7, VacationTypeName = "Personal Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 8, VacationTypeName = "Bereavement Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 9, VacationTypeName = "Study Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 10, VacationTypeName = "Unpaid Leave", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_VacationType() { VacationTypeID = 11, VacationTypeName = "Compensatory Leave", ActiveYNID = 1, DeleteYNID = 0 }
      );

    
      modelBuilder.Entity<Settings_GenderType>().HasData(
       new Settings_GenderType { GenderTypeID = 1, GenderTypeName = "Male" },
       new Settings_GenderType { GenderTypeID = 2, GenderTypeName = "Female" },
       new Settings_GenderType { GenderTypeID = 3, GenderTypeName = "Other" }
   );

      modelBuilder.Entity<Settings_MaritalStatusType>().HasData(
          new Settings_MaritalStatusType { MaritalStatusTypeID = 1, MaritalStatusTypeName = "Single" },
          new Settings_MaritalStatusType { MaritalStatusTypeID = 2, MaritalStatusTypeName = "Married" }
      );

      modelBuilder.Entity<Settings_ReligionType>().HasData(
    new Settings_ReligionType { ReligionTypeID = 1, ReligionTypeName = "Muslim" },
    new Settings_ReligionType { ReligionTypeID = 2, ReligionTypeName = "Christian" },
    new Settings_ReligionType { ReligionTypeID = 3, ReligionTypeName = "Hindu" },
    new Settings_ReligionType { ReligionTypeID = 4, ReligionTypeName = "Jewish" },
    new Settings_ReligionType { ReligionTypeID = 5, ReligionTypeName = "Buddhist" },
    new Settings_ReligionType { ReligionTypeID = 6, ReligionTypeName = "Sikh" },
    new Settings_ReligionType { ReligionTypeID = 7, ReligionTypeName = "Jain" },
    new Settings_ReligionType { ReligionTypeID = 8, ReligionTypeName = "Zoroastrian" },
    new Settings_ReligionType { ReligionTypeID = 9, ReligionTypeName = "Shinto" },
    new Settings_ReligionType { ReligionTypeID = 10, ReligionTypeName = "Taoist" },
    new Settings_ReligionType { ReligionTypeID = 11, ReligionTypeName = "Bahá'í" },
    new Settings_ReligionType { ReligionTypeID = 12, ReligionTypeName = "Atheist" },
    new Settings_ReligionType { ReligionTypeID = 13, ReligionTypeName = "Agnostic" },
    new Settings_ReligionType { ReligionTypeID = 14, ReligionTypeName = "Other" }
);

      modelBuilder.Entity<Settings_CountryType>().HasData(
        new Settings_CountryType { CountryTypeID = 1, CountryTypeName = "Afghanistan" },
        new Settings_CountryType { CountryTypeID = 2, CountryTypeName = "Albania" },
        new Settings_CountryType { CountryTypeID = 3, CountryTypeName = "Algeria" },
        new Settings_CountryType { CountryTypeID = 4, CountryTypeName = "Andorra" },
        new Settings_CountryType { CountryTypeID = 5, CountryTypeName = "Angola" },
        new Settings_CountryType { CountryTypeID = 6, CountryTypeName = "Antigua and Barbuda" },
        new Settings_CountryType { CountryTypeID = 7, CountryTypeName = "Argentina" },
        new Settings_CountryType { CountryTypeID = 8, CountryTypeName = "Armenia" },
        new Settings_CountryType { CountryTypeID = 9, CountryTypeName = "Australia" },
        new Settings_CountryType { CountryTypeID = 10, CountryTypeName = "Austria" },
        new Settings_CountryType { CountryTypeID = 11, CountryTypeName = "Azerbaijan" },
        new Settings_CountryType { CountryTypeID = 12, CountryTypeName = "Bahamas" },
        new Settings_CountryType { CountryTypeID = 13, CountryTypeName = "Bahrain" },
        new Settings_CountryType { CountryTypeID = 14, CountryTypeName = "Bangladesh" },
        new Settings_CountryType { CountryTypeID = 15, CountryTypeName = "Barbados" },
        new Settings_CountryType { CountryTypeID = 16, CountryTypeName = "Belarus" },
        new Settings_CountryType { CountryTypeID = 17, CountryTypeName = "Belgium" },
        new Settings_CountryType { CountryTypeID = 18, CountryTypeName = "Belize" },
        new Settings_CountryType { CountryTypeID = 19, CountryTypeName = "Benin" },
        new Settings_CountryType { CountryTypeID = 20, CountryTypeName = "Bhutan" },
        new Settings_CountryType { CountryTypeID = 21, CountryTypeName = "Bolivia" },
        new Settings_CountryType { CountryTypeID = 22, CountryTypeName = "Bosnia and Herzegovina" },
        new Settings_CountryType { CountryTypeID = 23, CountryTypeName = "Botswana" },
        new Settings_CountryType { CountryTypeID = 24, CountryTypeName = "Brazil" },
        new Settings_CountryType { CountryTypeID = 25, CountryTypeName = "Brunei" },
        new Settings_CountryType { CountryTypeID = 26, CountryTypeName = "Bulgaria" },
        new Settings_CountryType { CountryTypeID = 27, CountryTypeName = "Burkina Faso" },
        new Settings_CountryType { CountryTypeID = 28, CountryTypeName = "Burundi" },
        new Settings_CountryType { CountryTypeID = 29, CountryTypeName = "Cabo Verde" },
        new Settings_CountryType { CountryTypeID = 30, CountryTypeName = "Cambodia" },
        new Settings_CountryType { CountryTypeID = 31, CountryTypeName = "Cameroon" },
        new Settings_CountryType { CountryTypeID = 32, CountryTypeName = "Canada" },
        new Settings_CountryType { CountryTypeID = 33, CountryTypeName = "Central African Republic" },
        new Settings_CountryType { CountryTypeID = 34, CountryTypeName = "Chad" },
        new Settings_CountryType { CountryTypeID = 35, CountryTypeName = "Chile" },
        new Settings_CountryType { CountryTypeID = 36, CountryTypeName = "China" },
        new Settings_CountryType { CountryTypeID = 37, CountryTypeName = "Colombia" },
        new Settings_CountryType { CountryTypeID = 38, CountryTypeName = "Comoros" },
        new Settings_CountryType { CountryTypeID = 39, CountryTypeName = "Congo, Democratic Republic of the" },
        new Settings_CountryType { CountryTypeID = 40, CountryTypeName = "Congo, Republic of the" },
        new Settings_CountryType { CountryTypeID = 41, CountryTypeName = "Costa Rica" },
        new Settings_CountryType { CountryTypeID = 42, CountryTypeName = "Croatia" },
        new Settings_CountryType { CountryTypeID = 43, CountryTypeName = "Cuba" },
        new Settings_CountryType { CountryTypeID = 44, CountryTypeName = "Cyprus" },
        new Settings_CountryType { CountryTypeID = 45, CountryTypeName = "Czech Republic" },
        new Settings_CountryType { CountryTypeID = 46, CountryTypeName = "Denmark" },
        new Settings_CountryType { CountryTypeID = 47, CountryTypeName = "Djibouti" },
        new Settings_CountryType { CountryTypeID = 48, CountryTypeName = "Dominica" },
        new Settings_CountryType { CountryTypeID = 49, CountryTypeName = "Dominican Republic" },
        new Settings_CountryType { CountryTypeID = 50, CountryTypeName = "East Timor" },
        new Settings_CountryType { CountryTypeID = 51, CountryTypeName = "Ecuador" },
        new Settings_CountryType { CountryTypeID = 52, CountryTypeName = "Egypt" },
        new Settings_CountryType { CountryTypeID = 53, CountryTypeName = "El Salvador" },
        new Settings_CountryType { CountryTypeID = 54, CountryTypeName = "Equatorial Guinea" },
        new Settings_CountryType { CountryTypeID = 55, CountryTypeName = "Eritrea" },
        new Settings_CountryType { CountryTypeID = 56, CountryTypeName = "Estonia" },
        new Settings_CountryType { CountryTypeID = 57, CountryTypeName = "Eswatini" },
        new Settings_CountryType { CountryTypeID = 58, CountryTypeName = "Ethiopia" },
        new Settings_CountryType { CountryTypeID = 59, CountryTypeName = "Fiji" },
        new Settings_CountryType { CountryTypeID = 60, CountryTypeName = "Finland" },
        new Settings_CountryType { CountryTypeID = 61, CountryTypeName = "France" },
        new Settings_CountryType { CountryTypeID = 62, CountryTypeName = "Gabon" },
        new Settings_CountryType { CountryTypeID = 63, CountryTypeName = "Gambia" },
        new Settings_CountryType { CountryTypeID = 64, CountryTypeName = "Georgia" },
        new Settings_CountryType { CountryTypeID = 65, CountryTypeName = "Germany" },
        new Settings_CountryType { CountryTypeID = 66, CountryTypeName = "Ghana" },
        new Settings_CountryType { CountryTypeID = 67, CountryTypeName = "Greece" },
        new Settings_CountryType { CountryTypeID = 68, CountryTypeName = "Grenada" },
        new Settings_CountryType { CountryTypeID = 69, CountryTypeName = "Guatemala" },
        new Settings_CountryType { CountryTypeID = 70, CountryTypeName = "Guinea" },
        new Settings_CountryType { CountryTypeID = 71, CountryTypeName = "Guinea-Bissau" },
        new Settings_CountryType { CountryTypeID = 72, CountryTypeName = "Guyana" },
        new Settings_CountryType { CountryTypeID = 73, CountryTypeName = "Haiti" },
        new Settings_CountryType { CountryTypeID = 74, CountryTypeName = "Honduras" },
        new Settings_CountryType { CountryTypeID = 75, CountryTypeName = "Hungary" },
        new Settings_CountryType { CountryTypeID = 76, CountryTypeName = "Iceland" },
        new Settings_CountryType { CountryTypeID = 77, CountryTypeName = "India" },
        new Settings_CountryType { CountryTypeID = 78, CountryTypeName = "Indonesia" },
        new Settings_CountryType { CountryTypeID = 79, CountryTypeName = "Iran" },
        new Settings_CountryType { CountryTypeID = 80, CountryTypeName = "Iraq" },
        new Settings_CountryType { CountryTypeID = 81, CountryTypeName = "Ireland" },
        new Settings_CountryType { CountryTypeID = 82, CountryTypeName = "Israel" },
        new Settings_CountryType { CountryTypeID = 83, CountryTypeName = "Italy" },
        new Settings_CountryType { CountryTypeID = 84, CountryTypeName = "Jamaica" },
        new Settings_CountryType { CountryTypeID = 85, CountryTypeName = "Japan" },
        new Settings_CountryType { CountryTypeID = 86, CountryTypeName = "Jordan" },
        new Settings_CountryType { CountryTypeID = 87, CountryTypeName = "Kazakhstan" },
        new Settings_CountryType { CountryTypeID = 88, CountryTypeName = "Kenya" },
        new Settings_CountryType { CountryTypeID = 89, CountryTypeName = "Kiribati" },
        new Settings_CountryType { CountryTypeID = 90, CountryTypeName = "Korea, North" },
        new Settings_CountryType { CountryTypeID = 91, CountryTypeName = "Korea, South" },
        new Settings_CountryType { CountryTypeID = 92, CountryTypeName = "Kosovo" },
        new Settings_CountryType { CountryTypeID = 93, CountryTypeName = "Kuwait" },
        new Settings_CountryType { CountryTypeID = 94, CountryTypeName = "Kyrgyzstan" },
        new Settings_CountryType { CountryTypeID = 95, CountryTypeName = "Laos" },
        new Settings_CountryType { CountryTypeID = 96, CountryTypeName = "Latvia" },
        new Settings_CountryType { CountryTypeID = 97, CountryTypeName = "Lebanon" },
        new Settings_CountryType { CountryTypeID = 98, CountryTypeName = "Lesotho" },
        new Settings_CountryType { CountryTypeID = 99, CountryTypeName = "Liberia" },
        new Settings_CountryType { CountryTypeID = 100, CountryTypeName = "Libya" },
        new Settings_CountryType { CountryTypeID = 101, CountryTypeName = "Liechtenstein" },
        new Settings_CountryType { CountryTypeID = 102, CountryTypeName = "Lithuania" },
        new Settings_CountryType { CountryTypeID = 103, CountryTypeName = "Luxembourg" },
        new Settings_CountryType { CountryTypeID = 104, CountryTypeName = "Madagascar" },
        new Settings_CountryType { CountryTypeID = 105, CountryTypeName = "Malawi" },
        new Settings_CountryType { CountryTypeID = 106, CountryTypeName = "Malaysia" },
        new Settings_CountryType { CountryTypeID = 107, CountryTypeName = "Maldives" },
        new Settings_CountryType { CountryTypeID = 108, CountryTypeName = "Mali" },
        new Settings_CountryType { CountryTypeID = 109, CountryTypeName = "Malta" },
        new Settings_CountryType { CountryTypeID = 110, CountryTypeName = "Marshall Islands" },
        new Settings_CountryType { CountryTypeID = 111, CountryTypeName = "Mauritania" },
        new Settings_CountryType { CountryTypeID = 112, CountryTypeName = "Mauritius" },
        new Settings_CountryType { CountryTypeID = 113, CountryTypeName = "Mexico" },
        new Settings_CountryType { CountryTypeID = 114, CountryTypeName = "Micronesia" },
        new Settings_CountryType { CountryTypeID = 115, CountryTypeName = "Moldova" },
        new Settings_CountryType { CountryTypeID = 116, CountryTypeName = "Monaco" },
        new Settings_CountryType { CountryTypeID = 117, CountryTypeName = "Mongolia" },
        new Settings_CountryType { CountryTypeID = 118, CountryTypeName = "Montenegro" },
        new Settings_CountryType { CountryTypeID = 119, CountryTypeName = "Morocco" },
        new Settings_CountryType { CountryTypeID = 120, CountryTypeName = "Mozambique" },
        new Settings_CountryType { CountryTypeID = 121, CountryTypeName = "Myanmar" },
        new Settings_CountryType { CountryTypeID = 122, CountryTypeName = "Namibia" },
        new Settings_CountryType { CountryTypeID = 123, CountryTypeName = "Nauru" },
        new Settings_CountryType { CountryTypeID = 124, CountryTypeName = "Nepal" },
        new Settings_CountryType { CountryTypeID = 125, CountryTypeName = "Netherlands" },
        new Settings_CountryType { CountryTypeID = 126, CountryTypeName = "New Zealand" },
        new Settings_CountryType { CountryTypeID = 127, CountryTypeName = "Nicaragua" },
        new Settings_CountryType { CountryTypeID = 128, CountryTypeName = "Niger" },
        new Settings_CountryType { CountryTypeID = 129, CountryTypeName = "Nigeria" },
        new Settings_CountryType { CountryTypeID = 130, CountryTypeName = "North Macedonia" },
        new Settings_CountryType { CountryTypeID = 131, CountryTypeName = "Norway" },
        new Settings_CountryType { CountryTypeID = 132, CountryTypeName = "Oman" },
        new Settings_CountryType { CountryTypeID = 133, CountryTypeName = "Pakistan" },
        new Settings_CountryType { CountryTypeID = 134, CountryTypeName = "Palau" },
        new Settings_CountryType { CountryTypeID = 135, CountryTypeName = "Panama" },
        new Settings_CountryType { CountryTypeID = 136, CountryTypeName = "Papua New Guinea" },
        new Settings_CountryType { CountryTypeID = 137, CountryTypeName = "Paraguay" },
        new Settings_CountryType { CountryTypeID = 138, CountryTypeName = "Peru" },
        new Settings_CountryType { CountryTypeID = 139, CountryTypeName = "Philippines" },
        new Settings_CountryType { CountryTypeID = 140, CountryTypeName = "Poland" },
        new Settings_CountryType { CountryTypeID = 141, CountryTypeName = "Portugal" },
        new Settings_CountryType { CountryTypeID = 142, CountryTypeName = "Qatar" },
        new Settings_CountryType { CountryTypeID = 143, CountryTypeName = "Romania" },
        new Settings_CountryType { CountryTypeID = 144, CountryTypeName = "Russia" },
        new Settings_CountryType { CountryTypeID = 145, CountryTypeName = "Rwanda" },
        new Settings_CountryType { CountryTypeID = 146, CountryTypeName = "Saint Kitts and Nevis" },
        new Settings_CountryType { CountryTypeID = 147, CountryTypeName = "Saint Lucia" },
        new Settings_CountryType { CountryTypeID = 148, CountryTypeName = "Saint Vincent and the Grenadines" },
        new Settings_CountryType { CountryTypeID = 149, CountryTypeName = "Samoa" },
        new Settings_CountryType { CountryTypeID = 150, CountryTypeName = "San Marino" },
        new Settings_CountryType { CountryTypeID = 151, CountryTypeName = "Sao Tome and Principe" },
        new Settings_CountryType { CountryTypeID = 152, CountryTypeName = "Saudi Arabia" },
        new Settings_CountryType { CountryTypeID = 153, CountryTypeName = "Senegal" },
        new Settings_CountryType { CountryTypeID = 154, CountryTypeName = "Serbia" },
        new Settings_CountryType { CountryTypeID = 155, CountryTypeName = "Seychelles" },
        new Settings_CountryType { CountryTypeID = 156, CountryTypeName = "Sierra Leone" },
        new Settings_CountryType { CountryTypeID = 157, CountryTypeName = "Singapore" },
        new Settings_CountryType { CountryTypeID = 158, CountryTypeName = "Slovakia" },
        new Settings_CountryType { CountryTypeID = 159, CountryTypeName = "Slovenia" },
        new Settings_CountryType { CountryTypeID = 160, CountryTypeName = "Solomon Islands" },
        new Settings_CountryType { CountryTypeID = 161, CountryTypeName = "Somalia" },
        new Settings_CountryType { CountryTypeID = 162, CountryTypeName = "South Africa" },
        new Settings_CountryType { CountryTypeID = 163, CountryTypeName = "South Sudan" },
        new Settings_CountryType { CountryTypeID = 164, CountryTypeName = "Spain" },
        new Settings_CountryType { CountryTypeID = 165, CountryTypeName = "Sri Lanka" },
        new Settings_CountryType { CountryTypeID = 166, CountryTypeName = "Sudan" },
        new Settings_CountryType { CountryTypeID = 167, CountryTypeName = "Suriname" },
        new Settings_CountryType { CountryTypeID = 168, CountryTypeName = "Sweden" },
        new Settings_CountryType { CountryTypeID = 169, CountryTypeName = "Switzerland" },
        new Settings_CountryType { CountryTypeID = 170, CountryTypeName = "Syria" },
        new Settings_CountryType { CountryTypeID = 171, CountryTypeName = "Taiwan" },
        new Settings_CountryType { CountryTypeID = 172, CountryTypeName = "Tajikistan" },
        new Settings_CountryType { CountryTypeID = 173, CountryTypeName = "Tanzania" },
        new Settings_CountryType { CountryTypeID = 174, CountryTypeName = "Thailand" },
        new Settings_CountryType { CountryTypeID = 175, CountryTypeName = "Togo" },
        new Settings_CountryType { CountryTypeID = 176, CountryTypeName = "Tonga" },
        new Settings_CountryType { CountryTypeID = 177, CountryTypeName = "Trinidad and Tobago" },
        new Settings_CountryType { CountryTypeID = 178, CountryTypeName = "Tunisia" },
        new Settings_CountryType { CountryTypeID = 179, CountryTypeName = "Turkey" },
        new Settings_CountryType { CountryTypeID = 180, CountryTypeName = "Turkmenistan" },
        new Settings_CountryType { CountryTypeID = 181, CountryTypeName = "Tuvalu" },
        new Settings_CountryType { CountryTypeID = 182, CountryTypeName = "Uganda" },
        new Settings_CountryType { CountryTypeID = 183, CountryTypeName = "Ukraine" },
        new Settings_CountryType { CountryTypeID = 184, CountryTypeName = "United Arab Emirates" },
        new Settings_CountryType { CountryTypeID = 185, CountryTypeName = "United Kingdom" },
        new Settings_CountryType { CountryTypeID = 186, CountryTypeName = "United States" },
        new Settings_CountryType { CountryTypeID = 187, CountryTypeName = "Uruguay" },
        new Settings_CountryType { CountryTypeID = 188, CountryTypeName = "Uzbekistan" },
        new Settings_CountryType { CountryTypeID = 189, CountryTypeName = "Vanuatu" },
        new Settings_CountryType { CountryTypeID = 190, CountryTypeName = "Vatican City" },
        new Settings_CountryType { CountryTypeID = 191, CountryTypeName = "Venezuela" },
        new Settings_CountryType { CountryTypeID = 192, CountryTypeName = "Vietnam" },
        new Settings_CountryType { CountryTypeID = 193, CountryTypeName = "Yemen" },
        new Settings_CountryType { CountryTypeID = 194, CountryTypeName = "Zambia" },
        new Settings_CountryType { CountryTypeID = 195, CountryTypeName = "Zimbabwe" }
    );
      modelBuilder.Entity<Settings_SalaryOption>().HasData(
    new Settings_SalaryOption { SalaryOptionId = 1, SalaryOptionName = "Monthly" },
    new Settings_SalaryOption { SalaryOptionId = 2, SalaryOptionName = "Hourly" },
    new Settings_SalaryOption { SalaryOptionId = 3, SalaryOptionName = "Project Based" },
    new Settings_SalaryOption { SalaryOptionId = 4, SalaryOptionName = "Daily" },
    new Settings_SalaryOption { SalaryOptionId = 5, SalaryOptionName = "Commission-Based" },
    new Settings_SalaryOption { SalaryOptionId = 6, SalaryOptionName = "Piece Rate" },
    new Settings_SalaryOption { SalaryOptionId = 7, SalaryOptionName = "Contractual" },
    new Settings_SalaryOption { SalaryOptionId = 8, SalaryOptionName = "Biweekly" }
);


      modelBuilder.Entity<Settings_ContractType>().HasData(
        new Settings_ContractType { ContractTypeId = 1, ContractTypeName = "Limited" },
        new Settings_ContractType { ContractTypeId = 2, ContractTypeName = "Unlimited" }
    );

      modelBuilder.Entity<Settings_DeductionValue>().HasData(
        new Settings_DeductionValue { DeductionValueID = 1, DeductionValueText = "x4" },
        new Settings_DeductionValue { DeductionValueID = 2, DeductionValueText = "x3" },
        new Settings_DeductionValue { DeductionValueID = 3, DeductionValueText = "x2" },
        new Settings_DeductionValue { DeductionValueID = 4, DeductionValueText = "1x" },
        new Settings_DeductionValue { DeductionValueID = 5, DeductionValueText = "2x" },
        new Settings_DeductionValue { DeductionValueID = 6, DeductionValueText = "3x" },
        new Settings_DeductionValue { DeductionValueID = 7, DeductionValueText = "4x" }
    );
      modelBuilder.Entity<Settings_DeductionClass>().HasData(
        new Settings_DeductionClass { ClassID = 1, ClassName = "Absent" },
        new Settings_DeductionClass { ClassID = 2, ClassName = "Late" }
        // Add more items here if needed
    );

      modelBuilder.Entity<Settings_EndOfServiceReasonType>().HasData(
        new Settings_EndOfServiceReasonType { EndOfServiceReasonTypeId = 1, EndOfServiceReasonTypeName = "Resignation" },
        new Settings_EndOfServiceReasonType { EndOfServiceReasonTypeId = 2, EndOfServiceReasonTypeName = "Retirement" },
        new Settings_EndOfServiceReasonType { EndOfServiceReasonTypeId = 3, EndOfServiceReasonTypeName = "Contract Expiration" },
        new Settings_EndOfServiceReasonType { EndOfServiceReasonTypeId = 4, EndOfServiceReasonTypeName = "Termination for Cause" },
        new Settings_EndOfServiceReasonType { EndOfServiceReasonTypeId = 5, EndOfServiceReasonTypeName = "Death" }
        // Add more items here if needed
    );

   }
  }
}
