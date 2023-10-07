namespace GameApp.HttpClient
{
    public class GameAppHttpClient : GameAppHttpClientV1Base
    {
        public GameAppHttpClient(System.Net.Http.HttpClient httpClient) : base(httpClient)
        {
        }
    }
}
