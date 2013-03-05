﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _7Wonders.Host
{
    class Host
    {

        public Host()
        {
            GameManager gameManager = new GameManager();
            EventHandlerServiceImpl eventHandlerService = new EventHandlerServiceImpl();
            MessageSerializerServiceImpl messageSerializerService = new MessageSerializerServiceImpl();
            NetServiceImpl netService = new NetServiceImpl();
            netService.setEventHandler(eventHandlerService);
            eventHandlerService.setGameManager(gameManager);
            gameManager.setNetService(netService);
            gameManager.setMessageSerializer(messageSerializerService);
            messageSerializerService.setNetService(netService);
        }
    }
}
