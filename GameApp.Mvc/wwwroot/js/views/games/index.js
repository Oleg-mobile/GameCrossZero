

document.addEventListener('DOMContentLoaded', async () => {

    const gameModal = new bootstrap.Modal(document.getElementById('gameModal'));

    gameModal.show();
    document.querySelector('table').onclick = function (e) {
        let element = e.target;
        if (element.tagName === 'TD')
            //element.innerText = 'крестик';
            element.innerHTML = "<img src='/img/lobby.ico' alt='figure'/>";
    }
});



