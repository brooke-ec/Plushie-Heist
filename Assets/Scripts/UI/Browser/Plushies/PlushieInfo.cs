using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlushieInfo
{
    public string plushieName;
    public Sprite plushieIcon;
    public List<SkillTreeUnlockable> unlockableSkills;

    [HideInInspector] public bool unlocked = false;
    [HideInInspector] public int plushieNumber = 0;
}
