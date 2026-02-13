using Cysharp.Threading.Tasks;

public interface IWindow
{
    UniTask OpenAsync();
    UniTask CloseAsync();
}
