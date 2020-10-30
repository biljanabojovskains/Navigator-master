namespace Navigator.Models.Abstract
{
    public interface ILegenda
    {
        int Id { get; set; }
        int OpfatId { get; set; }
        int TipNaPodatokId { get; set; }
        string Path { get; set; }
    }
}