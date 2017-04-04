using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Ember_Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            Process login = new Process();
            login.StartInfo.FileName = "Ember.exe";
            login.StartInfo.Arguments = "-login --debug";

            Process world = new Process();
            world.StartInfo.FileName = "Ember.exe";
            world.StartInfo.Arguments = "-world --debug";
            Console.ForegroundColor = ConsoleColor.DarkRed;
            try
            {
                login.Start();
            }
            catch
            {
                Console.WriteLine("Unable to find 'Ember.exe' .. \n\nPress ENTER to exit");
                Console.Read();
            }
            try
            {
                world.Start();
            }
            catch
            {
                Console.WriteLine("Unable to find 'Ember.exe' .. \n\nPress ENTER to exit");
                Console.Read();
            }
            
        }
    }
}
