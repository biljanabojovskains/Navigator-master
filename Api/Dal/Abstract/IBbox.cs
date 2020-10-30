namespace Api.Dal.Abstract
{
    public interface IBbox
    {
        double Left { get; set; }
        double Bottom { get; set; }
        double Right { get; set; }
        double Top { get; set; }
    }
}
