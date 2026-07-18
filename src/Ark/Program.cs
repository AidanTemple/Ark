namespace Ark
{
    public static class Program
    {
        [System.STAThread]
        static void Main()
        {
            using (var game = new Main())
            {
                game.Run();
            }
        }
    }
}
