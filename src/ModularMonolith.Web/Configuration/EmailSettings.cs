namespace ModularMonolith.Web.Configuration;

public class EmailSettings
{
    required public string SenderEmail { get; set; }

    required public string Sender { get; set; }

    required public string Host { get; set; }

    public int Port { get; set; }
}
