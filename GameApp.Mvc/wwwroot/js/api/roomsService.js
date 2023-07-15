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

    async enter(room) {
        await axios
            .post(this.url + '/Enter', room)
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async exit(room) {
        await axios
            .post(this.url + '/Exit', room)
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async delete(id) {
        await axios
            .delete(this.url + '/Delete', {
                params: {
                    id,
                },
            })
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