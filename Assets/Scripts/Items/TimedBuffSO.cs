using System.Collections.Generic;
using UnityEngine;

public enum BuffGroup { None, AttackBuff, SpeedBuff, DefenseBuff } // để chống chồng buff cùng nhóm (tuỳ chọn)
public enum BuffStackRule { Stack, ReplaceGroup, IgnoreIfActive }

[CreateAssetMenu(menuName = "RPG/Items/Timed Buff")]
public class TimedBuffSO : ItemSO
{
    [Header("Buff Effects")]
    public List<StatModifier> effects;   // đặt duration > 0
    public BuffGroup group = BuffGroup.None;
    public BuffStackRule stackRule = BuffStackRule.Stack;

    private void Reset() { kind = ItemKind.TimedBuff; }

    public void UseOn(PlayerStats target)
    {
        if (!target) return;

        // (tuỳ chọn) xử lý quy tắc chồng buff theo group
        if (group != BuffGroup.None && stackRule != BuffStackRule.Stack)
        {
            // xoá buff cũ cùng group nếu rule = ReplaceGroup
            // (Cách nhanh: lọc theo duration>0 và StatType trùng nhau; hoặc bạn có thể thêm 'sourceId' vào StatModifier để nhận diện gốc)
            if (stackRule == BuffStackRule.ReplaceGroup)
            {
                foreach (var s in target.stats)
                {
                    for (int i = s.modifiers.Count - 1; i >= 0; i--)
                    {
                        // ví dụ đơn giản: coi như mọi modifier có duration>0 thuộc "buff"
                        if (s.modifiers[i].duration > 0f) s.modifiers.RemoveAt(i);
                    }
                }
            }
            // IgnoreIfActive: bạn có thể tự kiểm tra “đang có buff nhóm này” và bỏ qua (cần thêm metadata để nhận diện)
        }

        target.AddModifiers(effects); // PlayerStats sẽ tự đếm ngược và gỡ
    }
}
