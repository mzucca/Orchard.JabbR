using Orchard;
namespace JabbR.Services
{
    public interface IKeyProvider : ISingletonDependency
    {
        byte[] EncryptionKey { get; }
        byte[] VerificationKey { get; }
    }
}