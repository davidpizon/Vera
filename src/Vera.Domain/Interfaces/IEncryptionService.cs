namespace Vera.Domain.Interfaces;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string cipherText, CancellationToken cancellationToken = default);
}
