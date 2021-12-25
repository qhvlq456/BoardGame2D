using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloStone : ConcaveStone
{    
    public override void Awake() {
        base.Awake();
        var manager = GameObject.Find("GameManager").GetComponent<OthelloManager>();
        manager.saveStones.Add(this);
    }
    public override void Update()
    {
        base.Update();
        SetImageType();
    }
}
