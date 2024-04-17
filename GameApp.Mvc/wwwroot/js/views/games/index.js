
const _gameModalNode = document.getElementById('gameModal');
const _roomId = _gameModalNode.dataset.roomId;  // dataset is not defined
const _gameModal = new bootstrap.Modal(_gameModalNode);

_gameModal.show();

let cross = new Image();
let zero = new Image();

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

//const gameInfo = await gamesService.gameInfo(roomId);
//document.querySelector('#whoseMove').textContent = gameInfo.WhoseMoveId;



