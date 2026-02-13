using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameLauncher : MonoBehaviour
{
    private WindowService _windowService;

    [Inject]
    public void Construct(WindowService windowService)
    {
        _windowService = windowService;
    }

    public async void StartGame()
    {
        await UniTask.Delay(500);

        var inventory = await _windowService.OpenWindow<InventoryWindow>("Prefabs/InventoryWindow");

        Debug.Log("Инвентарь готов к работе!");
    }
}
