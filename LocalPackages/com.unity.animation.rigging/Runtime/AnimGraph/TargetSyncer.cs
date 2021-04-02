using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace UnityEngine.Animations.Rigging
{
    public struct PropertyData
    {
        public int compoment;
        public int property;
        public PropertyStreamHandle handle;

        // http://www.open-std.org/jtc1/sc22/wg21/docs/papers/2014/n3876.pdf
        public static uint hash_combine(uint a, uint b)
        {
            const uint magic = 0x9e3779b9;
            a ^= b + magic + (a << 6) + (a >> 2);
            return a;
        }
    }

    public struct TargetSyncer : System.IDisposable
    {
        public NativeList<PropertyStreamHandle> propertyHandles;
        public NativeList<TransformStreamHandle> streamHandles;
        public NativeList<Vector3> positions;
        public NativeList<Vector3> localPositions;
        public NativeList<Quaternion> quaternions;
        public NativeList<Quaternion> localQuaternions;
        public NativeHashMap<int, TransformStreamHandle> mappingHandle;
        public NativeHashMap<uint, PropertyData> propertyMapping;
        public NativeList<float> propertyValues;

        public static TargetSyncer Create(int size)
        {
            return new TargetSyncer()
            {
                streamHandles = new NativeList<TransformStreamHandle>(size, Allocator.Persistent),
                positions = new NativeList<Vector3>(size, Allocator.Persistent),
                localPositions = new NativeList<Vector3>(size, Allocator.Persistent),
                quaternions = new NativeList<Quaternion>(size, Allocator.Persistent),
                localQuaternions = new NativeList<Quaternion>(size, Allocator.Persistent),
                mappingHandle = new NativeHashMap<int, TransformStreamHandle>(size, Allocator.Persistent),
                propertyMapping = new NativeHashMap<uint, PropertyData>(size, Allocator.Persistent),
                propertyHandles = new NativeList<PropertyStreamHandle>(size, Allocator.Persistent),
                propertyValues = new NativeList<float>(size, Allocator.Persistent)
            };
        }

        public void Clear()
        {
            if (streamHandles.IsCreated)
                streamHandles.Clear();
            if (propertyHandles.IsCreated)
                propertyHandles.Clear();

            if (positions.IsCreated)
                positions.Clear();
            if (localPositions.IsCreated)
                localPositions.Clear();
            if (quaternions.IsCreated)
                quaternions.Clear();
            if (localQuaternions.IsCreated)
                localQuaternions.Clear();
            if (propertyValues.IsCreated)
                propertyValues.Clear();
        }

        public void Dispose()
        {
            if (streamHandles.IsCreated)
                streamHandles.Dispose();
            if (propertyHandles.IsCreated)
                propertyHandles.Dispose();

            if (positions.IsCreated)
                positions.Dispose();
            if (localPositions.IsCreated)
                localPositions.Dispose();
            if (quaternions.IsCreated)
                quaternions.Dispose();
            if (localQuaternions.IsCreated)
                localQuaternions.Dispose();
            if (mappingHandle.IsCreated)
                mappingHandle.Dispose();

            if (propertyMapping.IsCreated)
                propertyMapping.Dispose();
            if (propertyValues.IsCreated)
                propertyValues.Dispose();
        }

        public void BindProperty(Animator animator, Component component, string property, float value)
        {
            int propertyHash = property.GetHashCode();
            int componentHash = component.GetInstanceID();
            uint hash = PropertyData.hash_combine((uint) propertyHash, (uint) componentHash);

            if (propertyMapping.TryGetValue(hash, out PropertyData data) == false)
            {
                data = new PropertyData()
                {
                    compoment = componentHash,
                    property = propertyHash,
                    handle = animator.BindStreamProperty(component.transform, component.GetType(), property)
                };
                propertyMapping.Add(hash, data);
            }

            if (data.compoment != componentHash || data.property != propertyHash)
            {
                Debug.LogError("发生hash碰撞需要调整参数");
            }

            propertyHandles.Add(data.handle);
            propertyValues.Add(value);
        }

        public void Bind(Animator animator, Transform transform)
        {
            if (mappingHandle.TryGetValue(transform.GetInstanceID(), out TransformStreamHandle handle))
            {
                streamHandles.Add(handle);
            }
            else
            {
                handle = animator.BindStreamTransform(transform);
                mappingHandle.Add(transform.GetInstanceID(), handle);
                streamHandles.Add(handle);
            }

            positions.Add(transform.position);
            localPositions.Add(transform.localPosition);
            quaternions.Add(transform.rotation);
            localQuaternions.Add(transform.localRotation);
        }

        public void Sync(ref AnimationStream stream)
        {
            for (int i = 0, count = streamHandles.Length; i < count; ++i)
            {
                var sceneHandle = streamHandles[i];
                if (!sceneHandle.IsValid(stream))
                    continue;
                sceneHandle.SetLocalPosition(stream, localPositions[i]);
                sceneHandle.SetLocalRotation(stream, localQuaternions[i]);
                sceneHandle.SetGlobalTR(stream, positions[i], quaternions[i], false);
                streamHandles[i] = sceneHandle;
            }

            for (int i = 0, count = propertyHandles.Length; i < count; ++i)
            {
                var propertyHandle = propertyHandles[i];
                if (!propertyHandle.IsValid(stream))
                    continue;

                propertyHandle.SetFloat(stream, propertyValues[i]);
            }
        }
    }
}