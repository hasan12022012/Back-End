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