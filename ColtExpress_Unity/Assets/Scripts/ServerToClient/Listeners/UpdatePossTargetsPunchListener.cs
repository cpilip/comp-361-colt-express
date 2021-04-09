using Coffee.UIEffects;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePossTargetPunchListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /*
         * List<Character> targets
         * bool shotgunCanBePunched
         * updatePossTargetPunch
         */

        JObject o = JObject.Parse(data);
        List<Character> targets = o.SelectToken("targets").ToObject<List<Character>>();
        bool shotgunCanBePunched = o.SelectToken("shotgunCanBePunched").ToObject<bool>();
        string availableTargets = "";

        foreach (Character c in targets)
        {
            availableTargets += (c.ToString() + " ");

            GameObject target = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(0).gameObject;
            target.GetComponent<UIShiny>().enabled = true;
            target.GetComponent<Button>().enabled = true;
            target.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseTargetPunch);

        }

        if (shotgunCanBePunched)
        {
            availableTargets += (Character.Shotgun.ToString() + " ");
            GameUIManager.gameUIManagerInstance.togglePunchShotgunButton(true);
        }

        GameUIManager.gameUIManagerInstance.unlockSidebar();

        Debug.Log("[UpdatePossTargetsListener] Targets are: " + availableTargets);
    }
}
