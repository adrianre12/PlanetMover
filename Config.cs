using System.Xml.Serialization;


namespace WorldMover
{
    public class Config
    {
        const string configFilename = "Config-WorldMover.xml";

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
        public bool ConfigLoaded;

        public FromInfo From { get; set; }
        public ToInfo To { get; set; }


        public Config()
        {
            ConfigLoaded = false;
        }

        public static Config Load()
        {

            Config config = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            if (File.Exists(configFilename))
            {
                try
                {
                    using (Stream reader = new FileStream(configFilename, FileMode.Open))
                    {
                        config = (Config)serializer.Deserialize(reader);
                        config.ConfigLoaded = true;
                        Console.WriteLine("Config loaded");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading config: {ex}");
                    return new Config();
                }
                return config;
            }

            config = new Config();
            config.From = new FromInfo(new Vector3D(), new Vector3D());
            config.To = new ToInfo(new Vector3D());
            config.ConfigLoaded = false;

            using (Stream writer = new FileStream(configFilename, FileMode.Create))
            {
                serializer.Serialize(writer, config);
                Console.WriteLine("New Config file created");
            }
            return config;
        }
    }

}
