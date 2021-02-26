using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPITestSystemInfo : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
#if UNITY_IOS
        if(SystemInfo.batteryStatus == BatteryStatus.Charging)
        {

        }
        else
        {

        }
        NetworkReachability networkReachability = Application.internetReachability;
        switch(networkReachability)
        {
            case NetworkReachability.NotReachable:
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                break;
        }
#endif
    }
}
