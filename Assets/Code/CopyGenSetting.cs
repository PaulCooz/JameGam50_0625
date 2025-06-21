using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JamSpace
{
    [CreateAssetMenu(fileName = "CommandGenSetting", menuName = "Setting/Command", order = 0)]
    public sealed class CopyGenSetting : ScriptableObject
    {
        [SerializeField]
        private Vector2Int minMaxWidth;
        [SerializeField]
        private Vector2Int minMaxHeight;
        [SerializeField]
        private Vector2Int minMaxOpPerRow;

        public LevelData Get(System.Random rand)
        {
            var width = rand.NextMinMax(minMaxWidth);
            var height = rand.NextMinMax(minMaxHeight);
            var grid = new List<Operator[]>();
            for (var i = 0; i < height; i++)
            {
                var row = new Operator[width];
                for (var j = 0; j < width; j++)
                    row[j] = Operator.Empty;

                var ops = rand.NextMinMax(minMaxOpPerRow);
                var indexes = rand.NextIndexes(ops, width);
                foreach (var index in indexes)
                    row[index] = OperatorExt.GetRandNonEmpty(rand, index, width);

                grid.Add(row);
            }

            var timeToSolve = Utils.SexyPow(2, width) * OperatorExt.Count * width * height;
            Debug.Log($"TIME:{timeToSolve}");
            return new(timeToSolve, width, height, grid.SelectMany(r => r).ToArray());
        }
    }
}