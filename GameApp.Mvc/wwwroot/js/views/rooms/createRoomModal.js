import _roomsService from '../../Api/roomsService.js';
const _modalForCreateRoom = new bootstrap.Modal(document.getElementById('createModal')),
	_roomNameInput = document.querySelector('#roomName'),
	_roomPasswordInput = document.querySelector('#roomPassword');

const _initEventCreateRoomModal = () => {
	document
		.querySelector('#createRoom')
		.addEventListener('click', async () => {
			const dto = {
				name: _roomNameInput.value,
				password: _roomPasswordInput.value
			};

			if ((/\s/g).test(dto.password)) {
				document.querySelector('#roomPasswordMessage').textContent = "Пароль не корректный";
			} else {
				await _roomsService.create(dto);
				_modalForCreateRoom.hide();
				await initRooms();
				await redirectToRoom();
			}
		});
}

export default _initEventCreateRoomModal;