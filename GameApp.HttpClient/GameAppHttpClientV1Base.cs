using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.HttpClient
{
    public partial class GameAppHttpClientV1Base
    {
        public GameAppHttpClientV1Base(System.Net.Http.HttpClient httpClient) : this(null, httpClient)
        {
        }
    }
}
