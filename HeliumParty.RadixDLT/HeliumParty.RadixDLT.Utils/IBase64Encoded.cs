namespace HeliumParty.RadixDLT
{
    public interface IBase64Encoded
    {
        string Base64 { get; }

        byte[] Base64Array { get; }
    }
}