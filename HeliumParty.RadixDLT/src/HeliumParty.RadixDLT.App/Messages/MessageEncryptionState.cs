using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Messages
{
    public enum MessageEncryptionState
    {
        /**
         * Specifies that the data in the DecryptedMessage object WAS originally
         * encrypted and has been successfully decrypted to it's present byte array.
         */
        DECRYPTED,
        /**
		 * Specifies that the data in the DecryptedMessage object was NOT
		 * encrypted and the present data byte array just represents the original data.
		 */
        NOT_ENCRYPTED,
        /**
		 * Specifies that the data in the DecryptedMessage object WAS encrypted
		 * but could not be decrypted. The present data byte array represents the still
		 * encrypted data.
		 */
        CANNOT_DECRYPT,
    }
}
