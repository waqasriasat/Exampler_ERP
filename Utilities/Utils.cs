using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Utilities
{
  public class Utils
  {
    private readonly AppDBContext _appDBContext;
    public Utils(AppDBContext appDBContext)
    {
      _appDBContext = appDBContext;
    }

    public async Task<List<SelectListItem>> GetActiveYNIDList()
    {
      var ActiveYNIDList = await _appDBContext.Settings_ActiveYNIDTypes.ToListAsync();

      var selectList = ActiveYNIDList.Select(r => new SelectListItem { Value = r.ActiveYNID.ToString(), Text = r.ActiveName}).ToList();
      return selectList;
    }

    public async Task<List<SelectListItem>> GetDeleteYNIDList()
    {
      var DeleteYNIDList = await _appDBContext.Settings_DeleteYNIDTypes.ToListAsync();

      var selectList = DeleteYNIDList.Select(r => new SelectListItem { Value = r.DeleteYNID.ToString(), Text = r.DeleteName}).ToList();
      return selectList;
    }
    public async Task<List<SelectListItem>> GetEmployee()
    {
      var Employees = await _appDBContext.HR_Employees.ToListAsync();

      var selectList = Employees.Select(r => new SelectListItem { Value = r.EmployeeID.ToString(), Text = r.FirstName +' '+r.FatherName + ' ' + r.FamilyName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    //public async Task<List<SelectListItem>> GetEmployees()
    //{
    //  var employees = await _appDBContext.HR_Employees
    //.Where(e => e.ActiveYNID == 1)
    //.ToListAsync();

    //  var selectList = employees.Select(r => new SelectListItem { Value = r.EmployeeID.ToString(), Text = r.FirstName + r.FatherName + r.FamilyName }).ToList();
    //  selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
    //  return selectList;
    //}
    public async Task<List<SelectListItem>> GetGender()
    {
      var genderOptions = await _appDBContext.Settings_GenderTypes.ToListAsync();

      var GenderList = genderOptions.Select(g => new SelectListItem
      {
        Value = g.GenderTypeID.ToString(),
        Text = g.GenderTypeName
      }).ToList();

      GenderList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return GenderList;
    }
    public async Task<List<SelectListItem>> GetMaritalStatus()
    {
      var maritalStatusOptions = await _appDBContext.Settings_MaritalStatusTypes.ToListAsync();

      var MaritalStatusList = maritalStatusOptions.Select(ms => new SelectListItem
      {
        Value = ms.MaritalStatusTypeID.ToString(),
        Text = ms.MaritalStatusTypeName
      }).ToList();

      MaritalStatusList.Insert(0, new SelectListItem { Value = "0", Text = "Select Marital Status" });

      return MaritalStatusList;
    }
    public async Task<List<SelectListItem>> GetReligion()
    {
      var religionOptions = await _appDBContext.Settings_ReligionTypes.ToListAsync();

      var ReligionList = religionOptions.Select(r => new SelectListItem
      {
        Value = r.ReligionTypeID.ToString(),
        Text = r.ReligionTypeName
      }).ToList();

      ReligionList.Insert(0, new SelectListItem { Value = "0", Text = "Select Religion" });

      return ReligionList;
    }
   
    public async Task<List<SelectListItem>> GetCountries()
    {
      var GetcountriesList = await _appDBContext.Settings_CountryTypes.ToArrayAsync();
      var countriesList = GetcountriesList.Select(r => new SelectListItem
      {
        Value = r.CountryTypeID.ToString(),
        Text = r.CountryTypeName
      }).ToList();

      countriesList.Insert(0, new SelectListItem { Value = "0", Text = "Select Country" });

      return countriesList;
    }
    public async Task<List<SelectListItem>> GetBranchs()
    {
      var branchs = await _appDBContext.Settings_BranchTypes
        .Where(e => e.ActiveYNID == 1 && e.DeleteYNID != 1)
        .ToListAsync();

      var selectList = branchs.Select(r => new SelectListItem { Value = r.BranchTypeID.ToString(), Text = r.BranchTypeName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetBranchsWithoutZeroLine()
    {
      var branchs = await _appDBContext.Settings_BranchTypes.ToListAsync();

      var selectList = branchs.Select(r => new SelectListItem { Value = r.BranchTypeID.ToString(), Text = r.BranchTypeName }).ToList();
      return selectList;
    }
    public async Task<List<SelectListItem>> GetDepartments()
    {
      var departments = await _appDBContext.Settings_DepartmentTypes.ToListAsync();

      var selectList = departments.Select(r => new SelectListItem { Value = r.DepartmentTypeID.ToString(), Text = r.DepartmentTypeName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetDesignations()
    {
      var designations = await _appDBContext.Settings_DesignationTypes.ToListAsync();

      var selectList = designations.Select(r => new SelectListItem { Value = r.DesignationTypeID.ToString(), Text = r.DesignationTypeName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetQualifications()
    {
      var qualifications = await _appDBContext.Settings_QualificationTypes.ToListAsync();

      var selectList = qualifications.Select(r => new SelectListItem { Value = r.QualificationTypeID.ToString(), Text = r.QualificationTypeName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetUsers()
    {
      var users = await _appDBContext.CR_Users.ToListAsync();

      var selectList = users.Select(r => new SelectListItem { Value = r.UserID.ToString(), Text = CR_CipherKey.Decrypt(r.UserName) }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }

    public async Task<List<SelectListItem>> GetRoles()
    {
      var roles = await _appDBContext.Settings_RoleTypes
    .Where(e => e.ActiveYNID == 1)
    .ToListAsync();

      var selectList = roles.Select(r => new SelectListItem { Value = r.RoleTypeID.ToString(), Text = r.RoleTypeName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetSalaryOptions()
    {
      var salaryOptions = await _appDBContext.Settings_SalaryOptions.ToListAsync();

      var SalaryTypeList = salaryOptions.Select(r => new SelectListItem
      {
        Value = r.SalaryOptionId.ToString(),
        Text = r.SalaryOptionName
      }).ToList();

      SalaryTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return SalaryTypeList;
    }
    public async Task<List<SelectListItem>> GetContractTypes()
    {
      var contractTypes = await _appDBContext.Settings_ContractTypes.ToListAsync();

      var ContractTypeList = contractTypes.Select(r => new SelectListItem
      {
        Value = r.ContractTypeId.ToString(),
        Text = r.ContractTypeName
      }).ToList();

      ContractTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return ContractTypeList;
    }
     public async Task<List<SelectListItem>> GetEndOfServiceReasonTypes()
    {
      var EndOfServiceReasonTypes = await _appDBContext.Settings_EndOfServiceReasonTypes.ToListAsync();

      var EndOfServiceReasonTypeList = EndOfServiceReasonTypes.Select(r => new SelectListItem
      {
        Value = r.EndOfServiceReasonTypeId.ToString(),
        Text = r.EndOfServiceReasonTypeName
      }).ToList();

      EndOfServiceReasonTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return EndOfServiceReasonTypeList;
    }
    public async Task<List<SelectListItem>> GetDeductionValueList()
    {
      var deductionValues = await _appDBContext.Settings_DeductionValues
          .Select(d => new SelectListItem
          {
            Value = d.DeductionValueID.ToString(),
            Text = d.DeductionValueText
          })
          .ToListAsync();

      deductionValues.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return deductionValues;
    }
    public async Task<List<SelectListItem>> GetClassIDList()
    {
      var classIDList = await _appDBContext.Settings_DeductionClasses
          .Select(c => new SelectListItem
          {
            Value = c.ClassID.ToString(),
            Text = c.ClassName
          })
          .ToListAsync();

      classIDList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return classIDList;
    }
    public async Task<List<SelectListItem>> GetDeductionTypes()
    {
      var deductionTypeList = await _appDBContext.Settings_DeductionTypes
          .Select(c => new SelectListItem
          {
            Value = c.DeductionTypeID.ToString(),
            Text = c.DeductionTypeName
          })
          .ToListAsync();

      deductionTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return deductionTypeList;
    }
    //GetOverTimeTypes
    public async Task<List<SelectListItem>> GetOverTimeTypes()
    {
      var overTimeTypeList = await _appDBContext.Settings_OverTimeTypes
          .Select(c => new SelectListItem
          {
            Value = c.OverTimeTypeID.ToString(),
            Text = c.OverTimeTypeName
          })
          .ToListAsync();

      overTimeTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return overTimeTypeList;
    }
    public async Task<List<SelectListItem>> GetOverTimeRates()
    {
      var overTimeRateList = await _appDBContext.Settings_OverTimeRates
          .Select(c => new SelectListItem
          {
            Value = c.OverTimeRateTypeID.ToString(), // Use OvertimeRateTypeID here
            Text = c.OverTimeRateValue.ToString() // Ensure OvertimeRateValue is a string
          })
          .ToListAsync();

      // Add the default "0" item
      overTimeRateList.Insert(0, new SelectListItem { Value = "0", Text = "0" });

      return overTimeRateList;
    }
    public async Task<List<SelectListItem>> GetVacationTypes()
    {
      var vacationTypeList = await _appDBContext.Settings_VacationTypes
          .Select(c => new SelectListItem
          {
            Value = c.VacationTypeID.ToString(),
            Text = c.VacationTypeName
          })
          .ToListAsync();

      vacationTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return vacationTypeList;
    }
    public async Task<List<SelectListItem>> GetVacationDates()
    {
      var vacationDateList = await _appDBContext.HR_Vacations
          .Select(c => new SelectListItem
          {
            Value = c.VacationID.ToString(),
            Text = c.Date.ToString()
          })
          .ToListAsync();

      vacationDateList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return vacationDateList;
    }
    public async Task<List<SelectListItem>> GetVacationDatesByVacationD(int vacationID)
    {
      var vacationDateList = await _appDBContext.HR_Vacations
          .Where(v => v.VacationID == vacationID) // Filter by EmployeeID
          .Select(v => new SelectListItem
          {
            Value = v.VacationID.ToString(),
            Text = v.Date.ToString("dd-MMM-yyyy")
          })
          .ToListAsync();

      // Add a default 'Please Select' option
     
      return vacationDateList;
    }
    public async Task<List<SelectListItem>> GetVacationDatesByEmployeeID(int employeeID)
    {
      var vacationDateList = await _appDBContext.HR_Vacations
          .Where(v => v.EmployeeID == employeeID) // Filter by EmployeeID
          .Select(v => new SelectListItem
          {
            Value = v.VacationID.ToString(),
            Text = v.Date.ToString("dd-MMM-yyyy")
          })
          .ToListAsync();

      // Add a default 'Please Select' option
      vacationDateList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return vacationDateList;
    }
    public async Task<List<SelectListItem>> GetVacationDatesByEmployeeIDWithoutSettled(int employeeID)
    {
      var vacationDateList = await _appDBContext.HR_Vacations
     .Where(v => v.EmployeeID == employeeID &&
                 !_appDBContext.HR_VacationSettles.Select(vs => vs.VacationID).Contains(v.VacationID)) // Filter by EmployeeID and exclude settled vacations
     .Select(v => new SelectListItem
     {
       Value = v.VacationID.ToString(),
       Text = v.Date.ToString("dd-MMM-yyyy") // Format the date
     })
     .ToListAsync();
     
      // Add a default 'Please Select' option
      vacationDateList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return vacationDateList;
    }
    public async Task<List<SelectListItem>> GetSalaryTypeList()
    {
      var SalaryTypeList = await _appDBContext.Settings_SalaryTypes
          .Select(d => new SelectListItem
          {
            Value = d.SalaryTypeID.ToString(),
            Text = d.SalaryTypeName
          })
          .ToListAsync();

      SalaryTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return SalaryTypeList;
    }
    public async Task<List<SelectListItem>> GetEmployeeRequestTypes()
    {
      var EmployeeRequestTypeList = await _appDBContext.Settings_EmployeeRequestTypes
          .Select(d => new SelectListItem
          {
            Value = d.EmployeeRequestTypeID.ToString(),
            Text = d.EmployeeRequestTypeName
          })
          .ToListAsync();

      EmployeeRequestTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return EmployeeRequestTypeList;
    }
    public async Task<List<SelectListItem>> GetProcessTypes()
    {
      var processTypeList = await _appDBContext.Settings_ProcessTypes
          .Select(d => new SelectListItem
          {
            Value = d.ProcessTypeID.ToString(),
            Text = d.ProcessTypeName
          })
          .ToListAsync();

      processTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return processTypeList;
    }
    public async Task<List<SelectListItem>> GetMonthsTypes()
    {
      try
      {
        var monthTypeList = await _appDBContext.Settings_MonthTypes
            .Select(d => new SelectListItem
            {
              Value = d.MonthTypeID.ToString(),
              Text = d.MonthTypeName
            })
            .ToListAsync();

        monthTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

        return monthTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetMonthsTypesWithoutZeroLine()
    {
      try
      {
        var monthTypeList = await _appDBContext.Settings_MonthTypes
            .Select(d => new SelectListItem
            {
              Value = d.MonthTypeID.ToString(),
              Text = d.MonthTypeName
            })
            .ToListAsync();

 
        return monthTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<dynamic> GetDirectManager()
    {
      var directManagerList = await _appDBContext.CR_Users
           .Select(d => new SelectListItem
           {
             Value = d.UserID.ToString(),
             Text = CR_CipherKey.Decrypt(d.UserName)
            })
           .ToListAsync();

      directManagerList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return directManagerList;
    }

    public async Task<dynamic> GetSearchingEmployee(string term)
    {
      var employees = await _appDBContext.HR_Employees
          .Where(b => b.DeleteYNID != 1 &&
              (
                  (b.FirstName + " " + b.FatherName + " " + b.FamilyName).Contains(term) ||
                  (b.DepartmentType.DepartmentTypeName != null && b.DepartmentType.DepartmentTypeName.Contains(term)) ||
                  (b.BranchType.BranchTypeName != null && b.BranchType.BranchTypeName.Contains(term)) ||
                  (b.DesignationType.DesignationTypeName != null && b.DesignationType.DesignationTypeName.Contains(term)) ||
                  (b.Phone1 != null && b.Phone1.Contains(term)) ||
                  (b.Phone2 != null && b.Phone2.Contains(term)) ||
                  (b.Mobile != null && b.Mobile.Contains(term)) ||
                  (b.Whatsapp != null && b.Whatsapp.Contains(term)) ||
                  (b.IDNumber != null && b.IDNumber.Contains(term)) ||
                  (b.PassportNumber != null && b.PassportNumber.Contains(term))
              )
          )
          .Include(b => b.BranchType)
          .Include(b => b.DepartmentType)
          .Include(b => b.DesignationType)
          .Select(e => new
          {
            id = e.EmployeeID,
            label = $@"
                <div class='employee-suggestion'>
                    <strong>{e.FirstName} {e.FatherName} {e.FamilyName}</strong><br />
                    <span>{e.DepartmentType.DepartmentTypeName}, {e.BranchType.BranchTypeName}</span><br />
                    <span>{e.Phone1 ?? e.Phone2 ?? e.Mobile ?? e.Whatsapp}</span>
                </div>",
            Name = $"{e.FirstName} {e.FatherName} {e.FamilyName}" // Full name to be returned

          })
          .ToListAsync();

      return employees;
    }

  }
}
