using System.Windows;
using CAFEHOLIC.dao;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioSMSApi
{
    private readonly string accountSid;
    private readonly string authToken;
    private readonly string fromPhone;
    private ILogger<TwilioSMSApi> logger=new DBContext().GetLogger<TwilioSMSApi>();

    public TwilioSMSApi(string sid, string token, string from)
    {
        accountSid = sid;
        authToken = token;
        fromPhone = from;
        TwilioClient.Init(accountSid, authToken);
    }

    public bool SendSMS(string toPhone, string message)
    {
        try
        {
            var to = new PhoneNumber(toPhone);
            var from = new PhoneNumber(fromPhone);
            logger.LogInformation($"Sending SMS to {toPhone}: {message}"+$"from{from}");

            var msg = MessageResource.Create(
                to: to,
                from: from,
                body: message
            );

            return msg.ErrorCode == null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi gửi SMS: {ex.Message}");
            return false;
        }
    }
}
