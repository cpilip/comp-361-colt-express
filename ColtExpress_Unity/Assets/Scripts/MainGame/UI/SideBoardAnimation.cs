using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoardAnimation : MonoBehaviour
{
    public GameObject SideBoard;
    public GameObject sideboardBlocker;
    public void ShowHideBoard() {
        if (SideBoard != null) {
            Animator animator = SideBoard.GetComponent<Animator>();
            if (animator != null) {
                bool isOpen = animator.GetBool("showBoard");
                animator.SetBool("showBoard", !isOpen);
            }

            bool isEnabled = sideboardBlocker.active;
            if (isEnabled)
            {
                sideboardBlocker.SetActive(false);
            } else
            {
                sideboardBlocker.SetActive(true);
            }
        }
    }
}
