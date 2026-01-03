function deleteShelf(shelfId) {
  $.ajax({
    type: "delete",
    url: "Delete/" + shelfId,
    success: function (data) {
      window.location.href = "/";
    },
  });
}

function confirmDeleteModal(message, callback) {
  var confirmIndex = true;

  var newMessage = message.replace(/(?:\r\n|\r|\n)/g, "<br>");
  $("#modal-body").html("" + newMessage + "");
  $("#staticBackdrop").modal("show");

  $("#confirm_no").on("click", function () {
    if (confirmIndex) {
      callback(false);
      $("#staticBackdrop").modal("hide");
      confirmIndex = false;
    }
  });

  $("#confirm_yes").on("click", function () {
    if (confirmIndex) {
      callback(true);
      $("#staticBackdrop").modal("hide");
      confirmIndex = false;
    }
  });
}

function openDeleteModal(shelfid) {
  confirmDeleteModal(
    "Are you sure you want to delete shelf?",
    function (confirm) {
      if (confirm) {
        deleteShelf(shelfid);
      }
    },
  );
}

function loadEditPartial(shelfId) {
  $.get("/Shelf/Edit/" + shelfId, function (data) {
    $("#editModalContent").html(data); // Load the partial into the modal body

    const modalEl = document.querySelector("#myModal");
    const myModal = bootstrap.Modal.getOrCreateInstance(modalEl); // returns existing or creates new
    myModal.show();
    // let modalInstance = new bootstrap.Modal(
    //   document.getElementById("editShelfModal"),
    // );
    // modalInstance.show();
    // $("#editShelfModal").modal("show");
  }).fail(function () {
    alert("Failed to load Edit Shelf form.");
  });
}
