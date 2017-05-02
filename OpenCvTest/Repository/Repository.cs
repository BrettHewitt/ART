using AutomatedRodentTracker.RepositoryInterface;

namespace AutomatedRodentTracker.Repository
{
    public class Repository : IRepository
    {
        public T GetValue<T>(string valueName)
        {
            return (T)Properties.Settings.Default[valueName];
        }

        public void SetValue<T>(string valueName, T value)
        {
            Properties.Settings.Default[valueName] = value;
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
