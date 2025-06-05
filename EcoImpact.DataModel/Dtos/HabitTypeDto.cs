public class HabitTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Factor { get; set; } // impacto ambiental
    public string Unit { get; set; } = string.Empty;
}