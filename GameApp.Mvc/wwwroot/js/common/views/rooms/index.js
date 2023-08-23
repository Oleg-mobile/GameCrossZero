import roomsService from "../../Api/roomsService.js";
import APP_CONSTS from "../../common/appConsts.js";

document.addEventListener('DOMContentLoaded', async () => {

	const createRoomModal = new bootstrap.Modal(document.getElementById('createModal'))
	const roomModal = new bootstrap.Modal(document.getElementById('roomModal'))

	const redirectToRoom = async () => {
		const currentRoom = await roomsService.getCurrentRoom();
		if (!currentRoom) {
			return;
		}

		document.querySelector('#playerNickname').textContent = currentRoom.player.nickname;
		document.querySelector('#player img').src = `${APP_CONSTS.SERVER_URL}avatars/${currentRoom.player.avatar}`;

		if (currentRoom.opponent)
		{
			document.querySelector('#opponentNickname').textContent = currentRoom.opponent.nickname;
			document.querySelector('#opponent img').src = `${APP_CONSTS.SERVER_URL}avatars/${currentRoom.opponent.avatar}`;
		}

		roomModal.show();
	};

	await redirectToRoom();


	const initRooms = async () => {
		const roomsContainer = document.querySelector('.rooms__items');
		roomsContainer.textContent = '';

		const rooms = await roomsService.getAll();
		rooms.forEach(room => {
			roomsContainer.insertAdjacentHTML('beforeend', `
				<div class="rooms__item">
					<div class="rooms__name">
						<img
							class="rooms__img"
							src="/img/lobby.ico"
							alt="room logo"
						/>
						${room.name}
					</div>
					<div class="rooms__info">
						${(room.isProtected || '') && `
						<img
							class= "rooms__protection"
							src = "/img/locked-padlock.ico"
							alt = "lock"
						/> `}
						${room.countPlayersInRoom} / 2
					</div>
				</div>`);
        });
    }

	await initRooms();

	const roomNameInput = document.querySelector('#roomName');
	const roomPasswordInput = document.querySelector('#roomPassword');

	document.querySelector('#createRoom').addEventListener('click', async () => {
		const dto = {
			name: roomNameInput.value,
			password: roomPasswordInput.value,
			managerId: 2
		}

		await roomsService.create(dto);
		createRoomModal.hide();
		await initRooms();
		await redirectToRoom();
	});
})