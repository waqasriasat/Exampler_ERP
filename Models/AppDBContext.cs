using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Models
{
  public class AppDBContext : DbContext
  {
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
    public DbSet<Settings_Role> Settings_Roles { get; set; }
    public DbSet<Settings_Branch> Settings_Branchs { get; set; }
    public DbSet<Settings_Department> Settings_Departments { get; set; }
    public DbSet<Settings_Section> Settings_Sections { get; set; }
    public DbSet<Settings_Qualification> Settings_Qualifications { get; set; }
    public DbSet<Settings_SubQualification> Settings_SubQualifications { get; set; }
    public DbSet<Settings_Designation> Settings_Designations { get; set; }
    public DbSet<Settings_SalaryType> Settings_SalaryTypes { get; set; }
    public DbSet<Settings_DeductionType> Settings_DeductionTypes { get; set; }
    public DbSet<Settings_VacationType> Settings_VacationTypes { get; set; }
    public DbSet<Settings_ProcessType> Settings_ProcessTypes { get; set; }
    public DbSet<Settings_EmployeeStatusType> Settings_EmployeeStatusTypes { get; set; }
    public DbSet<Settings_ActiveYNIDType> Settings_ActiveYNIDTypes { get; set; }
    public DbSet<Settings_DeleteYNIDType> Settings_DeleteYNIDTypes { get; set; }
    public DbSet<Settings_Gender> Settings_Genders { get; set; }
    public DbSet<Settings_MaritalStatus> Settings_MaritalStatuses { get; set; }
    public DbSet<Settings_Religion> Settings_Religions { get; set; }
    public DbSet<Settings_Country> Settings_Countrys { get; set; }
    public DbSet<Settings_SalaryOption> Settings_SalaryOptions { get; set; }
    public DbSet<Settings_ContractType> Settings_ContractTypes { get; set; }
    public DbSet<Settings_DeductionValue> Settings_DeductionValues { get; set; }
    public DbSet<Settings_DeductionClass> Settings_DeductionClasses { get; set; }
    public DbSet<CR_ProcessTypeApproval> CR_ProcessTypeApprovals { get; set; }
    public DbSet<CR_ProcessTypeApprovalDetail> CR_ProcessTypeApprovalDetails { get; set; }
    public DbSet<CR_ProcessTypeApprovalDetailDoc> CR_ProcessTypeApprovalDetailDocs { get; set; }
    public DbSet<CR_ProcessTypeApprovalSetup> CR_ProcessTypeApprovalSetups { get; set; }
    public DbSet<CR_ProcessTypeForward> CR_ProcessTypeForwards { get; set; }
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
    public DbSet<Settings_EndOfServiceReasonType> Settings_EndOfServiceReasonTypes { get; set; }
    public DbSet<Settings_EmployeeRequestType> Settings_EmployeeRequestTypes { get; set; }
    public DbSet<HR_EmployeeRequest> HR_EmployeeRequests { get; set; }
    public DbSet<HR_VacationSettle> HR_VacationSettles { get; set; }
    public DbSet<HR_DocumentUpload> HR_DocumentUploads { get; set; }
    public DbSet<Settings_OverTimeRate> Settings_OverTimeRates { get; set; }
    public DbSet<Settings_OverTimeType> Settings_OverTimeTypes { get; set; }
    public DbSet<Settings_MonthType> Settings_MonthTypes { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
     new Settings_ProcessType() { ProcessTypeID = 8, ProcessTypeName = "Allowance", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 9, ProcessTypeName = "Deduction", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 10, ProcessTypeName = "Fixed Deduction", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 11, ProcessTypeName = "Vacation", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 12, ProcessTypeName = "Vacation Settle", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 13, ProcessTypeName = "PayRoll", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_ProcessType() { ProcessTypeID = 14, ProcessTypeName = "Employee Request", ActiveYNID = 1, DeleteYNID = 0 }

     );

      modelBuilder.Entity<Settings_Role>().HasData(
     new Settings_Role() { RoleID = 1, RoleName = "Admin", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 2, RoleName = "General Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 3, RoleName = "ERP Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 4, RoleName = "Finance Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 5, RoleName = "Human Resources Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 6, RoleName = "Procurement Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 7, RoleName = "Inventory Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 8, RoleName = "Sales Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 9, RoleName = "Production Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 10, RoleName = "Project Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 11, RoleName = "Customer Service Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 12, RoleName = "Quality Control Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 13, RoleName = "IT Support Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 14, RoleName = "Data Analyst", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 15, RoleName = "Compliance Officer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 16, RoleName = "Logistics Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 17, RoleName = "Security Officer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 18, RoleName = "Training Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 19, RoleName = "Product Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 20, RoleName = "Marketing Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 21, RoleName = "Vendor/Supplier Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 22, RoleName = "Change Management Lead", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 23, RoleName = "Audit Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 24, RoleName = "Legal Advisor", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 25, RoleName = "Product Customization Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 26, RoleName = "Vendor Relationship Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 27, RoleName = "Business Analyst", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 28, RoleName = "Data Migration Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 29, RoleName = "Integration Specialist", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 30, RoleName = "Support Desk Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 31, RoleName = "Process Improvement Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 32, RoleName = "Risk Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 33, RoleName = "User Experience (UX) Designer", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 34, RoleName = "Master Data Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 35, RoleName = "Budget Manager", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_Role() { RoleID = 36, RoleName = "Product Development Manager", ActiveYNID = 1, DeleteYNID = 0 }
 );

      modelBuilder.Entity<CR_User>().HasData(

      new CR_User()
      {
        UserID = 1,
        UserName = "v0tnPv0i6sOwp4eLjWF4rA==", //ibs
        Password = "LiGbnDcHpE6eeqlmOScrOA==", //A@123456
        ActiveYNID = 1,
        DeleteYNID = 0,
        RoleID = 1,
        EmployeeID = 0
      });
      modelBuilder.Entity<Settings_Branch>().HasData(
      new Settings_Branch() { BranchID = 1, BranchName = "Head Office", ActiveYNID = 1, DeleteYNID = 0 }
      );

      modelBuilder.Entity<Settings_Department>().HasData(
    new Settings_Department() { DepartmentID = 1, DepartmentName = "Human Resources (HR)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 2, DepartmentName = "Finance", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 3, DepartmentName = "Procurement", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 4, DepartmentName = "Sales", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 5, DepartmentName = "Marketing", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 6, DepartmentName = "Operations", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 7, DepartmentName = "Production/Manufacturing", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 8, DepartmentName = "Logistics", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 9, DepartmentName = "Information Technology (IT)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 10, DepartmentName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 11, DepartmentName = "Research and Development (R&D)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 12, DepartmentName = "Quality Assurance", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 13, DepartmentName = "Legal", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 14, DepartmentName = "Compliance", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 15, DepartmentName = "Public Relations (PR)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 16, DepartmentName = "Administration", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 17, DepartmentName = "Facilities Management", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 18, DepartmentName = "Supply Chain Management", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 19, DepartmentName = "Risk Management", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 20, DepartmentName = "Project Management Office (PMO)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 21, DepartmentName = "Business Development", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 22, DepartmentName = "Strategy and Planning", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 23, DepartmentName = "Product Development", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 24, DepartmentName = "Training and Development", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 25, DepartmentName = "Corporate Communications", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 26, DepartmentName = "Internal Audit", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 27, DepartmentName = "Data Analytics/Business Intelligence", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 28, DepartmentName = "Security (including Physical and Cybersecurity)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 29, DepartmentName = "Health and Safety", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 30, DepartmentName = "Customer Relations Management (CRM)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 31, DepartmentName = "Legal and Compliance", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 32, DepartmentName = "Vendor Management", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 33, DepartmentName = "Environment, Health, and Safety (EHS)", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 34, DepartmentName = "Sustainability", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 },
    new Settings_Department() { DepartmentID = 35, DepartmentName = "Investor Relations", ActiveYNID = 1, DeleteYNID = 0, BranchID = 1 }
);

      modelBuilder.Entity<Settings_Section>().HasData(
      new Settings_Section() { SectionID = 1, SectionName = "Recruitment", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 2, SectionName = "Payroll", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 3, SectionName = "Employee Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 4, SectionName = "Training and Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 5, SectionName = "Compensation and Benefits", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 6, SectionName = "Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 7, SectionName = "Performance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 1 },
      new Settings_Section() { SectionID = 8, SectionName = "Accounts Payable", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 9, SectionName = "Accounts Receivable", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 10, SectionName = "General Ledger", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 11, SectionName = "Budgeting and Forecasting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 12, SectionName = "Tax Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 13, SectionName = "Financial Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 14, SectionName = "Internal Audit", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 15, SectionName = "Treasury", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 2 },
      new Settings_Section() { SectionID = 16, SectionName = "Vendor Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 17, SectionName = "Contract Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 18, SectionName = "Purchase Order Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 19, SectionName = "Inventory Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 20, SectionName = "Supplier Relationship Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 21, SectionName = "Strategic Sourcing", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 3 },
      new Settings_Section() { SectionID = 22, SectionName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 23, SectionName = "Sales Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 24, SectionName = "Sales Analytics", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 25, SectionName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 26, SectionName = "Lead Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 27, SectionName = "Territory Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 4 },
      new Settings_Section() { SectionID = 28, SectionName = "Digital Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 29, SectionName = "Content Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 30, SectionName = "Brand Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 31, SectionName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 32, SectionName = "Public Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 33, SectionName = "Advertising", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 34, SectionName = "Social Media Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 5 },
      new Settings_Section() { SectionID = 35, SectionName = "Process Optimization", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 36, SectionName = "Operations Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 37, SectionName = "Facility Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 38, SectionName = "Workflow Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 39, SectionName = "Resource Allocation", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 40, SectionName = "Vendor Coordination", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 6 },
      new Settings_Section() { SectionID = 41, SectionName = "Production Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 42, SectionName = "Quality Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 43, SectionName = "Maintenance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 44, SectionName = "Inventory Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 45, SectionName = "Supply Chain Coordination", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 46, SectionName = "Process Engineering", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 7 },
      new Settings_Section() { SectionID = 47, SectionName = "Transportation Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 48, SectionName = "Warehouse Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 49, SectionName = "Inventory Control", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 50, SectionName = "Distribution", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 51, SectionName = "Logistics Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 52, SectionName = "Supplier Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 53, SectionName = "Fleet Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 8 },
      new Settings_Section() { SectionID = 54, SectionName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 55, SectionName = "Client Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 56, SectionName = "Support Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 57, SectionName = "Service Quality", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 58, SectionName = "Customer Feedback", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 59, SectionName = "Service Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 60, SectionName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 61, SectionName = "Client Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 62, SectionName = "Service Innovations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 63, SectionName = "Customer Experience", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 9 },
      new Settings_Section() { SectionID = 64, SectionName = "Technical Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 65, SectionName = "IT Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 66, SectionName = "Infrastructure", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 67, SectionName = "Systems Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 68, SectionName = "Network Administration", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 69, SectionName = "Software Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 70, SectionName = "Security Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 10 },
      new Settings_Section() { SectionID = 71, SectionName = "Product Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 72, SectionName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 73, SectionName = "Development Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 74, SectionName = "Product Design", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 75, SectionName = "Product Lifecycle", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 76, SectionName = "Innovation Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 77, SectionName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 78, SectionName = "Market Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 79, SectionName = "Business Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 11 },
      new Settings_Section() { SectionID = 80, SectionName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 81, SectionName = "Sales Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 82, SectionName = "Lead Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 83, SectionName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 84, SectionName = "Sales Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 85, SectionName = "Market Penetration", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 86, SectionName = "Revenue Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 87, SectionName = "Contract Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 88, SectionName = "Customer Contracts", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 89, SectionName = "Customer Acquisition", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 12 },
      new Settings_Section() { SectionID = 90, SectionName = "Corporate Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 91, SectionName = "Strategic Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 92, SectionName = "Business Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 93, SectionName = "Risk Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 94, SectionName = "Corporate Governance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 95, SectionName = "Policy Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 96, SectionName = "Operational Excellence", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 97, SectionName = "Performance Metrics", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 98, SectionName = "Change Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 13 },
      new Settings_Section() { SectionID = 99, SectionName = "Compliance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 100, SectionName = "Regulatory Affairs", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 101, SectionName = "Audit & Controls", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 102, SectionName = "Legal Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 103, SectionName = "Ethics & Standards", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 104, SectionName = "Compliance Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 105, SectionName = "Data Protection", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 106, SectionName = "Privacy Policies", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 107, SectionName = "Insurance & Risk", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 14 },
      new Settings_Section() { SectionID = 108, SectionName = "Legal Affairs", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 109, SectionName = "Contract Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 110, SectionName = "Litigation Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 111, SectionName = "Intellectual Property", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 112, SectionName = "Employment Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 113, SectionName = "Legal Advice", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 114, SectionName = "Regulatory Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 115, SectionName = "Corporate Legal", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 116, SectionName = "Legal Documentation", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 117, SectionName = "Environmental Law", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 15 },
      new Settings_Section() { SectionID = 118, SectionName = "Human Resources", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 119, SectionName = "Talent Acquisition", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 120, SectionName = "Employee Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 121, SectionName = "Compensation & Benefits", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 122, SectionName = "Training & Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 123, SectionName = "Performance Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 124, SectionName = "HR Policies", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 125, SectionName = "Employee Engagement", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 126, SectionName = "Workplace Safety", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 127, SectionName = "Organizational Development", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 16 },
      new Settings_Section() { SectionID = 128, SectionName = "Finance & Accounting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 129, SectionName = "Financial Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 130, SectionName = "Budgeting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 131, SectionName = "Accounting Standards", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 132, SectionName = "Tax Compliance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 133, SectionName = "Financial Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 134, SectionName = "Audit & Assurance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 135, SectionName = "Cash Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 136, SectionName = "Investment Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 137, SectionName = "Payroll Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 17 },
      new Settings_Section() { SectionID = 138, SectionName = "Marketing Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 139, SectionName = "Brand Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 140, SectionName = "Digital Marketing", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 141, SectionName = "Market Research", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 142, SectionName = "Advertising", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 143, SectionName = "Public Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 144, SectionName = "Content Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 145, SectionName = "Social Media", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 146, SectionName = "Event Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 147, SectionName = "Customer Outreach", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 18 },
      new Settings_Section() { SectionID = 148, SectionName = "Sales Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 149, SectionName = "Sales Planning", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 150, SectionName = "Lead Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 151, SectionName = "Account Management", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 152, SectionName = "Sales Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 153, SectionName = "Revenue Generation", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 154, SectionName = "Client Relations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 155, SectionName = "Sales Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 156, SectionName = "Sales Performance", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 157, SectionName = "Customer Insights", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 158, SectionName = "Pricing Strategy", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 159, SectionName = "Competitive Analysis", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 160, SectionName = "Sales Reporting", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 19 },
      new Settings_Section() { SectionID = 161, SectionName = "Customer Service", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 162, SectionName = "Support Operations", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 163, SectionName = "Client Support", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 164, SectionName = "Help Desk", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 165, SectionName = "Customer Care", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 166, SectionName = "Support Training", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 },
      new Settings_Section() { SectionID = 167, SectionName = "Service Improvement", ActiveYNID = 1, DeleteYNID = 0, DepartmentID = 20 }
  );

      modelBuilder.Entity<Settings_Qualification>().HasData(
          new Settings_Qualification() { QualificationID = 1, QualificationName = "Matriculation", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_Qualification() { QualificationID = 2, QualificationName = "Intermediate", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_Qualification() { QualificationID = 3, QualificationName = "Bachelor’s Degree", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_Qualification() { QualificationID = 4, QualificationName = "Master’s Degree", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_Qualification() { QualificationID = 5, QualificationName = "MPhil", ActiveYNID = 1, DeleteYNID = 0 },
          new Settings_Qualification() { QualificationID = 6, QualificationName = "Ph.D.", ActiveYNID = 1, DeleteYNID = 0 }
      );


      modelBuilder.Entity<Settings_SubQualification>().HasData(
    new Settings_SubQualification { SubQualificationID = 1, SubQualificationName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 1 },
    new Settings_SubQualification { SubQualificationID = 2, SubQualificationName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 1 },
    new Settings_SubQualification { SubQualificationID = 3, SubQualificationName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 1 },
    new Settings_SubQualification { SubQualificationID = 4, SubQualificationName = "Pre-Medical", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 2 },
    new Settings_SubQualification { SubQualificationID = 5, SubQualificationName = "Pre-Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 2 },
    new Settings_SubQualification { SubQualificationID = 6, SubQualificationName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 2 },
    new Settings_SubQualification { SubQualificationID = 7, SubQualificationName = "Humanities", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 2 },
    new Settings_SubQualification { SubQualificationID = 8, SubQualificationName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 3 },
    new Settings_SubQualification { SubQualificationID = 9, SubQualificationName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 3 },
    new Settings_SubQualification { SubQualificationID = 10, SubQualificationName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 3 },
    new Settings_SubQualification { SubQualificationID = 11, SubQualificationName = "Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 4 },
    new Settings_SubQualification { SubQualificationID = 12, SubQualificationName = "Business", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 4 },
    new Settings_SubQualification { SubQualificationID = 13, SubQualificationName = "Information Technology", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 4 },
    new Settings_SubQualification { SubQualificationID = 14, SubQualificationName = "Science", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 5 },
    new Settings_SubQualification { SubQualificationID = 15, SubQualificationName = "Commerce", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 5 },
    new Settings_SubQualification { SubQualificationID = 16, SubQualificationName = "Arts", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 5 },
    new Settings_SubQualification { SubQualificationID = 17, SubQualificationName = "Engineering", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 6 },
    new Settings_SubQualification { SubQualificationID = 18, SubQualificationName = "Business", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 6 },
    new Settings_SubQualification { SubQualificationID = 19, SubQualificationName = "Information Technology", ActiveYNID = 1, DeleteYNID = 0, QualificationID = 6 }
);

      modelBuilder.Entity<Settings_Designation>().HasData(
    new Settings_Designation() { DesignationID = 1, DesignationName = "CEO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 2, DesignationName = "COO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 3, DesignationName = "CFO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 4, DesignationName = "CIO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 5, DesignationName = "CTO", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 6, DesignationName = "General Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 7, DesignationName = "Regional Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 8, DesignationName = "Department Head", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 9, DesignationName = "Director", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 10, DesignationName = "Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 11, DesignationName = "Assistant Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 12, DesignationName = "Team Lead", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 13, DesignationName = "Supervisor", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 14, DesignationName = "Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 15, DesignationName = "Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 16, DesignationName = "Officer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 17, DesignationName = "IT Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 18, DesignationName = "Systems Analyst", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 19, DesignationName = "Database Administrator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 20, DesignationName = "Network Administrator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 21, DesignationName = "Software Developer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 22, DesignationName = "Customer Service Representative", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 23, DesignationName = "Administrative Assistant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 24, DesignationName = "HR Assistant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 25, DesignationName = "Accountant", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 26, DesignationName = "Sales Executive", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 27, DesignationName = "Marketing Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 28, DesignationName = "Business Development Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 29, DesignationName = "Production Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 30, DesignationName = "Supply Chain Analyst", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 31, DesignationName = "Logistics Coordinator", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 32, DesignationName = "Quality Assurance Specialist", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 33, DesignationName = "Compliance Officer", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 34, DesignationName = "Project Manager", ActiveYNID = 1, DeleteYNID = 0 },
    new Settings_Designation() { DesignationID = 35, DesignationName = "Project Coordinator", ActiveYNID = 1, DeleteYNID = 0 }
);

      modelBuilder.Entity<Settings_SalaryType>().HasData(
 new Settings_SalaryType() { SalaryTypeID = 1, SalaryTypeName = "Basic Salary", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 2, SalaryTypeName = "House Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 3, SalaryTypeName = "Transport Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 4, SalaryTypeName = "Medical Allowance", ActiveYNID = 1, DeleteYNID = 0 },
 new Settings_SalaryType() { SalaryTypeID = 5, SalaryTypeName = "Food Allowance", ActiveYNID = 1, DeleteYNID = 0 }
 );

      modelBuilder.Entity<Settings_DeductionType>().HasData(
     new Settings_DeductionType() { DeductionTypeID = 1, DeductionTypeName = "Income Tax", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 2, DeductionTypeName = "Social Security Contributions", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 3, DeductionTypeName = "Health Insurance Premiums", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 4, DeductionTypeName = "Pension Contributions", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 5, DeductionTypeName = "Loan Repayments", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 6, DeductionTypeName = "Union Dues", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 7, DeductionTypeName = "Garnishments", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 8, DeductionTypeName = "Absent Days or Leave Without Pay", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 9, DeductionTypeName = "Wage Advances", ActiveYNID = 1, DeleteYNID = 0 },
     new Settings_DeductionType() { DeductionTypeID = 10, DeductionTypeName = "Miscellaneous Deductions", ActiveYNID = 1, DeleteYNID = 0 }
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

    
      modelBuilder.Entity<Settings_Gender>().HasData(
       new Settings_Gender { GenderID = 1, GenderName = "Male" },
       new Settings_Gender { GenderID = 2, GenderName = "Female" },
       new Settings_Gender { GenderID = 3, GenderName = "Other" }
   );

      modelBuilder.Entity<Settings_MaritalStatus>().HasData(
          new Settings_MaritalStatus { MaritalStatusID = 1, MaritalStatusName = "Single" },
          new Settings_MaritalStatus { MaritalStatusID = 2, MaritalStatusName = "Married" }
      );

      modelBuilder.Entity<Settings_Religion>().HasData(
    new Settings_Religion { ReligionID = 1, ReligionName = "Muslim" },
    new Settings_Religion { ReligionID = 2, ReligionName = "Christian" },
    new Settings_Religion { ReligionID = 3, ReligionName = "Hindu" },
    new Settings_Religion { ReligionID = 4, ReligionName = "Jewish" },
    new Settings_Religion { ReligionID = 5, ReligionName = "Buddhist" },
    new Settings_Religion { ReligionID = 6, ReligionName = "Sikh" },
    new Settings_Religion { ReligionID = 7, ReligionName = "Jain" },
    new Settings_Religion { ReligionID = 8, ReligionName = "Zoroastrian" },
    new Settings_Religion { ReligionID = 9, ReligionName = "Shinto" },
    new Settings_Religion { ReligionID = 10, ReligionName = "Taoist" },
    new Settings_Religion { ReligionID = 11, ReligionName = "Bahá'í" },
    new Settings_Religion { ReligionID = 12, ReligionName = "Atheist" },
    new Settings_Religion { ReligionID = 13, ReligionName = "Agnostic" },
    new Settings_Religion { ReligionID = 14, ReligionName = "Other" }
);

      modelBuilder.Entity<Settings_Country>().HasData(
        new Settings_Country { CountryID = 1, CountryName = "Afghanistan" },
        new Settings_Country { CountryID = 2, CountryName = "Albania" },
        new Settings_Country { CountryID = 3, CountryName = "Algeria" },
        new Settings_Country { CountryID = 4, CountryName = "Andorra" },
        new Settings_Country { CountryID = 5, CountryName = "Angola" },
        new Settings_Country { CountryID = 6, CountryName = "Antigua and Barbuda" },
        new Settings_Country { CountryID = 7, CountryName = "Argentina" },
        new Settings_Country { CountryID = 8, CountryName = "Armenia" },
        new Settings_Country { CountryID = 9, CountryName = "Australia" },
        new Settings_Country { CountryID = 10, CountryName = "Austria" },
        new Settings_Country { CountryID = 11, CountryName = "Azerbaijan" },
        new Settings_Country { CountryID = 12, CountryName = "Bahamas" },
        new Settings_Country { CountryID = 13, CountryName = "Bahrain" },
        new Settings_Country { CountryID = 14, CountryName = "Bangladesh" },
        new Settings_Country { CountryID = 15, CountryName = "Barbados" },
        new Settings_Country { CountryID = 16, CountryName = "Belarus" },
        new Settings_Country { CountryID = 17, CountryName = "Belgium" },
        new Settings_Country { CountryID = 18, CountryName = "Belize" },
        new Settings_Country { CountryID = 19, CountryName = "Benin" },
        new Settings_Country { CountryID = 20, CountryName = "Bhutan" },
        new Settings_Country { CountryID = 21, CountryName = "Bolivia" },
        new Settings_Country { CountryID = 22, CountryName = "Bosnia and Herzegovina" },
        new Settings_Country { CountryID = 23, CountryName = "Botswana" },
        new Settings_Country { CountryID = 24, CountryName = "Brazil" },
        new Settings_Country { CountryID = 25, CountryName = "Brunei" },
        new Settings_Country { CountryID = 26, CountryName = "Bulgaria" },
        new Settings_Country { CountryID = 27, CountryName = "Burkina Faso" },
        new Settings_Country { CountryID = 28, CountryName = "Burundi" },
        new Settings_Country { CountryID = 29, CountryName = "Cabo Verde" },
        new Settings_Country { CountryID = 30, CountryName = "Cambodia" },
        new Settings_Country { CountryID = 31, CountryName = "Cameroon" },
        new Settings_Country { CountryID = 32, CountryName = "Canada" },
        new Settings_Country { CountryID = 33, CountryName = "Central African Republic" },
        new Settings_Country { CountryID = 34, CountryName = "Chad" },
        new Settings_Country { CountryID = 35, CountryName = "Chile" },
        new Settings_Country { CountryID = 36, CountryName = "China" },
        new Settings_Country { CountryID = 37, CountryName = "Colombia" },
        new Settings_Country { CountryID = 38, CountryName = "Comoros" },
        new Settings_Country { CountryID = 39, CountryName = "Congo, Democratic Republic of the" },
        new Settings_Country { CountryID = 40, CountryName = "Congo, Republic of the" },
        new Settings_Country { CountryID = 41, CountryName = "Costa Rica" },
        new Settings_Country { CountryID = 42, CountryName = "Croatia" },
        new Settings_Country { CountryID = 43, CountryName = "Cuba" },
        new Settings_Country { CountryID = 44, CountryName = "Cyprus" },
        new Settings_Country { CountryID = 45, CountryName = "Czech Republic" },
        new Settings_Country { CountryID = 46, CountryName = "Denmark" },
        new Settings_Country { CountryID = 47, CountryName = "Djibouti" },
        new Settings_Country { CountryID = 48, CountryName = "Dominica" },
        new Settings_Country { CountryID = 49, CountryName = "Dominican Republic" },
        new Settings_Country { CountryID = 50, CountryName = "East Timor" },
        new Settings_Country { CountryID = 51, CountryName = "Ecuador" },
        new Settings_Country { CountryID = 52, CountryName = "Egypt" },
        new Settings_Country { CountryID = 53, CountryName = "El Salvador" },
        new Settings_Country { CountryID = 54, CountryName = "Equatorial Guinea" },
        new Settings_Country { CountryID = 55, CountryName = "Eritrea" },
        new Settings_Country { CountryID = 56, CountryName = "Estonia" },
        new Settings_Country { CountryID = 57, CountryName = "Eswatini" },
        new Settings_Country { CountryID = 58, CountryName = "Ethiopia" },
        new Settings_Country { CountryID = 59, CountryName = "Fiji" },
        new Settings_Country { CountryID = 60, CountryName = "Finland" },
        new Settings_Country { CountryID = 61, CountryName = "France" },
        new Settings_Country { CountryID = 62, CountryName = "Gabon" },
        new Settings_Country { CountryID = 63, CountryName = "Gambia" },
        new Settings_Country { CountryID = 64, CountryName = "Georgia" },
        new Settings_Country { CountryID = 65, CountryName = "Germany" },
        new Settings_Country { CountryID = 66, CountryName = "Ghana" },
        new Settings_Country { CountryID = 67, CountryName = "Greece" },
        new Settings_Country { CountryID = 68, CountryName = "Grenada" },
        new Settings_Country { CountryID = 69, CountryName = "Guatemala" },
        new Settings_Country { CountryID = 70, CountryName = "Guinea" },
        new Settings_Country { CountryID = 71, CountryName = "Guinea-Bissau" },
        new Settings_Country { CountryID = 72, CountryName = "Guyana" },
        new Settings_Country { CountryID = 73, CountryName = "Haiti" },
        new Settings_Country { CountryID = 74, CountryName = "Honduras" },
        new Settings_Country { CountryID = 75, CountryName = "Hungary" },
        new Settings_Country { CountryID = 76, CountryName = "Iceland" },
        new Settings_Country { CountryID = 77, CountryName = "India" },
        new Settings_Country { CountryID = 78, CountryName = "Indonesia" },
        new Settings_Country { CountryID = 79, CountryName = "Iran" },
        new Settings_Country { CountryID = 80, CountryName = "Iraq" },
        new Settings_Country { CountryID = 81, CountryName = "Ireland" },
        new Settings_Country { CountryID = 82, CountryName = "Israel" },
        new Settings_Country { CountryID = 83, CountryName = "Italy" },
        new Settings_Country { CountryID = 84, CountryName = "Jamaica" },
        new Settings_Country { CountryID = 85, CountryName = "Japan" },
        new Settings_Country { CountryID = 86, CountryName = "Jordan" },
        new Settings_Country { CountryID = 87, CountryName = "Kazakhstan" },
        new Settings_Country { CountryID = 88, CountryName = "Kenya" },
        new Settings_Country { CountryID = 89, CountryName = "Kiribati" },
        new Settings_Country { CountryID = 90, CountryName = "Korea, North" },
        new Settings_Country { CountryID = 91, CountryName = "Korea, South" },
        new Settings_Country { CountryID = 92, CountryName = "Kosovo" },
        new Settings_Country { CountryID = 93, CountryName = "Kuwait" },
        new Settings_Country { CountryID = 94, CountryName = "Kyrgyzstan" },
        new Settings_Country { CountryID = 95, CountryName = "Laos" },
        new Settings_Country { CountryID = 96, CountryName = "Latvia" },
        new Settings_Country { CountryID = 97, CountryName = "Lebanon" },
        new Settings_Country { CountryID = 98, CountryName = "Lesotho" },
        new Settings_Country { CountryID = 99, CountryName = "Liberia" },
        new Settings_Country { CountryID = 100, CountryName = "Libya" },
        new Settings_Country { CountryID = 101, CountryName = "Liechtenstein" },
        new Settings_Country { CountryID = 102, CountryName = "Lithuania" },
        new Settings_Country { CountryID = 103, CountryName = "Luxembourg" },
        new Settings_Country { CountryID = 104, CountryName = "Madagascar" },
        new Settings_Country { CountryID = 105, CountryName = "Malawi" },
        new Settings_Country { CountryID = 106, CountryName = "Malaysia" },
        new Settings_Country { CountryID = 107, CountryName = "Maldives" },
        new Settings_Country { CountryID = 108, CountryName = "Mali" },
        new Settings_Country { CountryID = 109, CountryName = "Malta" },
        new Settings_Country { CountryID = 110, CountryName = "Marshall Islands" },
        new Settings_Country { CountryID = 111, CountryName = "Mauritania" },
        new Settings_Country { CountryID = 112, CountryName = "Mauritius" },
        new Settings_Country { CountryID = 113, CountryName = "Mexico" },
        new Settings_Country { CountryID = 114, CountryName = "Micronesia" },
        new Settings_Country { CountryID = 115, CountryName = "Moldova" },
        new Settings_Country { CountryID = 116, CountryName = "Monaco" },
        new Settings_Country { CountryID = 117, CountryName = "Mongolia" },
        new Settings_Country { CountryID = 118, CountryName = "Montenegro" },
        new Settings_Country { CountryID = 119, CountryName = "Morocco" },
        new Settings_Country { CountryID = 120, CountryName = "Mozambique" },
        new Settings_Country { CountryID = 121, CountryName = "Myanmar" },
        new Settings_Country { CountryID = 122, CountryName = "Namibia" },
        new Settings_Country { CountryID = 123, CountryName = "Nauru" },
        new Settings_Country { CountryID = 124, CountryName = "Nepal" },
        new Settings_Country { CountryID = 125, CountryName = "Netherlands" },
        new Settings_Country { CountryID = 126, CountryName = "New Zealand" },
        new Settings_Country { CountryID = 127, CountryName = "Nicaragua" },
        new Settings_Country { CountryID = 128, CountryName = "Niger" },
        new Settings_Country { CountryID = 129, CountryName = "Nigeria" },
        new Settings_Country { CountryID = 130, CountryName = "North Macedonia" },
        new Settings_Country { CountryID = 131, CountryName = "Norway" },
        new Settings_Country { CountryID = 132, CountryName = "Oman" },
        new Settings_Country { CountryID = 133, CountryName = "Pakistan" },
        new Settings_Country { CountryID = 134, CountryName = "Palau" },
        new Settings_Country { CountryID = 135, CountryName = "Panama" },
        new Settings_Country { CountryID = 136, CountryName = "Papua New Guinea" },
        new Settings_Country { CountryID = 137, CountryName = "Paraguay" },
        new Settings_Country { CountryID = 138, CountryName = "Peru" },
        new Settings_Country { CountryID = 139, CountryName = "Philippines" },
        new Settings_Country { CountryID = 140, CountryName = "Poland" },
        new Settings_Country { CountryID = 141, CountryName = "Portugal" },
        new Settings_Country { CountryID = 142, CountryName = "Qatar" },
        new Settings_Country { CountryID = 143, CountryName = "Romania" },
        new Settings_Country { CountryID = 144, CountryName = "Russia" },
        new Settings_Country { CountryID = 145, CountryName = "Rwanda" },
        new Settings_Country { CountryID = 146, CountryName = "Saint Kitts and Nevis" },
        new Settings_Country { CountryID = 147, CountryName = "Saint Lucia" },
        new Settings_Country { CountryID = 148, CountryName = "Saint Vincent and the Grenadines" },
        new Settings_Country { CountryID = 149, CountryName = "Samoa" },
        new Settings_Country { CountryID = 150, CountryName = "San Marino" },
        new Settings_Country { CountryID = 151, CountryName = "Sao Tome and Principe" },
        new Settings_Country { CountryID = 152, CountryName = "Saudi Arabia" },
        new Settings_Country { CountryID = 153, CountryName = "Senegal" },
        new Settings_Country { CountryID = 154, CountryName = "Serbia" },
        new Settings_Country { CountryID = 155, CountryName = "Seychelles" },
        new Settings_Country { CountryID = 156, CountryName = "Sierra Leone" },
        new Settings_Country { CountryID = 157, CountryName = "Singapore" },
        new Settings_Country { CountryID = 158, CountryName = "Slovakia" },
        new Settings_Country { CountryID = 159, CountryName = "Slovenia" },
        new Settings_Country { CountryID = 160, CountryName = "Solomon Islands" },
        new Settings_Country { CountryID = 161, CountryName = "Somalia" },
        new Settings_Country { CountryID = 162, CountryName = "South Africa" },
        new Settings_Country { CountryID = 163, CountryName = "South Sudan" },
        new Settings_Country { CountryID = 164, CountryName = "Spain" },
        new Settings_Country { CountryID = 165, CountryName = "Sri Lanka" },
        new Settings_Country { CountryID = 166, CountryName = "Sudan" },
        new Settings_Country { CountryID = 167, CountryName = "Suriname" },
        new Settings_Country { CountryID = 168, CountryName = "Sweden" },
        new Settings_Country { CountryID = 169, CountryName = "Switzerland" },
        new Settings_Country { CountryID = 170, CountryName = "Syria" },
        new Settings_Country { CountryID = 171, CountryName = "Taiwan" },
        new Settings_Country { CountryID = 172, CountryName = "Tajikistan" },
        new Settings_Country { CountryID = 173, CountryName = "Tanzania" },
        new Settings_Country { CountryID = 174, CountryName = "Thailand" },
        new Settings_Country { CountryID = 175, CountryName = "Togo" },
        new Settings_Country { CountryID = 176, CountryName = "Tonga" },
        new Settings_Country { CountryID = 177, CountryName = "Trinidad and Tobago" },
        new Settings_Country { CountryID = 178, CountryName = "Tunisia" },
        new Settings_Country { CountryID = 179, CountryName = "Turkey" },
        new Settings_Country { CountryID = 180, CountryName = "Turkmenistan" },
        new Settings_Country { CountryID = 181, CountryName = "Tuvalu" },
        new Settings_Country { CountryID = 182, CountryName = "Uganda" },
        new Settings_Country { CountryID = 183, CountryName = "Ukraine" },
        new Settings_Country { CountryID = 184, CountryName = "United Arab Emirates" },
        new Settings_Country { CountryID = 185, CountryName = "United Kingdom" },
        new Settings_Country { CountryID = 186, CountryName = "United States" },
        new Settings_Country { CountryID = 187, CountryName = "Uruguay" },
        new Settings_Country { CountryID = 188, CountryName = "Uzbekistan" },
        new Settings_Country { CountryID = 189, CountryName = "Vanuatu" },
        new Settings_Country { CountryID = 190, CountryName = "Vatican City" },
        new Settings_Country { CountryID = 191, CountryName = "Venezuela" },
        new Settings_Country { CountryID = 192, CountryName = "Vietnam" },
        new Settings_Country { CountryID = 193, CountryName = "Yemen" },
        new Settings_Country { CountryID = 194, CountryName = "Zambia" },
        new Settings_Country { CountryID = 195, CountryName = "Zimbabwe" }
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
