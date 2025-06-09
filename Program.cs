namespace WorldMover
{
    class Command
    {
        private static Config config;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong number of arguments");
                return;
            }

            config = Config.Load();
            Console.WriteLine($"config {config.To.PlanetCenter}");
        }

        private static void Test()
        {
            Vector3D v1 = new Vector3D(0, 1, 2);
            Vector3D v2 = new Vector3D(1, 2, 3);
            Console.WriteLine($"v1={v1}, v2={v2}");
            Console.WriteLine($"v1+v2={v1 + v2}");
            Console.WriteLine($"v1-v2={v1 - v2}");

        }
    }
}