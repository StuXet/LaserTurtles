using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementalModifiers
{
    None,
    Sweet,
    Sour,
    Salty,
    Spicy,
    Cool
}

[CreateAssetMenu(menuName = "Scriptable Object/ElementalModifierData")]
public class ElementalModifierData : ScriptableObject
{
    public Sprite[] ElementalIcons;
}
