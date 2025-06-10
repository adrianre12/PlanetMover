namespace WorldMover
{
    enum Mode
    {
        Move,
        Extract,
        Remove,
    }
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");
            if (args.Length != 4)
            {
                Console.WriteLine("Wrong number of arguments");
                Usage();
                return;
            }

            Mode mode;
            if (!Enum.TryParse<Mode>(args[0], true, out mode))
            {
                Console.WriteLine($"Unrecognised Mode: {args[0]}");
                Usage();
                return;
            }

            //Temp temp = new Temp();
            //temp.One();


            Configuration.Load(args[1]);
            Process process = new Process();
            process.Start(mode, args[2], args[3]);

        }

        static void Usage()
        {
            Console.WriteLine("Usage: WorldMover Mode ConfigFile InputFile OutputFile");
            Console.WriteLine("Mode: Move - Change planet and grids position");
            Console.WriteLine("Mode: Extract - Copy only planet and grids to output file");
            Console.WriteLine("Mode: Remove - Delete planet and grids");
        }

    }




}