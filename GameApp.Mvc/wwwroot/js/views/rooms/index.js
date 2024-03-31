import APP_CONSTS from '../../common/appConsts.js';
import _roomsService from '../../Api/roomsService.js';
import { _initRoomModalEvents, _initRoomModalContent } from './roomModal.js';
import _initEventCreateRoomModal from './createRoomModal.js';
import _initEventEnterToRoomModal from './enterToRoomModal.js';

const _roomModal = new bootstrap.Modal(document.getElementById('roomModal')),
	_passwordModal = new bootstrap.Modal(document.getElementById('passwordModal'));
let _roomsPassword = null;
let _currentRoom;

const initRooms = async () => {
	const roomContainer = document.querySelector('.rooms__items');
	const roomsList = await _roomsService.getAll();
	roomContainer.textContent = '';

	roomsList.forEach((room) => {
		roomContainer.insertAdjacentHTML(
			'beforeend',
			`
				<div class="rooms__item ${room.countPlayersInRoom >= APP_CONSTS.MAX_NUMBER_OF_PLAYERS ? 'disabled' : ''}"
						data-id="${room.id}" data-is-protected="${room.isProtected}">
					<div class="rooms__name">
						<img
							class="rooms__img"
							src="/img/lobby.ico"
							alt="room logo"
						/>
						<span>${room.name}</span>
					</div>
					<div class="rooms__info">
						${(room.isProtected || '') &&
			`
						<img
							class= "rooms__protection"
							src = "/img/locked-padlock.ico"
							alt = "lock"
						/> `
			}
						${room.countPlayersInRoom} / ${APP_CONSTS.MAX_NUMBER_OF_PLAYERS}
					</div>
				</div>`
		);
	});

	document
		.querySelectorAll('.rooms__item')
		.forEach((e) => e.addEventListener('click', onClickRoom));

	document
		.querySelector('#passwordBtn')
		.addEventListener('click', _initEventEnterToRoomModal);
};

// SirnalR
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
		await redirectToRoom();
	})
	.catch(function (err) {
		return console.error(err);
	});

const redirectToRoom = async () => {
	_currentRoom = await _roomsService.getCurrentRoom();
	if (!_currentRoom)
		return;

	_initRoomModalEvents({ currentRoom: _currentRoom, onExit: initRooms, connection });
	_initRoomModalContent(_currentRoom);

	_roomModal.show();
};

const onClickRoom = async function () {
	const roomsId = this.dataset.id,
		isProtected = this.dataset.isProtected;

	if (isProtected === "true") {
		document.querySelector('#passwordModal').dataset.room = roomsId;
		_passwordModal.show();
	} else {
		const tryToEnter = await _roomsService.enter(roomsId, _roomsPassword);
		if (tryToEnter) {
			await redirectToRoom();
		} else {
			// вход не удался
		}
	}
};

await _initEventEnterToRoomModal(_passwordModal, _roomsPassword);

await initRooms();

await _initEventCreateRoomModal();