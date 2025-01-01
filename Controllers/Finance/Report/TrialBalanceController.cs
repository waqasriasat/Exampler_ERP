using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class TrialBalanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public TrialBalanceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      // Fetch and group the data
      var rawData = await _appDBContext.FI_VoucherDetails
          .Include(v => v.HeadofAccount_Five)
              .ThenInclude(h5 => h5.HeadofAccount_Four)
              .ThenInclude(h4 => h4.HeadofAccount_Third)
              .ThenInclude(h3 => h3.HeadofAccount_Second)
              .ThenInclude(h2 => h2.HeadofAccount_First)
          .GroupBy(v => new
          {
            FirstName = v.HeadofAccount_Five.HeadofAccount_Four.HeadofAccount_Third.HeadofAccount_Second.HeadofAccount_First.HeadofAccount_FirstName,
            SecondName = v.HeadofAccount_Five.HeadofAccount_Four.HeadofAccount_Third.HeadofAccount_Second.HeadofAccount_SecondName,
            ThirdName = v.HeadofAccount_Five.HeadofAccount_Four.HeadofAccount_Third.HeadofAccount_ThirdName,
            FourthName = v.HeadofAccount_Five.HeadofAccount_Four.HeadofAccount_FourName,
            FifthName = v.HeadofAccount_Five.HeadofAccount_FiveName
          })
          .Select(g => new
          {
            g.Key.FirstName,
            g.Key.SecondName,
            g.Key.ThirdName,
            g.Key.FourthName,
            g.Key.FifthName,
            DebitTotal = g.Sum(x => x.DrAmt ?? 0),
            CreditTotal = g.Sum(x => x.CrAmt ?? 0)
          })
          .ToListAsync();

      // Build the tree structure
      var groupedTree = rawData
          .GroupBy(r => r.FirstName)
          .Select(firstGroup => new TrialBalanceTreeNode
          {
            Name = firstGroup.Key,
            Children = firstGroup.GroupBy(r => r.SecondName)
                  .Select(secondGroup => new TrialBalanceTreeNode
                  {
                    Name = secondGroup.Key,
                    Children = secondGroup.GroupBy(r => r.ThirdName)
                          .Select(thirdGroup => new TrialBalanceTreeNode
                          {
                            Name = thirdGroup.Key,
                            Children = thirdGroup.GroupBy(r => r.FourthName)
                                  .Select(fourthGroup => new TrialBalanceTreeNode
                                  {
                                    Name = fourthGroup.Key,
                                    Children = fourthGroup.Select(fifth => new TrialBalanceTreeNode
                                    {
                                      Name = fifth.FifthName,
                                      DebitTotal = fifth.DebitTotal,
                                      CreditTotal = fifth.CreditTotal
                                    }).ToList()
                                  }).ToList()
                          }).ToList()
                  }).ToList()
          })
          .ToList();

      //return View(groupedTree);
      return View("~/Views/Finance/Report/TrialBalance/TrialBalance.cshtml", groupedTree);
    }
  }
}
