using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using poolio.Model;
using Microsoft.Cognitive.LUIS;
using poolio.Controllers;
using System.Collections;
using System.Collections.Generic;

namespace poolio
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private LuisController _luisController = new LuisController();

        private Queue<string> _replyMessages = new Queue<string>();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (!DbController.DoesUserExist(activity.From.Name))
            {
                DbController.CreateUser(activity.From.Name);
                AddToReplyQueue("Hi there! This is the first time you're using Poolio, please make sure to update your address. Say \"update address\" for more info.");
            }

            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var intents = await _luisController.GetIntents(activity.Text);
                ProcessIntents(intents, activity);

                // return our reply to the user
                var replyMessage = GenerateReply();

                Activity reply = activity.CreateReply(!string.IsNullOrEmpty(replyMessage) ? replyMessage : "Sorry, I couldn't understand that. Try saying \"I need a ride\" or \"Become a driver\"");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private void ProcessIntents(Intent[] intents, Activity activity)
        {
            foreach (var intent in intents.Where(i => i.Score > 0.5))
            {
                ProcessActions(intent.Actions, activity);
            }
        }

        private void ProcessActions(Microsoft.Cognitive.LUIS.Action[] actions, Activity activity)
        {
            foreach (var action in actions)
            {
                if (action.Triggered)
                {
                    ExecuteAction(action.Name, action.Parameters, activity);
                }
                else
                {

                    AddToReplyQueue(DbController.GetFailureMessage(action.Name));
                }
            }
        }

        private void ExecuteAction(string action, Parameter[] parameters, Activity activity)
        {
            string username = activity.From.Name;

            switch (action)
            {
                case "Find Ride":

                    // Notify all current drivers within N miles that person X needs a ride
                    //  
                    //


                    AddToReplyQueue("");

                    break;

                case "Update Address":

                    var updatedAddress = parameters.Where(p => p.Name == "Address").FirstOrDefault()?.ParameterValues.FirstOrDefault().Entity;

                    if (DbController.UpdateUserAddress(username, updatedAddress))
                    {
                        AddToReplyQueue(updatedAddress.Count() > 0 ?
                        $"Great! Your address has been updated to {updatedAddress}."
                        : DbController.GetFailureMessage(action));
                    }
                    else
                    {
                        AddToReplyQueue("Uh oh. Your address failed to update. Can you please try again in a few moments?");
                    }

                    break;

                case "Become Driver":

                    if (!DbController.IsDriver(username))
                    {
                        DbController.BecomeDriver(username);
                        AddToReplyQueue("You are now a driver! You'll be notified of nearby coworkers looking for rides. Thanks for being awesome.");
                    }
                    else
                    {
                        AddToReplyQueue("You are already a driver. If you'd like to no longer be a driver, say \"Stop being a driver.\"");
                    }

                    break;

                default:
                case "":
                    break;

            }

            Console.WriteLine(action);
        }

        private void AddToReplyQueue(string message)
        {
            _replyMessages.Enqueue(message);
        }

        private string GenerateReply()
        {
            var reply = string.Join("\n", _replyMessages.ToArray());

            _replyMessages.Clear();

            return reply;
        }

    }
}