using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUiItem : MonoBehaviour
{
    [SerializeField] private Text textName = null;
    public void UpdateData(WeaponData data)
    {
        textName.text = data.name;
    }
}
