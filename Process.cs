using System.Text;
using System.Xml.Serialization;
using static WorldMover.Configuration;

namespace WorldMover
{
    public class Position
    {
        [XmlAttribute("x")]
        public double X { get; set; }
        [XmlAttribute("y")]
        public double Y { get; set; }
        [XmlAttribute("z")]
        public double Z { get; set; }

        public Position() { }
        public Position(Vector3D vector3D)
        {
            X = vector3D.X;
            Y = vector3D.Y;
            Z = vector3D.Z;
        }
        public Vector3D Vector3D()
        {
            return new Vector3D(X, Y, Z);
        }
    }

    internal class Process
    {
        private Vector3D transformation;
        private Vector3D newPlanetPosition;
        private StreamWriter outStreamWriter;
        private StreamReader inStreamReader;

        internal void Start(Mode mode, string inputFile, string outputFile)
        {
            Console.WriteLine($"Starting input={inputFile} output={outputFile}");
            Console.WriteLine($"PlanetCenter={Config.From.PlanetCenter}");
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
            bool outputSectorObject = false;
            bool isEntityBase = false;
            bool flush = false;
            bool isPlanet = false;
            bool isPositionAndOrientation = false;
            bool isThePlanet = false;
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
                            isEntityBase = true;

                            if (line.Contains("MyObjectBuilder_Planet"))
                                isPlanet = true;

                            break;
                        }

                    case bool b when line.Contains("</MyObjectBuilder_EntityBase>"):
                        {
                            Console.WriteLine("EntityBase End:");
                            flush = true;
                            break;
                        }

                    case bool b when isPlanet && line.Contains("<EntityId>"):
                        {
                            Console.WriteLine("TODO: Check it is the right planet.");

                            isThePlanet = true;
                            break;
                        }

                    case bool b when isEntityBase && line.Contains("<PositionAndOrientation>"):
                        {
                            isPositionAndOrientation = true;
                            break;
                        }

                    case bool b when isPositionAndOrientation && line.Contains("<Position "):
                        {
                            Position pos = line.Trim().Deserialize<Position>();
                            Vector3D position = pos.Vector3D();
                            var distance = Vector3D.Distance(position, Config.From.PlanetCenter);
                            Console.WriteLine(line);
                            Console.WriteLine($"distance={distance});");
                            switch (mode)
                            {
                                case Mode.Move:
                                    {
                                        if (isThePlanet)
                                        {
                                            // move the planet
                                            Position newPosition = new Position(position + transformation);
                                            string newLine = string.Concat(line.Substring(0, line.IndexOf("<Position ")), newPosition.Serialize());
                                            outputSectorObject = true;

                                            Console.WriteLine(newLine);
                                        }
                                        else
                                        {
                                            // check distance and move then
                                            if (distance > Config.From.IncludeEntitiesRadius)
                                                break;

                                            Position newPosition = new Position(position + transformation);
                                            string newLine = string.Concat(line.Substring(0, line.IndexOf("<Position ")), newPosition.Serialize());
                                            outputSectorObject = true;

                                            Console.WriteLine(newLine);
                                        }
                                        break;
                                    }
                                case Mode.Extract:
                                    {
                                        // is planet or grid in distance then
                                        if (distance > Config.From.IncludeEntitiesRadius)
                                            break;
                                        outputSectorObject = true;
                                        break;
                                    }
                                case Mode.Remove:
                                    {
                                        //is planet or grid in distance then 
                                        if (distance > Config.From.IncludeEntitiesRadius)
                                            break;
                                        outputSectorObject = false;
                                        //else true;
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("Error: Unimpemented mode");
                                        outputSectorObject = false;
                                        break;
                                    }
                            }
                        }
                        break;
                } // end of switch

                //decide flush or discard

                if (flush)
                {
                    if (outputSectorObject)
                    {
                        lineBuffer.AppendLine(line);
                        outStreamWriter.Write(lineBuffer.ToString());
                    }
                    lineBuffer.Clear();
                    flush = false;
                    isEntityBase = false;
                    isPlanet = false;
                    isPositionAndOrientation = false;
                    outputSectorObject = false;
                    continue;
                }
                if (isEntityBase)
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
