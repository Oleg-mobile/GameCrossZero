const token = Cookies.get("token");
console.log(Cookies.get());
if (token) {
    axios.defaults.headers.common["Authorization"] = "Bearer " + token;
}