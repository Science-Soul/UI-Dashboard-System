using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] UIConfig _uiConfig;
    public override void InstallBindings()
    {
        Container.Bind<WindowService>().AsSingle().NonLazy();
        Container.BindInstance(_uiConfig).AsSingle();
    }
}
