using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.HttpClient
{
    public class GameAppHttpClient : GameAppHttpClientV1Base
    {
        public GameAppHttpClient(System.Net.Http.HttpClient httpClient) : base(httpClient)
        {
        }
    }
}
