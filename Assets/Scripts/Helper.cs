using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public static class Helper
    {
        static Random _rng = new Random();

        /// <summary>
        /// Fisher-Yates Shuffle to mix an array
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="array">the array to shuffle</param>
        /// <returns>the shuffled array as a list</returns>
        public static List<T> YatesShuffle<T>(T[] array)
        {
            int n = array.Length;

            for (int i = 0; i < n; ++i)
            {
                int r = i + _rng.Next(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }

            return array.ToList();
        }
    }
}
