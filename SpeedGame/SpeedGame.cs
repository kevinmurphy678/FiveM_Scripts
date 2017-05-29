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

        private DateTime startTime;

        private Vehicle vehicle;
        public SpeedGame()
        {
            Tick += SpeedGame_Tick;

            EventHandlers["speed:spawn"] += new Action(delegate
            {
                StartSpeedGame();
            });

            EventHandlers["speed:scoreClient"] += new Action<dynamic, dynamic>((name, time) =>
            {
                TriggerEvent("chatMessage", "Speed Game", new int[] { 0, 255, 0 }, name + " scored " + time + " seconds!");
            });
        }

        private async void StartSpeedGame()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                // Game.PlayerPed.CurrentVehicle.Delete();
                vehicle = Game.PlayerPed.CurrentVehicle;
            }
            else
            {
                vehicle = await World.CreateVehicle(VehicleHash.Futo, Game.PlayerPed.Position, 120f);   
                vehicle.PlaceOnGround();
            }

            vehicle.Mods.LicensePlate = "DIE";

            //Model pedModel = new Model(PedHash.Prisoner01);
            //pedModel.Request();
            //while(!pedModel.IsLoaded) await BaseScript.Delay(1);
            // Ped ped = await World.CreatePed(pedModel, Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 1);
            // ped.SetIntoVehicle(vehicle, VehicleSeat.Passenger);


            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            CitizenFX.Core.UI.Screen.ShowNotification("Starting game of speed bomb..");

            isStarted = true;

            await Task.FromResult(0);
        }
        //tick
        private async Task SpeedGame_Tick()
        {
            if(isStarted)
            {
                if(Game.PlayerPed.IsInVehicle(vehicle))
                {
                    if (vehicle.Speed > 29 && !isTicking) {

                        isTicking = true;
                        CitizenFX.Core.UI.Screen.ShowNotification("Bomb Arming...");
                        startTime = DateTime.Now;
                        await BaseScript.Delay(2000);
                        CitizenFX.Core.UI.Screen.ShowNotification("Bomb Armed!");
                    }

                    if (isTicking && vehicle.Speed < 26)
                    {
                        World.AddExplosion(vehicle.Position, ExplosionType.Car, 1f, 0f);
                        vehicle.MarkAsNoLongerNeeded();
                        
                        CitizenFX.Core.UI.Screen.ShowNotification("You lost!");

                        isStarted = false;
                        isTicking = false;

                        DateTime end = DateTime.Now;
                        var diffInSeconds = Math.Abs((startTime - end).TotalSeconds);
                        TriggerServerEvent("speed:scoreServer",  Game.Player.Name + " in a " + Game.PlayerPed.CurrentVehicle.LocalizedName, diffInSeconds);
                   

                    }
                }

            }
            await Task.FromResult(0);
        }
    }
}
