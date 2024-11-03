// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



//$(document).ready(function () {
//  // Show success message with slide-in effect
//  if ($('#successMessage').length) {
//    $('#successMessage').css('display', 'block').addClass('show').delay(5000).slideUp('slow', function () {
//      console.log($('#successMessage'), "abc");
//      $(this).remove();
//    });
//  }

//  // Show error message with slide-in effect
//  if ($('#errorMessage').length) {
//    $('#errorMessage').css('display', 'block').addClass('show').delay(5000).slideUp('slow', function () {
//      console.log($('#successMessage'), "abc2");
//      $(this).remove();
//    });
//  }
//});
$(document).ready(function () {
  // Show success message with slide-in effect
  if ($('#successMessage').length) {
    $('#successMessage').css('display', 'block'); // Make it visible first
    setTimeout(function () {
      $('#successMessage').addClass('show'); // Add class to trigger the slide effect
    }, 10);

    $('#successMessage').delay(5000).slideUp('slow', function () {
      $(this).remove();
    });
  }

  // Show error message with slide-in effect
  if ($('#errorMessage').length) {
    $('#errorMessage').css('display', 'block'); // Make it visible first
    setTimeout(function () {
      $('#errorMessage').addClass('show'); // Add class to trigger the slide effect
    }, 10);

    $('#errorMessage').delay(5000).slideUp('slow', function () {
      $(this).remove();
    });
  }
});

function showAlert(message) {
  var alertElement = $('#customAlert');
  $('#customAlertText').text(message); // Set the alert text
  alertElement.css('display', 'block'); // Make it visible
  setTimeout(function () {
    alertElement.addClass('show'); // Add class to slide it in
  }, 10);

  setTimeout(function () {
    alertElement.removeClass('show'); // Remove the show class to hide the alert
    setTimeout(function () {
      alertElement.css('display', 'none'); // Hide it after transition ends
    }, 500);
  }, 3000); // Duration before hiding (3 seconds)
}

function hideAlert() {
  $('#customAlert').removeClass('show').css('display', 'none');
}
