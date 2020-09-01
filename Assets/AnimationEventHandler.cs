using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Animator animator;

    public void Landing(string name){
        FindObjectOfType<AudioManager>().Play("playerLand");


        
    }
}
