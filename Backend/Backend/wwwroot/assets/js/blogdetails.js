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

$(document).ready(function () {

    $(document).on("click", ".post", function (e) {
        e.preventDefault();
        let message = $(".message").val();
        let id = $(".product-id").attr("value");

        if (message != "") {
            $.ajax({
                url: `/blog/addcomment`,
                type: "Post",
                data: {
                    commentMessage: message,
                    productId: id
                },
                success: function (res) {
                    $(".comments-area").append(res);
                }
            });

        }
    });

    $(document).on("click", ".dlt", function (e) {
        e.preventDefault();
        let id = $(this).attr("comment-id");
        $.ajax({
            url: "/blog/deletecomment",
            type: "Post",
            data: {
                id: id
            },
            success: function () {
                $("button[comment-id='" + id + "']").parent().parent().parent().parent().remove();
            }
        });
    });
})