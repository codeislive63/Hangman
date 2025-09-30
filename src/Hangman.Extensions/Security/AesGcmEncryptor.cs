using System.Security.Cryptography;
using System.Text;

namespace Hangman.Extensions.Security;

/// <summary>Обёртка над AES-GCM: шифрование/дешифрование Base64 с AAD</summary>
public static class AesGcmEncryptor
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const byte FormatV1 = 0x01;

    private static AesGcm CreateGcm(byte[] key)
    {
#if NET9_0_OR_GREATER
        return new AesGcm(key, TagSize);
#else
#pragma warning disable SYSLIB0053
        return new AesGcm(key);
#pragma warning restore SYSLIB0053
#endif
    }

    /// <summary>Шифрует plaintext в Base64 с заданным ключом и AAD.</summary>
    public static string EncryptToBase64(string plaintext, byte[] key, ReadOnlySpan<byte> aad = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
        {
            throw new ArgumentException("AES key must be 16, 24, or 32 bytes.", nameof(key));
        }

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var pt = Encoding.UTF8.GetBytes(plaintext);
        var ct = new byte[pt.Length];
        var tag = new byte[TagSize];

        using var gcm = CreateGcm(key);
        gcm.Encrypt(nonce, pt, ct, tag, aad);

        var blob = new byte[1 + NonceSize + ct.Length + TagSize];
        blob[0] = FormatV1;

        Buffer.BlockCopy(nonce, 0, blob, 1, NonceSize);
        Buffer.BlockCopy(ct, 0, blob, 1 + NonceSize, ct.Length);
        Buffer.BlockCopy(tag, 0, blob, 1 + NonceSize + ct.Length, TagSize);

        return Convert.ToBase64String(blob);
    }

    /// <summary>Дешифрует Base64 в plaintext с заданным ключом и AAD</summary>
    public static string DecryptFromBase64(string base64, byte[] key, ReadOnlySpan<byte> aad = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length is not 16 and not 24 and not 32)
        {
            throw new ArgumentException("AES key must be 16, 24, or 32 bytes.", nameof(key));
        }

        var blob = Convert.FromBase64String(base64);

        if (blob.Length < 1 + NonceSize + TagSize)
        {
            throw new CryptographicException("Ciphertext is too short.");
        }

        if (blob[0] != FormatV1)
        {
            throw new CryptographicException($"Unsupported ciphertext format version: {blob[0]}.");
        }

        var nonce = blob.AsSpan(1, NonceSize);
        var ct = blob.AsSpan(1 + NonceSize, blob.Length - (1 + NonceSize + TagSize));
        var pt = new byte[ct.Length];
        var tag = blob.AsSpan(blob.Length - TagSize, TagSize);

        using var gcm = CreateGcm(key);
        gcm.Decrypt(nonce, ct, tag, pt, aad);

        return Encoding.UTF8.GetString(pt);
    }
}
