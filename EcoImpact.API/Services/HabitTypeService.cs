﻿using EcoImpact.DataModel.Models;
using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using EcoImpact.API.Mapper;

public class HabitTypeService : IHabitTypeService
{
    private readonly EcoDbContext _context;
    private readonly ILogger<HabitTypeService> _logger;
    private readonly IHabitTypeMapper _mapper;

    public HabitTypeService(EcoDbContext context, ILogger<HabitTypeService> logger,IHabitTypeMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HabitTypeDto>> GetAllAsync()
    {
        _logger.LogInformation("Obter todos os HabitTypes");

        var habits = await _context.HabitTypes.ToListAsync();
        return habits.Select(_mapper.ToDto);
    }

    public async Task<HabitTypeDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscar HabitType com ID: {Id}", id);

        var habit = await _context.HabitTypes.FindAsync(id);
        if (habit == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado", id);
            return null;
        }

        return _mapper.ToDto(habit);
    }

    public async Task<HabitType> CreateAsync(HabitTypeDto dto)
    {
        _logger.LogInformation("Criar HabitType: {Name}", dto.Name);

        var habit = _mapper.ToEntity(dto);

        _context.HabitTypes.Add(habit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType criado com sucesso. ID: {Id}", habit.HabitTypeId);

        return habit;
    }

    public async Task<HabitTypeDto?> UpdateAsync(Guid id, HabitTypeDto dto)
    {
        _logger.LogInformation("Atualizar HabitType com ID: {Id}", id);

        var existing = await _context.HabitTypes.FindAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado para atualização", id);
            return null;
        }

        existing.Name = dto.Name;
        existing.Unit = dto.Unit;
        existing.Factor = dto.Factor;

        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType com ID {Id} atualizado com sucesso", id);

        return _mapper.ToDto(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Tentando eliminar HabitType com ID: {Id}", id);

        var habit = await _context.HabitTypes.FindAsync(id);
        if (habit == null)
        {
            _logger.LogWarning("HabitType com ID {Id} não encontrado para eliminação", id);
            return false;
        }

        _context.HabitTypes.Remove(habit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("HabitType com ID {Id} eliminado com sucesso", id);
        return true;
    } 

    public async Task<string> ImportFromFileAsync(IFormFile file)
    {
        if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Formato de ficheiro inválido. Apenas ficheiros .json são permitidos.");

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();

        var habitDtos = JsonSerializer.Deserialize<List<HabitTypeDto>>(content);
        if (habitDtos == null || habitDtos.Count == 0)
            throw new ArgumentException("O ficheiro está vazio ou contém dados inválidos.");

        var habitEntities = habitDtos.Select(dto => _mapper.ToEntity(dto)).ToList();

        _context.HabitTypes.AddRange(habitEntities);
        await _context.SaveChangesAsync();

        return $"{habitEntities.Count} habit types imported successfully.";
    }

    public async Task<List<HabitTypeDto>> GetRandomQuizHabitTypesAsync(int count = 3)
    {
        return await _context.HabitTypes
            .OrderBy(h => Guid.NewGuid())
            .Take(count)
            .Select(h => new HabitTypeDto
            {
                Id = h.HabitTypeId,
                Name = h.Name,
                Factor = h.Factor,
                Unit = h.Unit
            })
            .ToListAsync();
    }

    public async Task<List<HabitTypeDto>> GetQuizHabitTypesByCategoryAsync()
    {
        var allHabits = await _context.HabitTypes.ToListAsync();

        var selected = allHabits
            .GroupBy(h => h.Category)
            .SelectMany(g => g.OrderBy(_ => Guid.NewGuid()).Take(1)) // 1 por categoria
            .ToList();

        return selected.Select(h => new HabitTypeDto
        {
            Id = h.HabitTypeId,
            Name = h.Name,
            Unit = h.Unit,
            Factor = h.Factor
        }).ToList();
    }
}
