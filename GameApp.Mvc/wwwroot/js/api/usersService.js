import APP_CONSTS from "../common/appConsts.js";

class UsersService {
    constructor() {
        this.url = APP_CONSTS.SERVER_URL + 'api/Users';
    }

    async getAvatar() {
        return await axios.get(this.url + '/GetAvatar')
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
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

    async create(user) {
        await axios
            .post(this.url + '/Create', user)
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async changeReady() {
        return await axios
            .post(this.url + '/ChangeReady')
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }
}

export default new UsersService();