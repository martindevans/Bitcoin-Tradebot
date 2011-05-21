using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket.Client;
using System.Net;

namespace MtgoxWebsockets.Data
{
    public class ObservableWebsocket
        :Subject<string>
    {
        public readonly WebSocket Websocket;

        public ObservableWebsocket(WebSocket socket)
        {
            this.Websocket = socket;

            Websocket.OnMessage += (a, b) =>
            {
                OnNext(b.Message);
            };
            Websocket.OnClose += (a, b) =>
            {
                OnCompleted();
            };
        }
    }
}
