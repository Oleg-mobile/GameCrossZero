import { RoomsApi } from "./../../api/api/RoomsApi.js"

document.addEventListener('DOMContentLoaded', () => {
    RoomsApi.apiRoomsGetAllGet(x => console.log(x));

})