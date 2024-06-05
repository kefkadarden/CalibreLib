
var ajaxCallUrl = 'CardGrid/BookList',  
    page = 0,  
    pagingEnabled = false,
    sortBy = localStorage.getItem('gridSortBy') ?? 'datedesc',
    pageSize = localStorage.getItem('gridPageSize') ?? 10,
    inCallback = false,  
    isReachedScrollEnd = false;  
  
function updateSortBy(_sortBy) {
    localStorage.setItem('gridSortBy', _sortBy);
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
    if (isReachedScrollEnd == false && pagingEnabled == false &&  
        ($(document).scrollTop() <= $(document).height() - $(window).height())) {  
        loadBooks(ajaxCallUrl);  
    }  
}  

function togglePagingEnabled(enable) {
    page = 0;
    isReachedScrollEnd = false;
    pagingEnabled = enable;
    localStorage.setItem('gridPagingEnabled', enable);
    const btnPage = $('#btnPage')[0];
    btnPage.checked = enable;
    $("#cardGridRow").empty();
    loadBooks(ajaxCallUrl);
    if (enable)
        $('#paginationToolbar').removeClass('visually-hidden');
    else
        $('#paginationToolbar').addClass('visually-hidden');
}

function pagingNavigation(direction) {
    
    if (direction === 'Prev') {
        if (page <= 1) {
            page = 1;
            return;
        }
        page-=2;
    }
    let lbl = document.getElementById('lblPageNum');     
    loadBooks(ajaxCallUrl, true);
    lbl.value = page;
}

function updatePageLabel() {
    $('#lblPageNum')[0].innerText = page;
}

function getPageCount() {
    if (pagingEnabled) {
        $.ajax({
            type: 'GET',
            url: 'CardGrid/GetPageCount',
            success: function (data) {
                $('#lblPageCount')[0].innerText = "Pages: " + data.pageCount;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    } else {
        $('#lblPageCount')[0].innerText = 'Pages: N/A';
    }
}

function loadBooks(ajaxCallUrl, isPaging) {  
    if (page > -1 && !inCallback) {  
        inCallback = true;  
        $("div#loading").show();  
        $.ajax({  
            type: 'GET',  
            url: ajaxCallUrl,  
            data: "pageNumber=" + page++ + "&sortBy=" + sortBy + "&pageSize=" + pageSize,  
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

    getPageCount();
}  