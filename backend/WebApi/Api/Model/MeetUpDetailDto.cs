using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Model;

public class MeetUpBriefDto
{
    [Required, Key]
    public int MeetUpId { get; set; }
    
    [MaxLength(255)]
    public string MeetUpName { get; set; }


    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    
    public string MeetUpLocation { get; set; }
}

public class MeetUpDetailDto : MeetUpBriefDto
{
    public string Description { get; set; }
    public string CheckList { get; set; }

}

public class MeetUps
{
    [Required, Key]
    public int MeetUpId { get; set; }
    [MaxLength(255)]
    public string MeetUpName { get; set; }
    public string Description { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    
    public string? CheckList { get; set; }
    public string? MeetUpLocation { get; set; }
}