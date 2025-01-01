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
}
