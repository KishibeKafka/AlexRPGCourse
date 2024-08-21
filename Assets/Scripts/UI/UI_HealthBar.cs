using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private RectTransform myTransform;
    private Slider slider;
    private CharacterStats myStats;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats =GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += updateHealthUI;

        updateHealthUI();
    }
    private void updateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }
    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }


    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= updateHealthUI;
    }
}
