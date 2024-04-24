import _gamesService from '../../Api/gamesService.js';

const _gameModalNode = document.getElementById('gameModal');
const _whoseMove = document.querySelector('#whoseMove');
const _figure = document.querySelector('#figure');
const _roomId = _gameModalNode.dataset.roomId;
const _gameModal = new bootstrap.Modal(_gameModalNode);
let _isMyFigureCross;

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

const gameInfo = await _gamesService.getInfo(_roomId);

document.querySelector('#gameId').textContent = gameInfo.gameId;

_isMyFigureCross = gameInfo.isMyFigureCross;
gameInfo.isMyStep ? _whoseMove.textContent = "Ваш" : _whoseMove.textContent = "оппонента";
_isMyFigureCross ? _figure.textContent = "крестик" : _figure.textContent = "нолик";

for (let step of gameInfo.steps) {
    console.log(step.isCross + " - " + step.cellNumber);
}

