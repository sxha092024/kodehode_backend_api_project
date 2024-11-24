namespace Structs;
public struct UploadAsset 
{
    public int Id;
    public byte[] Data;
    public string ContentType;
    public string Name;
    public Guid Guid;

    public override string ToString()
    {
        return $"UploadAsset{{ Id {Id} Data [{Data.Length}] ContentType {ContentType} Name {Name} Guid{Guid} }}";
    }
}