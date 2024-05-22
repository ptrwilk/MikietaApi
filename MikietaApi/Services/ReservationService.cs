using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IReservationService
{
    void Reserve(ReservationModel model);
    ReservationModel[] GetAll();
}

public class ReservationService : IReservationService
{
    private readonly DataContext _context;

    public ReservationService(DataContext context)
    {
        _context = context;
    }
    
    public void Reserve(ReservationModel model)
    {
        _context.Reservations.Add(new ReservationEntity
        {
            Name = model.Name,
            Comments = model.Comments,
            Email = model.Email,
            Phone = model.Phone,
            ReservationDate = model.ReservationDate,
            NumberOfPeople = model.NumberOfPeople
        });

        _context.SaveChanges();
    }

    public ReservationModel[] GetAll()
    {
        return _context.Reservations.ToList()
            .Select(Convert)
            .OrderByDescending(x => x.Number)
            .ToArray();
    }
    
    private ReservationModel Convert(ReservationEntity entity)
    {
        return new ReservationModel
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            Comments = entity.Comments,
            Email = entity.Email,
            Phone = entity.Phone,
            ReservationDate = entity.ReservationDate,
            NumberOfPeople = entity.NumberOfPeople
        };
    }
}