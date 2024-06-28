using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace CalibreLib.Views.Shared.Components
{
    public enum EPageToolbarType
    {
        BookCardGrid = 0,
        Categories = 1,
        Series = 2,
        Authors = 3,
        Publishers = 4,
        Languages = 5,
        Ratings = 6,
        FileFormats = 7,
        Archived = 8
    }
    public class ToolbarComponent : ViewComponent
    {
        public IViewComponentResult Invoke(EPageToolbarType type)
        {
            string content = string.Empty;
            switch (type)
            {
                case EPageToolbarType.BookCardGrid:
                    content = BookCardGridToolbar;
                    break;
                case EPageToolbarType.Categories:
                case EPageToolbarType.Series:
                case EPageToolbarType.Languages:
                case EPageToolbarType.Publishers:
                    content = AlphaNumericToolbar;
                    break;
                case EPageToolbarType.Authors:
                    content = AuthorsToolbar;
                    break;
                case EPageToolbarType.Ratings:
                case EPageToolbarType.FileFormats:
                    content = AlphaNumericSortToolbar;
                    break;
                case EPageToolbarType.Archived:
                    content = ArchivedBooksToolbar;
                    break;
            }
                
            return new HtmlContentViewComponentResult(new HtmlString(content));
        }

        private string AlphaNumericSortToolbar
        {
            get
            {
                return $"""
                        <input type="radio" class="btn-check" name="btnradio" id="title" autocomplete="off" onclick="updateSortBy('title');">
                          <label class="btn btn-outline-primary" for="title">Alphabetical</label>

                          <input type="radio" class="btn-check" name="btnradio" id="titledesc" autocomplete="off" onclick="updateSortBy('titledesc');">
                          <label class="btn btn-outline-primary" for="titledesc">Alphabetical reversed</label>
                    """;
            }
        }

        private string AlphaNumericToolbar
        {
            get
            {
                string numbers = string.Empty;
                string letters = string.Empty;
                string All = $"""
                                <input type="radio" class="btn-check" name="btnradio" id="allSort" autocomplete="off" onclick="updateSortBy('allSort');">
                                <label class="btn btn-outline-primary" for="allSort">All</label>
                              """;
                for(int i = 0; i <= 9; i++)
                {
                    numbers += $"""
                                    <input type="radio" class="btn-check" name="btnradio" id="numberSort{i}" autocomplete="off" onclick="updateSortBy('numberSort{i}');">
                                    <label class="btn btn-outline-primary" for="numberSort{i}">{i}</label>
                                """;
                }
                
                for (char c = 'A'; c <= 'Z'; c++) 
                {
                    letters += $"""
                                    <input type="radio" class="btn-check" name="btnradio" id="letterSort{c}" autocomplete="off" onclick="updateSortBy('letterSort{c}');">
                                    <label class="btn btn-outline-primary" for="letterSort{c}">{c}</label>
                                """;
                }

                return $"""
                        {AlphaNumericSortToolbar}
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
                                        <input type="radio" class="btn-check" name="btnradio" id="flipSort" autocomplete="off" onclick="updateSortBy('flipSort');">
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
                          <label class="btn btn-outline-primary" for="datedesc">Date newest</label>

                          <input type="radio" class="btn-check" name="btnradio" id="date" autocomplete="off" onclick="updateSortBy('date');">
                          <label class="btn btn-outline-primary" for="date">Date oldest</label>

                          {{AlphaNumericSortToolbar}}

                          <input type="radio" class="btn-check" name="btnradio" id="author" autocomplete="off" onclick="updateSortBy('author');">
                          <label class="btn btn-outline-primary" for="author">Author</label>

                          <input type="radio" class="btn-check" name="btnradio" id="authordesc" autocomplete="off" onclick="updateSortBy('authordesc');">
                          <label class="btn btn-outline-primary" for="authordesc">Author reversed</label>
                        """;
            }
        }

        private string BookCardGridToolbar
        {
            get
            {
                return $$"""
                <div class="btn-group mb-4" role="group">
                  {{MultiSortToolbar}}
                  {{PagerToolbar}}
                </div>
                """;
            }
        }

        private string PagerToolbar
        {
            get
            {
                return $$"""
                        <label class="btn-group-vertical ms-3 me-1" for="btnPage">Toggle Pages</label>
                        <input type="checkbox" class="ms-1" name="btnPage" id="btnPage" onclick="$(function () { const btnPage = $('#btnPage')[0]; togglePagingEnabled(btnPage.checked); });" /> <!--Need to implement the pagination here with hide and show. -->
  
                        <div id="divPageSize">
                        <label class="btn-group-vertical ms-3 ps-3 me-1 border-start border-dark" for="btnPage">Page Size</label>
                        <input type="text" autocomplete="false" inputmode="numeric" id="lblPageSize" class="fw-bold p-1 text-center" style="width:50px;" value="10" />
                        </div>
                        """;
            }
        }

        private string ArchivedBooksToolbar
        {
            get
            {
                return MultiSortToolbar;
            }
        }
    }
}
