using UnityEngine;
using TMPro;
using Zenject;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class InventoryWindow : BaseWindow
{
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemPrefab;

    private List<InventoryItem> _allItems = new();
    private readonly CompositeDisposable _disposables = new();

    private DiContainer _container; // Для правильного спавна предметов
    [Inject] public void Construct(DiContainer container)
    {
        _container = container;
    }

    // Используем Awake или Start, но не забываем очистку
    private void Start()
    {
        LoadMockData();

        _searchInput.onValueChanged.AsObservable()
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOnMainThread()
            .Subscribe(query => FilterItems(query))
            .AddTo(_disposables);

        UpdateList(_allItems);
    }

    private void FilterItems(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            UpdateList(_allItems);
            return;
        }

        var lowerQuery = query.ToLower();
        var filtered = _allItems
            .Where(i => i.Name.IndexOf(lowerQuery, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();

        UpdateList(filtered);
    }

    private void UpdateList(List<InventoryItem> items)
    {
        // Очистка контейнера (в идеале тут должен быть пул объектов)
        foreach (Transform child in _itemsContainer)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            var instance = _container.InstantiatePrefab(_itemPrefab, _itemsContainer);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
        }
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

    private void LoadMockData() // Загрузим тестовые объекты
    {
        _allItems = new List<InventoryItem>
        {
            new InventoryItem { Name = "Sword" },
            new InventoryItem { Name = "Shield" },
            new InventoryItem { Name = "Potion" }
        };
    }
}
