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
            alert(errorThrown);  
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
            alert(errorThrown);  
        }  
    });
}

function sendEPub(bookid) {
    $.ajax({
        type: 'POST',
        url: '/CardGrid/SendEPubToReader',
        data: "bookid=" + bookid,
        success: function (data) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
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
            alert(errorThrown);
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
            alert(errorThrown);
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