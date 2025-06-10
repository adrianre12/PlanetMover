using System.Xml.Serialization;

namespace WorldMover
{
    public class Position
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
        [XmlAttribute]
        public double Z { get; set; }

        public Position()
        {

        }
    }

    internal class Temp
    {


        //public Position position;
        public void One()
        {
            string line = "﻿<Position X=\"10759.237533857849\" Y=\"11085.651060095279\" Z=\"10985.962048027191\" />";

            Position position = line.Deserialize<Position>();
            Console.WriteLine($"Position: X={position.X} Y={position.Y} Z={position.Z}");

            line = position.Serialize();
            Console.WriteLine($"String: {line}");
        }
    }
}
