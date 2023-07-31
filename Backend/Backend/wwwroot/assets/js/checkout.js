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





// Country start///////////////////////////////////////////////////////////////////////////////////////
const optionMenu = document.querySelector(".select-menu"),
    selectBtn = optionMenu.querySelector(".select-btn"),
    options = optionMenu.querySelectorAll(".option"),
    sBtn_text = optionMenu.querySelector(".sBtn-text");
selectBtn.addEventListener("click", ()=>optionMenu.classList.toggle("active"));    
options.forEach(option => {
    option.addEventListener("click", () => {
        let selectedOption = option.querySelector(".option-text").innerText;
        sBtn_text.innerText = selectedOption;
        optionMenu.classList.remove("active")
    })
})      
// Country end///////////////////////////////////////////////////////////////////////////////////////
// district start///////////////////////////////////////////////////////////////////////////////////////
const districtMenu = document.querySelector(".district"),
    districtBtn = districtMenu.querySelector(".district-btn"),
    districts = districtMenu.querySelectorAll(".districtlist"),
    sDistrict_text = districtMenu.querySelector(".sDistrict-text");
districtBtn.addEventListener("click", ()=>districtMenu.classList.toggle("active"));    
districts.forEach(districtlist => {
    districtlist.addEventListener("click", () => {
        let selectedDistrict = districtlist.querySelector(".district-text").innerText;
        sDistrict_text.innerText = selectedDistrict;
        districtMenu.classList.remove("active")
    })
}) 
// district end///////////////////////////////////////////////////////////////////////////////////////








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