﻿namespace P01_RawData.Models
{
    public class Tire
    {
        public Tire(int age, double pressure)
        {
           this.Age = age;
           this.Pressure = pressure;
        }
        public int Age { get; set; }

        public double Pressure { get; }
    }
}