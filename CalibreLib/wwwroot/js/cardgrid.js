var ajaxCallUrl = "/CardGrid/BookList",
  page = 0,
  sortBy = localStorage.getItem("gridSortBy") ?? "datedesc",
  pageSize = localStorage.getItem("gridPageSize") ?? 30,
  inCallback = false,
  shelf = null,
  category = null,
  author = null,
  language = null,
  publisher = null,
  rating = null,
  series = null,
  archived = null,
  currentPageType = "",
  filterBy,
  $scroll;

function toggleScroll() {
  // $("#cardGridRow").infiniteScroll("destroy");
  $scroll = $("#cardGridRow").infiniteScroll({
    path: function () {
      page = this.loadCount;

      return ajaxCallUrl + "?" + loadBooksQuery(false);
    },
    append: ".grid-item",
    history: false,
  });
}
function updateSortBy(_sortBy) {
  localStorage.setItem("gridSortBy", _sortBy);
  sortBy = _sortBy;
  page = 0;
  //isReachedScrollEnd = false;
  window.scrollTo({
    top: 0,
    left: 0,
    behavior: "smooth",
  });

  $("#cardGridRow")[0].innerHTML = "";
  $scroll.infiniteScroll("destroy");
  $scroll = $scroll.infiniteScroll({
    path: function () {
      page = this.loadCount;

      return ajaxCallUrl + "?" + loadBooksQuery(false);
    },
    append: ".grid-item",
    history: false,
  });
  $scroll.infiniteScroll("loadNextPage");
}

function updateListSortBy(_sortBy) {
  loadListViewComponent(currentPageType, filterBy, _sortBy);
}

function updateFilterBy(_filterBy) {
  filterBy = _filterBy;
  loadListViewComponent(currentPageType, _filterBy);
}

function reverseAuthor() {
  if (currentPageType === "AuthorsReversed") {
    currentPageType = "Authors";
  } else {
    currentPageType = "AuthorsReversed";
  }

  if (!filterBy) filterBy = "All";

  loadListViewComponent(currentPageType, filterBy);
}

function loadListViewComponent(type, filter, sortBy) {
  $.ajax({
    url: "/CardGrid/LoadListViewComponent",
    type: "GET",
    data: { type: type, filter: filter, sortBy: sortBy },
    success: function (result) {
      $("#listViewComponent").html(result);
    },
    error: function (xhr, status, error) {
      console.error("Error loading component:", error);
    },
  });
}

// async function getPageCount() {
//   $.ajax({
//     type: "GET",
//     url: "/CardGrid/GetPageCount",
//     data: loadBooksQuery(false),
//     success: function (data) {
//       pageCount = data.pageCount;
//     },
//     error: function (XMLHttpRequest, textStatus, errorThrown) {
//       alert(errorThrown);
//     },
//   });
// }

function loadBooksQuery() {
  var url;
  if (typeof searchParamJSON !== "undefined" && searchParamJSON) {
    url = `pageNumber=${page}&sortBy=${sortBy}&searchModel=${JSON.stringify(searchParamJSON)}`;
  } else {
    url =
      "pageNumber=" +
      page +
      "&sortBy=" +
      sortBy +
      "&" +
      window.location.search.replace("?", "");

    var type, id;

    if (shelf != null) {
      type = "shelf";
      id = shelf;
    } else if (author != null) {
      type = "authors";
      id = author;
    } else if (publisher != null) {
      type = "publishers";
      id = publisher;
    } else if (rating != null) {
      type = "ratings";
      id = rating;
    } else if (category != null) {
      type = "categories";
      id = category;
    } else if (series != null) {
      type = "series";
      id = series;
    } else if (language != null) {
      type = "languages";
      id = language;
    } else if (archived != null) {
      type = "archived";
      id = archived;
    }
    if (type && id) {
      url += "&type=" + type + "&id=" + id;
    }
  }

  return url;
}

// function loadBooks(ajaxCallUrl, isPaging) {
//   if (page > -1 && !inCallback) {
//     inCallback = true;
//     showLoading();
//     $.ajax({
//       type: "GET",
//       url: ajaxCallUrl,
//       data: loadBooksQuery(false),
//       success: function (data, textstatus) {
//         if (data.replace(/(\r\n|\n|\r)/gm, "") != "") {
//           if (isPaging) $("#cardGridRow").empty();
//           $("#cardGridRow").append(data);
//
//           //var $items = $(data);
//           //$grid.isotope('appended', $items);
//           //updatePageLabel();
//         } else {
//           if (!isPaging) {
//             page = -1;
//             //isReachedScrollEnd = true;
//           } else {
//             page--;
//             //isReachedScrollEnd = true;
//             //updatePageLabel();
//           }
//         }
//
//         inCallback = false;
//         hideLoading();
//       },
//       error: function (XMLHttpRequest, textStatus, errorThrown) {
//         hideLoading();
//         alert(errorThrown);
//       },
//     });
//   }
// }

async function downloadBook(button) {
  console.log(button.getAttribute("data-url"));
  const url = button.getAttribute("data-url");
  const bookId = button.id.replace("download-link", "");
  console.log(bookId);

  try {
    const response = await fetch(url);

    if (response.status === 200) {
      const contentDisposition = response.headers.get("Content-Disposition");
      let fileName = "downloaded-file"; // Fallback name
      if (contentDisposition) {
        const fileNameMatch = contentDisposition.match(/filename=([^"]+;)/);
        if (fileNameMatch && fileNameMatch[0]) {
          fileName = fileNameMatch[1].replace(";", "");
        }
      }

      const blob = await response.blob();
      const downloadLink = document.createElement("a");
      downloadLink.href = window.URL.createObjectURL(blob);
      downloadLink.download = fileName;
      document.body.appendChild(downloadLink);
      downloadLink.click();
      document.body.removeChild(downloadLink);
    } else if (response.status === 404) {
      showAlert("Error: The book file was not found.", "error", bookId);
    } else {
      showAlert("Another error happened", "error", bookId);
    }
  } catch (error) {
    showAlert("An unexpected error occurred." + error, "error", bookId);
  }
}
