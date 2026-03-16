using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        if (animator == null || agent == null) return;
        
        float speed = agent.velocity.magnitude;
        
        // ПРЯМАЯ передача по имени (без hash)
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        
        // Отладка
        if (Time.frameCount % 10 == 0)
        {
            Debug.Log($"[DEBUG] Velocity: {speed:F3} | Animator Speed: {animator.GetFloat("Speed"):F3}");
        }
    }
}