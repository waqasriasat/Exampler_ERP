
function setupModalCrud(options) {
  const {
    controllerName,
    listContainerId,
    addLinkClass,
    editLinkClass,
    deleteLinkClass,
    printLinkClass,
    addModalContainerId,
    editModalContainerId,
    listPartialSelector
  } = options;

  const $listContainer = $('#' + listContainerId);
  const $addModalContainer = $('#' + addModalContainerId);
  const $editModalContainer = $('#' + editModalContainerId);

  // Edit
  $listContainer.on('click', '.' + editLinkClass, function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    $.get(`/${controllerName}/Edit`, { id }, function (data) {
      $editModalContainer.html(data);
      $editModalContainer.find('.modal').modal('show');
    });
  });

  $editModalContainer.on('click', '#saveChanges', function () {
    var formData = $editModalContainer.find('form').serialize();
    $.post(`/${controllerName}/Edit`, formData, function (response) {
      if (response.success) {
        $editModalContainer.find('.modal').modal('hide');
        reloadList();
      }
    });
  });

  // Add
  $('.' + addLinkClass).click(function () {
    $.get(`/${controllerName}/Create`, function (data) {
      $addModalContainer.html(data);
      $addModalContainer.find('.modal').modal('show');
    });
  });

  $addModalContainer.on('click', '#saveNew', function () {
    var formData = $addModalContainer.find('form').serialize();
    $.post(`/${controllerName}/Create`, formData, function (response) {
      if (response.success) {
        $addModalContainer.find('.modal').modal('hide');
        reloadList();
      }
    });
  });

  // Close modals
  $addModalContainer.on('click', '#Close', function () {
    $addModalContainer.find('.modal').modal('hide');
  });
  $editModalContainer.on('click', '#Close', function () {
    $editModalContainer.find('.modal').modal('hide');
  });

  // Delete
  $listContainer.on('click', '.' + deleteLinkClass, function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    $.post(`/${controllerName}/Delete`, { id }, function (response) {
      if (response.success) {
        reloadList();
      }
    });
  });

  // Print
  $(document).on('click', '.' + printLinkClass, function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    var url = `/${controllerName}/Print?id=${id}`;
    var printWindow = window.open(url, '_blank');
    printWindow.focus();
  });

  // Reload list
  function reloadList() {
    $.get(`/${controllerName}/Index`, function (partialHtml) {
      var html = $(partialHtml).find(listPartialSelector).html();
      $listContainer.html(html);
    });
  }
}

$(document).ready(function () {
  loadAttendanceChart();
});

function loadAttendanceChart() {
  $.ajax({
    url: '/Dashboards/GetAttendanceChartData',
    type: 'GET',
    success: function (result) {
      let categories = result.map(x => x.MonthName);
      let presentData = result.map(x => x.PresentCount);
      let absentData = result.map(x => (-1 * x.AbsentCount));
      let cardColor, headingColor, axisColor, shadeColor, borderColor;
      cardColor = config.colors.cardColor;
      headingColor = config.colors.headingColor;
      axisColor = config.colors.axisColor;
      borderColor = config.colors.borderColor;
      const options = {
        series: [{
          name: 'Present',
          data: presentData
        },
        {
          name: 'Absent',
          data: absentData
        }],
        chart: {
          height: 300,
          stacked: true,
          type: 'bar',
          toolbar: { show: false }
        },
        plotOptions: {
          bar: {
            horizontal: false,
            columnWidth: '33%',
            borderRadius: 12,
            startingShape: 'rounded',
            endingShape: 'rounded'
          }
        },
        colors: [config.colors.primary, config.colors.info],
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
          width: 6,
          lineCap: 'round',
          colors: [cardColor]
        },
        legend: {
          show: true,
          horizontalAlign: 'left',
          position: 'top',
          markers: {
            height: 8,
            width: 8,
            radius: 12,
            offsetX: -3
          },
          labels: {
            colors: axisColor
          },
          itemMargin: {
            horizontal: 10
          }
        },
        grid: {
          borderColor: borderColor,
          padding: {
            top: 0,
            bottom: -8,
            left: 20,
            right: 20
          }
        },
        xaxis: {
          categories: categories,
          labels: {
            style: {
              fontSize: '13px',
              colors: axisColor
            }
          },
          axisTicks: {
            show: false
          },
          axisBorder: {
            show: false
          }
        },
        yaxis: {
          labels: {
            style: {
              fontSize: '13px',
              colors: axisColor
            }
          }
        },
        responsive: [
          {
            breakpoint: 1700,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '32%'
                }
              }
            }
          },
          {
            breakpoint: 1580,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '35%'
                }
              }
            }
          },
          {
            breakpoint: 1440,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '42%'
                }
              }
            }
          },
          {
            breakpoint: 1300,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '48%'
                }
              }
            }
          },
          {
            breakpoint: 1200,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '40%'
                }
              }
            }
          },
          {
            breakpoint: 1040,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 11,
                  columnWidth: '48%'
                }
              }
            }
          },
          {
            breakpoint: 991,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '30%'
                }
              }
            }
          },
          {
            breakpoint: 840,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '35%'
                }
              }
            }
          },
          {
            breakpoint: 768,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '28%'
                }
              }
            }
          },
          {
            breakpoint: 640,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '32%'
                }
              }
            }
          },
          {
            breakpoint: 576,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '37%'
                }
              }
            }
          },
          {
            breakpoint: 480,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '45%'
                }
              }
            }
          },
          {
            breakpoint: 420,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '52%'
                }
              }
            }
          },
          {
            breakpoint: 380,
            options: {
              plotOptions: {
                bar: {
                  borderRadius: 10,
                  columnWidth: '60%'
                }
              }
            }
          }
        ],
        states: {
          hover: {
            filter: {
              type: 'none'
            }
          },
          active: {
            filter: {
              type: 'none'
            }
          }
        }
      };

      const chart = new ApexCharts(document.querySelector("#totalAttendanceRatio"), options);
      chart.render();
    }
  });
}

