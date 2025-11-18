using System.Collections.Generic;
using UnityEngine;

namespace Shared.Utils
{
    public static class UVMap
    {
        public static Dictionary<int, Vector4> Generate(Vector2Int gridSize, int startIndex, int endIndex)
        {
            var map = new Dictionary<int, Vector4>();
            var totalCells = gridSize.x * gridSize.y;
            var maxIndex = Mathf.Min(endIndex, totalCells - 1);

            var cellSize = new Vector2(1f / gridSize.x, 1f / gridSize.y);

            for (var i = startIndex; i <= maxIndex; i++)
            {
                var col = i % gridSize.x;
                var row = i / gridSize.x;

                var u = col * cellSize.x;
                var v = row * cellSize.y;

                map[i] = new Vector4(1, 1, u, v);
            }

            return map;
        }
    }
}