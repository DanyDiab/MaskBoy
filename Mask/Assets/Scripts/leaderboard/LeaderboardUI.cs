using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Transform contentRoot;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] int maxRows = 150;

    [Header("Top 3 Slots (optional, no scroll list needed)")]
    [SerializeField] TextMeshProUGUI firstText;
    [SerializeField] TextMeshProUGUI secondText;
    [SerializeField] TextMeshProUGUI thirdText;
    [SerializeField] bool fillTop3Slots = true;

    [Header("Optional")]
    [SerializeField] bool clearExistingChildren = true;

    void Start()
    {
        AutoBindTop3IfNeeded();
        Render();
    }

    public void Render()
    {
        List<LeaderboardStore.Entry> entries = LeaderboardStore.Load();

        if (fillTop3Slots)
        {
            AutoBindTop3IfNeeded();
            SetTop3(entries);
        }

        // If you're only using the 3 fixed slots, you can leave these unassigned.
        if (contentRoot == null || rowPrefab == null) return;

        if (clearExistingChildren)
        {
            for (int i = contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }
        }

        int count = Mathf.Min(maxRows, entries.Count);

        for (int i = 0; i < count; i++)
        {
            LeaderboardStore.Entry e = entries[i];
            GameObject row = Instantiate(rowPrefab, contentRoot);
            TextMeshProUGUI text = row.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                int rank = i + 1;
                text.text = $"<b>{rank,3}.  {e.initials}   {e.wave}</b>";
            }
        }
    }

    void SetTop3(List<LeaderboardStore.Entry> entries)
    {
        SetSlot(firstText, entries, 0, 1);
        SetSlot(secondText, entries, 1, 2);
        SetSlot(thirdText, entries, 2, 3);
    }

    void AutoBindTop3IfNeeded()
    {
        // If you have scene objects named "First", "Second", "Third" (as in your screenshot),
        // this will automatically grab the TextMeshProUGUI component on them or their children.
        if (firstText == null) firstText = FindSlotText("First");
        if (secondText == null) secondText = FindSlotText("Second");
        if (thirdText == null) thirdText = FindSlotText("Third");
    }

    static TextMeshProUGUI FindSlotText(string objName)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null) return null;
        return go.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    static void SetSlot(TextMeshProUGUI slot, List<LeaderboardStore.Entry> entries, int index, int rank)
    {
        if (slot == null) return;

        if (entries != null && index >= 0 && index < entries.Count && entries[index] != null)
        {
            var e = entries[index];
            slot.text = $"<b>{rank}. {e.initials}  {e.wave}</b>";
        }
        else
        {
            slot.text = $"<b>{rank}. ---</b>";
        }
    }
}

