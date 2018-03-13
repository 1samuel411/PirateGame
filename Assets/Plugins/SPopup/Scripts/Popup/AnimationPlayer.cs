using UnityEngine;
using System.Collections;

public class AnimationPlayer : MonoBehaviour
{

    private Animator animatior;
    private new Animation animation;

    public string startAnimation;
    public bool playStartAnimation;

    public enum Type { Mecanim, Legacy };
    public Type type;

    public void Start()
    {
        if (!playStartAnimation)
            return;

        Debug.Log("Startanim");
        if (type == Type.Mecanim)
        {
            if (!this.animatior)
                this.animatior = GetComponent<Animator>();

            this.animatior.Play(startAnimation);
        }

        if (type == Type.Legacy)
        {
            if (!this.animation)
                this.animation = GetComponent<Animation>();

            this.animation.Play(startAnimation);
        }
    }

    public void PlayAnimation(string animation)
    {
        if (type == Type.Mecanim)
        {
            if (!this.animatior)
                this.animatior = GetComponent<Animator>();

            this.animatior.Play(animation);
        }

        if (type == Type.Legacy)
        {
            if (!this.animation)
                this.animation = GetComponent<Animation>();

            this.animation.Play(animation);
        }
    }
}
