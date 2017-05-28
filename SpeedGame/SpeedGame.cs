using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveM_Scripts
{
    public class SpeedGame : BaseScript
    {
        private bool isStarted = false; //Started game
        private bool isTicking = false; //Reached start speed

        private Vehicle vehicle;
        public SpeedGame()
        {
            Tick += SpeedGame_Tick;
            
            EventHandlers["speed:spawn"] += new Action(delegate
            {
                StartSpeedGame();
            });

        }

        private async void StartSpeedGame()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.Delete();
            }

            vehicle = await World.CreateVehicle(VehicleHash.Futo, Game.PlayerPed.Position, 120f);
            vehicle.Mods.LicensePlate = "DIE";
            vehicle.PlaceOnGround();

            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            CitizenFX.Core.UI.Screen.ShowNotification("Starting game of speed bomb.......");

            isStarted = true;
        }
        //tick
        private async Task SpeedGame_Tick()
        {
            if(isStarted)
            {
                if(Game.PlayerPed.IsInVehicle(vehicle))
                {
                    if (vehicle.Speed > 30 && !isTicking) { isTicking = true; CitizenFX.Core.UI.Screen.ShowNotification("Bomb Armed!"); }

                        if (isTicking && vehicle.Speed < 30)
                    {
                        World.AddExplosion(vehicle.Position, ExplosionType.Car, 1f, 0f);
                        vehicle.MarkAsNoLongerNeeded();
                        
                        CitizenFX.Core.UI.Screen.ShowNotification("You lost!");

                        isStarted = false;
                        isTicking = false;
                    }
                }

            }
            await Task.FromResult(0);
        }
    }
}
