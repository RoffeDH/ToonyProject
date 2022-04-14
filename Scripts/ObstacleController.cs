using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] Obstacle obstacle;
        [SerializeField] float triggerTime;
        [SerializeField] float timerOffset;
        bool cycled;
        float timer;

        // Start is called before the first frame update
        void Start()
        {
            cycled = true;
        }

        // Update is called once per frame
        void Update()
        {
            timerOffset -= Time.deltaTime;
            if (timerOffset > 0)
                return;
            //if (timer < 0 && obstacle.CanTrigger())
            //{
            //    Debug.Log("Can trigger: " + obstacle.CanTrigger());
            //    cycled = false;
            //    obstacle.Trigger();
            //}
            //else if (timer < 0 && !cycled)
            //{
            //    Debug.Log("Cycled: " + cycled);
            //    cycled = true;
            //    timer = triggerTime;
            //}
            //else
            //{
            //    //Debug.Log("Timer: " + timer);
            //    timer -= Time.deltaTime;
            //}

            if(timer < 0 && obstacle.CanTrigger())
            {
                obstacle.Trigger();
                cycled = false;
            }

            if(obstacle.CanTrigger() && !cycled)
            {
                cycled = true;
                timer = triggerTime;
            }

            if(cycled)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
