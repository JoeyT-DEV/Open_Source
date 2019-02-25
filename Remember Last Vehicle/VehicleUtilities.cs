using GTA;

namespace Remember_Last_Vehicle
{
    public class VehicleUtilities
    {
        public SaveableVehicle CreateInfo(Vehicle vehicle)
        {
            var saveableVehicle = new SaveableVehicle();
            saveableVehicle.Handle = vehicle.Handle;
            saveableVehicle.VehicleName = vehicle.DisplayName;
            saveableVehicle.Vehicle = (uint) vehicle.Model.Hash;
            saveableVehicle.DirtLevel = vehicle.DirtLevel;
            saveableVehicle.PrimaryColor =  vehicle.PrimaryColor;
            saveableVehicle.SecondaryColor =  vehicle.PrimaryColor;
            saveableVehicle.DashboardColor = vehicle.DashboardColor;
            saveableVehicle.Position = vehicle.Position;
            saveableVehicle.Rotation = vehicle.Rotation;
            saveableVehicle.BodyHealth = vehicle.BodyHealth;
            saveableVehicle.Health = vehicle.Health;
            saveableVehicle.EngineHealth = vehicle.EngineHealth;
            saveableVehicle.EngineRunning = vehicle.EngineRunning;
            saveableVehicle.LeftHeadlightBroken = vehicle.LeftHeadLightBroken;
            saveableVehicle.RightHeadlightBroken = vehicle.RightHeadLightBroken;
            saveableVehicle.Livery = vehicle.Livery;
            saveableVehicle.LightsOn = vehicle.LightsOn;
            saveableVehicle.WindowTint = vehicle.WindowTint;
            saveableVehicle.RoofState = vehicle.RoofState;
            saveableVehicle.WheelType = vehicle.WheelType;
            return saveableVehicle;
        }

        public Vehicle CreateVehicleFromData(SaveableVehicle data)
        {
            var vehicle = World.CreateVehicle(new Model((int)data.Vehicle), data.Position);
            vehicle.DirtLevel = data.DirtLevel;
            vehicle.PrimaryColor = data.PrimaryColor;
            vehicle.SecondaryColor = data.PrimaryColor;
            vehicle.DashboardColor = data.DashboardColor;
            vehicle.Position = data.Position;
            vehicle.Rotation = data.Rotation;
            vehicle.BodyHealth = data.BodyHealth;
            vehicle.EngineHealth = data.EngineHealth;
            vehicle.EngineRunning = data.EngineRunning;
            vehicle.Livery = data.Livery;
            vehicle.LightsOn = data.LightsOn;
            vehicle.WindowTint = data.WindowTint;
            vehicle.RoofState = data.RoofState;
            vehicle.WheelType = data.WheelType;
            vehicle.IsPersistent = true;
            return vehicle;
        }
    }
}