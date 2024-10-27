using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_GlobalSetting
  {
    [Key]
    public int HR_GlobalSettingID { get; set; } = 1;
    public int LateAppID { get; set; } = 1;
    public int LateTypeID { get; set; } = 2;
    public int LateGraceMinute { get; set; } = 15;
    public int LateValueofHours { get; set; } = 12; // by %

    public int EarlyGoingAppID { get; set; } = 1;
    public int EarlyGoingTypeID { get; set; } = 2;
    public int EarlyGoingGraceMinute { get; set; } = 15;
    public int EarlyGoingValueofHours { get; set; } = 12; // by %

    public int EarlyComingAppID { get; set; } = 1;
    public int EarlyComingGraceMinute { get; set; } = 45;
    public int EarlyComingValueofHours { get; set; } = 12; // by %

    public int LateSeatingAppID { get; set; } = 1;
    public int LateSeatingGraceMinute { get; set; } = 45;
    public int LateSeatingValueofHours { get; set; } = 12; // by %

    public int AbsentAppID { get; set; } = 1;
    public int AbsentTypeID { get; set; } = 1;

    public int FlexibleDutyHourID { get; set; } = 1;

    public int WorkingDayInWeek { get; set; } = 5;
  }
}
