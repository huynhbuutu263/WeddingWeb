using System;
using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class WeddingCard : BaseEntity
{
    // COPILOT TASK:
    // 1. Add 'Title' (string)
    // 2. Add 'SlugUrl' (string) - This will be unique in the DB later.
    // 3. Add 'EventDate' (DateTime)
    // 4. Add 'TemplateId' (Guid) and the navigation property for 'Template'
    // 5. Create a constructor that strictly dictates these values.
    
    private WeddingCard() { } 
}
