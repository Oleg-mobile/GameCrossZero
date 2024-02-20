
document.addEventListener('DOMContentLoaded', async () => {

    const gameModal = new bootstrap.Modal(document.getElementById('gameModal'));

    gameModal.show();

    var cross = new Image();
    var zero = new Image();

    cross.src = "/img/cross.ico"; 
    zero.src = "/img/zero.ico";

    document.querySelector('table').onclick = function (e) {
        let element = e.target;
        if (element.tagName === 'TD') {
            if (element.style.backgroundImage.indexOf(cross.src) >= 0) {
                element.style.backgroundImage = "url(" + zero.src + ")";
            } else {
                element.style.backgroundImage = "url(" + cross.src + ")";
            }
        }
    }
});



