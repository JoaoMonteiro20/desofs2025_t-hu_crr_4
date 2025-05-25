using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using EcoImpact.DataModel.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UserChoiceService : IUserChoiceService
{
    private readonly EcoDbContext _context;
    private readonly ILogger<UserChoiceService> _logger;

    public UserChoiceService(EcoDbContext context, ILogger<UserChoiceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserChoice>> GetAllAsync() =>
        await _context.UserChoices
            .Include(uc => uc.User)
            .Include(uc => uc.HabitType)
            .ToListAsync();

    public async Task<UserChoice?> GetByIdAsync(Guid id) =>
        await _context.UserChoices
            .Include(uc => uc.User)
            .Include(uc => uc.HabitType)
            .FirstOrDefaultAsync(uc => uc.UserChoiceId == id);

        // alterado o método para calcular e adicionar o footprint
    public async Task<UserChoice> CreateAsync(UserChoiceDto dto)
    {
        var habitType = await _context.HabitTypes.FindAsync(dto.HabitTypeId);
        if (habitType == null)
            throw new Exception("HabitType não encontrado.");

        var footprint = dto.Quantity * habitType.Factor;
        var score = Math.Max(0, 100 - (footprint * 10)); // Exemplo de cálculo de EcoScore

        var choice = new UserChoice
        {
            UserChoiceId = Guid.NewGuid(),
            UserId = dto.UserId,
            HabitTypeId = dto.HabitTypeId,
            Quantity = dto.Quantity,
            Date = dto.Date,
            Footprint = Math.Round(footprint, 2)
        };

        var user = await _context.Users.FindAsync(dto.UserId);

        if (user != null)
        {
            user.EcoScore += score;
            _context.Users.Update(user);
        }


        _context.UserChoices.Add(choice);
        await _context.SaveChangesAsync();

        _logger.LogInformation("UserChoice registado: {Id}, Footprint: {Footprint}", choice.UserChoiceId, choice.Footprint);

        return choice;
    }
}
