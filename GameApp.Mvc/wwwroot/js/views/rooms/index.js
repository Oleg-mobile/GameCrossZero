import roomsService from '../../Api/roomsService.js';
import usersService from '../../Api/usersService.js';
import APP_CONSTS from '../../common/appConsts.js';

document.addEventListener('DOMContentLoaded', async () => {

	const modalForCreateRoom = new bootstrap.Modal(document.getElementById('createModal'));
	const roomModal = new bootstrap.Modal(document.getElementById('roomModal'));
	const passwordModal = new bootstrap.Modal(document.getElementById('passwordModal'));
	const playerReadyIcon = document.getElementById("playerready");
	const readyToPlayBtn = document.querySelector('.room__readybtn');
	const opponentReadyIcon = document.getElementById("opponentready");
	const opponentNickname = document.querySelector('#opponentNickname');
	const playbtn = document.querySelector('.room__playbtn');
	let currentRoomId;
	let roomsPassword = null;

	const redirectToRoom = async () => {
		const currentRoom = await roomsService.getCurrentRoom();
		if (!currentRoom)
			return;

		currentRoomId = currentRoom.id;

		document.querySelector('#currentRoomName').innerText = currentRoom.roomName;
		document.querySelector('#playerNickname').textContent = currentRoom.player.login;

		if (currentRoom.player.avatar) {
			document.querySelector('#player img').src =
				`${APP_CONSTS.SERVER_URL}avatars/${currentRoom.player.avatar}`;
		}

		if (currentRoom.opponent) {
			opponentNickname.textContent = currentRoom.opponent.login;
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

		if (currentRoom.player.isReadyToPlay && currentRoom.opponent.isReadyToPlay) {
			playbtn.classList.remove("d-none");
		} else {
			playbtn.classList.add("d-none");
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
				<div class="rooms__item ${room.countPlayersInRoom >= APP_CONSTS.MAX_NUMBER_OF_PLAYERS ? 'disabled' : ''}">
					<div class="rooms__name">
						<img
							data-id="${room.id}"
							data-isProtected="${room.isProtected}"
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
		});

		document
			.querySelectorAll('.rooms__item')
			.forEach((e) => e.addEventListener('click', enterToRoom));

		document
			.querySelector('#passwordBtn')
			.addEventListener('click', setRoomsPassword);
	};

	const enterToRoom = async (e) => {
		const roomsId = e.target.dataset.id;
		const isProtected = e.target.dataset.isprotected;

		if (isProtected) {
			//passwordModal.show();
		}

		await roomsService.enter(roomsId, roomsPassword);
		await redirectToRoom();
	};

	const setRoomsPassword = async () => {   // Не работает
		const secretRoomPasswordInput = document.querySelector('#secretRoomPasswordInput');
		roomsPassword = secretRoomPasswordInput.value;
		passwordModal.hide();
	}

	await initRooms();

	playbtn.addEventListener('click', async () => {
			location.href = "games/" + currentRoomId;
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
			await roomsService.exit();
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

		if (isReady) {     // Не срабатывает?
			opponentReadyIcon.classList.remove("d-none");

			if (currentRoom.player.isReadyToPlay)
				playbtn.classList.remove("d-none");

		} else {
			opponentReadyIcon.classList.add("d-none");
			playbtn.classList.add("d-none");
		}
	});

	connection.on('PlayerEntered', function (playerData) {
		console.log('PlayerEntered ' + playerData.login + ' ' + playerData.avatar);

		if (currentRoom.opponent) {
			opponentNickname.textContent = playerData.login;
			if (currentRoom.opponent.avatar) {
				document.querySelector('#opponent img').src =
					`${APP_CONSTS.SERVER_URL}avatars/${playerData.avatar}`;

			}
		}
	});

	connection.on('PlayerIsOut', function (playerData) {
		console.log('PlayerIsOut ' + playerData.login + ' ' + playerData.avatar);

		if (currentRoom.opponent) {
			opponentNickname.textContent = playerData.login;
			document.querySelector('#opponent img').src = playerData.avatar;
		}
	});
});