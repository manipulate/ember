namespace Ember {

    using Sockets = System.Net.Sockets;

    public class Penguin : ClientBase {
        private Database db = new Database("localhost");

        public int ID;
        public string RandomKey;
        public string Username;
        public string SWID;
        public string Email;

        public string outfit;
        public string color, head, face, neck, body, hand, feet, flag, photo;
        public int x, y, frame, age;
        public string coins;
        public string[] inventory;
        public string avatar;

        public int room;
        public int intRoomId;
        public int extRoomId;

        public Penguin(Sockets.TcpClient client) {
            this.client = client;
        }

        public void LoadPlayer()
        {
            RandomKey = null;

            outfit = db.Get(ID, "outfit");
            string[] clothes = outfit.Split('%');
            color = clothes[0];
            head = clothes[1];
            face = clothes[2];
            neck = clothes[3];
            body = clothes[4];
            hand = clothes[5];
            feet = clothes[6];
            flag = clothes[7];
            photo = clothes[8];

            avatar = db.Get(ID, "avatar");
            coins = db.Get(ID, "coins");
            inventory = db.Get(ID, "inventory").Split('%');
            age = 999;

            x = 5;
            y = 5;
            frame = 1;
        }

        public string GetPlayerString()
        {
            string[] player = new string[] {
                ID.ToString(),
                Username,
                "45",
                color,
                head,
                face,
                neck,
                body,
                hand,
                feet,
                flag,
                photo,
                x.ToString(),
                y.ToString(),
                frame.ToString(),
                "1",
                "146",
                avatar,
                "0"
            };
            return string.Join("|", player);
        }
    }

}
