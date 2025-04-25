using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Model;

[Table("Participations")]
public class Participation
{
    [Required, Key]
    public int ParticipationId { get; set; }

    [Required, ForeignKey("fk_participations_users")]
    public int UserId { get; set; }
    [Required, ForeignKey("fk_participations_meetups")]
    public int MeetUpId { get; set; }

    public bool HasAcceptedInvitation { get; set; }
    public bool HasAttended { get; set; }
    public int Rating { get; set; }
}