function loadGrowthAttendanceChartViaYears(year) {
  $.ajax({
    url: '/Dashboards/GetPresentRatioByYear',
    type: 'GET',
    data: { year: year },
    success: function (result) {

      let presentRatio = parseFloat(result.present);
      let absentRatio = parseFloat(result.absent);
      let vacationRatio = parseFloat(result.vacation);

      // update text labels
      $('#lblitemPresentCount').text(presentRatio + '% Positive Attendance');
      $('#lblitemAbsentCount').text(absentRatio + '%');
      $('#lblitemVacationCount').text(vacationRatio + '%');

      let cardColor, headingColor, axisColor, shadeColor, borderColor;
      cardColor = config.colors.cardColor;
      headingColor = config.colors.headingColor;
      axisColor = config.colors.axisColor;
      borderColor = config.colors.borderColor;

      
      const growthChartEl = document.querySelector('#growthAttendanceChartViaYear');
      if (growthChartEl) {
        const growthChart = new ApexCharts(growthChartEl, {
          series: [presentRatio, 100],     // dynamic ratio
          labels: ['Positive Ratio', 'Top Ratio'],
          chart: {
            height: 240,
            type: 'radialBar'
          },
          colors: ['#696cff', '#E0E0E0'],
          plotOptions: {
            radialBar: {
              show: true,
              background: '#f0f0f0',
              size: 150,
              offsetY: 10,
              startAngle: -150,
              endAngle: 150,
              hollow: {
                size: '55%'
              },
              track: {
                background: cardColor,
                strokeWidth: '100%'
              },
              dataLabels: {
                show: true,
                value: { offsetY: -25 },
                name: {
                  offsetY: 15,
                  color: headingColor,
                  fontSize: '15px',
                  fontWeight: '500',
                  fontFamily: 'Public Sans'
                },
                value: {
                  offsetY: -25,
                  color: headingColor,
                  fontSize: '22px',
                  fontWeight: '500',
                  fontFamily: 'Public Sans'
                }
              }
            }
          },
          colors: [config.colors.primary],
          fill: {
            type: 'gradient',
            gradient: {
              shade: 'dark',
              shadeIntensity: 0.5,
              gradientToColors: [config.colors.primary],
              inverseColors: true,
              opacityFrom: 1,
              opacityTo: 0.6,
              stops: [30, 70, 100]
            }
          },
          stroke: {
            dashArray: 5
          },
          grid: {
            padding: {
              top: -35,
              bottom: -10
            }
          },
          states: {
            hover: {
              filter: {
                type: 'none'
              }
            },
            active: {
              filter: {
                type: 'none'
              }
            }
          }
        });

        growthChart.render();
      }
    }
  });
}

$(document).ready(function () {
  // initial load for current year
  var currentYear = $('#growthReportId').text().trim();
  loadGrowthAttendanceChartViaYears(currentYear);

  // handle click of year from dropdown
  $('.year-item').on('click', function () {
    var yr = $(this).data('year');
    $('#growthReportId').text(yr);
    loadGrowthAttendanceChartViaYears(yr);
  });
});
