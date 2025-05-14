using EcoImpact.API.Services;
using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly EcoDbContext _context;
    private readonly IPasswordService _passwordService;

    public UserService(EcoDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Password = _passwordService.HashPassword(user, user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(Guid id, User updatedUser)
    {
        if (id != updatedUser.UserId) return false;

        _context.Entry(updatedUser).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FindAsync(id);

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
