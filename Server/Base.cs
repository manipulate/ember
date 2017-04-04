namespace Ember {
    using System;

    using Asterion;
    using Asterion.Out;

    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Sockets = System.Net.Sockets;

    public abstract class Base : Server {

        protected delegate void XmlHandler(Penguin penguin, XmlReader xmlReader);
        protected delegate void XtHandler(Penguin penguin, string packet);

        protected event XmlHandler VersionCheckHandler;
        protected event XmlHandler RandomKeyHandler;
        protected event XmlHandler LoginHandler;

        protected event XtHandler JoinWorldHandler;
        protected event XtHandler JoinRoomHandler;
        protected event XtHandler HeartbeatHandler;
        protected event XtHandler GetInventoryHandler;
        protected event XtHandler MailEngineHandler;
        protected event XtHandler GetMailHandler;
        protected event XtHandler GetLastRevHandler;
        protected event XtHandler GetFurnitureHandler;
        protected event XtHandler GetABTestHandler;
        protected event XtHandler GetPlayerAwardsHandler;
        protected event XtHandler GetPlayerPinsHandler;
        protected event XtHandler GetAgentStatusHandler;
        protected event XtHandler GetComMessagesHandler;
        protected event XtHandler GetPuffleDigCooldownHandler;
        protected event XtHandler GetPuffleCareInventoryHandler;
        
        private Dictionary<string, XmlHandler> XmlHandlers;
        private Dictionary<string, XtHandler> XtHandlers;

        protected ConcurrentDictionary<Sockets.TcpClient, Penguin> Penguins;
        

        public Base() {
            Penguins = new ConcurrentDictionary<Sockets.TcpClient, Penguin>();

            VersionCheckHandler += HandleVersionCheck;
            RandomKeyHandler += HandleRandomKey;
            LoginHandler += HandleLogin;

            JoinWorldHandler += HandleJoinWorld;
            JoinRoomHandler += HandleJoinRoom;
            HeartbeatHandler += HandleHeartbeat;
            GetInventoryHandler += HandleGetInventory;
            MailEngineHandler += HandleMailEngine;
            GetLastRevHandler += HandleGetLastRev;
            GetFurnitureHandler += HandleGetFurniture;
            GetABTestHandler += HandleABTestData;
            GetMailHandler += HandleGetMail;
            GetPlayerAwardsHandler += HandleGetPlayerAwards;
            GetPlayerPinsHandler += HandleGetPlayerPins;

            GetAgentStatusHandler += HandleGetAgentStatus;
            GetComMessagesHandler += HandleGetComMessages;
            GetPuffleDigCooldownHandler += HandleGetPuffleDigCooldown;
            GetPuffleCareInventoryHandler += HandleGetPuffleCareInventory;

            XmlHandlers = new Dictionary<string, XmlHandler>
            {
                {"verChk", VersionCheckHandler},
                {"rndK", RandomKeyHandler},
                {"login", LoginHandler}
            };

            XtHandlers = new Dictionary<string, XtHandler>
            {
                {"j#js", JoinWorldHandler},
                {"i#gi", GetInventoryHandler},
                {"j#jr", JoinRoomHandler},
                {"l#mst", MailEngineHandler},
                {"l#mg", GetMailHandler},
                {"u#glr", GetLastRevHandler},
                {"u#h", HeartbeatHandler},
                {"g#gii", GetFurnitureHandler},
                {"u#gabcms", GetABTestHandler},
                {"i#qpp", GetPlayerPinsHandler},
                {"i#qpa", GetPlayerAwardsHandler},
                {"f#epfga", GetAgentStatusHandler},
                {"f#epfgm", GetComMessagesHandler},
                {"p#getdigcooldown", GetPuffleDigCooldownHandler},
                {"p#pgpi", GetPuffleCareInventoryHandler},

            };
        }
        
        protected virtual void HandlePolicyRequest(Penguin penguin) { }
        protected virtual void HandleVersionCheck(Penguin penguin, XmlReader xmlReader) { }
        protected virtual void HandleRandomKey(Penguin penguin, XmlReader xmlReader) { }
        protected virtual void HandleLogin(Penguin penguin, XmlReader xmlReader) { }

        protected virtual void HandleJoinWorld(Penguin penguin, string data) { }
        protected virtual void HandleJoinRoom(Penguin penguin, string data) { }
        protected virtual void HandleHeartbeat(Penguin penguin, string data) { }
        protected virtual void HandleGetInventory(Penguin penguin, string data) { }
        protected virtual void HandleMailEngine(Penguin penguin, string data) { }
        protected virtual void HandleGetMail(Penguin penguin, string data) { }
        protected virtual void HandleGetLastRev(Penguin penguin, string data) { }
        protected virtual void HandleGetFurniture(Penguin penguin, string data) { }
        protected virtual void HandleABTestData(Penguin penguin, string data) { }
        protected virtual void HandleGetPlayerAwards(Penguin penguin, string data) { }
        protected virtual void HandleGetPlayerPins(Penguin penguin, string data) { }

        protected virtual void HandleGetAgentStatus(Penguin penguin, string data) { }
        protected virtual void HandleGetComMessages(Penguin penguin, string data) { }

        protected virtual void HandleGetPuffleDigCooldown(Penguin penguin, string data) { }
        protected virtual void HandleGetPuffleCareInventory(Penguin penguin, string data) { }

        protected virtual void HandleWorldData(Sockets.TcpClient client, string data)
        {
            Penguin penguin = Penguins[client];

            string[] action = data.Split('%');
            Logger.WriteOutput("Received world data", Logger.LogLevel.Debug);

            if (XtHandlers.ContainsKey(action[3]))
            {
                XtHandlers[action[3]](penguin, data);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Logger.WriteOutput("Missing handler for " + action[3], Logger.LogLevel.Warn);
                Console.ResetColor();
            }
        }

        protected virtual void HandleLoginData(Sockets.TcpClient client, string data)
        {
            Penguin penguin = Penguins[client];

            if (data == "<policy-file-request/>")
            {
                penguin.WriteData("<cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>");
            }
            else
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(data)))
                {
                    xmlReader.ReadToFollowing("body");

                    string loginAction = xmlReader.GetAttribute("action");

                    if (XmlHandlers.ContainsKey(loginAction))
                    {
                        XmlHandlers[loginAction](penguin, xmlReader);
                    }
                    else
                    {
                        Logger.WriteOutput("Missing handler for " + loginAction, Logger.LogLevel.Warn);
                    }

                    xmlReader.Dispose();
                }

            }
        }

        protected virtual void HandleReceive(Sockets.TcpClient client, string data) {
            Logger.WriteOutput("Received: " + data, Logger.LogLevel.Debug);
            char packetType = data[0];

            if (packetType == '<') HandleLoginData(client, data);
            else HandleWorldData(client, data);            
        }

        protected virtual void HandleConnect(Sockets.TcpClient client) {
            Penguin penguin = new Penguin(client);
            Penguins.TryAdd(client, penguin);

            Logger.WriteOutput("Client connected", Logger.LogLevel.Info);
        }

        protected virtual void HandleDisconnect(Sockets.TcpClient client) {
            Penguin disconnectedPenguin;
            Penguins.TryRemove(client, out disconnectedPenguin);

            Logger.WriteOutput("Client disconnected", Logger.LogLevel.Info);
        }

    }

}
