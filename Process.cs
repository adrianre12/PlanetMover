using static WorldMover.Configuration;

namespace WorldMover
{
    internal class Process
    {
        private Vector3D transformation;
        private Vector3D newPlanetPosition;


        internal void Start(string inputSBS)
        {
            var outputSBS = $"Moved-{inputSBS}";
            Console.WriteLine($"Starting input={inputSBS} output={outputSBS}");
            CalculateTransformation();
            CalculateNewPlanetPosition();

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


    }
}
