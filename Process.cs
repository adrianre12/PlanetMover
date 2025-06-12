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
        //private Vector3D newPlanetPosition;
        private StreamWriter outStreamWriter;
        private StreamReader inStreamReader;

        internal void Start(Mode mode, string inputFile, string outputFile)
        {
            Console.WriteLine($"Starting Mode={mode.ToString()} Input={inputFile} Output={outputFile}");
            Console.WriteLine($"From PlanetCenter={Config.From.PlanetCenter}");
            if (mode == Mode.Move)
            {
                Console.WriteLine($"To   PlanetCenter={Config.To.PlanetCenter}");
                CalculateTransformation();
            }
            //CalculateNewPlanetPosition();
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

        /*        internal void CalculateNewPlanetPosition()
                {
                    Console.WriteLine("CalculateNewPlanetPosition.");

                    var centerToPositionTransformation = Config.From.PlanetPosition - Config.From.PlanetCenter;
                    newPlanetPosition = Config.To.PlanetCenter + centerToPositionTransformation;

                    Console.WriteLine($"New PlanetPosition={newPlanetPosition}");

                }*/

        private void ParseSBS(Mode mode)
        {
            Console.WriteLine($"\nStarting {mode.ToString()}\n");
            string indent = "\t";
            bool nonSectorObjects = false;
            bool outputSectorObject = false;
            bool isEntityBase = false;
            bool flush = false;
            bool isPlanet = false;
            bool isPositionAndOrientation = false;
            bool isThePlanet = false;
            bool inChildren = false;
            bool disablePosition = false;
            int inProjectedGrids = 0;
            int entityLineCounter = 0;
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
                    case bool b when !inChildren && !isEntityBase && line.Contains("<MyObjectBuilder_EntityBase"):
                        {
                            Console.WriteLine($"EntityBase Start:\n{indent}{line.Trim()}");
                            isEntityBase = true;

                            if (line.Contains("MyObjectBuilder_Planet"))
                                isPlanet = true;

                            break;
                        }

                    case bool b when !inChildren && line.Contains("</MyObjectBuilder_EntityBase>"):
                        {
                            Console.WriteLine($"{indent}Selected: {outputSectorObject}");
                            Console.WriteLine($"EntityBase End: Lines={entityLineCounter + 1}\n");
                            flush = true;
                            break;
                        }

                    case bool b when line.Contains("<ProjectedGrids>"):
                        {
                            inProjectedGrids++;
                            Console.WriteLine($"*{indent}ProjectedGrids start: depth={inProjectedGrids}");

                            disablePosition = true;
                            break;
                        }

                    case bool b when line.Contains("</ProjectedGrids>"):
                        {
                            if (inProjectedGrids > 0)
                                inProjectedGrids--;
                            else
                                disablePosition = false;

                            Console.WriteLine($"*{indent}ProjectedGrids end: depth={inProjectedGrids}");
                            break;
                        }

                    case bool b when isPlanet && line.Contains("<EntityId>") && line.Contains(Config.From.PlanetEntityId):
                        {
                            Console.WriteLine($"{indent}Found the planet.");

                            isThePlanet = true;
                            break;
                        }

                    case bool b when isPlanet && line.Contains("<Name>"):
                        {
                            var start = line.IndexOf(">") + 1;
                            var end = line.IndexOf("<", start);
                            string name = line.Substring(start, end - start);
                            Console.WriteLine($"{indent}Planet Name: {name}");
                            break;
                        }


                    case bool b when isEntityBase && line.Contains("<PositionAndOrientation>"):
                        {
                            isPositionAndOrientation = true;
                            break;
                        }

                    case bool b when !disablePosition && isPositionAndOrientation && line.Contains("<Position "):
                        {
                            Position pos = line.Trim().Deserialize<Position>();
                            Vector3D position = pos.Vector3D();
                            double distance;
                            if (isPlanet)
                            {
                                distance = Vector3D.Distance(position, Config.From.PlanetPosition);
                            }
                            else
                            {
                                distance = Vector3D.Distance(position, Config.From.PlanetCenter);
                            }
                            Console.WriteLine($"{indent}Distance={distance}");
                            switch (mode)
                            {
                                case Mode.Move:
                                    {
                                        outputSectorObject = true;
                                        Console.WriteLine($"{indent}Original Position: X={position.X} Y={position.Y} Z={position.Z}");
                                        Position newPosition;
                                        if (isThePlanet)
                                        {
                                            // move the planet
                                            newPosition = new Position(position + transformation);
                                            line = string.Concat(line.Substring(0, line.IndexOf("<Position ")), newPosition.Serialize());
                                        }
                                        else
                                        {
                                            // check distance and move then
                                            if (distance > Config.From.IncludeEntitiesRadius)
                                                break;

                                            newPosition = new Position(position + transformation);
                                            line = string.Concat(line.Substring(0, line.IndexOf("<Position ")), newPosition.Serialize());
                                        }
                                        Console.WriteLine($"{indent}Updated  Position: X={newPosition.X} Y={newPosition.Y} Z={newPosition.Z}");

                                        break;
                                    }
                                case Mode.Extract:
                                    {
                                        Console.WriteLine($"{indent}Position: X={position.X} Y={position.Y} Z={position.Z}");

                                        // is planet or grid in distance then
                                        if (!isThePlanet && distance > Config.From.IncludeEntitiesRadius) // planets dont use the center.position
                                            break;
                                        outputSectorObject = true;
                                        break;
                                    }
                                case Mode.Remove:
                                    {
                                        Console.WriteLine($"{indent}Position: X={position.X} Y={position.Y} Z={position.Z}");

                                        //is planet or grid in distance then 
                                        outputSectorObject = true;
                                        if (!isThePlanet && distance > Config.From.IncludeEntitiesRadius) // planets dont use the center.position
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
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("<DisplayName>"):
                        {
                            var start = line.IndexOf(">") + 1;
                            var end = line.IndexOf("<", start);
                            string name = line.Substring(start, end - start);
                            Console.WriteLine($"{indent}Name: {name}");
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("<StorageName>"):
                        {
                            var start = line.IndexOf(">") + 1;
                            var end = line.IndexOf("<", start);
                            string name = line.Substring(start, end - start);
                            Console.WriteLine($"{indent}Storage Name: {name}");
                            break;
                        }

                    case bool b when line.Contains("<Children>"): // work arround for characters possibly grids
                        {
                            Console.WriteLine($"{indent}Start Children");
                            indent = "\t\t";
                            inChildren = true; // supress looking at MyObjectBuilder_EntityBase
                            break;
                        }

                    case bool b when line.Contains("</Children>"):
                        {
                            indent = "\t";
                            Console.WriteLine($"{indent}End Children");
                            inChildren = false;
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("<Pilot>"): // work arround for characters possibly grids
                        {
                            Console.WriteLine($"{indent}Start Pilot");
                            indent = "\t\t";
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("</Pilot>"):
                        {
                            indent = "\t";
                            Console.WriteLine($"{indent}End Pilot");
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("<PilotRelativeWorld>"):
                        {
                            //Console.WriteLine("disablePosition=false 0");
                            disablePosition = true;
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && line.Contains("</PilotRelativeWorld>"):
                        {
                            //Console.WriteLine("disablePosition=false 1");
                            disablePosition = false;
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && (line.Contains("<LocalPositionAndOrientation>") || line.Contains("<MasterToSlaveTransform>")):
                        {
                            disablePosition = true;
                            break;
                        }

                    case bool b when inProjectedGrids == 0 && (line.Contains("</LocalPositionAndOrientation>") || line.Contains("</MasterToSlaveTransform>")):
                        {
                            //Console.WriteLine("disablePosition=false 2");
                            disablePosition = false;
                            break;
                        }

                } // end of switch

                if (isEntityBase)
                {
                    entityLineCounter++;
                }

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
                    isThePlanet = false;
                    isPositionAndOrientation = false;
                    outputSectorObject = false;
                    inChildren = false;
                    disablePosition = false;
                    inProjectedGrids = 0;
                    entityLineCounter = 0;
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
