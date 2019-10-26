using Org.BouncyCastle.Math.EC;

namespace HeliumParty.RadixDLT.EllipticCurve.Managers
{
    public interface IECKeyManager
    {
        /// <summary>
        ///     Generate a random key pair based on https://en.wikipedia.org/wiki/Elliptic-curve_cryptography
        /// </summary>
        /// <param name="compressed">Should the public key part be compressed? 64 vs 32 bit</param>
        /// <returns>ECKeyPair</returns>
        ECKeyPair GetRandomKeyPair(bool compressed = true);

        /// <summary>
        ///     See <see cref="GetRandomKeyPair"/>. 
        ///     Generate Keypair from a Private Key
        /// </summary>
        /// <param name="privatekey"></param>
        /// <returns>ECKeyPair</returns>
        ECKeyPair GetKeyPair(ECPrivateKey privatekey, bool compressed = true);

        /// <summary>
        ///     Verify if the keypair holds a valid private key matching a public key
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns>bool</returns>
        bool VerifyKeyPair(ECKeyPair keyPair);

        /// <summary>
        ///     Get a Elliptice Curve Signature based on https://en.wikipedia.org/wiki/Elliptic_Curve_Digital_Signature_Algorithm
        /// </summary>
        /// <param name="privateKey">The private key used for signing</param>
        /// <param name="data">Data to be signed</param>
        /// <param name="beDeterministic">Ensures a constant signature if data and privatekey are the same.</param>
        /// <param name="enforceLowS"></param>
        /// <returns>ECSignature</returns>
        ECSignature GetECSignature(ECPrivateKey privateKey, byte[] data, bool beDeterministic = false, bool enforceLowS = true);

        /// <summary>
        ///     Verify if a ECSignature derives from the supplied publicKey and data
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="signature"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool VerifyECSignature(ECPublicKey publicKey, ECSignature signature, byte[] data);

        ECPoint GetECPoint(ECPublicKey publicKey);

        byte[] CalculateMAC(byte[] salt, byte[] iv, ECPublicKey publicKey, byte[] encrypted);

        byte[] Decrypt(ECPrivateKey privateKey, byte[] data);

        byte[] Encrypt(ECPublicKey publicKey, byte[] data);

        /// <summary>
        ///     Decrypt data that is encrypted by a shared private key. 
        ///     That shared private key is encrypted by to provided keyPairs privatekey
        /// </summary>
        /// <param name="keyPair"></param>
        /// <param name="sharedKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Decrypt(ECKeyPair keyPair, EncryptedPrivateKey sharedKey, byte[] data);
    }
}