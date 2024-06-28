// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function uploadFile() {
    let input = document.createElement('input');
    input.type = 'file';
    input.accept = ".rtf,.kepub,.prc,.odt,.cbr,.mp4,.mp3,.cbt,.cbz,.txt,.djvu,.lit,.opus,.html,.m4b,.wav,.m4a,.fb2,.docx,.azw3,.flac,.ogg,.epub,.mobi,.doc,.pdf,.azw";
    input.multiple;
    input.onchange = _ => {
        //submit filestream to upload controller to import as a book. Use Task management to schedule upload task. If only single file then redirect to the new book created once task is finished.
        let files = Array.from(input.files);
        console.log(files);
    };
    input.click();

}

function loadShelf(id) {
    $.ajax({  
        type: 'GET',  
        url: 'Shelf/Index',  
        data: "id=" + id,  
        success: function (data, textstatus) { 
            if (data.replace(/(\r\n|\n|\r)/gm, "") != '') {  
                if (isPaging)
                    $("#cardGridRow").empty();

                $("#cardGridRow").append(data);  
                updatePageLabel();
            }  
            else {  
                if (!isPaging) {
                    page = -1;
                    isReachedScrollEnd = true;
                } else {
                    page--;
                    isReachedScrollEnd = true;
                    updatePageLabel();
                }                
            }  
  
            inCallback = false;  
            $("div#loading").hide();  
        },  
        error: function (XMLHttpRequest, textStatus, errorThrown) {  
            alert(errorThrown);  
        }  
    });
}