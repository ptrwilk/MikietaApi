using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IReservationService
{
    void Reserve(ReservationModel model);
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
}