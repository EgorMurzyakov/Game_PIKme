using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class GoatControl : MonoBehaviour
{
    private enum GoatState { Idle, Action, Walk }
    private AudioSource audioSrc => GetComponent<AudioSource>();
    [SerializeField] private Transform[] targetPoints;
    private Transform target;
    private CharacterController charControl;
    private NavMeshAgent agent;
    private Animator animator;
    private GoatState currentState = GoatState.Idle;
    private bool canChageState = false;
    private Vector3 gravity;

    //[SerializeField] private AudioClip[] soundLists;
    [SerializeField] private AudioSource soundBeeee;
    [SerializeField] private AudioSource soundFootstep;
    //private float startPlaySound;
    //private int randomPeriodicity;    

    public void Start()
    {
        target = targetPoints[0];
        charControl = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        gravity = new Vector3(0f, -9.8f, 0f);
        //startPlaySound = Time.time;
        //randomPeriodicity = Random.Range(10, 50);
        PlayAnimation();
    }
    public void Update()
    {
        charControl.Move(gravity * Time.deltaTime);

        if (currentState == GoatState.Walk)
        {
            CheckDistance(); // ��������� ��������� �� ����
            GoToNextPoint(); // ��������� ��������
        }

        if (canChageState) 
        {
            UpdateState(); // ������ ��������� ��� �����������
        }

        //PlaySound();
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(transform.position, target.position) < 1.5f)
        {
            canChageState = true;
        }        
    }

    private void UpdateState()
    {
        int randomValue = Random.Range(0, 4);

        switch (currentState)
        {
            case GoatState.Idle:
                if (randomValue < 2)
                {
                    currentState = GoatState.Action;
                }
                else
                {
                    currentState = GoatState.Walk;
                    target = targetPoints[Random.Range(0, targetPoints.Length)];
                    GoToNextPoint();
                }                
                break;

            case GoatState.Action:
                if (randomValue < 2)
                {
                    currentState = GoatState.Action;
                }
                else if (randomValue == 2)
                {
                    currentState = GoatState.Idle;
                }
                else
                {
                    currentState = GoatState.Walk;
                    target = targetPoints[Random.Range(0, targetPoints.Length)];
                    GoToNextPoint();
                }
                break;

            case GoatState.Walk:
                if (randomValue < 2)
                {
                    currentState = GoatState.Action;
                }
                else
                {
                    currentState = GoatState.Idle;
                }
                break;
        }

        canChageState = false;
        PlayAnimation();
        Debug.Log(currentState);
    }

    private void GoToNextPoint()
    {
        // 1. ��������� ���� ������
        //target = targetPoints[Random.Range(0, targetPoints.Length)];
        agent.SetDestination(target.position);

        // 2. �������� ����������� �� ������
        Vector3 direction = agent.desiredVelocity;

        // 3. ����������� � ��������� ����� CharacterController
        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            // ��������
            charControl.Move(direction * 2f * Time.deltaTime);
        }
    }

    private void PlayAnimation()
    {
        animator.ResetTrigger("goIdle");
        animator.ResetTrigger("goWalk");
        animator.ResetTrigger("goAction");

        switch (currentState)
        {
            case GoatState.Idle:
                animator.SetTrigger("goIdle");
                break;
            case GoatState.Action:
                animator.SetTrigger("goAction");
                break;
            case GoatState.Walk:
                animator.SetTrigger("goWalk");
                break;
        }
    }

    private void PlaySound(int _i)
    {
        //if (Time.time > startPlaySound + randomPeriodicity)
        //{
        //    audioSrc.PlayOneShot(soundLists[0], 0.8f);
        //    startPlaySound = Time.time;
        //    randomPeriodicity = Random.Range(10, 50);
        //}
        float vol = 1f;
        switch (_i)
        {
            case 0:
                soundBeeee.Play();
                break;
            case 1:
                soundFootstep.Play();
                break;
        }
        //audioSrc.PlayOneShot(soundLists[_i], vol);
    }

    private void SetChangeStateTrue() // �������� �� ������������ �������
    {
        canChageState = true;
    }
}
