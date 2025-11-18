using System;
using System.Collections.Generic;
using Shared.Math;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.World
{
    public interface ICurve
    {
        List<Socket> GetLayout(int count, float gap);
    }

    public class BezierCurve : ICurve, IDisposable
    {
        private readonly Vector3[] _pointsOnCurve = new Vector3[4];
        private readonly Vector3[] _pointsOnRotatedCurve = new Vector3[4];
        
        private Quaternion _rotation;
        
        public BezierCurve(
            Socket root, 
            float edgeOffsetX, 
            Vector2 middleOffset)
        {
            UpdatePoints(root, edgeOffsetX, middleOffset);
        }

        public void UpdatePoints(Socket root, float edgeOffsetX, Vector2 middleOffset)
        {
            var points = new Vector3[] {
                new (-edgeOffsetX, 0, 0),
                new(-middleOffset.x, 0, middleOffset.y),
                new(middleOffset.x, 0, middleOffset.y),
                new(edgeOffsetX, 0, 0),
            };
            
            for (var i = 0; i < points.Length; ++i)
                _pointsOnCurve[i] = root.Position + points[i];
            for (var i = 0; i < points.Length; ++i)
                _pointsOnRotatedCurve[i] = root.Position + root.Rotation * points[i];
            
            _rotation = root.Rotation;
        }

        public List<Socket> GetLayout(int count, float gap)
        {
            if (count < 1)
                throw new Exception($"[BezierCurve] The required count must be greater than 0.");
            
            var standard = new List<Socket>();
            
            var tempLength = (count - 1) * gap;
            
            Func<int, float> getGap = (tempLength < 1.0f) ? 
                GetTByPresetGap : 
                GetTByDynamicGap;
            
            for (var i = 0; i < count; i++)
            {
                var t = getGap(i);
                
                var pos = GetPoint(t);
                var rot = GetRotation(t);
                
                
                
                standard.Add(new Socket(pos, rot));
            }
            
            return standard;

            float GetTByPresetGap(int index) => index * gap + 0.5f -tempLength / 2.0f;
            float GetTByDynamicGap(int index) => index * (1.0f / (count - 1.0f));
        }

        private Quaternion GetRotation(float t)
        {
            var direction = GetTangent(t);
            var baseRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
            
            const float yaw = 80f;
            var rotatedY = Quaternion.AngleAxis(yaw, Vector3.up) * baseRot;

            var rootRotation = _rotation;
            return rootRotation * rotatedY;

        }

        private Vector3 GetPoint(float t) =>
            Bezier.GetPoint(
                _pointsOnRotatedCurve[0], 
                _pointsOnRotatedCurve[1], 
                _pointsOnRotatedCurve[2], 
                _pointsOnRotatedCurve[3], 
                t);
        private Vector3 GetTangent(float t) =>
            Bezier.GetTangent(
                _pointsOnCurve[0],
                _pointsOnCurve[1],
                _pointsOnCurve[2],
                _pointsOnCurve[3], 
                t);
        
        
        public void Dispose() { }
        
#if UNITY_EDITOR
        public void DrawGizmos(int count, float gap)
        {
            var segments = GetLayout(count, gap);
            
            Gizmos.color = Color.yellow;
            for (var i = 1; i < segments.Count; i++) 
                Gizmos.DrawLine(segments[i].Position, segments[i-1].Position);
        }
        
#endif
    }
}