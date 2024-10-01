using Exampler_ERP.Models;
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
      var genderOptions = await _appDBContext.Settings_Genders.ToListAsync();

      var GenderList = genderOptions.Select(g => new SelectListItem
      {
        Value = g.GenderID.ToString(),
        Text = g.GenderName
      }).ToList();

      GenderList.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

      return GenderList;
    }
    public async Task<List<SelectListItem>> GetMaritalStatus()
    {
      var maritalStatusOptions = await _appDBContext.Settings_MaritalStatuses.ToListAsync();

      var MaritalStatusList = maritalStatusOptions.Select(ms => new SelectListItem
      {
        Value = ms.MaritalStatusID.ToString(),
        Text = ms.MaritalStatusName
      }).ToList();

      MaritalStatusList.Insert(0, new SelectListItem { Value = "0", Text = "Select Marital Status" });

      return MaritalStatusList;
    }
    public async Task<List<SelectListItem>> GetReligion()
    {
      var religionOptions = await _appDBContext.Settings_Religions.ToListAsync();

      var ReligionList = religionOptions.Select(r => new SelectListItem
      {
        Value = r.ReligionID.ToString(),
        Text = r.ReligionName
      }).ToList();

      ReligionList.Insert(0, new SelectListItem { Value = "0", Text = "Select Religion" });

      return ReligionList;
    }
   
    public List<SelectListItem> GetCountries()
    {
      var CountriesList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Select Country" },
                new SelectListItem { Value = "1", Text = "Afghanistan" },
                new SelectListItem { Value = "2", Text = "Albania" },
                new SelectListItem { Value = "3", Text = "Algeria" },
                new SelectListItem { Value = "4", Text = "Andorra" },
                new SelectListItem { Value = "5", Text = "Angola" },
                new SelectListItem { Value = "6", Text = "Anguilla" },
                new SelectListItem { Value = "7", Text = "Antigua and Barbuda" },
                new SelectListItem { Value = "8", Text = "Argentina" },
                new SelectListItem { Value = "9", Text = "Armenia" },
                new SelectListItem { Value = "10", Text = "Aruba" },
                new SelectListItem { Value = "11", Text = "Australia" },
                new SelectListItem { Value = "12", Text = "Austria" },
                new SelectListItem { Value = "13", Text = "Azerbaijan" },
                new SelectListItem { Value = "14", Text = "Bahamas" },
                new SelectListItem { Value = "15", Text = "Bahrain" },
                new SelectListItem { Value = "16", Text = "Bangladesh" },
                new SelectListItem { Value = "17", Text = "Barbados" },
                new SelectListItem { Value = "18", Text = "Belarus" },
                new SelectListItem { Value = "19", Text = "Belgium" },
                new SelectListItem { Value = "20", Text = "Belize" },
                new SelectListItem { Value = "21", Text = "Benin" },
                new SelectListItem { Value = "22", Text = "Bermuda" },
                new SelectListItem { Value = "23", Text = "Bhutan" },
                new SelectListItem { Value = "24", Text = "Bolivia" },
                new SelectListItem { Value = "25", Text = "Bosnia and Herzegovina" },
                new SelectListItem { Value = "26", Text = "Botswana" },
                new SelectListItem { Value = "27", Text = "Brazil" },
                new SelectListItem { Value = "28", Text = "Brunei Darussalam" },
                new SelectListItem { Value = "29", Text = "Bulgaria" },
                new SelectListItem { Value = "30", Text = "Burkina Faso" },
                new SelectListItem { Value = "31", Text = "Burundi" },
                new SelectListItem { Value = "32", Text = "Cambodia" },
                new SelectListItem { Value = "33", Text = "Cameroon" },
                new SelectListItem { Value = "34", Text = "Canada" },
                new SelectListItem { Value = "35", Text = "Cape Verde" },
                new SelectListItem { Value = "36", Text = "Cayman Islands" },
                new SelectListItem { Value = "37", Text = "Central African Republic" },
                new SelectListItem { Value = "38", Text = "Chad" },
                new SelectListItem { Value = "39", Text = "Chile" },
                new SelectListItem { Value = "40", Text = "China" },
                new SelectListItem { Value = "41", Text = "Colombia" },
                new SelectListItem { Value = "42", Text = "Comoros" },
                new SelectListItem { Value = "43", Text = "Congo" },
                new SelectListItem { Value = "44", Text = "Democratic Republic of the Congo" },
                new SelectListItem { Value = "45", Text = "Costa Rica" },
                new SelectListItem { Value = "46", Text = "Côte d'Ivoire" },
                new SelectListItem { Value = "47", Text = "Croatia" },
                new SelectListItem { Value = "48", Text = "Cuba" },
                new SelectListItem { Value = "49", Text = "Curaçao" },
                new SelectListItem { Value = "50", Text = "Cyprus" },
                new SelectListItem { Value = "51", Text = "Czech Republic" },
                new SelectListItem { Value = "52", Text = "Denmark" },
                new SelectListItem { Value = "53", Text = "Djibouti" },
                new SelectListItem { Value = "54", Text = "Dominica" },
                new SelectListItem { Value = "55", Text = "Dominican Republic" },
                new SelectListItem { Value = "56", Text = "Ecuador" },
                new SelectListItem { Value = "57", Text = "Egypt" },
                new SelectListItem { Value = "58", Text = "El Salvador" },
                new SelectListItem { Value = "59", Text = "Equatorial Guinea" },
                new SelectListItem { Value = "60", Text = "Eritrea" },
                new SelectListItem { Value = "61", Text = "Estonia" },
                new SelectListItem { Value = "62", Text = "Ethiopia" },
                new SelectListItem { Value = "63", Text = "Faroe Islands" },
                new SelectListItem { Value = "64", Text = "Fiji" },
                new SelectListItem { Value = "65", Text = "Finland" },
                new SelectListItem { Value = "66", Text = "France" },
                new SelectListItem { Value = "67", Text = "French Guiana" },
                new SelectListItem { Value = "68", Text = "French Polynesia" },
                new SelectListItem { Value = "69", Text = "Gabon" },
                new SelectListItem { Value = "70", Text = "Gambia" },
                new SelectListItem { Value = "71", Text = "Georgia" },
                new SelectListItem { Value = "72", Text = "Germany" },
                new SelectListItem { Value = "73", Text = "Ghana" },
                new SelectListItem { Value = "74", Text = "Gibraltar" },
                new SelectListItem { Value = "75", Text = "Greece" },
                new SelectListItem { Value = "76", Text = "Greenland" },
                new SelectListItem { Value = "77", Text = "Grenada" },
                new SelectListItem { Value = "78", Text = "Guadeloupe" },
                new SelectListItem { Value = "79", Text = "Guam" },
                new SelectListItem { Value = "80", Text = "Guatemala" },
                new SelectListItem { Value = "81", Text = "Guernsey" },
                new SelectListItem { Value = "82", Text = "Guinea" },
                new SelectListItem { Value = "83", Text = "Guinea-Bissau" },
                new SelectListItem { Value = "84", Text = "Guyana" },
                new SelectListItem { Value = "85", Text = "Haiti" },
                new SelectListItem { Value = "86", Text = "Holy See" },
                new SelectListItem { Value = "87", Text = "Honduras" },
                new SelectListItem { Value = "88", Text = "Hong Kong" },
                new SelectListItem { Value = "89", Text = "Hungary" },
                new SelectListItem { Value = "90", Text = "Iceland" },
                new SelectListItem { Value = "91", Text = "India" },
                new SelectListItem { Value = "92", Text = "Indonesia" },
                new SelectListItem { Value = "93", Text = "Iran" },
                new SelectListItem { Value = "94", Text = "Iraq" },
                new SelectListItem { Value = "95", Text = "Ireland" },
                new SelectListItem { Value = "96", Text = "Isle of Man" },
                new SelectListItem { Value = "97", Text = "Israel" },
                new SelectListItem { Value = "98", Text = "Italy" },
                new SelectListItem { Value = "99", Text = "Jamaica" },
                new SelectListItem { Value = "100", Text = "Japan" },
                new SelectListItem { Value = "101", Text = "Jersey" },
                new SelectListItem { Value = "102", Text = "Jordan" },
                new SelectListItem { Value = "103", Text = "Kazakhstan" },
                new SelectListItem { Value = "104", Text = "Kenya" },
                new SelectListItem { Value = "105", Text = "Kiribati" },
                new SelectListItem { Value = "106", Text = "Kuwait" },
                new SelectListItem { Value = "107", Text = "Kyrgyzstan" },
                new SelectListItem { Value = "108", Text = "Lao People's Democratic Republic" },
                new SelectListItem { Value = "109", Text = "Latvia" },
                new SelectListItem { Value = "110", Text = "Lebanon" },
                new SelectListItem { Value = "111", Text = "Lesotho" },
                new SelectListItem { Value = "112", Text = "Liberia" },
                new SelectListItem { Value = "113", Text = "Libya" },
                new SelectListItem { Value = "114", Text = "Liechtenstein" },
                new SelectListItem { Value = "115", Text = "Lithuania" },
                new SelectListItem { Value = "116", Text = "Luxembourg" },
                new SelectListItem { Value = "117", Text = "Macao" },
                new SelectListItem { Value = "118", Text = "North Macedonia" },
                new SelectListItem { Value = "119", Text = "Madagascar" },
                new SelectListItem { Value = "120", Text = "Malawi" },
                new SelectListItem { Value = "121", Text = "Malaysia" },
                new SelectListItem { Value = "122", Text = "Maldives" },
                new SelectListItem { Value = "123", Text = "Mali" },
                new SelectListItem { Value = "124", Text = "Malta" },
                new SelectListItem { Value = "125", Text = "Marshall Islands" },
                new SelectListItem { Value = "126", Text = "Martinique" },
                new SelectListItem { Value = "127", Text = "Mauritania" },
                new SelectListItem { Value = "128", Text = "Mauritius" },
                new SelectListItem { Value = "129", Text = "Mayotte" },
                new SelectListItem { Value = "130", Text = "Mexico" },
                new SelectListItem { Value = "131", Text = "Micronesia" },
                new SelectListItem { Value = "132", Text = "Moldova" },
                new SelectListItem { Value = "133", Text = "Monaco" },
                new SelectListItem { Value = "134", Text = "Mongolia" },
                new SelectListItem { Value = "135", Text = "Montenegro" },
                new SelectListItem { Value = "136", Text = "Montserrat" },
                new SelectListItem { Value = "137", Text = "Morocco" },
                new SelectListItem { Value = "138", Text = "Mozambique" },
                new SelectListItem { Value = "139", Text = "Myanmar" },
                new SelectListItem { Value = "140", Text = "Namibia" },
                new SelectListItem { Value = "141", Text = "Nauru" },
                new SelectListItem { Value = "142", Text = "Nepal" },
                new SelectListItem { Value = "143", Text = "Netherlands" },
                new SelectListItem { Value = "144", Text = "New Caledonia" },
                new SelectListItem { Value = "145", Text = "New Zealand" },
                new SelectListItem { Value = "146", Text = "Nicaragua" },
                new SelectListItem { Value = "147", Text = "Niger" },
                new SelectListItem { Value = "148", Text = "Nigeria" },
                new SelectListItem { Value = "149", Text = "Niue" },
                new SelectListItem { Value = "150", Text = "Norfolk Island" },
                new SelectListItem { Value = "151", Text = "North Korea" },
                new SelectListItem { Value = "152", Text = "Norway" },
                new SelectListItem { Value = "153", Text = "Oman" },
                new SelectListItem { Value = "154", Text = "Pakistan" },
                new SelectListItem { Value = "155", Text = "Palau" },
                new SelectListItem { Value = "156", Text = "Palestine, State of" },
                new SelectListItem { Value = "157", Text = "Panama" },
                new SelectListItem { Value = "158", Text = "Papua New Guinea" },
                new SelectListItem { Value = "159", Text = "Paraguay" },
                new SelectListItem { Value = "160", Text = "Peru" },
                new SelectListItem { Value = "161", Text = "Philippines" },
                new SelectListItem { Value = "162", Text = "Pitcairn" },
                new SelectListItem { Value = "163", Text = "Poland" },
                new SelectListItem { Value = "164", Text = "Portugal" },
                new SelectListItem { Value = "165", Text = "Puerto Rico" },
                new SelectListItem { Value = "166", Text = "Qatar" },
                new SelectListItem { Value = "167", Text = "Réunion" },
                new SelectListItem { Value = "168", Text = "Romania" },
                new SelectListItem { Value = "169", Text = "Russian Federation" },
                new SelectListItem { Value = "170", Text = "Rwanda" },
                new SelectListItem { Value = "171", Text = "Saint Barthélemy" },
                new SelectListItem { Value = "172", Text = "Saint Helena, Ascension and Tristan da Cunha" },
                new SelectListItem { Value = "173", Text = "Saint Kitts and Nevis" },
                new SelectListItem { Value = "174", Text = "Saint Lucia" },
                new SelectListItem { Value = "175", Text = "Saint Martin (French part)" },
                new SelectListItem { Value = "176", Text = "Saint Pierre and Miquelon" },
                new SelectListItem { Value = "177", Text = "Saint Vincent and the Grenadines" },
                new SelectListItem { Value = "178", Text = "Samoa" },
                new SelectListItem { Value = "179", Text = "San Marino" },
                new SelectListItem { Value = "180", Text = "Sao Tome and Principe" },
                new SelectListItem { Value = "181", Text = "Saudi Arabia" },
                new SelectListItem { Value = "182", Text = "Senegal" },
                new SelectListItem { Value = "183", Text = "Serbia" },
                new SelectListItem { Value = "184", Text = "Seychelles" },
                new SelectListItem { Value = "185", Text = "Sierra Leone" },
                new SelectListItem { Value = "186", Text = "Singapore" },
                new SelectListItem { Value = "187", Text = "Sint Maarten (Dutch part)" },
                new SelectListItem { Value = "188", Text = "Slovakia" },
                new SelectListItem { Value = "189", Text = "Slovenia" },
                new SelectListItem { Value = "190", Text = "Solomon Islands" },
                new SelectListItem { Value = "191", Text = "Somalia" },
                new SelectListItem { Value = "192", Text = "South Africa" },
                new SelectListItem { Value = "193", Text = "South Korea" },
                new SelectListItem { Value = "194", Text = "South Sudan" },
                new SelectListItem { Value = "195", Text = "Spain" },
                new SelectListItem { Value = "196", Text = "Sri Lanka" },
                new SelectListItem { Value = "197", Text = "Sudan" },
                new SelectListItem { Value = "198", Text = "Suriname" },
                new SelectListItem { Value = "199", Text = "Eswatini" },
                new SelectListItem { Value = "200", Text = "Sweden" },
                new SelectListItem { Value = "201", Text = "Switzerland" },
                new SelectListItem { Value = "202", Text = "Syrian Arab Republic" },
                new SelectListItem { Value = "203", Text = "Taiwan" },
                new SelectListItem { Value = "204", Text = "Tajikistan" },
                new SelectListItem { Value = "205", Text = "Tanzania" },
                new SelectListItem { Value = "206", Text = "Thailand" },
                new SelectListItem { Value = "207", Text = "Timor-Leste" },
                new SelectListItem { Value = "208", Text = "Togo" },
                new SelectListItem { Value = "209", Text = "Tokelau" },
                new SelectListItem { Value = "210", Text = "Tonga" },
                new SelectListItem { Value = "211", Text = "Trinidad and Tobago" },
                new SelectListItem { Value = "212", Text = "Tunisia" },
                new SelectListItem { Value = "213", Text = "Turkey" },
                new SelectListItem { Value = "214", Text = "Turkmenistan" },
                new SelectListItem { Value = "215", Text = "Turks and Caicos Islands" },
                new SelectListItem { Value = "216", Text = "Tuvalu" },
                new SelectListItem { Value = "217", Text = "Uganda" },
                new SelectListItem { Value = "218", Text = "Ukraine" },
                new SelectListItem { Value = "219", Text = "United Arab Emirates" },
                new SelectListItem { Value = "220", Text = "United Kingdom" },
                new SelectListItem { Value = "221", Text = "United States" },
                new SelectListItem { Value = "222", Text = "Uruguay" },
                new SelectListItem { Value = "223", Text = "Uzbekistan" },
                new SelectListItem { Value = "224", Text = "Vanuatu" },
                new SelectListItem { Value = "225", Text = "Venezuela" },
                new SelectListItem { Value = "226", Text = "Viet Nam" },
                new SelectListItem { Value = "227", Text = "Wallis and Futuna" },
                new SelectListItem { Value = "228", Text = "Western Sahara" },
                new SelectListItem { Value = "229", Text = "Yemen" },
                new SelectListItem { Value = "230", Text = "Zambia" }
            };

      return CountriesList;
    }
    public async Task<List<SelectListItem>> GetBranchs()
    {
      var branchs = await _appDBContext.Settings_Branchs.ToListAsync();

      var selectList = branchs.Select(r => new SelectListItem { Value = r.BranchID.ToString(), Text = r.BranchName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetDepartments()
    {
      var departments = await _appDBContext.Settings_Departments.ToListAsync();

      var selectList = departments.Select(r => new SelectListItem { Value = r.DepartmentID.ToString(), Text = r.DepartmentName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetDesignations()
    {
      var designations = await _appDBContext.Settings_Designations.ToListAsync();

      var selectList = designations.Select(r => new SelectListItem { Value = r.DesignationID.ToString(), Text = r.DesignationName }).ToList();
      selectList.Insert(0, new SelectListItem { Value = "Please Select", Text = "Please Select" });
      return selectList;
    }
    public async Task<List<SelectListItem>> GetQualifications()
    {
      var qualifications = await _appDBContext.Settings_Qualifications.ToListAsync();

      var selectList = qualifications.Select(r => new SelectListItem { Value = r.QualificationID.ToString(), Text = r.QualificationName }).ToList();
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
      var roles = await _appDBContext.Settings_Roles
    .Where(e => e.ActiveYNID == 1)
    .ToListAsync();

      var selectList = roles.Select(r => new SelectListItem { Value = r.RoleID.ToString(), Text = r.RoleName }).ToList();
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
  }
}
