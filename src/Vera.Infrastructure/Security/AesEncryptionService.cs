using System.Security.Cryptography;
using System.Text;
using Vera.Domain.Interfaces;

namespace Vera.Infrastructure.Security;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(string encryptionKey)
    {
        // In production, load from Azure Key Vault
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
    }

    public Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(plainText))
            return Task.FromResult(string.Empty);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV(); // Generate a random IV for each encryption

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        
        // Prepend IV to the ciphertext
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
        
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encrypted = msEncrypt.ToArray();
        return Task.FromResult(Convert.ToBase64String(encrypted));
    }

    public Task<string> DecryptAsync(string cipherText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(cipherText))
            return Task.FromResult(string.Empty);

        var buffer = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;
        
        // Extract IV from the beginning of the ciphertext
        var iv = new byte[aes.BlockSize / 8];
        Array.Copy(buffer, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var msDecrypt = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return Task.FromResult(srDecrypt.ReadToEnd());
    }
}
