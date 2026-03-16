using UnityEngine;
using UnityEngine.AI;

public class DebugAgent : MonoBehaviour
{
    void OnGUI()
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            GUILayout.Label("❌ NavMeshAgent НЕ НАЙДЕН");
            return;
        }

        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.BeginVertical("box");
        
        // БЕЗОПАСНАЯ проверка: сначала isOnNavMesh, потом остальные свойства
        if (!agent.isOnNavMesh)
        {
            GUILayout.Label("⚠️ Агент НЕ на NavMesh!");
            GUILayout.Label($"📍 Позиция: {transform.position}");
            GUILayout.Label("💡 Решение: опустите NPC на синюю сетку");
            GUILayout.EndVertical();
            GUILayout.EndArea();
            return; // НЕ вызываем остальные свойства, если не на навмеше!
        }
        
        GUILayout.Label($"✅ On NavMesh: {agent.isOnNavMesh}");
        GUILayout.Label($"⏸️ Is Stopped: {agent.isStopped}");
        GUILayout.Label($"🎯 Has Path: {agent.hasPath}");
        GUILayout.Label($"📏 Remaining: {agent.remainingDistance:F2}");
        GUILayout.Label($"⚡ Velocity: {agent.velocity.magnitude:F2}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}