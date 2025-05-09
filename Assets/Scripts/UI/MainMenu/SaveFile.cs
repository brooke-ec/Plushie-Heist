using Newtonsoft.Json;
using System;

public class SaveFile
{
    [JsonProperty("playtime")] private double playtime;
    public string formattedPlaytime => Math.Round(playtime / 3600, 0) + ":" + Math.Round((playtime % 3600) / 60, 0);
    public string slot;
}