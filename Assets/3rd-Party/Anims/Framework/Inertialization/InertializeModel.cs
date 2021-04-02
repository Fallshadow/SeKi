using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Framework
{
    public static class InertializeModel
    {
        static void SyncSide(ref quaternion quat, quaternion side)
        {
            if (dot(side, quat) < 0)
                quat.value = -quat.value;
        }

        public static float4 toAxisAngle(quaternion quat)
        {
            float4 q1 = quat.value;

            if (q1.w > 1)
            {
                math.normalize(q1);
            }

            float angle = 2 * math.acos(q1.w);
            float s = math.sqrt(1 - q1.w * q1.w);

            if (s < 0.001f)
            {
                return math.float4(q1.x, q1.y, q1.z, angle);
            }
            else
            {
                return math.float4(q1.x / s, q1.y / s, q1.z / s, angle);
            }
        }
        
        static float INERTIALIZATION_TIME_EPSILON = 1.0e-7f;

        public static float Inertialize(float x0, float v0, float tf, float t)
        {
            float tf1 = -5 * x0 / v0;

            if (tf1 > 0)
            {
                tf = min(tf, tf1);
            }

            if (tf < 0.00001f)
            {
                return 0f;
            }

            t = min(t, tf);

            float tf2 = tf * tf;
            float tf3 = tf2 * tf;
            float tf4 = tf3 * tf;
            float tf5 = tf4 * tf;

            float a0 = (-8 * v0 * tf - 20 * x0) / (tf * tf);

            float A = -(a0 * tf2 + 6 * v0 * tf + 12 * x0) / (2 * tf5);
            float B = (3 * a0 * tf2 + 16 * v0 * tf + 30 * x0) / (2 * tf4);
            float C = -(3 * a0 * tf2 + 12 * v0 * tf + 20 * x0) / (2 * tf3);

            float t2 = t * t;
            float t3 = t2 * t;
            float t4 = t3 * t;
            float t5 = t4 * t;

            return A * t5 + B * t4 + C * t3 + (a0 / 2) * t2 + v0 * t + x0;
        }

        public static float Inertialize(float prev, float curr, float target, float dt, float tf, float t)
        {
            float x0 = curr - target;
            float v0 = (curr - prev) / dt;

            return Inertialize(x0, v0, tf, t);
        }

        public static float3 InertializeMagnitude(float3 prev, float3 curr, float3 target, float dt, float tf, float t)
        {
            float3 vx0 = curr - target;
            float3 vxn1 = prev - target;

            float x0 = length(vx0);

            float3 vx0_dir = x0 > 0.00001f ? (vx0 / x0) : length(vxn1) > 0.00001f ? normalize(vxn1) : float3(1, 0, 0);

            float xn1 = dot(vxn1, vx0_dir);
            float v0 = (x0 - xn1) / dt;

            float xt = Inertialize(x0, v0, tf, t);

            float3 vxt = xt * vx0_dir + target;

            return vxt;
        }

        public static float3 InertializeDirect(float3 prev, float3 curr, float3 target, float dt, float tf, float t)
        {
            return target + float3(
                Inertialize(prev.x, curr.x, target.x, dt, tf, t),
                Inertialize(prev.y, curr.y, target.y, dt, tf, t),
                Inertialize(prev.z, curr.z, target.z, dt, tf, t));
        }

        public static quaternion InertializeMagnitude(quaternion prev, quaternion curr, quaternion target, float dt, float tf, float t)
        {
            if (length(target) < 0.0001f)
                target = quaternion(0, 0, 0, 1);
            if (length(curr) < 0.0001f)
                curr = quaternion(0, 0, 0, 1);
            if (length(prev) < 0.0001f)
                prev = quaternion(0, 0, 0, 1);


            quaternion q0 = normalize(mul(curr, inverse(target)));
            quaternion qn1 = normalize(mul(prev, inverse(target)));

            float4 q0_aa = toAxisAngle(q0);

            float3 vx0 = q0_aa.xyz;
            float x0 = q0_aa.w;

            float xn1 = 2 * atan(dot(qn1.value.xyz, vx0) / qn1.value.w);

            float v0 = (x0 - xn1) / dt;

            float xt = Inertialize(x0, v0, tf, t);
            quaternion qt = mul(Unity.Mathematics.quaternion.AxisAngle(vx0, xt), target);

            return normalize(qt);
        }

        public static quaternion InertializeDirect(quaternion prev, quaternion curr, quaternion target, float dt, float tf, float t)
        {
            SyncSide(ref prev, target);
            SyncSide(ref curr, target);

            return normalize(target.value + float4(
                Inertialize(prev.value.x, curr.value.x, target.value.x, dt, tf, t),
                Inertialize(prev.value.y, curr.value.y, target.value.y, dt, tf, t),
                Inertialize(prev.value.z, curr.value.z, target.value.z, dt, tf, t),
                Inertialize(prev.value.w, curr.value.w, target.value.w, dt, tf, t)));
        }
    }
}
