import roomsService from '../../Api/roomsService.js';
import APP_CONSTS from '../../common/appConsts.js';
import { initRoomModalEvents, initRoomModalContent } from './roomModal.js';

const modalForCreateRoom = new bootstrap.Modal(document.getElementById('createModal'));
const roomModal = new bootstrap.Modal(document.getElementById('roomModal'));
const passwordModal = new bootstrap.Modal(document.getElementById('passwordModal'));
let roomsPassword = null;
let currentRoom;

const initRooms = async () => {
	const roomsList = await roomsService.getAll();
	const roomContainer = document.querySelector('.rooms__items');
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
		.forEach((e) => e.addEventListener('click', enterToRoom));

	document
		.querySelector('#passwordBtn')
		.addEventListener('click', setRoomsPassword);
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

	const redirectToRoom = async () => {
		currentRoom = await roomsService.getCurrentRoom();
		if (!currentRoom)
			return;

		initRoomModalEvents({ currentRoom, onExit: initRooms, connection });
		initRoomModalContent(currentRoom);

		roomModal.show();
	};

	const enterToRoom = async function () {
		const roomsId = this.dataset.id;
		const isProtected = this.dataset.isProtected;

		if (isProtected === "true") {
			document.querySelector('#passwordModal').dataset.room = roomsId;
			passwordModal.show();
		} else {
			const tryToEnter = await roomsService.enter(roomsId, roomsPassword);
			if (tryToEnter) {
				await redirectToRoom();
			} else {
				// вход не удался
			}
		}
	};

	const setRoomsPassword = async () => {
		const secretRoomPasswordInput = document.querySelector('#secretRoomPasswordInput');
		roomsPassword = secretRoomPasswordInput.value;
		if (roomsPassword) {
			await roomsService.enter(document.querySelector('#passwordModal').dataset.room, roomsPassword);
			passwordModal.hide();
			await redirectToRoom();
		}
	}

	await initRooms();

	const roomNameInput = document.querySelector('#roomName');
	const roomPasswordInput = document.querySelector('#roomPassword');

	document
		.querySelector('#createRoom')
		.addEventListener('click', async () => {
			const dto = {
				name: roomNameInput.value,
				password: roomPasswordInput.value
			};

			if ((/\s/g).test(dto.password)) {
				document.querySelector('#roomPasswordMessage').textContent = "Пароль не корректный";
			} else {
				await roomsService.create(dto);
				modalForCreateRoom.hide();
				await initRooms();
				await redirectToRoom();
			}
		});

	connection
		.start()
		.then(async function () {
			console.log('connection Started...');
			await redirectToRoom();     // Срабатывает по нажатию Готов/Не готов
		})
		.catch(function (err) {
			return console.error(err);
		});