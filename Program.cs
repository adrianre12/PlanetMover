namespace WorldMover
{
    //Move Config-WorldMover.xml  SANDBOX_0_0_0_.sbs Output.tmp

    enum Mode
    {
        None,
        Move,
        Extract,
        Remove,
        Config,
    }
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");

            Mode mode = Mode.None;
            if (args.Length > 0 && !Enum.TryParse<Mode>(args[0], true, out mode))
            {
                Console.WriteLine($"Unrecognised Mode: {args[0]}");
                UsageAndExit();
                return;
            }

            switch (mode)
            {
                case Mode.Config:
                    {
                        CheckArgs(2, args);
                        Configuration.Create(args[1]);
                        System.Environment.Exit(0);
                        break;
                    }
                case Mode.Move:
                    break;
                case Mode.Extract:
                    break;
                case Mode.Remove:
                    break;

                default:
                    {
                        Console.WriteLine("Unimplemented Mode:");
                        System.Environment.Exit(1);
                        break;
                    }
            }

            //Temp temp = new Temp();
            //temp.One();

            CheckArgs(4, args);
            if (!Configuration.Load(args[1]))
            {
                UsageAndExit();
            }

            if (!File.Exists(args[2]))
            {
                Console.WriteLine($"Input file not found: {args[2]}");
                UsageAndExit();
            }

            Process process = new Process();
            process.Start(mode, args[2], args[3]);

        }

        static void CheckArgs(int required, string[] args)
        {
            if (args.Length != required)
            {
                Console.WriteLine("Wrong number of arguments.");
                UsageAndExit();
            }
        }

        static void UsageAndExit()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("Mode: Move - Change planet and grids position");
            Console.WriteLine("      WorldMover.exe Move ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Extract - Copy only planet and grids to output file");
            Console.WriteLine("      WorldMover.exe Extract ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Remove - Delete planet and grids");
            Console.WriteLine("      WorldMover.exe Remove ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Config - Create a new default config file");
            Console.WriteLine("      WorldMover.exe Create ConfigFileName");
            System.Environment.Exit(1);
        }

    }




}