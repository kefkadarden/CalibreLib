function deleteShelf(shelfId) {
    $.ajax({
        type: 'delete',
        url: 'Delete/'+shelfId,
        success: function (data) {
            window.location.href = "/";
        }
    })
}

function confirmDeleteModal(message, callback) {
    var confirmIndex = true;

    var newMessage = message.replace(/(?:\r\n|\r|\n)/g, "<br>");
    $('#modal-body').html("" + newMessage + "");
    $('#staticBackdrop').modal('show');

    $('#confirm_no').on("click", function() {
        if(confirmIndex) {
            callback(false);
            $('#staticBackdrop').modal('hide');
            confirmIndex = false;
        }
    });

    $('#confirm_yes').on("click", function() {
        if(confirmIndex) {
            callback(true);
            $('#staticBackdrop').modal('hide');
            confirmIndex = false;
        }
    });
}

function openDeleteModal(shelfid) {
    confirmDeleteModal("Are you sure you want to delete shelf?", function (confirm) {
        if (confirm) {
            deleteShelf(shelfid); 
        }
    });
}