namespace Ember
{
    using Asterion;
    using Asterion.Out;

    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    public class World : Base
    {
        Database db = new Database("localhost");
        public World()
        {
            Logger.WriteOutput("World initialized", Logger.LogLevel.Info);

            this.onReceive = HandleReceive;
            this.onConnect = HandleConnect;
            this.onDisconnect = HandleDisconnect;
        }

        protected override void HandleGetAgentStatus(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%epfga%-1%1%");
        }

        protected override void HandleGetComMessages(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%epfgm%-1%0%da fuq|"+Utils.time()+"|10%");
        }

        protected override void HandleGetPuffleDigCooldown(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%getdigcooldown%-1%120%");
            
        }

        protected override void HandleGetPuffleCareInventory(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%pgpi%"+penguin.intRoomId+"%$careInventory%");
            
        }

        protected override void HandleJoinRoom(Penguin penguin, string data)
        {
            Logger.WriteOutput("jr called", Logger.LogLevel.Error);
        }

        protected override void HandleJoinWorld(Penguin penguin, string data)
        {
            string[] packet = data.Split('%');
            if (Convert.ToInt32(packet[5]) != penguin.ID)
            {
                penguin.WriteData("%xt%e%-1%101%");
                return;
            }
            if (packet[6] != db.Get(penguin.ID, "loginKey"))
            {
                penguin.WriteData("%xt%e%-1%101%");
                db.Update(penguin.ID, "loginKey", "");
                return;
            }

            db.Update(penguin.ID, "loginKey", "");
            penguin.WriteData("%xt%activefeatures%-1%");
            penguin.WriteData("%xt%js%-1%1%0%1%1%");
            penguin.WriteData("%xt%gps%-1%" + penguin.ID + "%0%");

            penguin.LoadPlayer();

            penguin.WriteData("%xt%pgu%-1%0%"); //puffles %xt%pgu%-1%$puffles%
            penguin.WriteData("%xt%currencies%-1%1|0%"); //puffle quest shit

            string playerString = penguin.GetPlayerString();
            string loadPlayer = playerString + "|%" + penguin.coins + "%0%1440%" + Utils.time() + "%" + penguin.age + "%0%7521%%7%1%0%211843";
            
            penguin.WriteData("%xt%lp%-1%" + loadPlayer + "%");

            //join room
            string roomString = playerString + "%-22|Ember [BOT]|45|6|0|0|0|0|0|0|0|0|55|44|1|1|146|0|0%";
            penguin.WriteData("%xt%jr%1%100%"+roomString+"%");
            penguin.WriteData("%xt%ap%1%"+playerString+"%");
        }

        protected override void HandleHeartbeat(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%h%" + penguin.intRoomId + "%");
        }

        protected override void HandleGetInventory(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%gi%-1%" + String.Join("%", penguin.inventory) + "%");
        }

        protected override void HandleMailEngine(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%mst%-1%0%0%");
        }
        protected override void HandleGetMail(Penguin penguin, string data)
        {
            
        }
        protected override void HandleGetLastRev(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%glr%-1%10915%");
        }
        protected override void HandleGetFurniture(Penguin penguin, string data)
        {
            
        }
        protected override void HandleABTestData(Penguin penguin, string data)
        {
            Logger.WriteOutput("HandleABTestData() called");
        }
        protected override void HandleGetPlayerAwards(Penguin penguin, string data)
        {
            string[] packet = data.Split('%');
            penguin.WriteData("%xt%qpa%-1%" + packet[2] + "%%");
        }
        protected override void HandleGetPlayerPins(Penguin penguin, string data)
        {
            penguin.WriteData("%xt%qpp%-1%566|" + Utils.time() + "|0%%");
        }


        // badddddd, we need some spring cleaning
        protected override void HandleVersionCheck(Penguin penguin, XmlReader xmlReader)
        {
            Logger.WriteOutput("Received version check", Logger.LogLevel.Info);

            XDocument xApiStatus = new XDocument(
                new XElement("msg",
                    new XAttribute("t", "sys"),
                    new XElement("body", String.Empty,
                        new XAttribute("action", "apiOK"),
                        new XAttribute("r", 0))));

            string apiStatus = xApiStatus.ToString().Replace("  ", String.Empty).Replace(Environment.NewLine, String.Empty);
            penguin.WriteData(apiStatus);
        }
        protected override void HandleRandomKey(Penguin penguin, XmlReader xmlReader)
        {
            Logger.WriteOutput("Received rndK request", Logger.LogLevel.Info);

            penguin.RandomKey = Hashing.GenerateRandomKey();

            XDocument xRandomKey = new XDocument(
                new XElement("msg",
                    new XAttribute("t", "sys"),
                    new XElement("body", String.Empty,
                        new XAttribute("action", "rndK"),
                        new XAttribute("r", -1),
                            new XElement("k", penguin.RandomKey))));

            string randomKey = xRandomKey.ToString().Replace("  ", String.Empty).Replace(Environment.NewLine, String.Empty);
            penguin.WriteData(randomKey);
        }
        protected override void HandleLogin(Penguin penguin, XmlReader xmlReader)
        {
            Logger.WriteOutput("Received login request", Logger.LogLevel.Info);

            Database db = new Database("localhost");

            string player_string = "";
            string hashes = "";

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "nick")
                        {
                            xmlReader.MoveToContent();
                            player_string = xmlReader.ReadString();
                        }
                        if (xmlReader.Name == "pword")
                        {
                            xmlReader.MoveToContent();
                            hashes = xmlReader.ReadString();
                        }
                        break;
                }
            }

            string[] player_data = player_string.Split('|');
            int userid = Convert.ToInt32(player_data[0]);
            if (db.Get(userid, "swid") != player_data[1]) { penguin.WriteData("%xt%e%-1%101%"); return; }
            if (!db.VerifyUser(player_data[2])) { penguin.WriteData("%xt%e%-1%101%"); return; }
            if (!db.VerifyUserById(userid)) { penguin.WriteData("%xt%e%-1%101%"); return; }

            string[] hashes_arr = hashes.Split('#');
            if (hashes_arr[1] != db.Get(userid, "confirmationHash"))
            {
                penguin.WriteData("%xt%e%-1%101%");
                return;
            }
            else
            {
                penguin.ID = userid;
                penguin.Username = player_data[2];
                penguin.SWID = db.Get(penguin.ID, "swid");
                db.Update(penguin.ID, "confirmationHash", "");
                Logger.WriteOutput("Login successful");
                penguin.WriteData("%xt%l%-1%");
            }
        }
    }
}
