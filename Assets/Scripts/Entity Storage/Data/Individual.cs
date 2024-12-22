using System.Collections.Generic;

class Individual
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public string FAMC { get; set; }
    public List<string> FAMS { get; set; } = new List<string>();
}