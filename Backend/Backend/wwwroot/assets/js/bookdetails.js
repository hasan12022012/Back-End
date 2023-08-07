"use strict";

$(document).ready(function () {

    $('ul.tabs li').click(function () {
        var tab_id = $(this).attr('data-tab');

        $('ul.tabs li').removeClass('current');
        $('.tab-content').removeClass('current');

        $(this).addClass('current');
        $("#" + tab_id).addClass('current');
    });

    $(document).on("click", ".fa-trash", function (e) {
        let id = $(this).attr('comment-id');
        let btn = $(this);
        $.ajax({
            url: `/product/deletecomment`,
            type: "Post",
            data: {
                id: id
            },
            content: "application/x-www-from-urlencoded",
            success: function () {
                btn.parent().parent().parent().remove();
            }
        });
    });

    $(document).on("click", "#addToCart", function () {
        let id = $(this).attr('cart-id');
        let basketCount = $("#basketCount")
        let basketCurrentCount = $("#basketCount")
        $.ajax({
            method: "Post",
            url: "/basket/addbasket",
            data: {
                id: id
            },
            content: "application/x-www-from-urlencoded",
            success: function () {
                let scrollBasket = $('#basketCount2');
                let scrollBasketCount = $(scrollBasket).text();
                scrollBasketCount++;
                $(scrollBasket).text(scrollBasketCount);
                basketCurrentCount = scrollBasketCount;
                basketCount.html("")
                basketCount.append(basketCurrentCount)
            }
        });
    });
})




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