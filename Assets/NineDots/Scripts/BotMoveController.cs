using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NineDots
{
    public class BotMoveController : MonoBehaviour
    {
        Animator myAnimator;

        // Start is called before the first frame update
        void Start()
        {
            //get the animator component
            myAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        //enter clap state
        public void MakeClap()
        {
            //reset cheering trigger to ensure it is false
            myAnimator.ResetTrigger("StartCheeringTrig");
            //trigger clapping
            myAnimator.SetTrigger("StartClappingTrig");
        }

        //enter cheering state
        public void MakeCheer()
        {
            //reset clapping trigger to ensure it is false
            myAnimator.ResetTrigger("StartClappingTrig");
            //start cheering
            myAnimator.SetTrigger("StartCheeringTrig");
        }

        //reset to idle state
        public void ResetState()
        {
            //reset back to idle each frame
            myAnimator.SetBool("answerCorrect", false);
            myAnimator.SetBool("CheeringNow", false);
        }
    }
}
