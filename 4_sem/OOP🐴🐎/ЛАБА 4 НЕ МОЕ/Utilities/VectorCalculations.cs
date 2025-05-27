namespace oop3.Utilities
{
    internal class VectorCalculations
    {
        public static double Rad(double degrees) => degrees * Math.PI / 180;

        public static (int x, int y) getVector
            (int centerX, int centerY, int targetX, int targetY)
        {
            int vectorX = targetX - centerX;
            int vectorY = targetY - centerY;
            return (vectorX, vectorY);
        }

        public static (int x, int y) rotateVector
            (int vectorX, int vectorY, double angleRad)
        {
            int rotatedX = (int)(vectorX * Math.Cos(angleRad) - vectorY * Math.Sin(angleRad));
            int rotatedY = (int)(vectorX * Math.Sin(angleRad) + vectorY * Math.Cos(angleRad));
            return (rotatedX, rotatedY);
        }

        
    }
}
