namespace Game.Interfaces
{
    public interface IContextView<T>
    {
        T Context { get; }
        void UpdateView();
    }
}