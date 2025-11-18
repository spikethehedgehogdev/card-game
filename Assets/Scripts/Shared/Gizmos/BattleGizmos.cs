using Shared.Generated;
using Shared.Structs;
using UnityEngine;

namespace Shared.Gizmos
{
    public static class BattleGizmos
    {

        public static void DrawPile(Transform socket, float height, Color color)
        {
            DrawBlock(new Vector3(CardInfo.Size.x, CardInfo.Size.y, height),socket.position + new Vector3(0, height/2, 0), socket.rotation, color);
        }
        public static void DrawFakeCard(Transform socket, Color color)
        {
            DrawFakeCard(socket.position, socket.rotation, color);
        }
        public static void DrawFakeCard(Socket socket, Color color)
        {
            DrawFakeCard(socket.Position, socket.Rotation, color);
        }
        public static void DrawFakeCard(Vector3 position, Quaternion rotation, Color color)
        {
            DrawBlock(CardInfo.Size, position, rotation, color);
        }


        private static void DrawBlock(Vector3 size, Vector3 position, Quaternion rotation, Color color)
        {
            UnityEngine.Gizmos.color = color;

            var half = size * 0.5f;

            // 8 вершин параллелепипеда в локальных координатах
            var vertices = new Vector3[]
            {
                new(-half.x, -half.y, -half.z),
                new(half.x, -half.y, -half.z),
                new(half.x, -half.y, half.z),
                new(-half.x, -half.y, half.z),

                new(-half.x, half.y, -half.z),
                new(half.x, half.y, -half.z),
                new(half.x, half.y, half.z),
                new(-half.x, half.y, half.z),
            };

            // применяем поворот и позицию
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * vertices[i] + position;
            }

            // низ
            UnityEngine.Gizmos.DrawLine(vertices[0], vertices[1]);
            UnityEngine.Gizmos.DrawLine(vertices[1], vertices[2]);
            UnityEngine.Gizmos.DrawLine(vertices[2], vertices[3]);
            UnityEngine.Gizmos.DrawLine(vertices[3], vertices[0]);

            // верх
            UnityEngine.Gizmos.DrawLine(vertices[4], vertices[5]);
            UnityEngine.Gizmos.DrawLine(vertices[5], vertices[6]);
            UnityEngine.Gizmos.DrawLine(vertices[6], vertices[7]);
            UnityEngine.Gizmos.DrawLine(vertices[7], vertices[4]);

            // вертикали
            UnityEngine.Gizmos.DrawLine(vertices[0], vertices[4]);
            UnityEngine.Gizmos.DrawLine(vertices[1], vertices[5]);
            UnityEngine.Gizmos.DrawLine(vertices[2], vertices[6]);
            UnityEngine.Gizmos.DrawLine(vertices[3], vertices[7]);
            
        }
    }
}