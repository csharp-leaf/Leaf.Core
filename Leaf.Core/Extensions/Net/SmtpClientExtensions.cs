using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Leaf.Core.Extensions.Net
{
     public static class SmtpClientExtensions
    {
         /// <summary>
        /// Sends the specified message to an SMTP server for delivery as an asynchronous operation.
        /// </summary>
        /// <param name="client">
        /// The <see cref="SmtpClient"/> instance.
        /// </param>
        /// <param name="message">The <see cref="MailMessage"/> to send.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="client"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="message"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task SendMailAsync(this SmtpClient client, MailMessage message, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (!cancellationToken.CanBeCanceled) return client.SendMailAsync(message);

            var tcs = new TaskCompletionSource<object>();
            var registration = default(CancellationTokenRegistration);
            SendCompletedEventHandler handler = null;

            handler = (sender, e) => {
                if (e.UserState != tcs) 
                    return;

                try
                {
                    if (handler == null) 
                        return;

                    client.SendCompleted -= handler;
                    handler = null;
                }
                finally
                {
                    registration.Dispose();

                    if (e.Error != null)
                        tcs.TrySetException(e.Error);
                    else if (e.Cancelled)
                        tcs.TrySetCanceled();
                    else
                        tcs.TrySetResult(null);
                }
            };

            client.SendCompleted += handler;

            try
            {
                client.SendAsync(message, tcs);
                registration = cancellationToken.Register(client.SendAsyncCancel);
            }
            catch
            {
                client.SendCompleted -= handler;
                registration.Dispose();
                throw;
            }

            return tcs.Task;
        }

    }
}
