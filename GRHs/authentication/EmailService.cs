using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms; // Add this to use MessageBox

public static class EmailService
{
    public static void SendEmail(string toEmail, string subject, string body)
    {
        // Set up the sender email and password
        string fromEmail = "5elements.darb2@gmail.com"; // Your email address
        string password = "tnlb omid zpsb xwtc"; // Your email password

        // Create the email message
        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body =  body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        // Set up the SMTP client
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) // SMTP server for Gmail
        {
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true
        };

        try
        {
            // Send the email
            smtpClient.Send(mailMessage);
            MessageBox.Show("Email sent successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SmtpException smtpEx)
        {
            string errorMessage = "SMTP error occurred while sending email: " + smtpEx.Message;
            if (smtpEx.Message.Contains("authentication"))
            {
                errorMessage = "Authentication failed. Check your email address and password.";
            }
            else if (smtpEx.Message.Contains("could not connect"))
            {
                errorMessage = "Could not connect to the SMTP server. Check the server address and port.";
            }
            else if (smtpEx.Message.Contains("timeout"))
            {
                errorMessage = "SMTP server connection timed out.";
            }
            // Handle other specific cases or provide a general message
            MessageBox.Show(errorMessage, "SMTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (FormatException formatEx)
        {
            string errorMessage = "Email format error: " + formatEx.Message + "\nCheck the format of the email addresses.";
            MessageBox.Show(errorMessage, "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (InvalidOperationException invalidOpEx)
        {
            string errorMessage = "Invalid operation error: " + invalidOpEx.Message + "\nCheck the configuration of the SMTP client.";
            MessageBox.Show(errorMessage, "Invalid Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while sending email: " + ex.Message;
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

   
}
