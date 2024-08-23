using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace Cackle.ConsoleApp.Features;

/// <summary>
///     Provides the service of sending an email in the event of an unhandled exception.
/// </summary>
public static class ErrorNotify
{
    private const string Subject = "Exception encountered in %%ApplicationName%%";

    private const string HtmlContent = """
                                       <p>An exception was encountered in <b>%%ApplicationName%%</b>.</p>
                                       <p>Command executed:</p>
                                       <code>%%FullPath%% %%Arguments%%</code>
                                       <p>Exception encountered:</p>
                                       <code>%%Exception%%</code>
                                       <p>Stack trace:</p>
                                       <code>%%StackTrace%%</code>
                                       <p>Inner exception, if any:</p>
                                       <code>%%InnerException%%</code>
                                       <p>Inner stack trace, if any:</p>
                                       <code>%%InnerStackTrace%%</code>
                                       """;

    private const string TextContent = """
                                       An exception was encountered in %%ApplicationName%%

                                       Command executed:
                                       %%FullPath%% %%Arguments%%

                                       Exception encountered:
                                       %%Exception%%

                                       Stack trace:
                                       %%StackTrace%%

                                       Inner exception, if any:
                                       %%InnerException%%

                                       Inner stack trace, if any:
                                       %%InnerStackTrace%%
                                       """;

    /// <summary>
    ///     Execute a method that expects an argument of an array of strings, such as
    ///     <c>CommandHostBuilder.Create().Build().Run</c>, then catch any uncaught exceptions before sending an email
    ///     notification of the details.
    /// </summary>
    /// <param name="method">
    ///     A method that expects a single <see langword="string[]" /> and returns an <see langword="int" />.
    /// </param>
    /// <param name="args">An array of strings used to call the method.</param>
    /// <param name="configureMail">
    ///     SMTP and mail message options for the outgoing message. <b>Note:</b> The properties
    ///     <see cref="ErrorMailOptions.From" /> and <see cref="ErrorMailOptions.SmtpHost" />; and least one of
    ///     <see cref="ErrorMailOptions.To" />, <see cref="ErrorMailOptions.Cc" />, or <see cref="ErrorMailOptions.Bcc" /> are
    ///     required.
    /// </param>
    /// <returns>If no exception is encountered, forwards the method return; otherwise an exception is thrown.</returns>
    public static int MailOnException(Func<string[], int> method, string[] args, Action<ErrorMailOptions> configureMail)
    {
        var errorMailOptions = new ErrorMailOptions { SmtpHost = string.Empty, From = string.Empty };
        configureMail(errorMailOptions);
        return MailOnException(method, args, errorMailOptions);
    }

    /// <summary>
    ///     Execute a method that expects an argument of an array of strings, such as
    ///     <c>CommandHostBuilder.Create().Build().Run</c>, then catch any uncaught exceptions before sending an email
    ///     notification of the details.
    /// </summary>
    /// <param name="method">
    ///     A method that expects a single <see langword="string[]" /> and returns an <see langword="int" />.
    /// </param>
    /// <param name="args">An array of strings used to call the method.</param>
    /// <param name="configureMail">
    ///     SMTP and mail message options for the outgoing message. <b>Note:</b> The properties
    ///     <see cref="ErrorMailOptions.From" /> and <see cref="ErrorMailOptions.SmtpHost" />; and least one of
    ///     <see cref="ErrorMailOptions.To" />, <see cref="ErrorMailOptions.Cc" />, or <see cref="ErrorMailOptions.Bcc" /> are
    ///     required.
    /// </param>
    /// <returns>If no exception is encountered, forwards the method return; otherwise an exception is thrown.</returns>
    public static int MailOnException(Func<string[], int> method, string[] args, ErrorMailOptions configureMail)
    {
        try
        {
            return method.Invoke(args);
        }
        catch (Exception ex)
        {
            SendMail(configureMail, ex);
            throw;
        }
    }

    /// <summary>
    ///     Execute a method that expects an argument of an array of strings, such as
    ///     <c>CommandHostBuilder.Create().Build().RunAsync</c>, then catch any uncaught exceptions before sending an email
    ///     notification of the details.
    /// </summary>
    /// <param name="method">
    ///     A method that expects a single <see langword="string[]" /> and returns an <see langword="int" />.
    /// </param>
    /// <param name="args">An array of strings used to call the method.</param>
    /// <param name="configureMail">
    ///     SMTP and mail message options for the outgoing message. <b>Note:</b> The properties
    ///     <see cref="ErrorMailOptions.From" /> and <see cref="ErrorMailOptions.SmtpHost" />; and least one of
    ///     <see cref="ErrorMailOptions.To" />, <see cref="ErrorMailOptions.Cc" />, or <see cref="ErrorMailOptions.Bcc" /> are
    ///     required.
    /// </param>
    /// <returns>If no exception is encountered, forwards the method return; otherwise an exception is thrown.</returns>
    public static Task<int> MailOnException(Func<string[], Task<int>> method, string[] args,
        Action<ErrorMailOptions> configureMail)
    {
        var errorMailOptions = new ErrorMailOptions { SmtpHost = string.Empty, From = string.Empty };
        configureMail(errorMailOptions);
        return MailOnException(method, args, errorMailOptions);
    }

