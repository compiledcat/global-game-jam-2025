using PrimeTween;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector3.zero;
        Tween.Scale(transform, 1f, 0.5f, Ease.OutBack);
    }
}
