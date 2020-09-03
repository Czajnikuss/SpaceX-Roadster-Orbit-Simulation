using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Calc 
{
	public static double Deg2Rad = Math.PI / 180.0;

	public static int AU2KM = 149597871;

	public static Vector3Double CalculateOrbitalPosition(double semimajorAxis, double eccentricity, double inclination, double longitudeOfAscendingNode, double periapsisArgument, double trueAnomaly)
	{
		double num = semimajorAxis * (double)AU2KM * Math.Pow(10.0, -3.0);
		double d = inclination * Deg2Rad;
		double num2 = longitudeOfAscendingNode * Deg2Rad;
		double num3 = periapsisArgument * Deg2Rad;
		double num4 = trueAnomaly * Deg2Rad;
		double num5 = num * (1.0 - eccentricity * eccentricity) / (1.0 + eccentricity * Math.Cos(num4));
		double num6 = Math.Acos((eccentricity + Math.Cos(num4)) / (1.0 + eccentricity * Math.Cos(num4)));
		Math.Atan2(Math.Sqrt(1.0 - eccentricity * eccentricity) * Math.Sin(num6), Math.Cos(num6) - eccentricity);
		double x = num5 * (Math.Cos(num4 + num3) * Math.Cos(num2) - Math.Sin(num4 + num3) * Math.Cos(d) * Math.Sin(num2));
		double y = num5 * (Math.Cos(num4 + num3) * Math.Cos(num2) + Math.Sin(num4 + num3) * Math.Cos(d) * Math.Sin(num2));
		double z = num5 * Math.Sin(num4 + num3) * Math.Cos(d);
		return new Vector3Double(x, y, z);
	}
}
[System.Serializable]
public struct DatePositionRaw
{
    public DateTime date;
    public double semimajorAxis;
    public double eccentricity;
    public double inclination;
    public double longitudeOfAscendingNode;
    public double periapsisArgument;
    public double trueAnomaly; 
}
[System.Serializable]
public struct Vector3Double
{
	public double x;

	public double y;

	public double z;

	public Vector3Double(double x, double y, double z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}
}

