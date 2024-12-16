using UnityEngine;

namespace Utils
{
    public static class QuaternionUtils
    {
        public static Quaternion GetRandomHexRotation()
        {
            var degrees = 60 * Random.Range(0, 6);
            var rotation = Quaternion.Euler(0, degrees, 0);
            return rotation;
        } 
    }
}