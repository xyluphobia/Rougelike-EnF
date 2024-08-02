using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RingElement", menuName = "RadialMenu/Element", order = 1)]
public class RadialMenuElement : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public bool SelectionAvailable;
    public RadialMenuRing NextRing;
}
