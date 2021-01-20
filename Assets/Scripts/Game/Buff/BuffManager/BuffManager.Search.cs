using System.Collections.Generic;

namespace ASeKi.battle
{
    public partial class BuffManager : Singleton<BuffManager>
    {
        public bool IsExistBuffWithId(ulong entity, int buffId)
        {
            Carrier carrier = getCarrier(entity);
            return carrier.IsExistBuffWithId(entity, buffId);
        }

        public bool IsExistBuffWithType(ulong entity, int buffType)
        {
            Carrier carrier = getCarrier(entity);
            return carrier.IsExistBuffWithType(entity, buffType);
        }

        public bool IsExistBuffWithSort(ulong entity, int buffSort)
        {
            Carrier carrier = getCarrier(entity);
            return carrier.IsExistBuffWithSort(entity, buffSort);
        }

        public bool IsExistBuffWithGroup(ulong entity, int buffGroup)
        {
            Carrier carrier = getCarrier(entity);
            return carrier.IsExistBuffWithGroup(entity, buffGroup);
        }

    }
}
