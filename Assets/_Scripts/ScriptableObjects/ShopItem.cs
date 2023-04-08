using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="new_item", menuName ="ScriptableObject/ShopItem", order =1)]
public class ShopItem : ScriptableObject
{
    public Sprite Sprite;
    public int Cost;
}
