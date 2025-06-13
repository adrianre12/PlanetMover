namespace WorldMover
{
    //Move Config-WorldMover.xml  SANDBOX_0_0_0_.sbs Output.tmp

    enum Mode
    {
        None,
        Move,
        Filter,
        Remove,
        Extract,
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
                case Mode.Filter:
                    break;
                case Mode.Remove:
                    break;
                case Mode.Extract:
                    break;
                default:
                    {
                        Console.WriteLine("Unimplemented Mode:");
                        UsageAndExit();
                        break;
                    }
            }



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
            Console.WriteLine("Mode: Filter - Output only planet and grids and output to a valid SBS file. Does not Move.");
            Console.WriteLine("      WorldMover.exe Extract ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Remove - Delete planet and grids and output to a valid SBS file. Does not Move.");
            Console.WriteLine("      WorldMover.exe Remove ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Extract - Copy only planet and grids to output file, this is not a valid SBS. Does not Move.");
            Console.WriteLine("      WorldMover.exe Extract ConfigFileName InputFileName OutputFileName\n");
            Console.WriteLine("Mode: Config - Create a new default config file");
            Console.WriteLine("      WorldMover.exe Config ConfigFileName");
            System.Environment.Exit(1);
        }

    }




}