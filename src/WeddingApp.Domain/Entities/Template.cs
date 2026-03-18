using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class Template : BaseEntity
{
    public string Name { get; private set; }
    public string HtmlStructure { get; private set; }
    
    private Template() { }
    
    public Template(string name, string htmlStructure)
    {
        Name = name;
        HtmlStructure = htmlStructure;
    }
}
