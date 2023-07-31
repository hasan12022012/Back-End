"use strict";

$(document).ready(function () {

    $('ul.tabs li').click(function () {
        var tab_id = $(this).attr('data-tab');

        $('ul.tabs li').removeClass('current');
        $('.tab-content').removeClass('current');

        $(this).addClass('current');
        $("#" + tab_id).addClass('current');
    })

})




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





const stars = document.querySelectorAll(".stars i");
// Loop through the "stars" NodeList
stars.forEach((star, index1) => {
    // Add an event listener that runs a function when the "click" event is triggered
    star.addEventListener("click", () => {
        // Loop through the "stars" NodeList Again
        stars.forEach((star, index2) => {
            // Add the "active" class to the clicked star and any stars with a lower index
            // and remove the "active" class from any stars with a higher index
            index1 >= index2 ? star.classList.add("active") : star.classList.remove("active");
        });
    });
});










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