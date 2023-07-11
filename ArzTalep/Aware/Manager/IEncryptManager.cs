namespace Aware.Manager
{
    public interface IEncryptManager
    {
        string Decrypt(string cipherText);
        string Encrypt(string clearText);
    }
}