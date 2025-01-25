using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ButtonType
{
    Start,
    Quit
}

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ButtonType _buttonType;
    [SerializeField] private Button _button;

    private void OnValidate()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            switch (_buttonType)
            {
                case ButtonType.Start:
                    SceneManager.LoadScene("Scenes/Main");
                    break;
                case ButtonType.Quit:
                    Debug.Log("bye bye :)");
                    Application.Quit();
                    break;
            }
        });
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Tween.ScaleX(transform, 1.1f, 0.25f, Ease.InOutCubic);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Tween.ScaleX(transform, 1f, 0.25f, Ease.InOutCubic);
    }
}