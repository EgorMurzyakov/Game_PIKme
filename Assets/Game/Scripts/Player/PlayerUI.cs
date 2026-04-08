using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject boostIcon;

    public void Start()
    {
        boostIcon.SetActive(false);
    }

    public void SetHitPointUI(float _value)
    {
        hpSlider.value = _value;
    }

    public void BoostIconOn()
    {
        boostIcon.SetActive(true);
    }
    public void BoostIconOff()
    {
        boostIcon.SetActive(false);
    }
}
