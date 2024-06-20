import APP_CONSTS from '../../common/appConsts.js';
import _gamesService from '../../Api/gamesService.js';
import _roomsService from '../../Api/roomsService.js';

const _gameModalNode = document.getElementById('gameModal'),
    _whoseMove = document.querySelector('#whoseMove'),
    _figure = document.querySelector('#figure'),
    _roomId = +_gameModalNode.dataset.roomId,
    _gameModal = new bootstrap.Modal(_gameModalNode),
    _table = document.querySelector('table'),
    _cellsMap = _table.getElementsByTagName('td'),
    _exitbtn = document.querySelector('#exitGame');
let _isMyFigureCross,
    _myFigure,
    _currentRoom;

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
    for (var i = 0; i < gameInfo.steps.length; i++) {
        gameInfo.steps[i].isCross ?
            _cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + cross.src + ")" :
            _cellsMap[gameInfo.steps[i].cellNumber].style.backgroundImage = "url(" + zero.src + ")";
    }
}

initField();

_table.onclick = async function (e) {
    let element = e.target;
    if (element.tagName === 'TD' && gameInfo.isMyStep) {
        if (element.style.backgroundImage === "") {
            element.style.backgroundImage = _myFigure;
            const cellNumber = +element.dataset.cellNumber;

            await _gamesService.doStep(cellNumber);

            _whoseMove.textContent = "оппонента";
            _table.style.cursor = 'not-allowed';

            gameInfo.isMyStep = false;
        }
    }
}

if (gameInfo.isMyStep === true) {
    _whoseMove.textContent = "Ваш";
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
    console.log('StepResult ' + stepInfo.cellNumber + ' ' + stepInfo.isGameFinished);

    if (!stepInfo.isGameFinished) {
        if (_isMyFigureCross === true) {
            _cellsMap[stepInfo.cellNumber].style.backgroundImage = "url(" + zero.src + ")";
        } else {
            _cellsMap[stepInfo.cellNumber].style.backgroundImage = "url(" + cross.src + ")";
        }

        _whoseMove.textContent = "Ваш";
        _table.style.cursor = 'default';
    } else {
        if (stepInfo.cellNumber == null) {
            console.log('You won!');
        } else {
            console.log('Draw!');
        }

        _currentRoom = _roomsService.getCurrentRoom();
        if (_currentRoom.isPlayerRoomManager) {
            _exitbtn.classList.remove("d-none");
        }
    }
});



connection.on('ExitGame', async function (roomId) {
    console.log('ExitGame ' + roomId);

    await _gamesService.exitGame(_currentRoom.id);
    location.href = "rooms/" + currentRoom.id;
});

_exitbtn.addEventListener('click', async () => {
    await _gamesService.exitGame(_currentRoom.id);
    location.href = "rooms/" + currentRoom.id;
});