using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Skinning/SkinData")]
public class SkinData : ScriptableObject
{
    [SerializeField]
    List<SkinSlot> skin_slots;

    public bool are_slots_valid
    {
        get
        {
            return skin_slots != null && skin_slots.Count == Enum.GetValues(typeof(SkinTag)).Length;
        }
    }

    // used by Skinning class
    public Color GetSkin(SkinTag tag)
    {
        if(skin_slots == null || skin_slots.Count < 0)
        {
            Debug.LogError("<b>[SkinData]</b> : Skin slots are not setup");
            return Color.magenta;
        }

        SkinSlot slot = skin_slots.Find((item) => { return tag == item.tag; });

        if(slot != null)
            return slot.color;
        else
            return Color.black;
    }

    public void FixSlots()
    {
        if(skin_slots == null)
            skin_slots = new List<SkinSlot>();

        int index = 0;

        foreach (SkinTag tag in Enum.GetValues(typeof(SkinTag)))
        {
            SkinSlot slot = skin_slots.Find((item) => { return item.tag == tag; });

            if(slot == null)
                skin_slots.Insert(index, new SkinSlot(tag, Color.black));

            index++;
        }
    }

    [Serializable]
    public class SkinSlot
    {
        public SkinTag tag;
        public Color color;

        public SkinSlot(SkinTag tag, Color color)
        {
            this.tag = tag;
            this.color = color;
        }
    }
}