public interface ISaveSystem
{
    public void Save<T>(T saveData);
    public T Load<T>();
}
