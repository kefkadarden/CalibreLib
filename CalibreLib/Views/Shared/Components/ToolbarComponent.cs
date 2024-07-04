﻿using CalibreLib.Models;
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
                case EFilterType.Ratings:
                case EFilterType.FileFormats:
                    content = AlphaNumericSortToolbar;
                    break;
                case EFilterType.Archived:
                    content = ArchivedBooksToolbar;
                    break;
                case EFilterType.Shelf:
                    content = MultiSortToolbar;
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
