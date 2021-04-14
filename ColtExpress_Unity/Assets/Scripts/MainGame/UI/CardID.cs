using CardSpace;
using GameUnitSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardID : MonoBehaviour
{
    public bool isBulletCard = false;
    public bool isHidden = false;
    public bool playedByGhost = false;
    public ActionKind kind;
    public Character c;
}
