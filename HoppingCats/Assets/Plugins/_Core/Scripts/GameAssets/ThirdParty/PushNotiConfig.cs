using System;
using System.Collections.Generic;

[Serializable]
public class PushNotificationConfig
{
    public int hourToPush = 8; // 8AM & 8PM
    public int days = 7;
    // public int interval = 12 * 60 * 60 * 1000; // 1/2 day
    public List<string> contents = new();
    // int maxPush = 14;
    // public int ingameDelay = 4 * 60 * 60; // seconds
}