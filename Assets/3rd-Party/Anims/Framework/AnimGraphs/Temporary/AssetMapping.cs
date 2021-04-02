using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AssetMapping : ScriptableObject
    {
        private static AssetMapping mapping;

        public static void Init(AssetMapping m)
        {
            //mapping = m;

            if (mapping == null)
            {
                mapping = new AssetMapping();
            }

            if (mapping.clips == null)
            {
                mapping.clips = m.clips;
            }
            else
            {
                mapping.clips = mapping.clips.Concat(m.clips).ToArray();
                mapping.clipMapping.Clear();
                mapping.clipMapping = null;
                mapping.Init();
            }

            if (mapping.avatars == null)
            {
                mapping.avatars = m.avatars;
            }
            else
            {
                mapping.avatars = mapping.avatars.Concat(m.avatars).ToArray();
                mapping.avatarMapping.Clear();
                mapping.avatarMapping = null;
                mapping.Init();
            }
        }

        [System.Serializable]
        public struct AnimationClipAsset
        {
            public AnimationClip clip;
            public int hash;
        }

        [System.Serializable]
        public struct AvatarMaskAsset
        {
            public AvatarMask avatarMask;
            public int hash;
        }

        private void Init()
        {
            if (clipMapping == null)
            {
                clipMapping = new Dictionary<int, AnimationClip>();
                for (int i = 0; i < clips.Length; i++)
                {
                    AddIfNotContains(clipMapping, clips[i].hash, clips[i].clip);
                }
            }

            if (avatarMapping == null)
            {
                avatarMapping = new Dictionary<int, AvatarMask>();
                for (int i = 0; i < avatars.Length; i++)
                {
                    if (avatars[i].hash == 0)
                        continue;

                    //可能多个角色共用avatar了
                    AddIfNotContains(avatarMapping, avatars[i].hash, avatars[i].avatarMask);
                }
            }
        }

        public AnimationClipAsset[] clips;
        public AvatarMaskAsset[] avatars;

        public static AnimationClip GetAnimationClip(int hash)
        {
            mapping.Init();
            AnimationClip clip = null;
            mapping.clipMapping.TryGetValue(hash, out clip);
            return clip;
        }

        public static AvatarMask GetAvatarMask(int hash)
        {
            mapping.Init();
            AvatarMask avatar = null;
            mapping.avatarMapping.TryGetValue(hash, out avatar);
            return avatar;
        }

        private Dictionary<int, AnimationClip> clipMapping;
        private Dictionary<int, AvatarMask> avatarMapping;


        public static void AddIfNotContains<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
            {
                return;
            }
            else
            {
                source.Add(key, value);
            }
        }
    }


}