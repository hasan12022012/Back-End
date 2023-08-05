// filterbyrating js start////////////////////////////////////////////////////////////////////////////////////
const optionMenu = document.querySelector(".filterbyrating"),
    selectBtn = optionMenu.querySelector(".select-btn"),
    options = optionMenu.querySelectorAll(".option"),
    sBtn_text = optionMenu.querySelector(".sBtn-text");

selectBtn.addEventListener("click", () => optionMenu.classList.toggle("active"));

options.forEach(option => {
    option.addEventListener("click", () => {
        let selectedOption = option.querySelector(".option-text").innerText;
        sBtn_text.innerText = selectedOption;
        optionMenu.classList.remove("active")
    })

})
// filterbyrating js end////////////////////////////////////////////////////////////////////////////////////


$(document).ready(function () {
    $(document).on("keyup", "#search", function () {
        let inputVal = $(this).val().trim();

        $(".carousel li").slice(0).remove();
        $.ajax({
            url: "product/search",
            type: "Get",
            contentType: "application/x-www-form-urlencoded",
            data: {
                search: inputVal
            },

            success: function (res) {
                $(".carousel").append(res);
            }
        });
    });

    $('.filter-genre').on('click', function (e) {
        let genreId = $(this).attr('genre-id');

        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/filtergenre",
            data: { id: genreId },
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });

    $('.filter-author').on('click', function (e) {
        let authorId = $(this).attr('author-id');

        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/filterauthor",
            data: { id: authorId },
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });

    $('.name').on('click', function () {
        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/sortname",
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });

    $('.old').on('click', function () {
        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/sortold",
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });

    $('.new').on('click', function () {
        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/sortnew",
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });

    $('.low').on('click', function () {
        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/sortlow",
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });


    $('.high').on('click', function () {
        $(".carousel li").slice(0).remove();

        $.ajax({
            type: "get",
            url: "product/sorthigh",
            success: function (res) {
                $('.carousel').append(res);
            },
        });
    });
})



// district start///////////////////////////////////////////////////////////////////////////////////////
const browseMenu = document.querySelector(".browse-by-popularity"),
    browseBtn = browseMenu.querySelector(".browse-by-popularity-btn"),
    browseList = browseMenu.querySelectorAll(".browse_list"),
    sBrowse_text = browseMenu.querySelector(".sBrowse-by-popularity-btn-text");

browseBtn.addEventListener("click", () => browseMenu.classList.toggle("active"));

browseList.forEach(browse_list => {
    browse_list.addEventListener("click", () => {
        let selectedBrowseText = browse_list.querySelector(".browse-text").innerText;
        sBrowse_text.innerText = selectedBrowseText;
        browseMenu.classList.remove("active")
    })

})
// district end///////////////////////////////////////////////////////////////////////////////////////









// back-to-top js start///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var btn = $('#back-to-top');

$(window).scroll(function () {
    if ($(window).scrollTop() > 300) {
        btn.addClass('show');
    } else {
        btn.removeClass('show');
    }
});

btn.on('click', function (e) {
    e.preventDefault();
    $('html, body').animate({ scrollTop: 0 }, '300');
});
// back-to-top js end///////////////////////////////////////////////////////////////////////////////////////////////////////////////////






// filterbyprice start/////////////////////////////////////////////////////////////////////////////////////
const range = document.querySelectorAll(".range-slider span input");
progress = document.querySelector(".range-slider .progress");
let gap = 0.1;
const inputValue = document.querySelectorAll(".numberVal input");

range.forEach((input) => {
    input.addEventListener("input", (e) => {
        let minRange = parseInt(range[0].value);
        let maxRange = parseInt(range[1].value);

        if (maxRange - minRange < gap) {
            if (e.target.className === "range-min") {
                range[0].value = maxRange - gap;
            } else {
                range[1].value = minRange + gap;
            }
        } else {
            progress.style.left = (minRange / range[0].max) * 100 + "%";
            progress.style.right = 100 - (maxRange / range[1].max) * 100 + "%";
            inputValue[0].value = minRange;
            inputValue[1].value = maxRange;
        }
    });
});
// filterbyprice end/////////////////////////////////////////////////////////////////////////////////////











function headerScroll() {
    window.addEventListener("scroll", function () {
        var header = document.querySelector("header");
        var scrollPosition = window.scrollY || document.documentElement.scrollTop;
        if (scrollPosition > 80) {
            header.style.position = "fixed";
            header.style.width = "100%";
        } else {
            header.style.position = "static";
            header.style.width = "auto";
        }
    });
}

headerScroll();