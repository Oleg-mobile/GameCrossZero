import roomsService from "../../Api/roomsService.js";

document.addEventListener('DOMContentLoaded', async () => {
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

	const createRoomModal = new bootstrap.Modal(document.getElementById('createModal'))
	const roomModal = new bootstrap.Modal(document.getElementById('roomModal'))
	roomModal.show();

	document.querySelector('#createRoom').addEventListener('click', async () => {
		const dto = {
			name: roomNameInput.value,
			password: roomPasswordInput.value,
			managerId: 1
		}

		await roomsService.create(dto);
		createRoomModal.hide();
		await initRooms();
	});
})