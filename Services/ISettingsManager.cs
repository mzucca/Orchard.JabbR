using Orchard;

namespace JabbR.Services
{
    public interface ISettingsManager : IDependency
    {
        ApplicationSettings Load();
        void Save(ApplicationSettings settings);
    }
}
