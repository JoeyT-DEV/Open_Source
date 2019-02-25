using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using System.Linq;

namespace Remember_Last_Vehicle
{
    public class MainGTAscript : Script
    {
        private bool firstTime = true;
        private bool _runVehicleInitializer = true;
        private VehicleUtilities _vehicleUtilities = new VehicleUtilities();

        ScriptSettings config; // Includes settings file (.ini)
        int vehicleLimit; // Max amount of vehicles that can be saved
        Keys saveKey; // Key to save the vehicle
        Keys unsaveKey; // Key to unsave the vehicle
        Keys menuKey; // Key to open menu with saved vehicles

        MenuPool menuPool; // Pool that contains all the menus
        UIMenu mainMenu; // The main menu
        UIMenu vehicleMenu;
        UIMenuItem vehicleItem; // Selectable Main Menu Item
        UIMenuItem unsaveButton; // Selectable Main Menu Item
        UIMenuItem exitButton; // Selectable Main Menu Item
        UIMenuItem loadVehicles; // Selectable Main Menu Item

        List<Vehicle> savedVehicles = new List<Vehicle>(); // List with saved vehicles

        public MainGTAscript()
        {
            new Model();

            MainMenu();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            Interval = 10;

            config = ScriptSettings.Load(@"C:\Program Files\Rockstar Games\Grand Theft Auto V\scripts\SaveVehicles.ini"); // Loads the specified file
            vehicleLimit = config.GetValue("Configuration", "vehicleLimit", 5); // Gets the vehicleLimit from the inifile
            saveKey = config.GetValue("Configuration", "saveKey", Keys.X); // Gets the saveKey from the inifile
            unsaveKey = config.GetValue("Configuration", "unsaveKey", Keys.C); // Gets the unsaveKey from the inifile
            menuKey = config.GetValue("Configuration", "menuKey", Keys.T); // Gets the menuKey from the inifile

            var basePath = config.GetValue("Configuration", "vehiclePersistencePath", @"C:\GTA_SaveVehicles");

            //Initialiseer het pad in de json klasse
            JsonVehicleStorage.InitializeBasePath(basePath);
        }

        void MainMenu()
        {
            // Initializing the menu
            menuPool = new MenuPool();
            mainMenu = new UIMenu("SaveVehicles 3.0", "");
            menuPool.Add(mainMenu); // Adds mainMenu to the pool
            vehicleMenu = menuPool.AddSubMenu(mainMenu, "Saved Vehicles"); // Submenu options

            loadVehicles = new UIMenuItem("Load All");
            mainMenu.AddItem(loadVehicles); // Adds menu item to the main menu
            unsaveButton = new UIMenuItem("Remove All");
            mainMenu.AddItem(unsaveButton); // Adds menu item to the main menu
            exitButton = new UIMenuItem("Exit");
            mainMenu.AddItem(exitButton); // Adds menu item to the main menu

            mainMenu.OnItemSelect += onMainMenuItemSelect;
        }

        void onMainMenuItemSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if(item == loadVehicles)
            {
                SaveableVehicle[] saveVehicles = new SaveableVehicle[0];
                try
                {
                    //Haal opgeslagen auto's op
                    saveVehicles = JsonVehicleStorage.GetVehicles();
                }
                catch(Exception e)
                {
                    UI.Notify(e.Message);
                }

                try
                {
                    foreach(var saveVehicle in saveVehicles)
                    {
                        //Zet opgeslagen auto's om naar GTA objecten en creëer ze in het spel
                        Vehicle vehicle = null;
                        try
                        {
                            vehicle = _vehicleUtilities.CreateVehicleFromData(saveVehicle);
                        }
                        catch(Exception e)
                        {
                            UI.Notify(e.Message);
                        }

                        if(vehicle == null)
                        {
                            return;
                        }

                        //Sla auto op in de opgeslagen auto lijst
                        savedVehicles.Add(vehicle);

                        try
                        {
                            var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, vehicle); // Sets blip on vehicle
                            blip.IsShortRange = true;
                            Function.Call(Hash.SET_BLIP_SPRITE, blip, 225); // Sets vehicle blip icon
                            Function.Call(Hash.SET_BLIP_DISPLAY, blip, 2); // Displays the blip icon
                        }
                        catch(Exception e)
                        {
                            UI.Notify(e.Message);
                        }
                    }
                }
                catch(Exception e)
                {
                    UI.Notify(e.Message);
                }
            }

            if(item == unsaveButton)
            {
                while(savedVehicles.Count > 0)
                {
                    Vehicle listVehicle = savedVehicles[savedVehicles.Count - 1];
                    savedVehicles.Remove(listVehicle);

                    var saveableVehicle = _vehicleUtilities.CreateInfo(listVehicle);

                    JsonVehicleStorage.RemoveVehicle(saveableVehicle);

                    listVehicle.IsPersistent = false;
                    listVehicle.CurrentBlip.Remove();
                    listVehicle.Delete();
                }

                UI.Notify("All Vehicles Unsaved and Removed");
            }

