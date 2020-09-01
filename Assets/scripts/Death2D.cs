using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death2D : MonoBehaviour{
    [SerializeField]Transform spawn;

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player")){
            collision.transform.position = spawn.position;
        }
    }
}
