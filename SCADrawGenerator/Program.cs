using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

Console.WriteLine("SinoCalligraphy Drawing Generator Ver: 1.1.0.");
Console.WriteLine("Powered by qyl27 of SinoCraft Project Team.");
Console.WriteLine("Only Windows is supported!");

if (args.Length != 7)
{
    Console.WriteLine("Usage: <Source file path> <Horizontal frame count> <Vertical frame count> <Author name> <Ink type> <Paper color> <Draw name>.");
    return;
}

var image = (Bitmap) Image.FromFile(args[0]);

int.TryParse(args[1], out var horizontal);
int.TryParse(args[2], out var vertical);

var sum = horizontal * vertical;

const int frameSize = 32;

if (image.Width != horizontal * frameSize
    || image.Height != vertical * frameSize)
{
    Console.WriteLine("Image is illegal!");
    return;
}

for (var i = 0; i < horizontal; i++)
{
    for (var j = 0; j < vertical; j++)
    {
        var stringBuilder = new StringBuilder();

        var author = JsonConvert.SerializeObject(ChatComponent.WithAuthor(args[3]), new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore, 
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        stringBuilder.Append($"{{author:'{author}',");
        
        var title = JsonConvert.SerializeObject(ChatComponent.WithName(args[6], i, j), new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore, 
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        stringBuilder.Append($"title:'{title}',");

        stringBuilder.Append("pixels:[B;");
    
        for (var k = 0; k < frameSize; k++)
        {
            for (var l = 0; l < frameSize; l++)
            {
                var color = image.GetPixel(k + frameSize * i, l + frameSize * j);

                var alpha = int.Parse(color.A.ToString("D"));
                var a = alpha / 16;
                stringBuilder.Append(a + "B");
            
                if (l % 32 != 31)
                {
                    stringBuilder.Append(',');
                }
            }
        
            if (k % 32 != 31)
            {
                stringBuilder.Append(',');
            }
        }
                         
        stringBuilder.Append(']');
        stringBuilder.Append($",ink:\"{args[4]}\",paper:{args[5]},");
        stringBuilder.Append("version:\"1\"}");

        var path = new FileInfo(args[0]);
        File.WriteAllText($"{path.Directory}/{path.Name}-{i}-{j}.txt", stringBuilder.ToString());
    }
}

public class ChatComponent
{
    public string Text { get; set; } = "Infinity_rain";
    public string Color { get; set; } = "aqua";
    public string Bold { get; set; } = "false";

    public ChatComponent[] Extra { get; set; } = null;

    public static ChatComponent WithAuthor(string name)
    {
        return new ChatComponent
        {
            Text = name,
            Extra = new [] {
                new ChatComponent
                {
                    Text = " with ",
                    Color = "grey",
                    Extra = new [] {
                        new ChatComponent
                        {
                            Text = "SCADrawGenerator",
                            Color = "gold",
                            Bold = "true",
                            Extra = null
                        }
                    }
                }
            }
        };
    }

    public static ChatComponent WithName(string name, int x, int y)
    {
        return new ChatComponent
        {
            Text = name,
            Extra = new []
            {
                new ChatComponent
                {
                    Text = $" [{x}, {y}]",
                    Color = "gold"
                }
            }
        };
    }
}
