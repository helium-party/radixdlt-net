using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HeliumParty.RadixDLT.App.Identity
{
    public class RadixIdentityManager
    {
        protected IECKeyManager _keyManager;
        
        public IECKeyManager KeyManager
        {
            get
            {
                if (_keyManager == null)
                    _keyManager = new ECKeyManager();
                return _keyManager;
            }
            set
            {
                _keyManager = value;
            }
        }


        /// <summary>
        ///     Create a radix identity from base64 encoded privatekey
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public IRadixIdentity FromPrivateKeyBase64(string privateKey)
        {
            var ecPrivKey = new ECPrivateKey(RadixConstants.StandardEncoding.GetBytes(privateKey));
            return GetIdentity(ecPrivKey);
        }

        /// <summary>
        ///     Generate a new Radix Identity that is not yet stored anywhere.
        /// </summary>
        /// <returns></returns>
        public IRadixIdentity CreateNew()
        {
            return new LocalRadixIdentity(_keyManager.GetRandomKeyPair());
        }

        /// <summary>
        ///     Load up a private key from a file. If the file does not exist it will create a file with a random private key
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IRadixIdentity LoadOrCreateFile(FileInfo file)
        {
            if (file.Exists)
            {
                var bytes = File.ReadAllBytes(file.FullName);
                if (bytes.Length != 32)
                    throw new ApplicationException($"File{file.FullName} does not contain a valid key");

                var ecPrivKey = new ECPrivateKey(bytes);
                return GetIdentity(ecPrivKey);
            } else
            {
                using (var stream = File.Create(file.FullName))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var pair = _keyManager.GetRandomKeyPair();
                        writer.Write(pair.PrivateKey.Base64Array);
                        writer.Close();

                        return new LocalRadixIdentity(pair);
                    }
                }
            }
        }

        /// <summary>
        ///     Load up a private key from a file. If the file does not exist it will create a file with a random private key
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IRadixIdentity LoadOrCreateFile(string file)
        {
            var info = new FileInfo(file);
            return LoadOrCreateFile(info);
        }

        protected virtual IRadixIdentity GetIdentity(ECPrivateKey privKey)
        {
            return new LocalRadixIdentity(_keyManager.GetKeyPair(privKey));
        }
    }
}
