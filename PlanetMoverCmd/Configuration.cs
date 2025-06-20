﻿using System.Xml.Serialization;


namespace WorldMover
{
    public class Configuration
    {
        internal static Configuration Config;

        public struct FromInfo
        {
            public string PlanetEntityId { get; set; }
            public Vector3D PlanetCenter { get; set; }
            public Vector3D PlanetPosition { get; set; }
            public double IncludeEntitiesRadius { get; set; }

            public FromInfo(Vector3D planetCenter, Vector3D planetPosition, string planetEntityId = "0", double includeEntitiesRadius = 0)
            {
                this.PlanetEntityId = planetEntityId;
                this.PlanetCenter = planetCenter;
                this.PlanetPosition = planetPosition;
                this.IncludeEntitiesRadius = includeEntitiesRadius;
            }
        }

        public struct ToInfo
        {
            public Vector3D PlanetCenter { get; set; }

            public ToInfo(Vector3D planetCenter)
            {
                this.PlanetCenter = planetCenter;
            }
        }

        [XmlIgnore]
        public bool IsLoaded;

        public FromInfo From { get; set; }
        public ToInfo To { get; set; }


        public Configuration()
        {
            IsLoaded = false;
        }

        public static bool Load(string configFilename)
        {

            Config = new Configuration();
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

            if (File.Exists(configFilename))
            {
                try
                {
                    using (Stream reader = new FileStream(configFilename, FileMode.Open))
                    {
                        Config = (Configuration)serializer.Deserialize(reader);
                        Config.IsLoaded = true;
                        Console.WriteLine("Config loaded");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading config: {ex}");
                    Config = new Configuration();
                }
                return Config.IsLoaded;
            }

            Console.WriteLine("Config file not found");
            return false;
        }

        public static void Create(string configFilename)
        {
            Config = new Configuration();
            Config.From = new FromInfo(new Vector3D(), new Vector3D());
            Config.To = new ToInfo(new Vector3D());
            Config.IsLoaded = false;
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

            using (Stream writer = new FileStream(configFilename, FileMode.Create))
            {
                serializer.Serialize(writer, Config);
                Console.WriteLine("New Config file created");
            }
        }
    }

}
