import APP_CONSTS from '../common/appConsts.js';
import _usersService from '../Api/usersService.js';

const token = Cookies.get("token");
if (token) {
    axios.defaults.headers.common["Authorization"] = "Bearer " + token;
}

const initUserProfile = async () => {
    const userLogin = document.querySelector('.user-area__login'),
        userAvatar = document.querySelector('.user-area__avatar');

    const userContent = await _usersService.getAvatar();

    if (userContent.avatar != null) {
        userAvatar.src = APP_CONSTS.SERVER_URL + 'avatars/' + userContent.avatar;
    }
    
    userLogin.textContent = userContent.login;

    document.body.style.display = "block";
};

initUserProfile();