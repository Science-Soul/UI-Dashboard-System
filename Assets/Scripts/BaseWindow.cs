using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

[RequireComponent(typeof(CanvasGroup))]
public class BaseWindow : MonoBehaviour, IWindow
{
    [SerializeField] private float _fadeDuration = 0.3f;
    private CanvasGroup _canvasGroup;

    protected CancellationToken DestroyToken => this.GetCancellationTokenOnDestroy();

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public virtual async UniTask OpenAsync()
    {
        _canvasGroup.DOKill();
        gameObject.SetActive(true);

        await _canvasGroup.DOFade(1, _fadeDuration)
            .SetUpdate(true) // Чтобы работало даже при TimeScale = 0.
            .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, DestroyToken); // Чтобы UniTask не пытался работать при уничтожении объекта.
    }

    public virtual async UniTask CloseAsync()
    {
        await _canvasGroup.DOFade(0, _fadeDuration)
            .SetUpdate(true) // Чтобы работало даже при TimeScale = 0.
            .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, DestroyToken);

        gameObject.SetActive(false);
    }
}
