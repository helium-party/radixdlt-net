using System.IO;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Encryption;

namespace HeliumParty.RadixDLT.Identity
{
    public interface IRadixIdentityManager
    {
        IECKeyManager KeyManager { get; set; }

        IRadixIdentity CreateNew();
        KeyStore CreateStore(LocalExposedRadixIdentity localIdentity, string password);
        IRadixIdentity FromPrivateKeyBase64(string privateKey);
        IRadixIdentity LoadKeyStore(KeyStore store, string password);
        IRadixIdentity LoadOrCreateEncryptedFile(FileInfo file, string password);
        IRadixIdentity LoadOrCreateEncryptedFile(string file, string password);
        IRadixIdentity LoadOrCreateFile(FileInfo file);
        IRadixIdentity LoadOrCreateFile(string file);
    }
}