using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Animator anim;
    public CinemachineBrain theBrain;

    public void Shake()
    {   
        theBrain.enabled = false;
        anim.SetTrigger("shake");
    }

    public void StopShake()
    {
        theBrain.enabled = true;
    }
}
