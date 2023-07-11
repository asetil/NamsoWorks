namespace Worchart.BL.Encryption
{
    public interface IEncryptManager
    {
        string Decrypt(string cipherText);
        string Encrypt(string clearText);
    }
}