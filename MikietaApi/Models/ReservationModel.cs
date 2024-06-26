﻿using System.Text.Json.Serialization;

namespace MikietaApi.Models;

public class ReservationModel
{
    public Guid? Id { get; set; }
    public int Number { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int NumberOfPeople { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Comments { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReservationStatusType Status { get; set; }
    public bool EmailSent { get; set; }
}

public class SendEmailModel
{
    public Guid ReservationId { get; set; }
    public string Message { get; set; } = null!;
}