import APP_CONSTS from '../../common/appConsts.js';
import _gamesService from '../../Api/gamesService.js';

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

const initField = async function () {
    var cellsMap = _table.getElementsByTagName('td');

    for (var i = 0; i < gameInfo.steps.length; i++) {
        gameInfo.steps[i].isCross ?
            cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + cross.src + ")" :
            cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + zero.src + ")";
    }
}

initField();

const makeStep = async function () {
    var cellNumber = 0;
    _table.onclick = function (e) {
        let element = e.target;
        if (element.tagName === 'TD') {
            if (element.style.backgroundImage === "") {
                element.style.backgroundImage = _myFigure;
                cellNumber = +element.dataset.cellNumber;
            }
        }
    }
    return cellNumber;
}

if (gameInfo.isMyStep === true) {
    _whoseMove.textContent = "Ваш";

    let cellsNumber = makeStep();  // promise ?
    await _gamesService.doStep(cellsNumber);

} else {
    _whoseMove.textContent = "оппонента";
    _table.style.cursor = 'not-allowed';
}


let connection = new signalR.HubConnectionBuilder()
    .withUrl(APP_CONSTS.SERVER_URL + 'gameHub', {
        withCredentials: false,
        accessTokenFactory: () => {
            return Cookies.get("token");
        },
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection
    .start()
    .then(async function () {
        console.log('connection Started...');
    })
    .catch(function (err) {
        return console.error(err);
    });

connection.on('StepResult', function (stepInfo) {
    console.log('StepResult ' + stepInfo.CellNumber + ' ' + stepInfo.IsGameFinished);

});

