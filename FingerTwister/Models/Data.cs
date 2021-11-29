namespace FingerTwister.Models;

public class Data
{
    public int lastID { get; set; }
    public int nextID { get; set; }
    public bool finish { get; set; }
    public int level { get; set; }

    public int player { get; set; }
    public bool started { get; set; }
    public string scores { get; set; }
}
