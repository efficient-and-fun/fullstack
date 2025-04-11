using System.ComponentModel.DataAnnotations;

namespace WebApi.Model;

public class MeetUpBreefDto
{
    public int MeetUpId { get; set; }
    
    [MaxLength(255)]
    public string MeetUpName { get; set; }

    public string Description { get; set; }

    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
}

public class MeetUpDetailDto : MeetUpBreefDto
{

    public string CheckList { get; set; }

    public string MeetUpLocation { get; set; }
}