import usersService from '../Api/usersService.js';
import APP_CONSTS from '../common/appConsts.js';

const token = Cookies.get("token");
if (token) {
    axios.defaults.headers.common["Authorization"] = "Bearer " + token;
}

const initUserProfile = async () => {
    const userLogin = document.querySelector('.user-area__login');
    const userAvatar = document.querySelector('.user-area__avatar');

    const userContent = await usersService.getAvatar();

    if (userContent.avatar != null) {
        userAvatar.src = APP_CONSTS.SERVER_URL + 'avatars/' + userContent.avatar;
    }
    
    userLogin.textContent = userContent.login;

    document.body.style.display = "block";
};

initUserProfile();