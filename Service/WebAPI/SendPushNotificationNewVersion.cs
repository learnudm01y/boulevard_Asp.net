using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Google.Apis.Auth.OAuth2;

namespace Boulevard.Service.WebAPI
{
    public class SendPushNotificationNewVersion
    {
        public class Data
        {

            public string body
            {
                get;
                set;
            }

            public string title
            {
                get;
                set;
            }

            public string key_1
            {
                get;
                set;
            }

            public string key_2
            {
                get;
                set;
            }

        }





        public class Message
        {


            public string token
            {
                get;
                set;
            }

            public Data data
            {
                get;
                set;
            }

            public Notification notification
            {
                get;
                set;
            }

        }

        public class Notification
        {

            public string title
            {
                get;
                set;
            }

            public string body
            {
                get;
                set;
            }

        }

        public class Root
        {

            public Message message
            {
                get;
                set;
            }

        }

        public void GenerateFCM_Auth_SendNotifcn(string deviceToken, string title, string Message)

        {
            //----------Generating Bearer token for FCM---------------

            try
            {
                string fileName = System.Web.Hosting.HostingEnvironment.MapPath("~/content/NotificationJson/boulevard-a50a0-firebase-adminsdk-fbsvc-3458339bb7.json"); //Download from Firebase Console ServiceAccount

                string scopes = "https://www.googleapis.com/auth/firebase.messaging";
                var bearertoken = ""; // Bearer Token in this variable
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))

                {

                    bearertoken = GoogleCredential
                      .FromStream(stream) // Loads key file
                      .CreateScoped(scopes) // Gathers scopes requested
                      .UnderlyingCredential // Gets the credentials
                      .GetAccessTokenForRequestAsync().Result; // Gets the Access Token

                }

                ///--------Calling FCM-----------------------------

                var clientHandler = new HttpClientHandler();
                var client = new HttpClient(clientHandler);

                client.BaseAddress = new Uri("https://fcm.googleapis.com/v1/projects/boulevard-a50a0/messages:send"); // FCM HttpV1 API

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Accept.Add("Authorization", "Bearer " + bearertoken);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearertoken); // Authorization Token in this variable

                //---------------Assigning Of data To Model --------------

                Root rootObj = new Root();
                rootObj.message = new Message();

                rootObj.message.token = deviceToken; //FCM Token id

                rootObj.message.data = new Data();


                rootObj.message.data.title = "Data Title";
                rootObj.message.data.body = "Data Body";
                rootObj.message.data.key_1 = "Sample Key";
                rootObj.message.data.key_2 = "Sample Key2";
                rootObj.message.notification = new Notification();
                rootObj.message.notification.title = title;
                rootObj.message.notification.body = Message;

                //-------------Convert Model To JSON ----------------------

                var jsonObj = new JavaScriptSerializer().Serialize(rootObj);

                //------------------------Calling Of FCM Notify API-------------------

                var data = new StringContent(jsonObj, Encoding.UTF8, "application/json");
                data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync("https://fcm.googleapis.com/v1/projects/boulevard-a50a0/messages:send", data).Result; // Calling The FCM httpv1 API

                //---------- Deserialize Json Response from API ----------------------------------

                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                var responseObj = new JavaScriptSerializer().DeserializeObject(jsonResponse);
            }
            catch (Exception ex)
            {


            }

        }
    }
}