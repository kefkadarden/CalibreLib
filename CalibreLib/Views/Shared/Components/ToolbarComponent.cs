using CalibreLib.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace CalibreLib.Views.Shared.Components
{
    //public enum EPageToolbarType
    //{
    //    BookCardGrid = 0,
    //    Categories = 1,
    //    Series = 2,
    //    Authors = 3,
    //    Publishers = 4,
    //    Languages = 5,
    //    Ratings = 6,
    //    FileFormats = 7,
    //    Archived = 8,
    //    Shelf = 9
    //}
    public class ToolbarComponent : ViewComponent
    {
        public IViewComponentResult Invoke(EFilterType type)
        {
            string content = string.Empty;
            switch (type)
            {
                case EFilterType.BookCardGrid:
                    content = BookCardGridToolbar;
                    break;
                case EFilterType.Categories:
                case EFilterType.Series:
                case EFilterType.Languages:
                case EFilterType.Publishers:
                    content = AlphaNumericToolbar;
                    break;
                case EFilterType.Authors:
                    content = AuthorsToolbar;
                    break;
                //case EFilterType.Ratings:
                //case EFilterType.FileFormats:
                  //  content = AlphaNumericSortToolbar;
                    //break;
                case EFilterType.Archived:
                    content = ArchivedBooksToolbar;
                    break;
                case EFilterType.Shelf:
                    content = MultiSortToolbar;
                    break;
            }
                
            return new HtmlContentViewComponentResult(new HtmlString(content));
        }

        private struct EAlphaNumericSortType
        {
            public string name;
            public string funcName;
        }

        private EAlphaNumericSortType sortByList = new EAlphaNumericSortType() { funcName = "updateListSortBy", name = "btnradioSort" };
        private EAlphaNumericSortType sortByBook = new EAlphaNumericSortType() { funcName = "updateSortBy", name = "btnradio" };

        private string AlphaNumericSortToolbar(EAlphaNumericSortType sortType)
        {
                return $"""
                        <input type="radio" class="btn-check" name="{sortType.name}" id="title" autocomplete="off" onclick="{sortType.funcName}('title');">
                          <label class="btn btn-outline-primary" for="title"><i class="fa-solid fa-arrow-up-a-z"></i></label>

                          <input type="radio" class="btn-check" name="{sortType.name}" id="titledesc" autocomplete="off" onclick="{sortType.funcName}('titledesc');">
                          <label class="btn btn-outline-primary" for="titledesc"><i class="fa-solid fa-arrow-down-z-a"></i></label>
                    """;
        }

        private string AlphaNumericToolbar
        {
            get
            {
                string numbers = string.Empty;
                string letters = string.Empty;
                string All = $"""
                                <input type="radio" class="btn-check" name="btnradio" id="allSort" checked autocomplete="off" onclick="updateFilterBy('All');">
                                <label class="btn btn-outline-primary" for="allSort">All</label>
                              """;
                for(int i = 0; i <= 9; i++)
                {
                    numbers += $"""
                                    <input type="radio" class="btn-check" name="btnradio" id="numberSort{i}" autocomplete="off" onclick="updateFilterBy('{i}');">
                                    <label class="btn btn-outline-primary" for="numberSort{i}">{i}</label>
                                """;
                }
                
                for (char c = 'A'; c <= 'Z'; c++) 
                {
                    letters += $"""
                                    <input type="radio" class="btn-check" name="btnradio" id="letterSort{c}" autocomplete="off" onclick="updateFilterBy('{c}');">
                                    <label class="btn btn-outline-primary" for="letterSort{c}">{c}</label>
                                """;
                }

                return $"""
                        {AlphaNumericSortToolbar(sortByList)}
                        {All}
                        {numbers}
                        {letters}
                        """;
            }
        }

        private string AuthorsToolbar
        {
            get
            {
                string flipSort = $"""
                                        <input type="radio" class="btn-check" name="btnradioReverse" id="flipSort" autocomplete="off" onclick="reverseAuthor();">
                                        <label class="btn btn-outline-primary" for="flipSort">B,A <-> A,B</label>
                                    """;
                return flipSort + AlphaNumericToolbar;
            }
        }

        private string MultiSortToolbar
        {
            get
            {
                return $$"""
                        <input type="radio" class="btn-check" name="btnradio" id="datedesc" autocomplete="off" checked onclick="updateSortBy('datedesc');">
                          <label class="btn btn-outline-primary" for="datedesc"><i class="fa-regular fa-calendar-days"></i><i class="fa-solid fa-down-long"></i></label>

                          <input type="radio" class="btn-check" name="btnradio" id="date" autocomplete="off" onclick="updateSortBy('date');">
                          <label class="btn btn-outline-primary" for="date"><i class="fa-regular fa-calendar-days"></i><i class="fa-solid fa-up-long"></i></label>

                          {{AlphaNumericSortToolbar(sortByBook)}}

                          <input type="radio" class="btn-check" name="btnradio" id="author" autocomplete="off" onclick="updateSortBy('author');">
                          <label class="btn btn-outline-primary" for="author"><i class="fa-solid fa-user-pen"></i><i class="fa-solid fa-down-long"></i></label>

                          <input type="radio" class="btn-check" name="btnradio" id="authordesc" autocomplete="off" onclick="updateSortBy('authordesc');">
                          <label class="btn btn-outline-primary" for="authordesc"><i class="fa-solid fa-user-pen"></i><i class="fa-solid fa-up-long"></i></label>
                        """;
            }
        }

        private string BookCardGridToolbar
        {
            get
            {
                //{{PagerToolbar}}
                return $$"""
                <div class="btn-group mb-4" role="group">
                  {{MultiSortToolbar}}
                  
                </div>
                """;
            }
        }

        //private string PagerToolbar
        //{
        //    get
        //    {
        //        return $$"""
        //                <label class="btn-group-vertical ms-3 me-1" for="btnPage">Toggle Pages</label>
        //                <input type="checkbox" class="ms-1" name="btnPage" id="btnPage" onclick="$(function () { const btnPage = $('#btnPage')[0]; togglePagingEnabled(btnPage.checked); });" /> <!--Need to implement the pagination here with hide and show. -->
  
        //                <div id="divPageSize">
        //                <label class="btn-group-vertical ms-3 ps-3 me-1 border-start border-dark" for="btnPage">Page Size</label>
        //                <input type="text" autocomplete="false" inputmode="numeric" id="lblPageSize" class="fw-bold p-1 text-center" style="width:50px;" value="10" />
        //                </div>
        //                """;
        //    }
        //}

        private string ArchivedBooksToolbar
        {
            get
            {
                return MultiSortToolbar;
            }
        }
    }
}
