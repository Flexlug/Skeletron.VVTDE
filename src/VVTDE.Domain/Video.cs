using System;

namespace VVTDE.Domain;

public class Video
{
    public Guid Id { get; set; }
    public Guid Guid { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}