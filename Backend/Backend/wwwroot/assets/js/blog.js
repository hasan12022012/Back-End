// back-to-top js start///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var btn = $('#back-to-top');

$(window).scroll(function() {
  if ($(window).scrollTop() > 300) {
    btn.addClass('show');
  } else {
    btn.removeClass('show');
  }
});

btn.on('click', function(e) {
  e.preventDefault();
  $('html, body').animate({scrollTop:0}, '300');
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
    $(document).on("keyup", "#search", function () {
        let inputVal = $(this).val().trim();
        $(".news .blog-list").slice(0).remove();
        $.ajax({
            url: "/blog/search",
            type: "Get",
            contentType: "application/x-www-form-urlencoded",
            data: {
                search: inputVal
            },

            success: function (res) {
                $(".news").append(res);
            }
        });
    });

    $('.filter-tag').on('click', function (e) {
        let tagId = $(this).attr('tag-id');

        $(".news .blog-list").slice(0).remove();

        $.ajax({
            type: "get",
            url: "/blog/filtertag",
            data: { id: tagId },
            success: function (res) {
                $('.news').append(res);
            },
        });
    });

    $('.filter-category').on('click', function (e) {
        let categoryId = $(this).attr('category-id');

        $(".news .blog-list").slice(0).remove();

        $.ajax({
            type: "get",
            url: "/blog/filtercategory",
            data: { id: categoryId },
            success: function (res) {
                $('.news').append(res);
            },
        });
    });
});