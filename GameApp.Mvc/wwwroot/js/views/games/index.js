﻿import _gamesService from '../../Api/gamesService.js';

const _gameModalNode = document.getElementById('gameModal');
const _whoseMove = document.querySelector('#whoseMove');
const _figure = document.querySelector('#figure');
const _roomId = +_gameModalNode.dataset.roomId;
const _gameModal = new bootstrap.Modal(_gameModalNode);
const _table = document.querySelector('table');
let _isMyFigureCross;
let _myFigure;

_gameModal.show();

let cross = new Image();
let zero = new Image();

cross.src = "/img/cross.ico"; 
zero.src = "/img/zero.ico";

//_table.onclick = function (e) {
//    let element = e.target;
//    if (element.tagName === 'TD') {
//        if (element.style.backgroundImage.indexOf(cross.src) >= 0) {
//            element.style.backgroundImage = "url(" + zero.src + ")";
//        } else {
//            element.style.backgroundImage = "url(" + cross.src + ")";
//        }
//    }
//}

const gameInfo = await _gamesService.getInfo(_roomId);

document.querySelector('#gameId').textContent = gameInfo.gameId;

_isMyFigureCross = gameInfo.isMyFigureCross;

if (_isMyFigureCross === true) {
    _figure.textContent = "крестик";
    _myFigure = "url(" + cross.src + ")";
} else {
    _figure.textContent = "нолик";
    _myFigure = "url(" + zero.src + ")";
}

//var cellsMap = _table.cells;
var cellsMap = _table.getElementsByTagName('td');

for (var i = 0; i < gameInfo.steps.length; i++) {
    gameInfo.steps[i].isCross ?
        cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + cross.src + ")" :
        cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + zero.src + ")";
}

if (gameInfo.isMyStep === true) {
    _whoseMove.textContent = "Ваш";

    //for (var i = 0; i < cellsMap.length; i++) {
    //    cellsMap[i].addEventListener('click', function () {
    //        if (cellsMap[i].style.backgroundImage === "none") {
    //            cellsMap[i].style.backgroundImage = _myFigure;
    //        }
    //    });
    //}

    _table.onclick = function (e) {
        let element = e.target;
        if (element.tagName === 'TD') {
            if (element.style.backgroundImage === "") {
                element.style.backgroundImage = _myFigure;
            }
        }
    }


} else {
    _whoseMove.textContent = "оппонента";
    _table.style.cursor = 'not-allowed';
}

