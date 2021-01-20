using UnityEngine;

namespace ASeKi.battle
{
    // 绘制BUFF区域
    public class BuffAreaData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public ConfigTable.BuffAreaData Config;

#if UNITY_EDITOR
        private GameObject gizmosObj = null;
#endif
        public void SetData(Vector3 Position,Quaternion Rotation,int configID)
        {
            this.Position = Position;
            this.Rotation = Rotation;
            this.Config = new ConfigTable.BuffAreaData(configID);
            Init();
        }

        public void Init()
        {
#if UNITY_EDITOR
            createGizmosObj();
#endif
        }

        public void Release()
        {
#if UNITY_EDITOR
            deleteGizmosObj();
#endif
        }

#if UNITY_EDITOR
        void createGizmosObj()
        {
            //if(Config.IsNull)
            //{
            //    return;
            //}
            //gizmosObj = new GameObject("Buff_Area_" + Config.id, typeof(ASeKi.debug.BuffAreaGizmosComponent));
            //gizmosObj.GetComponent<ASeKi.debug.BuffAreaGizmosComponent>().SetData(this);
        }

        void deleteGizmosObj()
        {
            if(gizmosObj != null)
            {
                GameObject.Destroy(gizmosObj);
            }
        }
#endif
    }
}
