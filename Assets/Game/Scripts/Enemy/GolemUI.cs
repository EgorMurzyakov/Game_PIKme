using UnityEngine;
using UnityEngine.UI;

public class GolemUI : EnemyUi
{
    [SerializeField] private Slider hpSliderB;
    [SerializeField] private Slider hpSliderC;

    public override void Start()
    {
        hpSlider.value = 1f;
        hpSliderB.value = 1f;
        hpSliderC.value = 1f;

        mainCamera = Camera.main;
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvasTransform = canvas.transform;
        }
    }

    public void SetHitPointUI(float _curHP, float _maxHP, float _second, float _third)
    {
        if (_curHP > _second)
            hpSlider.value = Mathf.Max(_curHP - _second, 0) / (_maxHP - _second);
        else if (_curHP > _third)
        {
            hpSlider.value = 0f;
            hpSliderB.value = Mathf.Max(_curHP - _third, 0) / (_second - _third);
        }
        else
        {
            hpSlider.value = 0f;
            hpSliderB.value = 0f;
            hpSliderC.value = _curHP / _third;
        }
    }

}
