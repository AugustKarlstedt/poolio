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
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var intents = await _luisController.GetIntents(activity.Text);
                ProcessIntents(intents);                

                // return our reply to the user
                Activity reply = activity.CreateReply(GenerateReply());
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

        private void ProcessIntents(Intent[] intents)
        {
            foreach (var intent in intents)
            {
                ProcessActions(intent.Actions);
            }
        }

        private void ProcessActions(Microsoft.Cognitive.LUIS.Action[] actions)
        {
            foreach (var action in actions)
            {
                if (action.Triggered)
                {
                    ExecuteAction(action.Name, action.Parameters);
                }
                else
                {
                    using (var db = new Model.PoolioEntities())
                    {
                        AddToReplyQueue(db.ActionFailureMessages.Where(afm => afm.Name == action.Name).FirstOrDefault().FailureMessage);
                    }
                }
            }
        }

        private void ExecuteAction(string action, Parameter[] parameters)
        {
            switch (action)
            {                
                case "Find Ride":

                    break;

                case "Update Address":

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