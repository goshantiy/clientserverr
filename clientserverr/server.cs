using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class server
{
    static void Main()
    {
        TcpListener server = null;

        try
        {
            // Set the TcpListener on port 8080
            Int32 port = 8080;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[1024];
            String data = null;

            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                try
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a UTF-8 string.
                        data = Encoding.UTF8.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data received and perform actions accordingly
                        string response = ProcessCommand(data);

                        // Send the response back to the client.
                        byte[] msg = Encoding.UTF8.GetBytes(response);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", response);
                    }
                }
                catch (IOException ex)
                {
                    // Handle IOException when client disconnects
                    Console.WriteLine($"Client disconnected: {ex.Message}");
                }
                finally
                {
                    // Shutdown and end connection
                    client.Close();
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    private static string ProcessCommand(string command)
    {
        string response = "";

        if (command.StartsWith("ls"))
        {
            // Command: ls
            if (command.Length > 2)
            {
                string directoryPath = command.Substring(2).Trim();
                response = ListFiles(directoryPath);
            }
            else
            {
                response = "Invalid 'ls' command. Specify a directory path.";
            }
        }
        else if (command.StartsWith("type"))
        {
            // Command: type
            if (command.Length > 4)
            {
                string filePath = command.Substring(4).Trim();
                response = Type(filePath);
            }
            else
            {
                response = "Invalid 'type' command. Specify a file path.";
            }
        }
        else if (command.StartsWith("pwd"))
        {
            // Command: pwd
            response = GetCurrentDirectory();
        }
        else
        {
            // Unknown command
            response = "Unknown command";
        }

        return response;
    }

    private static string ListFiles(string directoryPath)
    {
        try
        {
            string[] files = Directory.GetFiles(directoryPath);
            return string.Join("\n", files);
        }
        catch (Exception e)
        {
            return "Error listing files: " + e.Message;
        }
    }

    private static string Type(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            return content;
        }
        catch (Exception e)
        {
            return "Error reading file: " + e.Message;
        }
    }

    private static string GetCurrentDirectory()
    {
        try
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            return "Current working directory: " + currentDirectory;
        }
        catch (Exception e)
        {
            return "Error getting current directory: " + e.Message;
        }
    }
}
