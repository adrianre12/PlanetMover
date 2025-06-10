using System.Text;
using static WorldMover.Configuration;

namespace WorldMover
{
    internal class Process
    {
        private Vector3D transformation;
        private Vector3D newPlanetPosition;
        private StreamWriter outStreamWriter;
        private StreamReader inStreamReader;

        internal void Start(Mode mode, string inputFile, string outputFile)
        {
            Console.WriteLine($"Starting input={inputFile} output={outputFile}");
            CalculateTransformation();
            CalculateNewPlanetPosition();
            using (var outFileStream = new FileStream(outputFile, FileMode.Create))
            using (outStreamWriter = new StreamWriter(outFileStream))
            using (var inFileStream = new FileStream(inputFile, FileMode.Open))
            using (inStreamReader = new StreamReader(inFileStream))

                ParseSBS(mode);

        }

        internal void CalculateTransformation()
        {
            Console.WriteLine("CalculateTransformation.");

            transformation = Config.To.PlanetCenter - Config.From.PlanetCenter;

            Console.WriteLine($"Transformation: {transformation}");
        }

        internal void CalculateNewPlanetPosition()
        {
            Console.WriteLine("CalculateNewPlanetPosition.");

            var centerToPositionTransformation = Config.From.PlanetPosition - Config.From.PlanetCenter;
            newPlanetPosition = Config.To.PlanetCenter + centerToPositionTransformation;

            Console.WriteLine($"New PlanetPosition={newPlanetPosition}");

        }

        private void ParseSBS(Mode mode)
        {
            bool nonSectorObjects = false;
            bool collect = false;
            bool flush = false;
            var lineBuffer = new StringBuilder();

            switch (mode)
            {
                case Mode.Move:
                    {
                        nonSectorObjects = true;
                        break;
                    }
                case Mode.Extract:
                    {
                        nonSectorObjects = false;
                        break;
                    }
                case Mode.Remove:
                    {
                        nonSectorObjects = true;
                        break;
                    }
            }
            while (!inStreamReader.EndOfStream)
            {
                var line = inStreamReader.ReadLine();

                //look for <MyObjectBuilder_EntityBase
                switch (true)
                {
                    case bool b when line.Contains("<MyObjectBuilder_EntityBase"):
                        {
                            Console.WriteLine($"EntityBase Start: {line}");
                            collect = true;
                            break;
                        }

                    case bool b when line.Contains("</MyObjectBuilder_EntityBase>"):
                        {
                            Console.WriteLine("EntityBase End:");
                            flush = true;
                            break;
                        }
                }
                //start collecting
                //decide flush or discard

                if (flush)
                {
                    lineBuffer.AppendLine(line);
                    outStreamWriter.Write(lineBuffer.ToString());
                    lineBuffer.Clear();
                    flush = false;
                    collect = false;
                    continue;
                }
                if (collect)
                {
                    lineBuffer.AppendLine(line);
                    continue;
                }
                if (nonSectorObjects)
                {
                    outStreamWriter.WriteLine(line);
                }
            }

            if (lineBuffer.Length > 0)
            {
                Console.WriteLine($"Error: LineBuffer not empty:\n{lineBuffer.ToString()}");
            }
            outStreamWriter.Flush();
            outStreamWriter.Close();
        }

    }
}
