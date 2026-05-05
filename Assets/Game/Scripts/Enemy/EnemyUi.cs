using UnityEngine;
using UnityEngine.UI;

public class EnemyUi : MonoBehaviour
{
    [SerializeField] protected Slider hpSlider;
    protected Transform canvasTransform;
    protected Camera mainCamera;

    public virtual void Start()
    {
        hpSlider.value = 1f;
        mainCamera = Camera.main;
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvasTransform = canvas.transform;
        }
    }

    void LateUpdate()
    {
        if (canvasTransform != null && mainCamera != null)
        {
            // Поворачиваем Canvas
            canvasTransform.LookAt(canvasTransform.position + mainCamera.transform.rotation * Vector3.forward,
                                   mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void SetHitPointUI(float _curHP, float _maxHP)
    {
        hpSlider.value = _curHP / _maxHP;
    }

    public void DeleteHpSlider()
    {
        canvasTransform.gameObject.SetActive(false);
    }

}
