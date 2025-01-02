namespace Exampler_ERP.Models.Temp
{
  public class VoucharIndexViewModel
  {
  }
  public class JournalVoucherIndexViewModel
  {
    public FI_Voucher Vouchers { get; set; }

  }
  public class TransferVoucherIndexViewModel
  {
    public FI_Voucher Vouchers { get; set; }

  }
  public class PaymentVoucherIndexViewModel
  {
    public FI_Voucher Vouchers { get; set; }

  }
  public class ReceivedVoucherIndexViewModel
  {
    public FI_Voucher Vouchers { get; set; }

  }
  public class LodgementReportModel
  {
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? HeadofAccount_ID { get; set; }
    public decimal? OpeningBalance { get; set; }
  }
  public class TrialBalanceTreeNode
  {
    public string? Name { get; set; }
    public double? DebitTotal { get; set; }
    public double? CreditTotal { get; set; }
    public double? OpeningBalance { get; set; }
    public double? CurrentBalance { get; set; }
    public List<TrialBalanceTreeNode>? Children { get; set; } = new List<TrialBalanceTreeNode>();
  }
  public class TrialBalanceReportModel
  {
    public int? Month { get; set; }
    public int? Year { get; set; }
  }
  public class TrialBalanceReportViewModel
  {
    public int? TransactionCount { get; set; }
    public string? AccountName { get; set; }
    public Double? DrAmt { get; set; }
    public Double? CrAmt { get; set; }
  }
  public class IncomeSheetReportModel
  {
    public int? Month { get; set; }
    public int? Year { get; set; }
  }

  public class IncomeSheetReportViewModel
  {
    public int AccountID { get; set; }
    public string AccountName { get; set; }
    public double DrAmt { get; set; } // Debit Amount
    public double CrAmt { get; set; } // Credit Amount
    public int TransactionCount { get; set; }
  }

  public class IncomeSheetViewModel
  {
    public List<IncomeSheetReportViewModel> Revenues { get; set; } = new();
    public List<IncomeSheetReportViewModel> Expenses { get; set; } = new();
    public double TotalRevenues { get; set; }
    public double TotalExpenses { get; set; }
    public double NetIncome => TotalRevenues - TotalExpenses;
  }
}
