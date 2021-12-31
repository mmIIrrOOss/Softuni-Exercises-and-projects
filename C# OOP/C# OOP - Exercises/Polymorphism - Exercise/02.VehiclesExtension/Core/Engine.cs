﻿namespace _02.VehiclesExtension.Core
{
    using System;
    using Models;
    using Contracts;
    using Factories;

    public class Engine : IEngine
    {
        private readonly VehicleFactory vehicleFactory;

        public Engine()
        {
            this.vehicleFactory = new VehicleFactory();
        }
        public void Run()
        {
            Vehicle car = ProcessVehicleInfo();
            Vehicle truck = ProcessVehicleInfo();
            Vehicle bus = ProcessVehicleInfo();
            int count = int.Parse(Console.ReadLine());
            for (int i = 0; i < count; i++)
            {
                string[] args = Console.ReadLine().Split();
                string type = args[0];
                string vehicleType = args[1];
                double distanceKm = double.Parse(args[2]);
                try
                {
                    if (type == "Drive")
                    {
                        try
                        {
                            if (vehicleType == "Car")
                            {
                                Drive(car, distanceKm);
                            }
                            else if (vehicleType == "Truck")
                            {
                                Drive(truck, distanceKm);
                            }
                            else if (vehicleType == "Bus")
                            {
                                Drive(bus, distanceKm);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else if(type == "DriveEmpty")
                    {
                        Drive(bus, distanceKm);
                    }
                    else if (type == "Refuel")
                    {
                        try
                        {
                            if (vehicleType == "Car")
                            {
                                Refuel(car, distanceKm);
                            }
                            else if (vehicleType == "Truck")
                            {
                                Refuel(truck, distanceKm);
                            }
                            else if (vehicleType == "Bus")
                            {
                                Refuel(bus, distanceKm);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    Console.WriteLine(ioe.Message);
                }

            }
            Console.WriteLine(car);
            Console.WriteLine(truck);
            Console.WriteLine(bus);
        }
        private void Refuel(Vehicle vehicle, double fuelQuantity)
        {
            vehicle.Refuel(fuelQuantity);
        }
        private void Drive(Vehicle vehicle, double kilometers)
        {
            Console.WriteLine(vehicle.Drive(kilometers));
        }
        private Vehicle ProcessVehicleInfo()
        {
            string[] args = Console.ReadLine().Split();
            string vehicleType = args[0];
            double fuelQyantity = double.Parse(args[1]);
            double fuelConspumption = double.Parse(args[2]);
            double tankCapacity = double.Parse(args[3]);
            if (args.Length == 4)
            {
                Vehicle currentVehicle = vehicleFactory.
                CreateVehicle(vehicleType, fuelQyantity, fuelConspumption, tankCapacity);
                return currentVehicle;
            }
            else
            {
                int countOfPeople = int.Parse(args[4]);
                Vehicle currentVehicle = vehicleFactory.
               CreateVehicle(vehicleType, fuelQyantity, fuelConspumption, tankCapacity,countOfPeople);
                return currentVehicle;
            }
        }

    }
}
