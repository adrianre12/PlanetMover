namespace WorldMover
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong number of arguments");
                return;
            }

            Configuration.Load();
            Process process = new Process();
            process.Start(args[0]);
        }


    }
}