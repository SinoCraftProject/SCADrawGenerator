using System.Drawing;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("SinoBrush Drawing Generator Ver: 1.2.0.");
Console.WriteLine("Powered by qyl27 of SinoCraft Project Team.");
Console.WriteLine("Only Windows is supported!");

if (args.Length != 3)
{
    Console.WriteLine("Usage: <Source file path> <Width> <Height>.");
    return;
}

var image = (Bitmap)Image.FromFile(args[0]);

int.TryParse(args[1], out var w);
int.TryParse(args[2], out var h);

if (image.Width != w
    || image.Height != h)
{
    Console.WriteLine("Image not fit!");
    return;
}

var draw = new Draw
{
    Size = new Draw.DrawSize
    {
        X = w,
        Y = h
    },
    Date = 1,
    Pixels = new int[w * h]
};

for (var i = 0; i < w; i++)
{
    for (var j = 0; j < h; j++)
    {
        var color = image.GetPixel(i, j);

        var alpha = int.Parse(color.A.ToString("D"));
        var a = alpha / 16;
        var index = i * w + j;
        draw.Pixels[index] = a;
    }
}

var f = new FileInfo(args[0]);

var str = JsonSerializer.Serialize(draw, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
File.WriteAllText($"{f.Directory}/{f.Name}.txt", str);
Console.WriteLine("Done!");

public class Draw
{
    public long Date { get; set; }
    public int[] Pixels { get; set; }
    public DrawSize Size { get; set; }
    public DrawColor Color { get; set; } = new();
    public string Author { get; set; } = "{\"translate\":\"sinobrush.drawing.author.unknown\"}";
    public string Title { get; set; } = "{\"translate\":\"sinobrush.drawing.title.unknown\"}";
    public int Version { get; set; } = 1;

    public class DrawSize
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class DrawColor
    {
        public int Paper { get; set; } = -1; // White
        public int Ink { get; set; } = -16777216; // Black
    }
}
