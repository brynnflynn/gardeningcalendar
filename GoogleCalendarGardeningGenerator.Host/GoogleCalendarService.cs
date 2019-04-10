using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace GoogleCalendarGardeningGenerator.Host
{
    public sealed class GoogleCalendarService
    {
        private static readonly string[] Scopes = { CalendarService.Scope.CalendarEvents, CalendarService.Scope.Calendar };
        private static readonly string ApplicationName = "GardeningCalendarGenerator";

        private static readonly Lazy<GoogleCalendarService> lazy =
            new Lazy<GoogleCalendarService>(() => new GoogleCalendarService());

        public static GoogleCalendarService Instance => lazy.Value;

        public CalendarService CalendarService { get; }

        private GoogleCalendarService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            CalendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}