            if(item == exitButton)
            {
                mainMenu.GoBack();
            }
        }

        public void OnTick(object sender, EventArgs eventArgs)
        {
            if(menuPool != null)
            {
                menuPool.ProcessMenus(); // Loads all the menus
            }

            foreach(Vehicle veh in savedVehicles)
            {
                if(veh.Health == 0 || veh.IsDead || !veh.Exists())
                {
                    veh.CurrentBlip.Remove();
                    veh.IsPersistent = false;
                    savedVehicles.Remove(veh);
                    UI.Notify("Vehicle " + veh.DisplayName + " Destroyed");

                    var saveableVehicle = _vehicleUtilities.CreateInfo(veh);

                    JsonVehicleStorage.RemoveVehicle(saveableVehicle);
                }
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == menuKey && !menuPool.IsAnyMenuOpen())
            {
                mainMenu.Visible = !mainMenu.Visible;
                vehicleMenu.MenuItems.Clear();

                foreach(Vehicle veh in savedVehicles)
                {
                    if(vehicleMenu.MenuItems.Count <= savedVehicles.Count)
                    {
                        vehicleItem = new UIMenuItem(veh.DisplayName);
                        vehicleMenu.AddItem(vehicleItem); // Adds menu item to the main menu
                    }
                }
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            var keys = new[] { saveKey, unsaveKey };

            if(keys.Any(x => x == e.KeyCode))
            {
                var player = Game.Player.Character;
                var currentVeh = player.CurrentVehicle;

                if(e.KeyCode == saveKey && player.IsInVehicle())
                {
                    if(!savedVehicles.Contains(currentVeh))
                    {
                        if(savedVehicles.Count < vehicleLimit)
                        {
                            var blip = Function.Call<Blip>(Hash.ADD_BLIP_FOR_ENTITY, player.CurrentVehicle);
                            blip.IsShortRange = true;
                            Function.Call(Hash.SET_BLIP_SPRITE, blip, 225);
                            Function.Call(Hash.SET_BLIP_DISPLAY, blip, 2);

                            currentVeh.IsPersistent = true;
                            savedVehicles.Add(currentVeh);

                            //Zet de huidige auto om naar het object wat opgeslagen kan worden (oftewel het json object)
                            var saveableVehicle = _vehicleUtilities.CreateInfo(currentVeh);

                            //Sla de omgezette auto op
                            JsonVehicleStorage.SaveVehicle(saveableVehicle);

                            UI.Notify(currentVeh.DisplayName + " Saved");
                        }

                        else
                        {
                            UI.Notify("You can't save more than " + vehicleLimit + " Vehicles");
                        }
                    }

                    else
                    {
                        UI.Notify("Current Vehicle Already Saved");
                    }
                }

                else if(e.KeyCode == unsaveKey && player.IsInVehicle() && savedVehicles.Contains(currentVeh))
                {
                    savedVehicles.Remove(currentVeh);                    

                    //Zet de auto om naar het json object
                    var saveableVehicle = _vehicleUtilities.CreateInfo(currentVeh);

                    //Haal de auto uit de opgeslagen auto's
                    JsonVehicleStorage.RemoveVehicle(saveableVehicle);

                    currentVeh.IsPersistent = false;
                    currentVeh.CurrentBlip.Remove();

                    UI.Notify("Current Vehicle Unsaved");
                }

                else if(e.KeyCode == unsaveKey && player.IsOnFoot)
                {
                    if(savedVehicles.Count > 0)
                    {
                        // WORKS, BUT DOESNT REMOVE THE VEHICLE FROM THE JSON FILE
                        try
                        {
                            Vehicle listVehicle = savedVehicles[savedVehicles.Count - 1];
                            savedVehicles.Remove(listVehicle);

                            var saveableVehicle = _vehicleUtilities.CreateInfo(listVehicle);

                            JsonVehicleStorage.RemoveVehicle(saveableVehicle);

                            listVehicle.IsPersistent = false;
                            listVehicle.CurrentBlip.Remove();

                            UI.Notify(listVehicle.DisplayName + " Unsaved");

                        }

                        catch(Exception ex)
                        {
                            UI.ShowSubtitle(ex.Message);
                        }
                      }

                    else
                    {
                        UI.Notify("You don't have any Saved Vehicles");
                    }
                }

                else if(e.KeyCode == saveKey && player.IsOnFoot)
                {
                    UI.Notify("Player is not in Vehicle");
                }
            }
        }
    }
}