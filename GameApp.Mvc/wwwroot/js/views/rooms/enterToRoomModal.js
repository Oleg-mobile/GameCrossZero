import _roomsService from '../../Api/roomsService.js';

const _initEventEnterToRoomModal = async (passwordModal, roomsPassword) => {
	const secretRoomPasswordInput = document.querySelector('#secretRoomPasswordInput');
	roomsPassword = secretRoomPasswordInput.value;
	if (roomsPassword) {
		await _roomsService.enter(document.querySelector('#passwordModal').dataset.room, roomsPassword);
		passwordModal.hide();
		await redirectToRoom();
	}
}

export default _initEventEnterToRoomModal;