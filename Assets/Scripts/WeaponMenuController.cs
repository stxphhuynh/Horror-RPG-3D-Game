using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMenuController : MonoBehaviour
{
    public GameObject weaponMenu;
    public GameObject[] weapons;
    public int currentWeaponIndex = 0;

    public KeyCode toggleKey = KeyCode.I;
    // Start is called before the first frame update
    void Start()
    {
        // hidden menu
        weaponMenu.SetActive(false);
        EquipWeapon(currentWeaponIndex);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            weaponMenu.SetActive(!weaponMenu.activeSelf);
        }
    }

    public void EquipWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0) { return; }
        if (index < 0 || index >= weapons.Length) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
        currentWeaponIndex = index;
    }
}
