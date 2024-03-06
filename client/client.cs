using System;
using System.Net.Sockets;
using System.Text;

class client
{
    static void Main()
    {
        try
        {
            // Set the server IP address and port number
            string serverIp = "127.0.0.1";
            int port = 8080;

            // Create a TcpClient.
            TcpClient client = new TcpClient(serverIp, port);

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            string command;

            do
            {
                // Read a command from the console.
                Console.Write("Enter command (or 'bye' to exit): ");
                command = Console.ReadLine();

                // Send the command to the server.
                byte[] data = Encoding.UTF8.GetBytes(command);
                stream.Write(data, 0, data.Length);

                // Check for the exit condition.
                if (command.ToLower() == "bye")
                {
                    break;
                }

                // Receive and display the server response.
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Server response:\n" + serverResponse);
            } while (true);

            // Close the stream and TcpClient when done.
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        Console.WriteLine("Press enter to exit...");
        Console.ReadLine();
    }
}
