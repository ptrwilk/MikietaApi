using Microsoft.AspNetCore.SignalR;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Factories;
using MikietaApi.Hubs;
using MikietaApi.Models;
using MikietaApi.SendEmail;
using MikietaApi.SendEmail.Reservation.Models;

namespace MikietaApi.Services;

public interface IReservationService
{
    bool Reserve(ReservationModel model);
    ReservationModel[] GetAll();
    ReservationModel Update(ReservationModel model);
    ReservationModel SendEmail(SendEmailModel model);
}

public class ReservationService : IReservationService
{
    private readonly DataContext _context;
    private readonly IEmailSender<ReservationEmailSenderModel> _emailSender;
    private readonly IEmailReply<ReservationEmailReplyModel> _emailReply;
    private readonly IEmailSenderModelFactory _emailSenderFactory;
    private readonly IHubContext<MessageHub, IMessageHub> _hub;

    public ReservationService(DataContext context, IEmailSender<ReservationEmailSenderModel> emailSender,
        IHubContext<MessageHub, IMessageHub> hub, IEmailReply<ReservationEmailReplyModel> emailReply,
        IEmailSenderModelFactory emailSenderFactory)
    {
        _context = context;
        _emailSender = emailSender;
        _hub = hub;
        _emailReply = emailReply;
        _emailSenderFactory = emailSenderFactory;
    }

    public bool Reserve(ReservationModel model)
    {
        var settings = _context.Settings.ToArray();
        var messageId = _emailSender.Send(_emailSenderFactory.Create<ReservationEmailSenderModel>(settings, x =>
        {
            x.RecipientEmail = model.Email;
            x.NumberOfPeople = model.NumberOfPeople;
            x.ReservationDate = model.ReservationDate.ToLocalTime();
        }));

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

        _hub.Clients.All.ReservationMade();

        return true;
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

    public ReservationModel SendEmail(SendEmailModel model)
    {
        var entity = _context.Reservations.First(x => x.Id == model.ReservationId);

        if (entity.EmailSent)
        {
            //TODO: To i inne exception wrzucać do logów natomiast klient niech ich nie dostaje, przynajmnniej nie caly callstack
            throw new InvalidOperationException("An email has already been sent.");
        }

        var settings = _context.Settings.ToArray();
        _emailReply.Reply(_emailSenderFactory.Create<ReservationEmailReplyModel>(settings, x =>
        {
            x.Message = model.Message;
            x.MessageId = entity.MessageId!;
            x.RecipientEmail = entity.Email;
        }));

        entity.EmailSent = true;

        _context.SaveChanges();

        return Convert(entity);
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
            Status = entity.Status,
            EmailSent = entity.EmailSent
        };
    }
}