
var ajaxCallUrl = 'CardGrid/BookList',  
    page = 0,  
    sortBy = 'datedesc',
    inCallback = false,  
    isReachedScrollEnd = false;  
  
function updateSortBy(_sortBy) {
    sortBy = _sortBy;
    page = 0;
    isReachedScrollEnd = false;
    window.scrollTo({
        top: 0,
        left: 0,
        behavior: "smooth",
    });
    $("#cardGridRow").empty();
    loadBooks(ajaxCallUrl);
}  
var scrollHandler = function () {  
    if (isReachedScrollEnd == false &&  
        ($(document).scrollTop() <= $(document).height() - $(window).height())) {  
        loadBooks(ajaxCallUrl);  
    }  
}  
function loadBooks(ajaxCallUrl) {   
    if (page > -1 && !inCallback) {  
        inCallback = true;  
        $("div#loading").show();  
        $.ajax({  
            type: 'GET',  
            url: ajaxCallUrl,  
            data: "pageNumber=" + page++ + "&sortBy=" + sortBy,  
            success: function (data, textstatus) { 
                if (data.replace(/(\r\n|\n|\r)/gm, "") != '') {  
                    $("#cardGridRow").append(data);  
                }  
                else {  
                    page = -1;  
                    isReachedScrollEnd = true;
                }  
  
                inCallback = false;  
                $("div#loading").hide();  
            },  
            error: function (XMLHttpRequest, textStatus, errorThrown) {  
                alert(errorThrown);  
            }  
        });  
    }  
}  