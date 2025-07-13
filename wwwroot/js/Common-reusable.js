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
