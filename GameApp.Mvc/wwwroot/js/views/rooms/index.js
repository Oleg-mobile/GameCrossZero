import roomsService from '../../Api/roomsService.js';
import usersService from '../../Api/usersService.js';
import gamesService from '../../Api/gamesService.js';
import APP_CONSTS from '../../common/appConsts.js';

document.addEventListener('DOMContentLoaded', async () => {

	var modalForCreateRoom = new bootstrap.Modal(document.getElementById('createModal'));
	var roomModal = new bootstrap.Modal(document.getElementById('roomModal'));
	var playerReadyIcon = document.getElementById("playerready");
	var readyToPlayBtn = document.querySelector('.room__readybtn');
	var opponentReadyIcon = document.getElementById("opponentready")
	var roomsMap = new Map();

	const redirectToRoom = async () => {
		const currentRoom = await roomsService.getCurrentRoom();
		if (!currentRoom)
			return;

		document.querySelector('#currentRoomName').innerText = currentRoom.roomName;
		document.querySelector('#playerNickname').textContent = currentRoom.player.login;

		if (currentRoom.player.avatar) {
			document.querySelector('#player img').src =
				`${APP_CONSTS.SERVER_URL}avatars/${currentRoom.player.avatar}`;
		}

		if (currentRoom.opponent) {
			document.querySelector('#opponentNickname').textContent =
				currentRoom.opponent.login;
			if (currentRoom.opponent.avatar) {
				document.querySelector('#opponent img').src =
					`${APP_CONSTS.SERVER_URL}avatars/${currentRoom.opponent.avatar}`;
			}
		}

		if (currentRoom.player.isReadyToPlay) {
			playerReadyIcon.classList.remove("d-none");
			readyToPlayBtn.textContent = "Не готов";
		} else {
			playerReadyIcon.classList.add("d-none");
			readyToPlayBtn.textContent = "Готов";
		}

		if (currentRoom.opponent != null && currentRoom.opponent.isReadyToPlay) {
			opponentReadyIcon.classList.remove("d-none");
		} else {
			opponentReadyIcon.classList.add("d-none");
		}

		roomModal.show();
	};

	await redirectToRoom();

	const initRooms = async () => {
		const roomsList = await roomsService.getAll();
		const roomContainer = document.querySelector('.rooms__items');
		roomContainer.textContent = '';

		roomsList.forEach((room) => {
			roomContainer.insertAdjacentHTML(
				'beforeend',
				`
				<div class="rooms__item">
					<div class="rooms__name">
						<img
							data-id="${room.id}"
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

			roomsMap.set(room.name, room.id);
		});

		addEventForElement();
	};

	const addEventForElement = () => {
		document
			.querySelectorAll('.rooms__img')
			.forEach((e) => e.addEventListener('click', enterToRoom));
	};

	const enterToRoom = async (e) => {

		const roomsName = document.getElementById('roomsname').innerHTML;

		if (!roomsMap.has(roomsName))
			return;

		const roomsId = roomsMap.get(roomsName);
		const roomsId1 = e.target.dataset.id;  // Или так
		const dto = {
			roomId: roomsId,
			password: null  // Заглушка
		}

		await roomsService.enter(dto);
		await redirectToRoom();
	};

	await initRooms();

	document
		.querySelector('.room__playbtn')
		.addEventListener('click', async () => {
			const currentRoom = await roomsService.getCurrentRoom();  // Использовать глобальную?
			if (!currentRoom)
				return;

			const gameModal = new bootstrap.Modal(document.getElementById('gameModal'));  // ?
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
			modalForCreateRoom.hide();
			await initRooms();
			await redirectToRoom();
		});

	document
		.querySelector('#exitRoomBtn')
		.addEventListener('click', async () => {
			const currentRoom = await roomsService.getCurrentRoom();  // Использовать глобальную?
			if (!currentRoom)
				return;

			const dto = {
				roomId: currentRoom.player.currentRoomId,
				userId: currentRoom.player.id,
			};

			await roomsService.exit(dto);
			roomModal.hide();
			await initRooms();
		});

	readyToPlayBtn
		.addEventListener('click', async () => {
			let isReady = await usersService.changeReady();

			if (isReady) {
				playerReadyIcon.classList.remove("d-none");
				readyToPlayBtn.textContent = "Не готов";
			} else {
				playerReadyIcon.classList.add("d-none");
				readyToPlayBtn.textContent = "Готов";
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
			opponentReadyIcon.classList.remove("d-none");
		} else {
			opponentReadyIcon.classList.add("d-none");
		}
	});
});