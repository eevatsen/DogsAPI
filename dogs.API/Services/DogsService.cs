using dogs.Models;
using dogs.Data;
using Microsoft.EntityFrameworkCore;

namespace dogs.Services;

public class DogService
{
    private readonly DogsDbContext _context;

    public DogService(DogsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dog>> GetDogsAsync(DogsQueryParams queryParams)
    {
        var query = _context.Dogs.AsQueryable();

        // sort 
        if (!string.IsNullOrEmpty(queryParams.Attribute))
        {
            query = SortDogs(query, queryParams.Attribute, queryParams.Order);
        }

        // pagination
        query = query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize);

        return await query.ToListAsync();
    }

    public async Task<bool> DogNameExistsAsync(string name)
    {
        return await _context.Dogs.AnyAsync(d => d.Name == name);
    }

    public async Task<Dog> CreateDogAsync(CreateDogRequest request)
    {
        var dog = new Dog
        {
            Name = request.Name,
            Color = request.Color,
            TailLength = request.TailLength,
            Weight = request.Weight
        };

        _context.Dogs.Add(dog);
        await _context.SaveChangesAsync();

        return dog;
    }

    private IQueryable<Dog> SortDogs(IQueryable<Dog> query, string attribute, string? order)
    {
        bool descending = order?.ToLower() == "desc";

        switch (attribute.ToLower())
        {
            case "name":
                return descending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);

            case "color":
                return descending ? query.OrderByDescending(d => d.Color) : query.OrderBy(d => d.Color);

            case "tail_length":
                return descending ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength);

            case "weight":
                return descending ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight);

            default:
                return query;
        }
    }
}