using System.Text;
using TMPro;
using UnityEngine;

public class FloorSelectDebuffUI : MonoBehaviour
{
    [SerializeField] private TMP_Text debuffText;

    private void Start()
    {
        var stacks = ScreenManager.Instance != null ? ScreenManager.Instance.ActiveDebuffs : default;
        var text = new StringBuilder();

        AppendDebuff(text, "Movement Slow", stacks.Slow);
        AppendDebuff(text, "Reduced Jump Power", stacks.JumpPowerReduction);
        AppendDebuff(text, "Reduced Jump Count", stacks.JumpCountReduction);
        AppendDebuff(text, "Faster Falling", stacks.FallAcceleration);

        debuffText.text = text.Length > 0 ? text.ToString() : "No active debuffs";
    }

    private static void AppendDebuff(StringBuilder text, string name, int stacks)
    {
        if (stacks <= 0) return;
        if (text.Length > 0) text.AppendLine();
        text.Append(name).Append("   <color=#FFC483>×").Append(stacks).Append("</color>");
    }
}
