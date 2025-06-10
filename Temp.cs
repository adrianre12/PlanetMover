namespace WorldMover
{

    internal class Temp
    {


        //public Position position;
        public void One()
        {
            string line = "\t<Position x=\"10759.237533857849\" y=\"11085.651060095279\" z=\"10985.962048027191\" />";
            Console.WriteLine($"Index={line.IndexOf("<Position")} prefix=[{line.Substring(0, line.IndexOf("<Position"))}]");

            Position position = line.Trim().Deserialize<Position>();
            Console.WriteLine($"Position: X={position.X} Y={position.Y} Z={position.Z}");

            line = position.Serialize();
            Console.WriteLine($"String: {line}");
            var v1 = new Vector3D(1, 2, 3);

            Vector3D v2 = position.Vector3D() + v1;

            Position p = new Position(v2);
            line = p.Serialize();
            Console.WriteLine($"String: {p}");
            Console.WriteLine($"Position: X={position.X} Y={position.Y} Z={position.Z}");

        }
    }
}
