
var ajaxCallUrl = '/CardGrid/BookList',
    page = 0,
    pagingEnabled = false,
    sortBy = localStorage.getItem('gridSortBy') ?? 'datedesc',
    pageSize = localStorage.getItem('gridPageSize') ?? 30,
    inCallback = false,
    isReachedScrollEnd = false,
    shelf = null,
    category = null,
    author = null,
    language = null,
    publisher = null,
    rating = null,
    series = null;
  
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
    if (enable) {
        $('#paginationToolbar').removeClass('visually-hidden');
        $('#divPageSize').removeClass('visually-hidden');
        pageSize = localStorage.getItem('gridPageSize') ?? 10;
    }
    else {
        $('#paginationToolbar').addClass('visually-hidden');
        $('#divPageSize').addClass('visually-hidden');
        pageSize = 30;
        
    }
    loadBooks(ajaxCallUrl);
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
            url: '/CardGrid/GetPageCount',
            data: loadBooksQuery(false),
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

function loadBooksQuery(pageIncrement = true) {
    

    var url = 'pageNumber=' + page + '&sortBy=' + sortBy + "&pageSize=" + pageSize + "&" + window.location.search.replace("?", "");

    if (pageIncrement)
        page++;

    if (shelf != null)
        url += "&shelf=" + shelf;

    if (author != null)
        url += "&author=" + author;

    if (publisher != null)
        url += "&publisher=" + publisher;

    if (rating != null)
        url += "&rating=" + rating;

    if (category != null)
        url += "&category=" + category;

    if (series != null)
        url += "&series=" + series;

    if (language != null)
        url += "&language=" + language;

    return url;
}

function loadBooks(ajaxCallUrl, isPaging) {  
    if (page > -1 && !inCallback) {  
        inCallback = true;  
        $("div#loading").show();  
        $.ajax({  
            type: 'GET',  
            url: ajaxCallUrl,  
            data: loadBooksQuery(),  
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

window.onload = () => {
    let lblPageNum = document.getElementById('lblPageNum');

    lblPageNum.addEventListener('keyup', (event) => {
        if (event.key === 'Enter') {
            page = lblPageNum.value - 1; //loadBooks increments so need to set page to prior number so it increments to the number entered.
            loadBooks(ajaxCallUrl, true);
            lblPageNum.value = page;
        }
    });
    lblPageNum.addEventListener('keypress', (event) => {
        if (!Number.parseInt(event.key) && event.key != '0') {
            event.preventDefault();
        }
    });

    let lblPageSize = document.getElementById('lblPageSize');

    lblPageSize.addEventListener('keyup', (event) => {
        if (event.key === 'Enter') {
            localStorage.setItem('gridPageSize', lblPageSize.value);
            pageSize = lblPageSize.value;
            page = lblPageNum.value - 1; //loadBooks increments so need to set page to prior number so it increments to the number entered.
            loadBooks(ajaxCallUrl, true);
            lblPageNum.value = page;
        }
    });
    lblPageSize.addEventListener('keypress', (event) => {
        if (!Number.parseInt(event.key) && event.key != '0') {
            event.preventDefault();
        }
    });
}