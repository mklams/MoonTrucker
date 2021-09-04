using System;

namespace MoonTrucker
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MoonTruckerGame())
                game.Run();
        }
    }
}
