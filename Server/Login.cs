namespace Ember {

    using Asterion;
    using Asterion.Out;

    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    public class Login : Base {

        public Login() {
            Logger.WriteOutput("Login initialized", Logger.LogLevel.Info);

            this.onReceive = HandleReceive;
            this.onConnect = HandleConnect;
            this.onDisconnect = HandleDisconnect;
        }

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

            string username = "";
            string password = "";

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "nick")
                        {
                            xmlReader.MoveToContent();
                            username = xmlReader.ReadString();
                        }
                        if (xmlReader.Name == "pword")
                        {
                            xmlReader.MoveToContent();
                            password = xmlReader.ReadString();
                        }
                        break;
                }
            }

            if (!db.VerifyUser(username))
            {
                penguin.WriteData("%xt%e%-1%101%");
                return;
            }

            string EncryptedPass = Hashing.HashPassword(db.Get(db.GetPlayerID(username), "password"), penguin.RandomKey);
            string FriendsKey = Hashing.GenerateMD5(Utils.ReverseString(username));

            db.Update(db.GetPlayerID(username), "loginKey", EncryptedPass);
            db.Update(db.GetPlayerID(username), "confirmationHash", Hashing.GenerateMD5(penguin.RandomKey));

            if (password != EncryptedPass)
            {
                penguin.WriteData("%xt%e%-1%101%");
            }
            else
            {
                penguin.WriteData("%xt%l%-1%" + db.GetPlayerID(username) + "|" + db.Get(db.GetPlayerID(username), "swid") + "|" + username + "|" + EncryptedPass + "|1|45|2|false|true|" + Utils.time() + "%" + db.Get(db.GetPlayerID(username), "confirmationHash") + "%" + FriendsKey + "%101,1%" + db.Get(db.GetPlayerID(username), "email") + "%");
            }            
        }
    }

}
