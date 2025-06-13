using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace PlanetMover
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_VirtualMass), false, new[] { "PlanetMoverBlockLarge" })]
    internal class PlanetMoverTools : MyGameLogicComponent
    {
        private IMyFunctionalBlock block;
        private MyPlanet closestPlanet;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            if (!MyAPIGateway.Session.IsServer)
                return;

            block = Entity as IMyFunctionalBlock;

            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;

        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            Log.Msg("Starting");
            closestPlanet = MyGamePruningStructure.GetClosestPlanet(block.GetPosition());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine($"<Configuration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            sb.AppendLine($"  <From>");
            sb.AppendLine($"    <PlanetEntityId>{closestPlanet.EntityId}</PlanetEntityId>");
            var center = closestPlanet.WorldMatrix.Translation;
            sb.AppendLine($"    <PlanetCenter X=\"{center.X - 0.5}\" Y=\"{center.Y - 0.5}\" Z=\"{center.Z - 0.5}\" />");
            var blc = closestPlanet.PositionLeftBottomCorner;
            sb.AppendLine($"    <!--This is the Position shown in the SBS file -->");
            sb.AppendLine($"    <PlanetPosition X=\"{blc.X - 0.5}\" Y=\"{blc.Y - 0.5}\" Z=\"{blc.Z - 0.5}\" />");

            sb.AppendLine($"<!--This is set to planet Maximum Radius plus 1000m, set to 0 to disable any other objects. -->");
            sb.AppendLine($"<IncludeEntitiesRadius>{closestPlanet.MaximumRadius + 1000}</IncludeEntitiesRadius>");
            sb.AppendLine($"  </From>");
            sb.AppendLine($"  <To>");
            sb.AppendLine($"    <!-- Change this position to the GPS values of the new location -->");
            sb.AppendLine($"    <PlanetCenter X=\"{center.X - 0.5}\" Y=\"{center.Y - 0.5}\" Z=\"{center.Z - 0.5}\" />");
            sb.AppendLine($"  </To>");
            sb.AppendLine($"</Configuration>");
            block.CustomData = sb.ToString();

            LogPlanetPositions();
        }

        internal void LogPlanetPositions()
        {
            Dictionary<string, Vector3D> planetPositions = new Dictionary<string, Vector3D>();
            MyAPIGateway.Entities.GetEntities(null, e =>
            {
                if (e is MyPlanet)
                {
                    var planet = e as MyPlanet;
                    if (planetPositions.ContainsKey(planet.StorageName))
                    {
                        Log.Msg($"Error duplicate planet name found: {planet.StorageName}");
                    }
                    var center = planet.WorldMatrix.Translation;
                    planetPositions.Add(planet.StorageName, center);
                    Log.Msg($"Planet={planet.StorageName} Center=[{center.ToString()}]");
                }
                return false;
            });
        }

    }
}