﻿
namespace CarManufacturer
{
	public class Tire
	{
		private int year;
		private double pressure;
		public Tire(int year, double pressure)
		{
			this.Year = year;
			this.Pressure = pressure;
		}
		public int Year 
		{
			get { return this.year; }
			private set { this.year = value; }
		}
		public double Pressure
		{
			get { return this.pressure; }
			private set { this.pressure = value; }
		}
	}
}
