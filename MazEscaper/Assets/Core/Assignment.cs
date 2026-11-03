namespace Core
{
    public class Assignment
    {
     public int ResidentId { get; }   
     public string TargetId { get; } 
     public int Points { get; set; } 
     
     public Assignment(int residentId, string targetId, int points)
        {
            ResidentId = residentId;
            TargetId = targetId;
            Points = points;
        }
    }
}