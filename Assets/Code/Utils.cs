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
    }
}