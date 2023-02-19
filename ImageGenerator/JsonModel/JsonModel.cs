namespace ImageGenerator;

public class JsonModel
{
    public Category[] Categories { get; set; } = new Category[]
    {
        new()
        {
            Id = Category.TextId,
            Name = "Text",
        },
        new()
        {
            Id = Category.TableId,
            Name = "Table",
        },
        new()
        {
            Id = Category.ImageId,
            Name = "Image",
        },
    };

    public List<ImageObject> Objects { get; set; } = new();
}