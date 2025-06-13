using System.Xml.Serialization;

namespace WorldMover
{
    public class Vector3D
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
        [XmlAttribute]
        public double Z { get; set; }

        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator +(Vector3D value1, Vector3D value2)
        {
            Vector3D result = new Vector3D();
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            return result;
        }

        public static Vector3D operator -(Vector3D value1, Vector3D value2)
        {
            Vector3D result = new Vector3D();
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        public static double Distance(Vector3D value1, Vector3D value2)
        {
            double num = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }
    }

}