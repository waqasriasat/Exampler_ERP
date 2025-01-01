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
            OpeningBalance = g.Sum(x => x.HeadofAccount_Five.OpeningBalance ?? 0),
            DebitTotal = g.Sum(x => x.DrAmt ?? 0),
            CreditTotal = g.Sum(x => x.CrAmt ?? 0),
            CurrentBalance = g.Sum(x => (x.HeadofAccount_Five.OpeningBalance ?? 0) + (x.DrAmt ?? 0) - (x.CrAmt ?? 0))
          })
          .ToListAsync();

      // Build the tree structure with aggregated values
      var groupedTree = rawData
          .GroupBy(r => r.FirstName)
          .Select(firstGroup => new TrialBalanceTreeNode
          {
            Name = firstGroup.Key,
            //OpeningBalance = firstGroup.Sum(x => x.OpeningBalance),
            //DebitTotal = firstGroup.Sum(x => x.DebitTotal),
            //CreditTotal = firstGroup.Sum(x => x.CreditTotal),
            //CurrentBalance = firstGroup.Sum(x => x.CurrentBalance),
            Children = firstGroup.GroupBy(r => r.SecondName)
                  .Select(secondGroup => new TrialBalanceTreeNode
                  {
                    Name = secondGroup.Key,
                    //OpeningBalance = secondGroup.Sum(x => x.OpeningBalance),
                    //DebitTotal = secondGroup.Sum(x => x.DebitTotal),
                    //CreditTotal = secondGroup.Sum(x => x.CreditTotal),
                    //CurrentBalance = secondGroup.Sum(x => x.CurrentBalance),
                    Children = secondGroup.GroupBy(r => r.ThirdName)
                          .Select(thirdGroup => new TrialBalanceTreeNode
                          {
                            Name = thirdGroup.Key,
                            //OpeningBalance = thirdGroup.Sum(x => x.OpeningBalance),
                            //DebitTotal = thirdGroup.Sum(x => x.DebitTotal),
                            //CreditTotal = thirdGroup.Sum(x => x.CreditTotal),
                            //CurrentBalance = thirdGroup.Sum(x => x.CurrentBalance),
                            Children = thirdGroup.GroupBy(r => r.FourthName)
                                  .Select(fourthGroup => new TrialBalanceTreeNode
                                  {
                                    Name = fourthGroup.Key,
                                    //OpeningBalance = fourthGroup.Sum(x => x.OpeningBalance),
                                    //DebitTotal = fourthGroup.Sum(x => x.DebitTotal),
                                    //CreditTotal = fourthGroup.Sum(x => x.CreditTotal),
                                    //CurrentBalance = fourthGroup.Sum(x => x.CurrentBalance),
                                    Children = fourthGroup.Select(fifth => new TrialBalanceTreeNode
                                    {
                                      Name = fifth.FifthName,
                                      OpeningBalance = fifth.OpeningBalance,
                                      DebitTotal = fifth.DebitTotal,
                                      CreditTotal = fifth.CreditTotal,
                                      CurrentBalance = fifth.CurrentBalance
                                    }).ToList()
                                  }).ToList()
                          }).ToList()
                  }).ToList()
          })
          .ToList();

      return View("~/Views/Finance/Report/TrialBalance/TrialBalance.cshtml", groupedTree);
    }

  }
}
