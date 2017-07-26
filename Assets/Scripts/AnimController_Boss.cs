using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController_Boss : MonoBehaviour {

    private Animator animator;
    private float timer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        timer = 0f;
    }
    // Use this for initialization
 //   void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
