using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject boostIconUI;
    //[SerializeField] private TMP_Text hpUI;

    public void Start()
    {
        boostIconUI.SetActive(false);
    }

    public void SetHitPointUI(float _curHP, float _maxHP)
    {
        //hpUI.text = _curHP.ToString();
        hpSlider.value = _curHP / _maxHP;
    }

    public void BoostIconOn()
    {
        boostIconUI.SetActive(true);
    }
    public void BoostIconOff()
    {
        boostIconUI.SetActive(false);
    }
}
