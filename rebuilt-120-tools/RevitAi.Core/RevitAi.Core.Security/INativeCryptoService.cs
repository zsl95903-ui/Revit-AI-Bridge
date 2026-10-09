namespace RevitAi.Core.Security;

public interface INativeCryptoService
{
	string GetSupabaseUrl();

	string GetHardwareFingerprint();

	string GetHardwareFingerprintV2();

	string GetHardwareFingerprintV1();

	byte[] EncryptData(byte[] plaintext, byte[] key, byte[] iv);

	byte[] DecryptData(byte[] ciphertext, byte[] key, byte[] iv);

	byte[] DeriveKeyFromPassword(string password, byte[] salt, int iterations, int keyLength);

	byte[] ComputeSha256(byte[] data);

	byte[] GenerateRandomIV();

	byte[] GenerateRandomSalt();

	byte[] GenerateRandomBytes(int length);

	byte[] EncryptString(string plaintext, byte[] key, byte[] iv);

	string DecryptString(byte[] ciphertext, byte[] key, byte[] iv);

	string GetDefaultMasterKey();
}
