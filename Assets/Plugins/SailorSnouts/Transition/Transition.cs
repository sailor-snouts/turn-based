using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Transition : MonoBehaviour
{
    public UnityAction onFadedIn = null;
    public UnityAction onFadedOut = null;
    protected Animator anim;

    protected void Start()
    {
        this.TryGetComponent();
    }

    private void TryGetComponent()
    {
        if(!this.anim) 
            this.anim = this.GetComponent<Animator>();
    }

    public void FadeIn()
    {
        this.TryGetComponent();
        this.anim.ResetTrigger("FadeOut");
        this.anim.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        this.TryGetComponent();
        this.anim.ResetTrigger("FadeIn");
        this.anim.SetTrigger("FadeOut");
    }

    protected virtual void TriggerOnFadeIn()
    {
        this.onFadedIn?.Invoke();
    }

    protected virtual void TriggerOnFadeOut()
    {
        this.onFadedOut?.Invoke();
    }
}
