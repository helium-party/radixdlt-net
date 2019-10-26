using HeliumParty.DependencyInjection;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Encryption;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Serialization.Dson;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HeliumParty.RadixDLT.Identity
{
    public class RadixIdentityManager : IRadixIdentityManager , ITransientDependency
    {
        private readonly IECKeyManager _keyManager;
        private readonly IEUIDManager _euidManager;
        private readonly IDsonManager _dsonManager;

        public RadixIdentityManager(IECKeyManager keyManager, IEUIDManager euidManager, IDsonManager dsonManager)
        {
            _keyManager = keyManager;
            _euidManager = euidManager;
            _dsonManager = dsonManager;
        }



        /// <summary>
        ///     Create a radix identity from base64 encoded privatekey
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public virtual IRadixIdentity FromPrivateKeyBase64(string privateKey)
        {
            var ecPrivKey = new ECPrivateKey(Convert.FromBase64String(privateKey));
            return GetIdentity(ecPrivKey);
        }

        /// <summary>
        ///     Generate a new Radix Identity that is not yet stored anywhere.
        /// </summary>
        /// <returns></returns>
        public virtual IRadixIdentity CreateNew()
        {
            return new LocalRadixIdentity(_keyManager, _euidManager, _dsonManager, _keyManager.GetRandomKeyPair());
        }

        /// <summary>
        ///     Load up a private key from a file. If the file does not exist it will create a file with a random private key
        ///     Warning : unencrypted
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual IRadixIdentity LoadOrCreateFile(FileInfo file)
        {
            if (file.Exists)
            {
                var bytes = File.ReadAllBytes(file.FullName);
                if (bytes.Length != 32)
                    throw new ApplicationException($"File{file.FullName} does not contain a valid key");

                var ecPrivKey = new ECPrivateKey(bytes);
                return GetIdentity(ecPrivKey);
            }
            else
            {
                using (var stream = File.Create(file.FullName))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var pair = _keyManager.GetRandomKeyPair();
                        writer.Write(pair.PrivateKey.Base64Array);
                        writer.Close();

                        return new LocalRadixIdentity(_keyManager, _euidManager, _dsonManager, pair);
                    }
                }
            }
        }

        /// <summary>
        ///     Load up a private key from a file. If the file does not exist it will create a file with a random private key
        ///     Warning : unencrypted
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual IRadixIdentity LoadOrCreateFile(string file)
        {
            var info = new FileInfo(file);
            return LoadOrCreateFile(info);
        }

        protected virtual IRadixIdentity GetIdentity(ECPrivateKey privKey)
        {
            return new LocalRadixIdentity(_keyManager, _euidManager, _dsonManager, _keyManager.GetKeyPair(privKey));
        }

        /// <summary>
        ///     This will load or create a json serialized KeyStore object.
        ///     Wich in turn contains the encrypted privatekey
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual IRadixIdentity LoadOrCreateEncryptedFile(FileInfo file, string password)
        {
            if (file.Exists)
            {
                var jsonstr = File.ReadAllText(file.FullName);
                var store = JsonConvert.DeserializeObject<KeyStore>(jsonstr);

                return LoadKeyStore(store, password);
            }
            else
            {
                var keyPair = _keyManager.GetRandomKeyPair();
                var store = PrivateKeyEncryptor.Encrypt(password, keyPair.PrivateKey);
                var jsonStr = JsonConvert.SerializeObject(store, Formatting.Indented);

                File.WriteAllText(file.FullName, jsonStr);

                return new LocalRadixIdentity(_keyManager, _euidManager, _dsonManager, keyPair);
            }
        }

        /// <summary>
        ///     This will load or create a json serialized KeyStore object.
        ///     Wich in turn contains the encrypted privatekey
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual IRadixIdentity LoadOrCreateEncryptedFile(string file, string password)
        {
            var info = new FileInfo(file);
            return LoadOrCreateEncryptedFile(info, password);
        }

        public virtual IRadixIdentity LoadKeyStore(KeyStore store, string password)
        {
            var privKey = PrivateKeyEncryptor.Decrypt(password, store);
            var keypair = _keyManager.GetKeyPair(privKey);

            return new LocalRadixIdentity(_keyManager,_euidManager, _dsonManager, keypair);
        }

        public virtual KeyStore CreateStore(LocalExposedRadixIdentity localIdentity, string password)
        {
            var pair = localIdentity.KeyPair;

            return PrivateKeyEncryptor.Encrypt(password, pair.PrivateKey);
        }

    }
}
