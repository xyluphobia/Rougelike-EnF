using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "RadialMenu/Ring", order = 1)]
public class RadialMenuRing : ScriptableObject
{
    public RadialMenuElement[] Elements;
}
