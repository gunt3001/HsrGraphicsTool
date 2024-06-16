namespace HsrGraphicsTool.Models.EnumUtils;

public record ValueDescription(object Value, object Description)
{
    public override string ToString()
    {
        return (string)Description;
    }
}