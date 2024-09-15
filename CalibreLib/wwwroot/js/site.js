// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2023 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */

(() => {
  'use strict'

  const getStoredTheme = () => localStorage.getItem('theme')
  const setStoredTheme = theme => localStorage.setItem('theme', theme)

  const getPreferredTheme = () => {
    const storedTheme = getStoredTheme()
    if (storedTheme) {
      return storedTheme
    }

    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
  }

  const setTheme = theme => {
    if (theme === 'auto') {
      document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'))
    } else {
      document.documentElement.setAttribute('data-bs-theme', theme)
    }
  }

  setTheme(getPreferredTheme())

  const showActiveTheme = (theme, focus = false) => {
    const themeSwitcher = document.querySelector('#bd-theme')

    if (!themeSwitcher) {
      return
    }

    const themeSwitcherText = document.querySelector('#bd-theme-text')
    const activeThemeIcon = document.querySelector('.theme-icon-active use')
    const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)
    const svgOfActiveBtn = btnToActive.querySelector('svg use').getAttribute('href')

    document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
      element.classList.remove('active')
      element.setAttribute('aria-pressed', 'false')
    })

    btnToActive.classList.add('active')
    btnToActive.setAttribute('aria-pressed', 'true')
    activeThemeIcon.setAttribute('href', svgOfActiveBtn)
    const themeSwitcherLabel = `${themeSwitcherText.textContent} (${btnToActive.dataset.bsThemeValue})`
    themeSwitcher.setAttribute('aria-label', themeSwitcherLabel)

    if (focus) {
      themeSwitcher.focus()
    }
  }

  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
    const storedTheme = getStoredTheme()
    if (storedTheme !== 'light' && storedTheme !== 'dark') {
      setTheme(getPreferredTheme())
    }
  })

  window.addEventListener('DOMContentLoaded', () => {
    showActiveTheme(getPreferredTheme())

    document.querySelectorAll('[data-bs-theme-value]')
      .forEach(toggle => {
        toggle.addEventListener('click', () => {
          const theme = toggle.getAttribute('data-bs-theme-value')
          setStoredTheme(theme)
          setTheme(theme)
          showActiveTheme(theme, true)
        })
      })
  })
})()

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