using System;
using System.Collections.Generic;
using UnityEngine;

public static class LeaderboardStore
{
    const string PlayerPrefsKey = "leaderboard.v1";

    [Serializable]
    public class Entry
    {
        public string initials;
        public int wave;
        public long timestampUtc;
    }

    [Serializable]
    class EntryList
    {
        public List<Entry> entries = new List<Entry>();
    }

    public static List<Entry> Load()
    {
        string json = PlayerPrefs.GetString(PlayerPrefsKey, "");
        if (string.IsNullOrEmpty(json)) return new List<Entry>();

        try
        {
            EntryList list = JsonUtility.FromJson<EntryList>(json);
            return list?.entries ?? new List<Entry>();
        }
        catch
        {
            return new List<Entry>();
        }
    }

    public static void AddScore(string initials, int wave, int keepTop = 150)
    {
        if (string.IsNullOrWhiteSpace(initials)) return;
        initials = initials.Trim().ToUpperInvariant();
        if (initials.Length != 3) return;

        List<Entry> entries = Load();
        entries.Add(new Entry
        {
            initials = initials,
            wave = wave,
            timestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        });

        entries.Sort((a, b) =>
        {
            int waveCmp = b.wave.CompareTo(a.wave); // descending
            if (waveCmp != 0) return waveCmp;
            return a.timestampUtc.CompareTo(b.timestampUtc); // older first
        });

        if (entries.Count > keepTop) entries.RemoveRange(keepTop, entries.Count - keepTop);

        EntryList wrapper = new EntryList { entries = entries };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
    }
}

