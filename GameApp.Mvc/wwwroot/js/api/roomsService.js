import APP_CONSTS from "../common/appConsts.js";

class RoomsService {
    constructor() {
        this.url = APP_CONSTS.SERVER_URL + 'api/Rooms';
    }

    async getAll() {
        return await axios.get(this.url + '/GetAll')
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    async create(room) {
        await axios
            .post(this.url + '/Create', room)
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async enter(roomId, password) {
        await axios
            .post(this.url + '/Enter', {
                    roomId,
                    password
            })
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async exit() {
        await axios
            .post(this.url + '/Exit')
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async delete() {
        await axios
            .delete(this.url + '/Delete')
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async getCurrentRoom() {
        return await axios.get(this.url + '/GetCurrentRoom')
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
    }
}

export default new RoomsService();