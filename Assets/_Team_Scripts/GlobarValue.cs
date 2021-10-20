using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserMap
{
    MAP1,
    MAP2,
    MAP3,
}

public enum TowerType
{
    MachineGun_Tower,
    Missile_Tower,
    Emp_Tower,
    Super_MachineGun_Tower,
}

public class GlobarValue
{
    public static UserMap g_UserMap = UserMap.MAP1;
    public static bool[] g_SpawnPoint;
    public static TowerType[] g_TowerType;
    
    public static void UserMapSetting(UserMap _map)
    {
        if (_map == UserMap.MAP1)
        {
            g_SpawnPoint = new bool[4];
            g_TowerType = new TowerType[2];
        }

        else if (_map == UserMap.MAP2)
        {
            g_SpawnPoint = new bool[12];
            g_TowerType = new TowerType[6];
        }

        else if (_map == UserMap.MAP3)
            g_SpawnPoint = new bool[4];
    }
}