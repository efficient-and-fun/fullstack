using System.ComponentModel.DataAnnotations;

namespace WebApi.Api.Model;

public class MeetUpBriefDto
{
    [Required, Key]
    public int MeetUpId { get; set; }
    
    [MaxLength(255)]
    public string MeetUpName { get; set; }


    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    [MaxLength(65535)]
    public string? MeetUpLocation { get; set; }
}

public class MeetUp : MeetUpBriefDto
{
    [MaxLength(65535)]
    public string? Description { get; set; }
    [MaxLength(65535)]
    public string? CheckList { get; set; }
    public int? MaxNumberOfParticipants { get; set; }

}

public class MeetUps
{
    [Required, Key]
    public int MeetUpId { get; set; }
    [MaxLength(255)]
    public string MeetUpName { get; set; }
    [MaxLength(65535)]
    public string? Description { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    [MaxLength(65535)]
    public string? CheckList { get; set; }
    [MaxLength(65535)]
    public string? MeetUpLocation { get; set; }
    public int? MaxNumberOfParticipants { get; set; }
}

public class MeetUpCreateDto
{
    [MaxLength(255)]
    public string MeetUpName { get; set; }
    [MaxLength(65535)]
    public string? Description { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    [MaxLength(65535)]
    public string? CheckList { get; set; }
    [MaxLength(65535)]
    public string? MeetUpLocation { get; set; }
    public int? MaxNumberOfParticipants { get; set; }
}