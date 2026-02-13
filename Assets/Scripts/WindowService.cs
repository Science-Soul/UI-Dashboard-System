using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class WindowService
{
    private readonly DiContainer _container;
    private readonly Dictionary<string, IWindow> _openedWindows = new();

    // Список окон, которые в данный момент находятся в процессе открытия
    private readonly HashSet<string> _loadingWindows = new();

    public WindowService(DiContainer container)
    {
        _container = container;
    }

    public async UniTask<T> OpenWindow<T>(string prefabPath) where T : IWindow
    {
        string windowId = typeof(T).Name;

        // Если окно уже открыто, то просто возвращаем его
        if (_openedWindows.TryGetValue(windowId, out var openedWindow))
            return (T)openedWindow;

        if (!_loadingWindows.Add(windowId))
        {
            return default;
        }

        try
        {
            // Загружаем префаб из ресурсов (здесь можно было бы использовать Adressables)
            var prefab = await Resources.LoadAsync<GameObject>(prefabPath) as GameObject;
            if (prefab == null) throw new Exception($"Префаб не найден в {prefabPath}");

            var windowInstance = _container.InstantiatePrefabForComponent<T>(prefab);
            _openedWindows.Add(windowId, windowInstance);

            await windowInstance.OpenAsync();
            return windowInstance;
        }
        finally
        {
            _loadingWindows.Remove(windowId);
        }
    }

    public async UniTask CloseWindow<T>() where T: IWindow
    {
        string windowId = typeof(T).Name;

        if (_openedWindows.TryGetValue(windowId, out var window))
        {
            _openedWindows.Remove(windowId); // Удаляем из словаря сразу, чтобы нельзя было вызвать закрытие несколько раз

            await window.CloseAsync();

            var mono = window as MonoBehaviour;
            if (mono != null && mono.gameObject != null)
            {
                UnityEngine.Object.Destroy(mono.gameObject);
            }
        }
    }
}
