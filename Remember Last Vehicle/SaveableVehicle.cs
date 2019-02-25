using GTA;
using GTA.Math;

namespace Remember_Last_Vehicle
{
    public class SaveableVehicle
    {
        public int Handle { get; set; }

        public string VehicleName { get; set; }

        public uint Vehicle { get; set; }

        public float DirtLevel { get; set; }

        public VehicleColor PrimaryColor { get; set; }

        public VehicleColor SecondaryColor { get; set; }

        public VehicleColor DashboardColor { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public float BodyHealth { get; set; }

        public float Health { get; set; }

        public float EngineHealth { get; set; }

        public bool EngineRunning { get; set; }

        public bool LeftHeadlightBroken { get; set; }

        public bool RightHeadlightBroken { get; set; }

        public int Livery { get; set; }

        public bool LightsOn { get; set; }

        public VehicleWindowTint WindowTint { get; set; }

        public VehicleRoofState RoofState { get; set; }

        public VehicleWheelType WheelType { get; set; }
    }
}