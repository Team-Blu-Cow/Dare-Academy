using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

[RequireComponent(typeof(Animator))]
public class GridEntityAnimationController : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private GameObject m_sprite;

    public float animatorSpeed;

    private void OnValidate()
    {
        m_animator = GetComponent<Animator>();
        Transform[] ts = GetComponentsInChildren<Transform>();
        bool spriteExists = false;
        foreach (var t in ts)
        {
            if (t.name == "sprite")
            {
                spriteExists = true;
                m_sprite = t.gameObject;
                break;
            }
        }

        if(!spriteExists)
        {
            m_sprite = new GameObject("sprite");
            m_sprite.AddComponent<Transform>();
            m_sprite.AddComponent<SpriteRenderer>();

            m_sprite.transform.parent = this.transform;
            m_sprite.transform.position = Vector3.zero;
        }
    }

    private void Start()
    {
        animatorSpeed =  1f/App.GetModule<LevelModule>().StepController.stepTime;
        m_animator.speed = animatorSpeed;
    }

    public void PlayAnimation(string animationName, float time)
    {
        if (m_animator.runtimeAnimatorController == null)
            return;

        if (!m_animator.HasState(0, Animator.StringToHash(animationName)))
            return;
        m_animator.speed = 1f/time;

        m_animator.Play(animationName, -1, 0f);
    }

    private void Update()
    {
        //if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        //{
        //    m_animator.speed = 1;
        //}

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            m_animator.Play("walk");
        }
    }
}
