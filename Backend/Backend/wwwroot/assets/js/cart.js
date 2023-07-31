
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













function incrementValue()
{
    var value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    if(value<10){
        value++;
            document.getElementById('number').value = value;
    }
}
function decrementValue()
{
    var value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    if(value>1){
        value--;
            document.getElementById('number').value = value;
    }

}









function incrementValuebtn()
{
    var value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    if(value<10){
        value++;
            document.getElementById('number').value = value;
    }
}
function decrementValuebtn()
{
    var value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    if(value>1){
        value--;
            document.getElementById('number').value = value;
    }

}













const countryMenu = document.querySelector(".country-menu"),
    countryBtn = countryMenu.querySelector(".country-btn"),
    countries = countryMenu.querySelectorAll(".country"),
    cBtn_text = countryMenu.querySelector(".cBtn-text");

countryBtn.addEventListener("click", ()=>countryMenu.classList.toggle("active"));    

countries.forEach(country => {
    country.addEventListener("click", () => {
        let selectedCountry = country.querySelector(".country-text").innerText;
        cBtn_text.innerText = selectedCountry;
        countryMenu.classList.remove("active")
    })

})      








const stateMenu = document.querySelector(".state-menu"),
    selectBtn = stateMenu.querySelector(".select-btn"),
    options = stateMenu.querySelectorAll(".option"),
    sBtn_text = stateMenu.querySelector(".sBtn-text");

selectBtn.addEventListener("click", ()=>stateMenu.classList.toggle("active"));    

options.forEach(option => {
    option.addEventListener("click", () => {
        let selectedOption = option.querySelector(".option-text").innerText;
        sBtn_text.innerText = selectedOption;
        stateMenu.classList.remove("active")
    })

})     