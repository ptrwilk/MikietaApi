using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.SendEmail;

namespace MikietaApi.Services;

public interface IReservationService
{
    void Reserve(ReservationModel model);
    ReservationModel[] GetAll();
    ReservationModel Update(ReservationModel model);
}

public class ReservationService : IReservationService
{
    private readonly DataContext _context;
    private readonly EmailSender _emailSender;

    public ReservationService(DataContext context, EmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }
    
    public void Reserve(ReservationModel model)
    {
        var messageId = _emailSender.Send(new EmailSenderModel
        {
            RecipientEmail = "ptrwilk@outlook.com", //TODO: zmienic potem na model.email
            NumberOfPeople = model.NumberOfPeople,
            ReservationDate = model.ReservationDate.ToLocalTime()
        });
        
        _context.Reservations.Add(new ReservationEntity
        {
            Name = model.Name,
            Comments = model.Comments,
            Email = model.Email,
            Phone = model.Phone,
            ReservationDate = model.ReservationDate,
            NumberOfPeople = model.NumberOfPeople,
            Status = ReservationStatusType.Waiting,
            CreatedAt = DateTime.Now,
            MessageId = messageId
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

    public ReservationModel Update(ReservationModel model)
    {
        var entity = _context.Reservations.First(x => x.Id == model.Id);

        entity.Status = model.Status;
    
        _context.SaveChanges();

        return model;
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
            NumberOfPeople = entity.NumberOfPeople,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status
        };
    }
}