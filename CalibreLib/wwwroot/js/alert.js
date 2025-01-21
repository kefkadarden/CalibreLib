function showAlert(message, type) {
    let alertTypeClass;
    switch (type) {
        case 'success':
            alertTypeClass = 'alert-success';
            break;
        case 'error':
            alertTypeClass = 'alert-danger';
            break;
        case 'warning':
            alertTypeClass = 'alert-warning';
            break;
        default:
            alertTypeClass = 'alert-primary';
            break;
    }

    const alertHtml = `
        <div id="dynamicAlert" class="alert ${alertTypeClass} alert-dismissible fade show" role="alert">
            <span id="alertMessage" class="flex-grow-1">${message}</span>
            <button type="button" class="btn-close-c" aria-label="Close" onclick="hideAlert()">
            <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    $('#alert-container').html(alertHtml);
}

function hideAlert() {
    $('#dynamicAlert').alert('close');
}
