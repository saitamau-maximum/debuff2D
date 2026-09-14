using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class HighScoreStore
{
    private const string PlayerKey = "HighScores.CurrentPlayerId";
    public static string SavePath => Path.Combine(Application.persistentDataPath, "high-scores.json");

    public static string CurrentPlayerId
    {
        get
        {
            string id = PlayerPrefs.GetString(PlayerKey, "");
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString("N");
                SelectPlayer(id);
            }
            return id;
        }
    }

    public static void SelectPlayer(string playerId)
    {
        if (string.IsNullOrWhiteSpace(playerId)) throw new ArgumentException("Player ID is required.", nameof(playerId));
        PlayerPrefs.SetString(PlayerKey, playerId);
        PlayerPrefs.Save();
    }

    [Serializable]
    private class SaveData
    {
        public List<Entry> entries = new List<Entry>();
    }

    [Serializable]
    private class Entry
    {
        public string playerId;
        public int floor;
        public int score;
    }

    private static SaveData Read()
    {
        if (!File.Exists(SavePath)) return new SaveData();
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        if (data == null || data.entries == null)
            throw new InvalidDataException("Invalid high score save file.");
        return data;
    }

    public static bool TryGetBest(string playerId, int floor, out int score)
    {
        ValidateKey(playerId, floor);
        var entry = Read().entries.Find(e => e != null && e.playerId == playerId && e.floor == floor);
        score = entry == null ? 0 : entry.score;
        return entry != null;
    }

    public static bool SaveIfHigher(string playerId, int floor, int score)
    {
        ValidateKey(playerId, floor);
        var data = Read();
        var entry = data.entries.Find(e => e != null && e.playerId == playerId && e.floor == floor);
        if (entry != null && score <= entry.score) return false;
        if (entry == null)
        {
            entry = new Entry { playerId = playerId, floor = floor };
            data.entries.Add(entry);
        }
        entry.score = score;
        Directory.CreateDirectory(Application.persistentDataPath);
        string temporaryPath = SavePath + ".tmp";
        File.WriteAllText(temporaryPath, JsonUtility.ToJson(data, true));
        if (File.Exists(SavePath)) File.Replace(temporaryPath, SavePath, null);
        else File.Move(temporaryPath, SavePath);
        return true;
    }

    private static void ValidateKey(string playerId, int floor)
    {
        if (string.IsNullOrWhiteSpace(playerId)) throw new ArgumentException("Player ID is required.", nameof(playerId));
        if (floor < 1) throw new ArgumentOutOfRangeException(nameof(floor));
    }
}
