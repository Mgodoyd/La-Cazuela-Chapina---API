using Api.DTOs;
using Api.Models;
using Api.Interface;  // Aquí está IRepository<T>
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class VoiceCommandService
{
    private readonly IRepository<ComandoVoz> _repository;

    public VoiceCommandService(IRepository<ComandoVoz> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ComandoVoz>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ComandoVoz?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(VoiceCommandDto dto)
    {
        // Validaciones manuales
        if (dto.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.");

        if (string.IsNullOrWhiteSpace(dto.TranscribedText))
            throw new ArgumentException("TranscribedText is required.");

        if (string.IsNullOrWhiteSpace(dto.Intent))
            throw new ArgumentException("Intent is required.");

        if (dto.Date > DateTime.UtcNow)
            throw new ArgumentException("Date cannot be in the future.");

        var entity = new ComandoVoz
        {
            UserId = dto.UserId,
            TranscribedText = dto.TranscribedText,
            Intent = dto.Intent,
            Date = dto.Date == default ? DateTime.UtcNow : dto.Date
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
    }
}
