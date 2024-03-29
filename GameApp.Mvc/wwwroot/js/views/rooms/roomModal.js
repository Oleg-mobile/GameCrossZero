import APP_CONSTS from '../../common/appConsts.js';
import _roomsService from '../../Api/roomsService.js';
import _usersService from '../../Api/usersService.js';

const _playbtn = document.querySelector('.room__playbtn'),
	_roomModal = new bootstrap.Modal(document.getElementById('roomModal')),
	_readyToPlayBtn = document.querySelector('.room__readybtn'),
	_playerReadyIcon = document.getElementById("playerReady"),
	_opponentReadyIcon = document.getElementById("opponentReady"),
	_opponentNickname = document.querySelector('#opponentNickname');

export const initRoomModalEvents = ({ currentRoom, onExit, connection }) => {
	_playbtn.addEventListener('click', async () => {
		location.href = "games/" + currentRoom.id;
	});

	document
		.querySelector('#exitRoomBtn')
		.addEventListener('click', async () => {
			await _roomsService.exit();
			_roomModal.hide();
			await onExit();
		});

	_readyToPlayBtn
		.addEventListener('click', async () => {
			let isReady = await _usersService.changeReady();

			if (isReady) {
				_playerReadyIcon.classList.remove("d-none");
				_readyToPlayBtn.textContent = "Не готов";
			} else {
				_playerReadyIcon.classList.add("d-none");
				_readyToPlayBtn.textContent = "Готов";
			}
		});

	connection.on('ChangeReady', function (isReady) {  // Кнопку Играть тоже выставлять?
		console.log('ChangeReady ' + isReady);

		if (isReady) {
			_opponentReadyIcon.classList.remove("d-none");

			if (currentRoom.player.isReadyToPlay && currentRoom.isPlayerRoomManager) {
				_playbtn.classList.remove("d-none");  // И disabled и d-none?
				_playbtn.classList.remove("disabled");
			}

		} else {
			_opponentReadyIcon.classList.add("d-none");
			_playbtn.classList.add("d-none");
			_playbtn.classList.add("disabled");
		}
	});

	connection.on('PlayerEntered', function (playerData) {
		console.log('PlayerEntered ' + playerData.login + ' ' + playerData.avatar);
		_opponentNickname.textContent = playerData.login;
		if (playerData.avatar) {
			document.querySelector('#opponent img').src =
				`${APP_CONSTS.SERVER_URL}avatars/${playerData.avatar}`;
		} else {
			document.querySelector('#opponent img').src =
				`/img/boy.ico`;
		}
	});

	connection.on('PlayerIsOut', function (playerData) {
		console.log('PlayerIsOut ' + playerData.login + ' ' + playerData.avatar);
		_opponentNickname.textContent = playerData.login;
		if (playerData.avatar)
			document.querySelector('#opponent img').src = playerData.avatar;
	});
}

export const initRoomModalContent = (currentRoom) => {
	document.querySelector('#currentRoomName').innerText = currentRoom.roomName;
	document.querySelector('#playerNickname').textContent = currentRoom.player.login;

	if (currentRoom.player.avatar) {
		document.querySelector('#player img').src =
			`${APP_CONSTS.SERVER_URL}avatars/${currentRoom.player.avatar}`;
	}

	if (currentRoom.player.isReadyToPlay) {
		_playerReadyIcon.classList.remove("d-none");
		_readyToPlayBtn.textContent = "Не готов";
	}

	if (currentRoom.opponent) {
		_opponentNickname.textContent = currentRoom.opponent.login;
		if (currentRoom.opponent.avatar) {
			document.querySelector('#opponent img').src =
				`${APP_CONSTS.SERVER_URL}avatars/${currentRoom.opponent.avatar}`;
		} else {
			document.querySelector('#opponent img').src =
				`/img/boy.ico`;
		}

		if (currentRoom.opponent.isReadyToPlay) {
			_opponentReadyIcon.classList.remove("d-none");

			if (currentRoom.player.isReadyToPlay && currentRoom.isPlayerRoomManager) {
				_playbtn.classList.remove("d-none");  // И disabled и d-none?
				_playbtn.classList.remove("disabled");
			}
		}
	}
}
