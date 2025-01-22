
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
    series = null,
    pageCount = 0,
    currentPageType = '',
    filterBy,
    $scroll;

function toggleScroll() {
    if (pagingEnabled) {
        $('#cardGridRow').infiniteScroll('destroy');
    } else {
        $scroll = $('#cardGridRow').infiniteScroll({
            path: function () {
                console.log('ts page before', page);
                console.log('ts pageIndex before', this.loadCount);
                page = this.loadCount;
                console.log('ts page after', page);
                console.log('ts pageIndex after', this.loadCount);
                console.log(ajaxCallUrl + "?" + loadBooksQuery(false));

                return ajaxCallUrl + "?" + loadBooksQuery(false);
            },
            append: '.grid-item',
            history: false,
        });
    }
}
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


    loadBooks(ajaxCallUrl, true);
}  

function updateListSortBy(_sortBy) {
    loadListViewComponent(currentPageType, filterBy, _sortBy);
}

function updateFilterBy(_filterBy) {
    filterBy = _filterBy;
    loadListViewComponent(currentPageType, _filterBy);
}

function reverseAuthor() {
    if (currentPageType === 'AuthorsReversed') {
        currentPageType = 'Authors';
    } else {
        currentPageType = 'AuthorsReversed';
    }

    if (!filterBy)
        filterBy = 'All';

    loadListViewComponent(currentPageType, filterBy);
}

function loadListViewComponent(type, filter, sortBy) {
    $.ajax({
        url: '/CardGrid/LoadListViewComponent',
        type: 'GET',
        data: { type: type, filter: filter, sortBy: sortBy },
        success: function (result) {
            $('#listViewComponent').html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error loading component:', error);
        }
    });
}
//var scrollHandler = function () {  
//    if (isReachedScrollEnd == false && pagingEnabled == false &&  
//        ($(document).scrollTop() <= $(document).height() - $(window).height())) {  
//        loadBooks(ajaxCallUrl);  
//    }  
//}  

function togglePagingEnabled(enable) {
    page = 0;
    isReachedScrollEnd = false;
    pagingEnabled = enable;

    toggleScroll();
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
    //loadBooks(ajaxCallUrl);
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
    if ($('#lblPageCount').length > 0)
        $('#lblPageNum')[0].innerText = page;
}

function setPageCount() {
    if ($('#lblPageCount').length > 0) {
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
}

async function getPageCount() {
        $.ajax({
            type: 'GET',
            url: '/CardGrid/GetPageCount',
            data: loadBooksQuery(false),
            success: function (data) {
                pageCount = data.pageCount;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
}

function loadBooksQuery(pageIncrement = false) {
    
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
        showLoading();
        console.log('start load',page,loadBooksQuery(false));
        $.ajax({  
            type: 'GET',  
            url: ajaxCallUrl,  
            data: loadBooksQuery(false),  
            success: function (data, textstatus) { 
                if (data.replace(/(\r\n|\n|\r)/gm, "") != '') {  
                    if (isPaging)
                        $("#cardGridRow").empty();
                    console.log('append data');
                    $("#cardGridRow").append(data);

                    //var $items = $(data);
                    //$grid.isotope('appended', $items);
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
                hideLoading(); 
            },  
            error: function (XMLHttpRequest, textStatus, errorThrown) {  
                hideLoading();
                alert(errorThrown);  
            }  
        });  
    }      
    setPageCount();
}  

window.onload = () => {

    let lblPageNum = document.getElementById('lblPageNum');

    if (!lblPageNum)
        return;
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