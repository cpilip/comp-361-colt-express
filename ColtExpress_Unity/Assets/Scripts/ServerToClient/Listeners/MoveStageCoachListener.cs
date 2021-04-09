using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStageCoachListener : UIEventListenable
{
    private bool atLocomotive = true;
    public override void updateElement(string data)
    {
        if (atLocomotive)
        {
            this.transform.position = new Vector3(this.transform.position.x + (-57), this.transform.position.y, this.transform.position.z);
            atLocomotive = false;
        }

        this.transform.position = new Vector3(this.transform.position.x + (-51), this.transform.position.y, this.transform.position.z);
    }

}
