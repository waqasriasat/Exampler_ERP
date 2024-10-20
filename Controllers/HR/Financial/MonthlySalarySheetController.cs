using Exampler_ERP.Controllers.HR.Employeement;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class MonthlySalarySheetController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonthlySalarySheetController> _logger;
    private readonly Utils _utils;

    public MonthlySalarySheetController(AppDBContext appDBContext, IConfiguration configuration, ILogger<MonthlySalarySheetController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }

    public async Task<IActionResult> Index(int month = 10, int year = 2024)
    {
      var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1)
          .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();

      // Use LINQ to process each employee and fetch their salary data
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(employee.EmployeeID.ToString(), month, year);
        return salaryData;
      });

      // Wait for all tasks to complete
      var salaryDataResults = await Task.WhenAll(tasks);

      // Flatten the results into a single list
      salarySheets.AddRange(salaryDataResults);

      return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", salarySheets);
    }
    private async Task<MonthlySalarySheetViewModel> GetMonthlySalarySheetAsync(string employeeId, int month, int year)
    {
      var salarySheet = new MonthlySalarySheetViewModel
      {
        EmployeeID = int.Parse(employeeId)
      };

      var connectionString = _configuration.GetConnectionString("AppDb");
      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var commandText = "EXEC GetMonthlySalarySheet @EmployeeID, @Month, @Year;";
        using (var command = new SqlCommand(commandText, connection))
        {
          command.Parameters.AddWithValue("@EmployeeID", employeeId);
          command.Parameters.AddWithValue("@Month", month);
          command.Parameters.AddWithValue("@Year", year);

          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              for (int i = 0; i < reader.FieldCount; i++)
              {
                string columnName = reader.GetName(i);

                // Handle nullable or non-nullable values
                var value = reader.IsDBNull(i) ? "0" : reader.GetValue(i).ToString();

                // Process each column by category
                if (columnName.StartsWith("Salary_"))
                {
                  salarySheet.SalaryDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                }
                else if (columnName.StartsWith("AdditionalAllowance_"))
                {
                  salarySheet.AdditionalAllowances.Add(columnName.Split('_')[1], decimal.Parse(value));
                }
                else if (columnName.StartsWith("OverTime_"))
                {
                  salarySheet.OvertimeDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                }
                else if (columnName.StartsWith("Deduction_"))
                {
                  salarySheet.Deductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                }
                else if (columnName.StartsWith("FixedDeduction_"))
                {
                  salarySheet.FixedDeductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                }
              }
            }
          }
        }
      }

      return salarySheet;
    }


    //private async Task<MonthlySalarySheetViewModel> GetMonthlySalarySheetAsync(string employeeId, int month, int year)
    //{
    //  var salarySheet = new MonthlySalarySheetViewModel
    //  {
    //    EmployeeID = int.Parse(employeeId) // Assuming EmployeeID is an integer
    //  };

    //  var connectionString = _configuration.GetConnectionString("AppDb"); // Adjust connection string name
    //  using (var connection = new SqlConnection(connectionString))
    //  {
    //    await connection.OpenAsync();

    //    // Execute the stored procedure to get salary details
    //    var commandText = "EXEC GetMonthlySalarySheet @EmployeeID, @Month, @Year;";
    //    using (var command = new SqlCommand(commandText, connection))
    //    {
    //      command.Parameters.AddWithValue("@EmployeeID", employeeId);
    //      command.Parameters.AddWithValue("@Month", month);
    //      command.Parameters.AddWithValue("@Year", year);

    //      using (var reader = await command.ExecuteReaderAsync())
    //      {
    //        while (await reader.ReadAsync())
    //        {
    //          // Adjusting the reading logic based on actual column types
    //          string salaryTypeName = reader.GetString(1); // Assuming SalaryTypeName is still a string

    //          // Check if the salary value is a double
    //          decimal salaryValue = reader.IsDBNull(2) ? 0 : (decimal)reader.GetDouble(2); // Assuming SalaryValue is a double

    //          // Assuming salary type names have unique prefixes for each dictionary
    //          if (salaryTypeName.StartsWith("Salary_"))
    //          {
    //            salarySheet.SalaryDetails.Add(int.Parse(salaryTypeName.Split('_')[1]), salaryValue);
    //          }
    //          else if (salaryTypeName.StartsWith("AdditionalAllowance_"))
    //          {
    //            salarySheet.AdditionalAllowances.Add(int.Parse(salaryTypeName.Split('_')[1]), salaryValue);
    //          }
    //          else if (salaryTypeName.StartsWith("OverTime_"))
    //          {
    //            salarySheet.OvertimeDetails.Add(int.Parse(salaryTypeName.Split('_')[1]), salaryValue);
    //          }
    //          else if (salaryTypeName.StartsWith("Deduction_"))
    //          {
    //            salarySheet.Deductions.Add(int.Parse(salaryTypeName.Split('_')[1]), salaryValue);
    //          }
    //          else if (salaryTypeName.StartsWith("FixedDeduction_"))
    //          {
    //            salarySheet.FixedDeductions.Add(int.Parse(salaryTypeName.Split('_')[1]), salaryValue);
    //          }
    //        }
    //      }
    //    }
    //  }

    //  return salarySheet;
    //}

    public async Task<IActionResult> PrintCard(int employeeId)
    {
      var employee = await _appDBContext.HR_Employees
          .Include(e => e.DesignationType)
          .Include(e => e.DepartmentType)
          .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

      // If employee is not found, return a 404 Not Found result
      if (employee == null)
      {
        return NotFound();
      }

      // Return the custom view for printing the employee card, passing the employee model
      return View("~/Views/HR/Financial/MonthlySalarySheet/PrintMonthlySalarySheet.cshtml", employee);
    }
  }
}
