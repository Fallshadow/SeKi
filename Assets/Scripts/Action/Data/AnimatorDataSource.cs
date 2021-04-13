using System.Collections.Generic;

namespace ASeKi.action.Data
{
    public class AnimatorDataSource
    {
        public ActionRoleType Type;

        public Dictionary<int, RuntimeAction> runtimeActions;

        private int counter = 0;

        public AnimatorDataSource(ActionRoleType Type, int capacity = 64)
        {
            this.Type = Type;
            
            runtimeActions = new Dictionary<int, RuntimeAction>(capacity);
        }

        public Dictionary<int, RuntimeAction> GetDatas()
        {
            counter++;
            return runtimeActions;
        }
        
        public void Add(int id, RuntimeAction data)
        {
            runtimeActions.AddIfNotContains(id, data);
        }

        public bool Destroy()
        {
            counter--;

            if (counter > 0)
            {
                debug.PrintSystem.Log("other still use RuntimeAction Map");
                return false;
            }
            
            Dictionary<int, RuntimeAction>.Enumerator itor = runtimeActions.GetEnumerator();
            while (itor.MoveNext())
            {
                itor.Current.Value.Dispose();
            }
            itor.Dispose();
            runtimeActions.Clear();
            runtimeActions = null;
            return true;
        }

        public void ForceDestroy()
        {
            Dictionary<int, RuntimeAction>.Enumerator itor = runtimeActions.GetEnumerator();
            while (itor.MoveNext())
            {
                itor.Current.Value.Dispose();
            }
            itor.Dispose();
            runtimeActions.Clear();
            runtimeActions = null;
        }
    }
}