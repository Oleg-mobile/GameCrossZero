﻿import APP_CONSTS from "../common/appConsts.js";

class GamesService {
    constructor() {
        this.url = APP_CONSTS.SERVER_URL + 'api/Games';
    }

    async start(roomId) {
        await axios
            .post(this.url + '/Start?roomId=' + roomId)
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

                document.location.href = '/';

                if (error.response.code == 500) {
                    document.location.href = '/';
                }
            });
    }

    async doStep(cellsNumber) {
        return await axios
            .post(this.url + '/DoStep?cellsNumber=' + cellsNumber)
            .then(function (response) { return response.data })
            .catch(function (error) {
                console.log(error);
            })
            .finally(function () {
            });
    }
}

export default new GamesService();