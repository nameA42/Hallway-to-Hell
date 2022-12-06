using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInToBlack : MonoBehaviour
{
    public void fadeInToBlack()
    {
        this.GetComponent<Animation>().Play();
    }
}
