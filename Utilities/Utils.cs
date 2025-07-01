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
    public async Task<List<SelectListItem>> GetActiveYNIDList()
    {
      var ActiveYNIDList = await _appDBContext.Settings_ActiveYNIDTypes.ToListAsync();

      var selectList = ActiveYNIDList.Select(r => new SelectListItem { Value = r.ActiveYNID.ToString(), Text = r.ActiveName }).ToList();
      return selectList;
    }

    public async Task<List<SelectListItem>> GetDeleteYNIDList()
    {
      var DeleteYNIDList = await _appDBContext.Settings_DeleteYNIDTypes.ToListAsync();

      var selectList = DeleteYNIDList.Select(r => new SelectListItem { Value = r.DeleteYNID.ToString(), Text = r.DeleteName }).ToList();
      return selectList;
    }
    public async Task<List<SelectListItem>> GetEmployee()
    {
      var Employees = await _appDBContext.HR_Employees.ToListAsync();

      var selectList = Employees.Select(r => new SelectListItem { Value = r.EmployeeID.ToString(), Text = r.FirstName + ' ' + r.FatherName + ' ' + r.FamilyName }).ToList();
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
      var branchs = await _appDBContext.Settings_BranchTypes
        .Where(e => e.ActiveYNID == 1 && e.DeleteYNID != 1)
        .ToListAsync();

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
    public async Task<List<SelectListItem>> GetHolidayTypes()
    {
      var HolidayTypes = await _appDBContext.Settings_HolidayTypes.ToListAsync();

      var HolidayTypeList = HolidayTypes.Select(r => new SelectListItem
      {
        Value = r.HolidayTypeID.ToString(),
        Text = r.HolidayTypeName
      }).ToList();

      HolidayTypeList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return HolidayTypeList;
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

    public async Task<List<SelectListItem>> GetHeadofAccount_GroupType()
    {
      try
      {
        var GroupList = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Balance Sheet" },
            new SelectListItem { Value = "2", Text = "Income Sheet" }
        };

        return await Task.FromResult(GroupList);
      }
      catch (Exception ex)
      {
        throw; // Rethrow the exception to handle it at a higher level
      }
    }
    public async Task<List<SelectListItem>> GetTransactionType()
    {
      try
      {
        var TransactionTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Debit" },
            new SelectListItem { Value = "2", Text = "Credit" }
        };

        return await Task.FromResult(TransactionTypeList);
      }
      catch (Exception ex)
      {
        throw; // Rethrow the exception to handle it at a higher level
      }
    }
    public async Task<List<SelectListItem>> GetIntrumentType()
    {
      try
      {
        var TransactionTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Cash" },
            new SelectListItem { Value = "2", Text = "Chaque" }
        };

        return await Task.FromResult(TransactionTypeList);
      }
      catch (Exception ex)
      {
        throw; // Rethrow the exception to handle it at a higher level
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_CategoryType()
    {
      try
      {
        var categoryTypeList = await _appDBContext.Settings_HeadofAccount_CategoryTypes
            .Select(d => new SelectListItem
            {
              Value = d.CategoryTypeID.ToString(),
              Text = d.CategoryTypeName
            })
            .ToListAsync();


        return categoryTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_First()
    {
      try
      {
        var firstList = await _appDBContext.Settings_HeadofAccount_Firsts
            .Select(d => new SelectListItem
            {
              Value = d.HeadofAccount_FirstID.ToString(),
              Text = d.HeadofAccount_FirstName
            })
            .ToListAsync();


        return firstList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_Second()
    {
      try
      {
        var secondList = await _appDBContext.Settings_HeadofAccount_Seconds
            .Select(d => new SelectListItem
            {
              Value = d.HeadofAccount_SecondID.ToString(),
              Text = d.HeadofAccount_SecondName
            })
            .ToListAsync();


        return secondList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_Third()
    {
      try
      {
        var thirdList = await _appDBContext.Settings_HeadofAccount_Thirds
            .Select(d => new SelectListItem
            {
              Value = d.HeadofAccount_ThirdID.ToString(),
              Text = d.HeadofAccount_ThirdName
            })
            .ToListAsync();


        return thirdList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_Four()
    {
      try
      {
        var fourList = await _appDBContext.Settings_HeadofAccount_Fours
            .Select(d => new SelectListItem
            {
              Value = d.HeadofAccount_FourID.ToString(),
              Text = d.HeadofAccount_FourName
            })
            .ToListAsync();


        return fourList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_Five()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
            .Select(d => new SelectListItem
            {
              Value = d.HeadofAccount_FiveID.ToString(),
              Text = d.HeadofAccount_FiveName
            })
            .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveOnlyCashandBank()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .AsNoTracking()
             .Include(d => d.HeadofAccount_Four)
             .ThenInclude(f => f.HeadofAccount_Third) // Ensures nested inclusion
             .Where(d => d.HeadofAccount_Four.HeadofAccount_ThirdID == 3)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }

    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveOnlyReceivable()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .Where(d => d.CategoryTypeID == 1)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveOnlyPayable()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .Where(d => d.CategoryTypeID == 2)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveOnlyVendor()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .Where(d => d.HeadofAccount_FourID == 12)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveIncomeTaxPayable()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .Where(d => d.HeadofAccount_FourID == 4)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetHeadofAccount_FiveSaleTaxPayable()
    {
      try
      {
        var fiveList = await _appDBContext.Settings_HeadofAccount_Fives
             .Where(d => d.HeadofAccount_FourID == 5)
             .Select(d => new SelectListItem
             {
               Value = d.HeadofAccount_FiveID.ToString(),
               Text = d.HeadofAccount_FiveName
             })
             .ToListAsync();


        return fiveList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> Get_FI_BankList()
    {
      try
      {
        var bankList = await _appDBContext.FI_Banks
            .Select(d => new SelectListItem
            {
              Value = d.BankID.ToString(),
              Text = d.BankName
            })
            .ToListAsync();


        return bankList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVoucherType()
    {
      try
      {
        var VoucherTypeList = await _appDBContext.Settings_VoucherTypes
            .Select(d => new SelectListItem
            {
              Value = d.VoucherTypeID.ToString(),
              Text = d.VoucherTypeName
            })
            .ToListAsync();


        return VoucherTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVoucherType_Journal()
    {
      try
      {
        var VoucherTypeList = await _appDBContext.Settings_VoucherTypes
          .Where(d => d.VoucherNature == "Journal")
            .Select(d => new SelectListItem
            {
              Value = d.VoucherTypeID.ToString(),
              Text = d.VoucherTypeName
            })
            .ToListAsync();


        return VoucherTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVoucherType_Transfer()
    {
      try
      {
        var VoucherTypeList = await _appDBContext.Settings_VoucherTypes
          .Where(d => d.VoucherNature == "Transfer")
            .Select(d => new SelectListItem
            {
              Value = d.VoucherTypeID.ToString(),
              Text = d.VoucherTypeName
            })
            .ToListAsync();


        return VoucherTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVoucherType_Payment()
    {
      try
      {
        var VoucherTypeList = await _appDBContext.Settings_VoucherTypes
          .Where(d => d.VoucherNature == "Payment")
            .Select(d => new SelectListItem
            {
              Value = d.VoucherTypeID.ToString(),
              Text = d.VoucherTypeName
            })
            .ToListAsync();


        return VoucherTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVoucherType_Received()
    {
      try
      {
        var VoucherTypeList = await _appDBContext.Settings_VoucherTypes
          .Where(d => d.VoucherNature == "Received")
            .Select(d => new SelectListItem
            {
              Value = d.VoucherTypeID.ToString(),
              Text = d.VoucherTypeName
            })
            .ToListAsync();


        return VoucherTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVendorList()
    {
      try
      {
        var VendorList = await _appDBContext.FI_Vendors
          .Include(d => d.HeadofAccount_Five)
            .Select(d => new SelectListItem
            {
              Value = d.VendorID.ToString(),
              Text = d.HeadofAccount_Five.HeadofAccount_FiveName
            })
            .ToListAsync();


        return VendorList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetVendorListbyComparison(int purchaseRequestId)
    {
      try
      {
        var vendorIdRow = await _appDBContext.PR_RequestForQuotations
    .Where(pr => pr.PurchaseRequestID == purchaseRequestId)
    .Select(pr => new
    {
      pr.QuotationVendorID1,
      pr.QuotationVendorID2,
      pr.QuotationVendorID3
    })
    .FirstOrDefaultAsync();

        if (vendorIdRow == null)
        {
          return new List<SelectListItem>();
        }

        // Ab directly `OR` condition
        var VendorListbyComparison = await _appDBContext.FI_Vendors
          .Include(d => d.HeadofAccount_Five)
            .Where(v =>
                v.VendorID == vendorIdRow.QuotationVendorID1 ||
                v.VendorID == vendorIdRow.QuotationVendorID2 ||
                v.VendorID == vendorIdRow.QuotationVendorID3
            )
            .Select(v => new SelectListItem
            {
              Value = v.VendorID.ToString(),
              Text = v.HeadofAccount_Five.HeadofAccount_FiveName
            })
            .ToListAsync();

        return VendorListbyComparison;

      }
      catch (Exception ex)
      {
        throw;
      }

    }



    //GetItemFromProcurementQueueList
    public async Task<List<SelectListItem>> GetItemFromProcurementQueueList()
    {
      try
      {
        var usedProcurementQueueIDs = _appDBContext.PR_PurchaseRequests
    .Select(p => p.ProcurementQueueID)
    .Distinct();

        var ItemList = await _appDBContext.PR_ProcurementQueues
            .Where(q => !usedProcurementQueueIDs.Contains(q.ProcurementQueueID))
            .Include(q => q.Item)
            .Select(d => new SelectListItem
            {
              Value = d.ProcurementQueueID.ToString(),
              Text = d.Item.ItemName + " (Qty-" + d.Quantity + ")"
            })
            .ToListAsync();

        return ItemList;

      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    //GetItemCategorys
    public async Task<List<SelectListItem>> GetItemList()
    {
      try
      {
        var ItemList = await _appDBContext.ST_Items
            .Select(d => new SelectListItem
            {
              Value = d.ItemID.ToString(),
              Text = d.ItemName
            })
            .ToListAsync();


        return ItemList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }


    //GetItemCategorys
    public async Task<List<SelectListItem>> GetItemCategorys()
    {
      try
      {
        var ItemCategoryTypeList = await _appDBContext.Settings_ItemCategoryTypes
            .Select(d => new SelectListItem
            {
              Value = d.ItemCategoryTypeID.ToString(),
              Text = d.ItemCategoryTypeName
            })
            .ToListAsync();


        return ItemCategoryTypeList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetItemUnits()
    {
      try
      {
        var ItemUnitList = await _appDBContext.Settings_UnitTypes
            .Select(d => new SelectListItem
            {
              Value = d.UnitTypeID.ToString(),
              Text = d.UnitTypeName
            })
            .ToListAsync();


        return ItemUnitList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetPriorityLevel()
    {
      try
      {
        var PriorityLevelList = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "LOW" },
            new SelectListItem { Value = "2", Text = "MED" },
            new SelectListItem { Value = "3", Text = "HGH" },
            new SelectListItem { Value = "4", Text = "URG" }
        };

        return await Task.FromResult(PriorityLevelList);
      }
      catch (Exception ex)
      {
        throw; // Rethrow the exception to handle it at a higher level
      }
    }
    public async Task<List<SelectListItem>> GetItemManufacturers()
    {
      try
      {
        var ItemManufacturerList = await _appDBContext.Settings_ManufacturerTypes
            .Select(d => new SelectListItem
            {
              Value = d.ManufacturerTypeID.ToString(),
              Text = d.ManufacturerTypeName
            })
            .ToListAsync();


        return ItemManufacturerList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetRequisitionStatus()
    {
      try
      {
        var RequisitionStatusList = await _appDBContext.Settings_RequisitionStatusTypes
            .Select(d => new SelectListItem
            {
              Value = d.RequisitionStatusTypeID.ToString(),
              Text = d.RequisitionStatusTypeName
            })
            .ToListAsync();


        return RequisitionStatusList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetIssuanceStatus()
    {
      try
      {
        var IssuanceStatusList = await _appDBContext.Settings_IssuanceStatusTypes
            .Select(d => new SelectListItem
            {
              Value = d.IssuanceStatusTypeID.ToString(),
              Text = d.IssuanceStatusTypeName
            })
            .ToListAsync();


        return IssuanceStatusList;
      }
      catch (Exception ex)
      {
        // Log the exception (ex.Message or ex.StackTrace)
        throw; // or handle it accordingly
      }
    }
    public async Task<List<SelectListItem>> GetItemComponentDataType()
    {
      try
      {
        var DataTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Integer" },
            new SelectListItem { Value = "2", Text = "Date" },
            new SelectListItem { Value = "3", Text = "String" },
            new SelectListItem { Value = "4", Text = "Decimal" }
        };

        return await Task.FromResult(DataTypeList);
      }
      catch (Exception ex)
      {
        throw; // Rethrow the exception to handle it at a higher level
      }
    }
    public async Task<int> PostUserIDGetEmployeeID(int userID)
    {
      try
      {
        var user = await _appDBContext.CR_Users
                            .Where(u => u.UserID == userID)
                            .FirstOrDefaultAsync();

        return user?.EmployeeID ?? 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching EmployeeID: {ex.Message}");
        return 0;
      }
    }

    public async Task<int> PostUserIDGetDepartmentID(int userID)
    {
      try
      {
        var departmentTypeId = await _appDBContext.HR_Employees
            .Join(_appDBContext.CR_Users,
                  emp => emp.EmployeeID,
                  us => us.EmployeeID,
                  (emp, us) => new { emp, us })
            .Where(x => x.us.UserID == userID)
            .Select(x => x.emp.DepartmentTypeID)
            .FirstOrDefaultAsync();

        return departmentTypeId ?? 0; // If departmentTypeId is null, return 0
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching DepartmentTypeID: {ex.Message}");
        return 0;
      }
    }

    public async Task<List<SelectListItem>> GetPendingRequisitions()
    {
      try
      {

        var PendingRequisitionsList = await _appDBContext.ST_MaterialRequisitions
     .Join(
         _appDBContext.Settings_DepartmentTypes,
         mr => mr.DepartmentTypeID,
         dt => dt.DepartmentTypeID,
         (mr, dt) => new
         {
           mr.RequisitionID,
           dt.DepartmentTypeName,
           mr.RequisitionDate,
           mr.RequisitionStatusTypeID
         })
     .Where(mr => mr.RequisitionStatusTypeID == 2 || mr.RequisitionStatusTypeID == 3)
     .Select(mr => new SelectListItem
     {
       Value = mr.RequisitionID.ToString(),
       Text = $"Department: {mr.DepartmentTypeName} - Requisition Date: {mr.RequisitionDate:dd/MM/yyyy}"
     })
     .ToListAsync();

        return PendingRequisitionsList;
      }
      catch (Exception ex)
      {
        // Log the exception details (if necessary)
        // Example: Log.Error(ex, "Error occurred while fetching pending requisitions");
        throw; // Rethrow the exception to handle it further up the call stack
      }
    }

  }
}
