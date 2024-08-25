function chkRead_Click(e) {    
    const bookid = e.id.replace('chkRead', '');
    console.log(bookid);
    //update database 
    $.ajax({  
        type: 'POST',  
        url: 'CardGrid/UpdateReadStatus',  
        data: "bookid=" + bookid + "&readStatus=" + e.checked,  
        success: function (data) { 
            
        },  
        error: function (XMLHttpRequest, textStatus, errorThrown) {  
            alert(XMLHttpRequest.responseJSON.detail);  
        }  
    });
}

function chkArchived_Click(e) {
    const bookid = e.id.replace('chkArchived', '');
    console.log(bookid);
    //update database 
    $.ajax({  
        type: 'POST',  
        url: '/CardGrid/UpdateArchivedStatus',  
        data: "bookid=" + bookid + "&isArchived=" + e.checked,  
        success: function (data) { 
            
        },  
        error: function (XMLHttpRequest, textStatus, errorThrown) {  
            alert(XMLHttpRequest.responseJSON.detail);  
        }  
    });
}

function sendToReader(bookid, format) {
    $.ajax({
        type: 'POST',
        url: '/CardGrid/SendToReader',
        data: "bookid=" + bookid + "&format=" + format,
        success: function (data) {
            $("#SuccessDiv" + bookid).removeClass("visually-hidden");
            $("#SuccessDiv" + bookid).html(data);
            console.log(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var error = XMLHttpRequest.responseJSON.title;
            $("#ErrorDiv" + bookid).html(error);
            $("#ErrorDiv" + bookid).removeClass("visually-hidden");
        }
    });
}

function addToShelf(shelfId, bookid) {
    $.ajax({
        type: 'POST',
        url: '/Shelf/Add',
        data: "shelfId="+shelfId+"&bookId=" + bookid,
        success: function (data) {
            refreshShelfSelection(bookid);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.detail);
        }
    });
}

function removeFromShelf(shelfId, bookid) {
    $.ajax({
        type: 'POST',
        url: '/Shelf/Remove',
        data: "shelfId="+shelfId+"&bookId=" + bookid,
        success: function (data) {
            refreshShelfSelection(bookid);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.detail);
        }
    });
}

function refreshShelfSelection(bookid) {
    $.ajax({
        type: 'GET',
        url: '/Shelf/RefreshShelfSelection?BookId='+bookid,
        success: function (data) {
            $('#shelfSelection' + bookid).html(data);
        },
    });
}
