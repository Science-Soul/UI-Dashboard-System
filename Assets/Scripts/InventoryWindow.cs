using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class InventoryWindow : BaseWindow
{
    [SerializeField] Button _closeButton;
    [SerializeField] TMP_InputField _searchInput;
    [SerializeField] Transform _itemsContainer;

    private List<InventoryItem> _allItems = new();
    private readonly CompositeDisposable _disposables = new();

    private DiContainer _container;
    private UIConfig _uiConfig;
    private WindowService _windowService;

    [Inject]
    public void Construct(DiContainer container, UIConfig config, WindowService windowService)
    {
        _container = container;
        _uiConfig = config;
        _windowService = windowService;
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

        _closeButton.onClick.AddListener(() => CloseSelf().Forget());
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
            var instance = _container.InstantiatePrefab(_uiConfig.ItemPrefab, _itemsContainer);
            instance.GetComponent<InventoryItemView>().Setup(item.Name);
        }
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

    private async UniTaskVoid CloseSelf()
    {
        await _windowService.CloseWindow<InventoryWindow>();
    }

    private void LoadMockData() // Загрузим тестовые объекты
    {
        _allItems = new List<InventoryItem>
        {
            new InventoryItem { Name = "Sword of Destiny" },
            new InventoryItem { Name = "Iron Shield" },
            new InventoryItem { Name = "Health Potion" },
            new InventoryItem { Name = "Mana Potion" },
            new InventoryItem { Name = "Golden Ring" },
            new InventoryItem { Name = "Sword of Air" },
            new InventoryItem { Name = "Wooden Shield" },
            new InventoryItem { Name = "Red Axe" },
            new InventoryItem { Name = "Ebony Bow" },
            new InventoryItem { Name = "Silver Ring" }
        };
    }
}
