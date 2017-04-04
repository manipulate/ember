namespace Ember {
    using System;
    using System.Diagnostics;
    using System.Reflection;
    class Program {
        public static Boolean debug = false;

        static void Main(string[] args) {
            Console.Title = "Ember Emulator";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("88888888888                      88\n" +
                              "88                               88\n" +
                              "88                               88\n" +
                              "88aaaaa      88,dPYba,,adPYba,   88,dPPYba,    ,adPPYba,  8b,dPPYba,\n" +
                              "88\"\"\"\"\"      88P'   \"88\"    \"8a  88P'    \"8a  a8P_____88  88P'   \"Y8\n" +
                              "88           88      88      88  88       d8  8PP\"\"\"\"\"\"\"  88\n" +
                              "88           88      88      88  88b,   ,a8\"  \"8b,   ,aa  88\n" +
                              "88888888888  88      88      88  8Y\"Ybbd8\"'    `\"Ybbd8\"'  88\n");
            Console.ResetColor();
            Console.WriteLine("Ember Emulator v0.1");
            Console.WriteLine("Build "+FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.Split('.')[2] + "." + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.Split('.')[3]+"\n\n");

            if (args.Length > 0)
            {
                if (args[1] == "--debug") debug = true;
                switch (args[0])
                {
                    case "-login":
                        Login login = new Login();
                        login.Start(6112);
                        break;
                    case "-world":
                        World world = new World();
                        world.Start(9875);
                        break;
                }
            }
            else
            {
                Asterion.Out.Logger.WriteOutput("Server mode not specified. Press ENTER to exit.", Asterion.Out.Logger.LogLevel.Error);
                Console.Read();
            }
        }
    }
}
