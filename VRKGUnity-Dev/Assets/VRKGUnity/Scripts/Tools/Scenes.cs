using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Scenes
{
    public static DeviceMode DeviceMode;

    public static string Persistent => $"{DeviceMode}_{nameof(Persistent)}";
    public static string MainMenu => $"{DeviceMode}_{nameof(MainMenu)}";
    public static string DataSynchro => $"{DeviceMode}_{nameof(DataSynchro)}";
    public static string KG => $"{DeviceMode}_{nameof(KG)}";
}


public enum DeviceMode
{
    VR,
    PC
}