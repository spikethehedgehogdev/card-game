using System;
using UnityEngine;

namespace Shared.Structs
{
    [Serializable]
    public struct Socket
    {
        public Socket(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Vector3 Position { get; }

        public Quaternion Rotation { get; }

        public Socket CopyWithOffset(Vector3 offset)
        {
            return new Socket(Position + Rotation * offset, Rotation);
        }
    }
}