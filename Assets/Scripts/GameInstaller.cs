using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WindowService>().AsSingle().NonLazy();

        // TODO: добавить еще контейнер
    }
}
