const slidesRef = document.querySelectorAll('.slide');
let currentIndex = 0;

let chek = false

function changeSlide() {
    if(!chek){
        return;
    }
  slidesRef[currentIndex].classList.remove('active');
  currentIndex = (currentIndex + 1) % slidesRef.length;
  slidesRef[currentIndex].classList.add('active');
}

setInterval(changeSlide, 3500);



function init(){
    if(slidesRef){
        chek=true
        return;
    }
    console.error("bad references")
}
init()