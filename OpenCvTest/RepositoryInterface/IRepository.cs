namespace AutomatedRodentTracker.RepositoryInterface
{
    public interface IRepository
    {
        T GetValue<T>(string valueName);
        void SetValue<T>(string valueName, T value);
        void Save();
    }
}
