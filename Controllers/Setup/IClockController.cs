using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.Setup
{
  [ApiController]
  [Route("iclock")]
  public class IClockController : Controller
  {
    private readonly AppDBContext _appDBContext;

    public IClockController(AppDBContext appDBContext)
    {
      _appDBContext = appDBContext;
    }
    [HttpGet("cdata")]
    public IActionResult Handshake([FromQuery] string SN, [FromQuery] string options)
    {
      LogToFile($"Handshake request: SN={SN}, options={options}");

      if (string.IsNullOrEmpty(SN))
      {
        return Content("Error: Serial number missing", "text/plain");
      }

      var device = _appDBContext.CR_AttDeviceRegistrations
                    .FirstOrDefault(d => d.SerialNumber == SN);

      if (device == null)
      {
        return Content("Error: No registered device found", "text/plain");
      }

      if (device.ActiveYNID != 1)
      {
        return Content("Error: Device is deactivated", "text/plain");
      }

      return Content("OK", "text/plain");
    }

    [HttpPost("cdata")]
    public async Task<IActionResult> Cdata([FromQuery] string SN, [FromQuery] string table, [FromQuery] string? Stamp = null)
    {
      try
      {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        LogToFile($"SN={SN} | Table={table} | Stamp={Stamp} | Body={body}");

        // Always save raw to Temp table
        _appDBContext.CR_AttDeviceTempDevicePosts.Add(new CR_AttDeviceTempDevicePost
        {
          SN = SN,
          TableName = table,
          RawData = body,
          CreatedAt = DateTime.Now
        });
        await _appDBContext.SaveChangesAsync();

        // Handle Options
        if (table.Equals("options", StringComparison.OrdinalIgnoreCase))
        {
          _appDBContext.CR_AttDeviceDeviceOptionLogs.Add(new CR_AttDeviceDeviceOptionLog
          {
            SN = SN,
            RawData = body,
            CreatedAt = DateTime.Now,
          });
          await _appDBContext.SaveChangesAsync();
        }

        // Handle Attendance logs
        else if (table.Equals("ATTLOG", StringComparison.OrdinalIgnoreCase))
        {
          var lines = body.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
          foreach (var line in lines)
          {
            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
              DateTime dt = DateTime.TryParse(parts[1].Trim(), out var t) ? t : DateTime.MinValue;

              //_appDBContext.CR_FaceAttendances.Add(new CR_FaceAttendance
              //{
              //  EmployeeID = int.Parse(parts[0].Trim()),
              //  MarkDate = dt,
              //  Device_SerialNo = SN,
              //  PunchType = int.Parse(parts[3].Trim()),
              //});
              var today = DateTime.Today;

              var existingAttendance = await _appDBContext.CR_FaceAttendances
                  .Where(a => a.EmployeeID == int.Parse(parts[0].Trim()) && a.MarkDate == dt && a.MarkSourceID == 1)
                  .FirstOrDefaultAsync();

              if (existingAttendance != null)
              {
                // Update record for "out" if "in" was already marked
                if (existingAttendance.MarkSourceID == 1)
                {
                  existingAttendance.MarkSourceID = 2;
                  existingAttendance.OutTime = dt;
             
                  // Calculate the duration between InTime and OutTime
                  var duration = existingAttendance.OutTime - existingAttendance.InTime;
                  existingAttendance.DHours = (int)duration.TotalHours;
                  existingAttendance.DMinutes = duration.Minutes;
                  existingAttendance.OutDevice_SerialNo = SN;
                  existingAttendance.OutPunchType = int.Parse(parts[3].Trim());
                  await _appDBContext.SaveChangesAsync();
                }
              }
              var attendance = new CR_FaceAttendance
              {
                EmployeeID = int.Parse(parts[0].Trim()),
                MarkDate = dt,
                MarkSourceID = 1,
                InTime = dt,
                InDevice_SerialNo = SN,
                InPunchType = int.Parse(parts[3].Trim())

              };

              _appDBContext.CR_FaceAttendances.Add(attendance);
              await _appDBContext.SaveChangesAsync();
            }
          }
          //await _appDBContext.SaveChangesAsync();
        }

        // Always reply OK
        return Content("OK", "text/plain");
      }
      catch (Exception ex)
      {
        LogToFile("ERROR in Cdata: " + ex.ToString());
        return Content("OK", "text/plain"); // still OK so device continues
      }
    }
    private static readonly ConcurrentQueue<string> _pendingCommands = new ConcurrentQueue<string>();
    [HttpGet("getrequest")]
    public IActionResult GetRequest([FromQuery] string SN, [FromQuery] string INFO)
    {
      LogToFile($"Polling from SN={SN}, INFO={INFO}");

      string command;

      // 1. If any command is pending, send it
      if (_pendingCommands.TryDequeue(out var pending))
      {
        command = pending;
      }
      else
      {
        // 2. Otherwise always ask for attendance logs
        command = "C: DATA QUERY ATTLOG";
      }

      return Content(command, "text/plain");
    }
    private void LogToFile(string text)
    {
      var baseDir = AppDomain.CurrentDomain.BaseDirectory;
      var path = Path.Combine(baseDir, "DeviceLog.txt");
      System.IO.File.AppendAllText(path, $"{DateTime.Now} | {text}{Environment.NewLine}");
    }
  }
}
