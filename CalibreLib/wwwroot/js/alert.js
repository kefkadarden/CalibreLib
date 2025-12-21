function showAlert(message, type, alertId) {
  alertId = alertId ?? "";
  let alertContainer = `#alert-container${alertId}`;

  let alertTypeClass;
  switch (type) {
    case "success":
      alertTypeClass = "alert-success";
      break;
    case "error":
      alertTypeClass = "alert-danger";
      break;
    case "warning":
      alertTypeClass = "alert-warning";
      break;
    default:
      alertTypeClass = "alert-primary";
      break;
  }

  const alertHtml = `
        <div id="dynamicAlert${alertId}" class="alert ${alertTypeClass} alert-dismissible fade show" role="alert">
            <span id="alertMessage" class="flex-grow-1">${message}</span>
            <button type="button" class="btn-close-c" aria-label="Close" onclick="hideAlert(${alertId})">
            <i class="fas fa-times"></i>
            </button>
        </div>
    `;

  $(alertContainer).html(alertHtml);
}

function hideAlert(alertId) {
  alertId = alertId ?? "";
  $(`#dynamicAlert${alertId}`).hide();
}
