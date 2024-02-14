

document.addEventListener('DOMContentLoaded', async () => {

    const gameModal = new bootstrap.Modal(document.getElementById('gameModal'));

    gameModal.show();

    var cross = new Image();
    cross.src = "/img/cross.ico";

    var zero = new Image();
    zero.src = "/img/zero.ico";

    document.querySelector('table').onclick = function (e) {
        let element = e.target;
        if (element.tagName === 'TD') {
            if (element.style.backgroundImage.indexOf(cross.src) >= 0) {
                element.style.backgroundImage = "url(" + zero.src + ")";
            }
            else {
                //element.innerHTML = "<img src='" + cross.src + "' alt='figure'/>";
                element.style.backgroundImage = "url(" + cross.src + ")";
            }
        }
    }
});



