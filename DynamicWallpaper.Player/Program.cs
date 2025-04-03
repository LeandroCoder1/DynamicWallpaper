namespace DynamicWallpaper.Player
{
    public class Program
    {
        private static Mutex mutex = new Mutex(true, "DynamicWallpaperPlayerMytex");

        static void Main(string[] args)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Console.WriteLine("Outra instância do Player já está em execução.");
                return;
            }

            Console.WriteLine("DynamicWallpaper.Player rodando em segundo plano...");

            // Mantem o processo ativo
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
