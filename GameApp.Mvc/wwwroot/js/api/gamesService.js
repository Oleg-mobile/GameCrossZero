import APP_CONSTS from "../common/appConsts.js";

class GamesService {
    constructor() {
        this.url = APP_CONSTS.SERVER_URL + 'api/Games';
    }

    async start(roomId) {
        await axios
            .post(this.url + '/Start', roomId)
            .then(function (response) { })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }

    async getInfo(roomId) {
        return await axios.get(this.url + '/GetInfo', {
            params: {
                roomId
            }
        })
            .then(function (response) {
                return response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
    }
}

export default new GamesService();