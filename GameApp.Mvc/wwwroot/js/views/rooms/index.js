import roomsService from '../../Api/roomsService.js';
import usersService from '../../Api/usersService.js';
import gamesService from '../../Api/gamesService.js';
import APP_CONSTS from '../../common/appConsts.js';

document.addEventListener('DOMContentLoaded', async () => {
	const createRoomModal = new bootstrap.Modal(
		document.getElementById('createModal')
	);
	const roomModal = new bootstrap.Modal(document.getElementById('roomModal'));

	const redirectToRoom = async () => {
		const currentRoom = await roomsService.getCurrentRoom();
		if (!currentRoom) {
			return;
		}

		document.querySelector('#currentRoomName').innerText =
			currentRoom.roomName;

		document.querySelector('#playerNickname').textContent =
			currentRoom.player.login;
		if (currentRoom.player.avatar) {
			document.querySelector(
				'#player img'
			).src = `${APP_CONSTS.SERVER_URL}avatars/${currentRoom.player.avatar}`;
		}

		if (currentRoom.opponent) {
			document.querySelector('#opponentNickname').textContent =
				currentRoom.opponent.login;
			if (currentRoom.opponent.avatar) {
				document.querySelector(
					'#opponent img'
				).src = `${APP_CONSTS.SERVER_URL}avatars/${currentRoom.opponent.avatar}`;
			}
		}

		if (currentRoom.player.isReadyToPlay) {
			document.getElementById("playerready").classList.remove("d-none");
			document.querySelector('.room__readybtn').textContent = "Не готов";
		} else {
			document.getElementById("playerready").classList.add("d-none");
			document.querySelector('.room__readybtn').textContent = "Готов";
		}

		if (currentRoom.opponent != null && currentRoom.opponent.isReadyToPlay) {
			document.getElementById("opponentready").classList.remove("d-none");
		} else {
			document.getElementById("opponentready").classList.add("d-none");
		}

		roomModal.show();
	};

	await redirectToRoom();

	var roomsMap = new Map();

	const initRooms = async () => {
		const roomsContainer = document.querySelector('.rooms__items');
		roomsContainer.textContent = '';

		const rooms = await roomsService.getAll();
		rooms.forEach((room) => {

			roomsMap.set(room.name, room.id);

			roomsContainer.insertAdjacentHTML(
				'beforeend',
				`
				<div class="rooms__item">
					<div class="rooms__name"
					     id="rooms__name">
						<img
							class="rooms__img"
							src="/img/lobby.ico"
							alt="room logo"
						/>
						<span id="roomsname">${room.name}</span>
					</div>
					<div class="rooms__info">
						${
							(room.isProtected || '') &&
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

			document
				.getElementById('rooms__name')
				.addEventListener('click', enterToRoom);

		});
	};

	await initRooms();

	async function enterToRoom() {

			const roomsName = document.getElementById('roomsname').innerHTML;

			if (!roomsMap.has(roomsName)) {
				return;
			}

			const roomId = roomsMap.get(roomsName);

			const dto = {
				roomId: roomId,
				password: null
			}

			await roomsService.enter(dto);
			await redirectToRoom();
		};

	//await enterToRoom();

	document
		.querySelector('.room__playbtn')
		.addEventListener('click', async () => {

			const gameModal = new bootstrap.Modal(document.getElementById('gameModal'));  // ?
			const currentRoom = await roomsService.getCurrentRoom();
			if (!currentRoom) {
				return;
			}

			let roomId = currentRoom.player.currentRoomId;

			await gamesService.start(roomId);
			gameModal.show();
		});


	const roomNameInput = document.querySelector('#roomName');
	const roomPasswordInput = document.querySelector('#roomPassword');

	document
		.querySelector('#createRoom')
		.addEventListener('click', async () => {
			const dto = {
				name: roomNameInput.value,
				password: roomPasswordInput.value
			};

			await roomsService.create(dto);
			createRoomModal.hide();
			await initRooms();
			await redirectToRoom();
		});

	document
		.querySelector('#exitRoomBtn')
		.addEventListener('click', async () => {
			const currentRoom = await roomsService.getCurrentRoom();
			if (!currentRoom) {
				return;
			}

			const dto = {
				roomId: currentRoom.player.currentRoomId,
				userId: currentRoom.player.id,
			};

			await roomsService.exit(dto);
			roomModal.hide();
			await initRooms();
		});

	document
		.querySelector('.room__readybtn')
		.addEventListener('click', async () => {
			let isReady = await usersService.changeReady();

			if (isReady) {
				document.getElementById("playerready").classList.remove("d-none");
				document.querySelector('.room__readybtn').textContent = "Не готов";
			} else {
				document.getElementById("playerready").classList.add("d-none");
				document.querySelector('.room__readybtn').textContent = "Готов";
			}
		});

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
		.then(function () {
			console.log('connection Started...');
		})
		.catch(function (err) {
			return console.error(err);
		});

	connection.on('ChangeReady', function (isReady) {
		console.log('ChangeReady ' + isReady);

		if (isReady) {
			document.getElementById("opponentready").classList.remove("d-none");
		} else {
			document.getElementById("opponentready").classList.add("d-none");
		}
	});
});
