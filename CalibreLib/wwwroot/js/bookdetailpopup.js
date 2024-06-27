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
        url: 'CardGrid/UpdateArchivedStatus',  
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
        url: 'CardGrid/SendEPubToReader',
        data: "bookid=" + bookid,
        success: function (data) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}