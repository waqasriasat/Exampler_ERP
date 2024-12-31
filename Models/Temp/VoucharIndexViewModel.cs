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

}
