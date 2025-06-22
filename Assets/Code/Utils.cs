using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public static class Utils
    {
        public static int NextMinMax(this System.Random rand, Vector2Int minMax) => rand.Next(minMax.x, minMax.y + 1);

        public static int[] NextIndexes(this System.Random rand, int count, int maxIndex)
        {
            var allIndexes = Enumerable.Range(0, maxIndex).ToArray();
            for (var i = 0; i < maxIndex; i++)
            {
                var swap = rand.Next(0, maxIndex);
                (allIndexes[i], allIndexes[swap]) = (allIndexes[swap], allIndexes[i]);
            }

            return allIndexes.Take(count).ToArray();
        }

        public static List<T> RandomShuffle<T>(this IReadOnlyList<T> list)
        {
            var res = new List<T>(list);
            for (var i = 0; i < res.Count; i++)
            {
                var swap = Random.Range(0, res.Count);
                (res[i], res[swap]) = (res[swap], res[i]);
            }

            return res;
        }

        public static void DestroyAll<T>(this Object _, List<T> objects) where T : Component
        {
            foreach (var b in objects)
                Object.Destroy(b.gameObject);
            objects.Clear();
        }

        public static void TryKill(this Tween t, bool complete = false)
        {
            if (t is { active: true })
                t.Kill(complete);
        }

        public static int SexyPow(int b, int p) // 🍷🗿
        {
            if (p == 0)
                return 1;
            var x = SexyPow(b, p / 2);
            return x * x * ((p & 1) == 1 ? b : 1);
        }

        public static double SexyPow(double b, int p) // 😎
        {
            if (p <= 0.0)
                return 1.0;
            var x = SexyPow(b, p / 2);
            return x * x * ((p & 1) == 1 ? b : 1.0);
        }
    }
}