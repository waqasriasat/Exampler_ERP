// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$(document).ready(function () {
  // Show success message with slide-in effect
  if ($('#successMessage').length) {
    $('#successMessage').css('display', 'block').addClass('show').delay(5000).slideUp('slow', function () {
      $(this).remove();
    });
  }

  // Show error message with slide-in effect
  if ($('#errorMessage').length) {
    $('#errorMessage').css('display', 'block').addClass('show').delay(5000).slideUp('slow', function () {
      $(this).remove();
    });
  }
});