    /// <summary>
    ///     Execute a method that expects an argument of an array of strings, such as
    ///     <c>CommandHostBuilder.Create().Build().RunAsync</c>, then catch any uncaught exceptions before sending an email
    ///     notification of the details.
    /// </summary>
    /// <param name="method">
    ///     A method that expects a single <see langword="string[]" /> and returns an <see langword="int" />.
    /// </param>
    /// <param name="args">An array of strings used to call the method.</param>
    /// <param name="configureMail">
    ///     SMTP and mail message options for the outgoing message. <b>Note:</b> The properties
    ///     <see cref="ErrorMailOptions.From" /> and <see cref="ErrorMailOptions.SmtpHost" />; and least one of
    ///     <see cref="ErrorMailOptions.To" />, <see cref="ErrorMailOptions.Cc" />, or <see cref="ErrorMailOptions.Bcc" /> are
    ///     required.
    /// </param>
    /// <returns>If no exception is encountered, forwards the method return; otherwise an exception is thrown.</returns>
    public static Task<int> MailOnException(Func<string[], Task<int>> method, string[] args,
        ErrorMailOptions configureMail)
    {
        try
        {
            return method.Invoke(args);
        }
        catch (Exception ex)
        {
            SendMail(configureMail, ex);
            throw;
        }
    }

    /// <summary>
    ///     Prepare email message and send.
    /// </summary>
    /// <param name="options"><see cref="ErrorMailOptions" /> for sending the message.</param>
    /// <param name="exception">The exception that was encountered.</param>
    private static void SendMail(ErrorMailOptions options, Exception exception)
    {
        // Retrieve some process information
        var process = ProcessInfo.GetCurrentProcess();

        // Build the message
        var message = GenerateMessage(options);

        // Build the content
        var content = message.IsBodyHtml ? HtmlContent : TextContent;
        content = content.Replace("%%ApplicationName%%", process.FriendlyName);
        content = content.Replace("%%FullPath%%", Path.Join(process.FullName, process.FriendlyName));
        content = content.Replace("%%Arguments%%", process.Arguments);
        content = content.Replace("%%Exception%%", exception.Message);
        content = content.Replace("%%StackTrace%%", exception.StackTrace);
        content = content.Replace("%%InnerException%%", exception.InnerException?.Message);
        content = content.Replace("%%InnerStackTrace%%", exception.InnerException?.StackTrace);

        message.Subject = Subject.Replace("%%ApplicationName%%", process.FriendlyName);
        message.Body = content;

        // Configure and send email
        using var smtpClient = new SmtpClient(options.SmtpHost, options.SmtpPort);
        if (options.SmtpUsername is not null)
        {
            if (options.SmtpPassword is not null)
                smtpClient.Credentials = new NetworkCredential(options.SmtpUsername, options.SmtpPassword);
            else if (options.SmtpSecurePassword is not null)
                smtpClient.Credentials = new NetworkCredential(options.SmtpUsername, options.SmtpSecurePassword);
        }

        smtpClient.EnableSsl = options.EnableSsl;
        smtpClient.Send(message);
    }

    /// <summary>
    ///     Generate the framework of an email message.
    /// </summary>
    private static MailMessage GenerateMessage(ErrorMailOptions options)
    {
        // Build the mail message
        var message = new MailMessage();
        message.From = new MailAddress(options.From);

        // Configure destinations
        if (options.To is not null)
            foreach (var to in options.To)
                message.To.Add(to);

        if (options.Cc is not null)
            foreach (var cc in options.Cc)
                message.CC.Add(cc);

        if (options.Bcc is not null)
            foreach (var bcc in options.Bcc)
                message.Bcc.Add(bcc);

        // Configure email options
        message.IsBodyHtml = options.UseHtml;

        return message;
    }

    /// <summary>
    ///     Identify information about the currently running process.
    /// </summary>
    private static class ProcessInfo
    {
        /// <summary>
        ///     Retrieve a few pieces of information about the currently running process.
        /// </summary>
        internal static CurrentProcess GetCurrentProcess()
        {
            var args = string.Join(' ', Environment.GetCommandLineArgs().Skip(1).ToArray());

            var currentProcess = Process.GetCurrentProcess();
            var module = currentProcess.MainModule;

            if (module is not null)
                return new CurrentProcess
                {
                    FriendlyName = currentProcess.ProcessName,
                    FullName = module.FileName,
                    Arguments = args
                };

            var processPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            return new CurrentProcess
            {
                FriendlyName = currentProcess.ProcessName,
                FullName = string.Join(processPath, ".exe"),
                Arguments = args
            };
        }

        /// <summary>
        ///     Information on the currently running process.
        /// </summary>
        internal class CurrentProcess
        {
            /// <summary>
            ///     The friendly name of the executable.
            /// </summary>
            public required string FriendlyName { get; init; }

            /// <summary>
            ///     The full file path to the currently running process.
            /// </summary>
            public required string FullName { get; init; }

            /// <summary>
            ///     Arguments used when launching the running process.
            /// </summary>
            public required string Arguments { get; init; }
        }
    }
}