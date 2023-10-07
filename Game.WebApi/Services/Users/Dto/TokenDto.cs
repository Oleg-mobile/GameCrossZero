namespace GameApp.WebApi.Services.Users.Dto
{
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
