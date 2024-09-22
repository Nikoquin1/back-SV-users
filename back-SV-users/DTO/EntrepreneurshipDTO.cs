public class EntrepreneurshipDTO
{
    public int Id { get; set; }
    public int Id_user { get; set; }
    public int Id_plan { get; set; }
    public required string Name { get; set; }
    public required string Logo { get; set; }
    public int NIT { get; set; }
    public required string Description { get; set; }
    public required string UserName { get; set; } 
}