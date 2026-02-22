using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    public void SetHitPointUI(float _value)
    {
        hpSlider.value = _value;
    }
}
