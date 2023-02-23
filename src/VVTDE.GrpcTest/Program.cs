using Grpc.Net.Client;
using Skeletron;

Console.Write("Enter GRPC server url :");
var grpcUrl = Console.ReadLine();

var channel = GrpcChannel.ForAddress("localhost:7261");
var client = new VVTDEBridge.VVTDEBridgeClient(channel);

while (true)
{
    Console.WriteLine("1. Send video request\n2. Fetch video");

    int inp = Convert.ToInt32(Console.ReadLine());

    switch (inp)
    {
        case 1:
            Console.Write("Enter video url: ");
            var videoUrl = Console.ReadLine();

            Console.Write("Enter image url: ");
            var imageUrl = Console.ReadLine();

            var videoRequest = new VideoRequest()
            {
                Description = "Sample description",
                Title = "Sample title",
                ImageUrl = imageUrl,
                Url = videoUrl
            };

            var videoRequestResponse = client.RequestDownloadVideo(videoRequest);
            Console.WriteLine($"Response: ");
            if (videoRequestResponse is null)
            {
                Console.WriteLine($"Guid: {videoRequestResponse.Guid}");
                Console.WriteLine($"AlreadyDownloaded: {videoRequestResponse.AlreadyDownloaded}");
            }
            else
            {
                Console.WriteLine("Null");
            }
            break;
        case 2:
            Console.Write("Enter GUID: ");
            var strGuid = Console.ReadLine();

            var fetchRequest = new FetchVideoRequest()
            {
                Guid = strGuid
            };

            var fetchResponse = client.FetchDownloadVideo(fetchRequest);

            Console.WriteLine("Fetch response: ");
            if (fetchResponse is null)
            {
                Console.WriteLine($"Download complete: {fetchResponse.DownloadComplete}");
            }
            else
            {
                Console.WriteLine("Null");
            }
            
            break;
        default:
            Console.WriteLine("No such option");
            break;
    }
}