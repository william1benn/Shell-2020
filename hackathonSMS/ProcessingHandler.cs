using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Newtonsoft.Json;
using Hackathon.Base;
using shortid;
using Hackathon.Types;
using RestSharp;


[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Hackathon
{
    public class ProcessingHandler
    {
        public async Task<object> SMSProcessing(SNSEvent evnt, ILambdaContext context)
        {
          try
            {
           foreach (var record in evnt.Records)
            {
            
            var snsRecord = record.Sns;
            Console.WriteLine($"[{record.EventSource} {snsRecord.Timestamp}] Message = {snsRecord.Message}");

	        Dictionary<string, string> MessageDictionary =                 
            JsonConvert.DeserializeObject<Dictionary<string, string>>(snsRecord.Message);

            var connection = new Connection();
            var resultList = new List<RegisterTypes>();
            string telephone = MessageDictionary["originationNumber"];

            var results = connection.getData($"SELECT * FROM CustomerI_Info WHERE telephone='{telephone}'");
            var checker = (System.Data.Common.DbDataReader) results;//cast a type

                    if(!checker.HasRows)
                    {
                    var messageBodyInformation = MessageDictionary["messageBody"];

                    Dictionary<string, string> ParseMessageForDb =                 
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(messageBodyInformation);

                    string name = ParseMessageForDb["Name"];
                    string address = ParseMessageForDb["Address"];

                    await insertData(name,telephone,address);
                    return "done";
                    } 
                else
                {
                    while (results.Read())
                    {
                        resultList.Add(addToList(results));
                    }
                    Console.WriteLine(JsonConvert.SerializeObject(resultList));
                    updateTwitter(JsonConvert.SerializeObject(resultList));
                    return JsonConvert.SerializeObject(resultList);
                }
            // Console.WriteLine("Some Number" + messageBodyInformation);       
            // Console.WriteLine("Some Number" + ParseMessageForDb["Name"]);
            // Console.WriteLine("Some Number" + ParseMessageForDb["Address"]);
            // Console.WriteLine("Some Number" + MessageDictionary["originationNumber"]);

            }
            return "this doesnt work"; 
            }catch (Exception ex)
            {
                Console.WriteLine("we have an error");
                throw ex; 
            }
        }
         private async Task insertData(string name, string tele, string address)
        {
                string uuidGen = ShortId.Generate(true);
                var conn = new Connection();
                // string query = $"INSERT INTO CustomerI_Info SET id= '{uuidGen}', name= '{name}', address ='{address}', telephone = '{tele}";
                Console.WriteLine($"INSERT INTO CustomerI_Info SET id= '{uuidGen}', name= '{name}', address ='{address}', telephone = '{tele}'");
                conn.getData($"INSERT INTO CustomerI_Info SET id= '{uuidGen}', name= '{name}', address ='{address}', telephone = '{tele}'");
                await Task.CompletedTask;
        }

        private static RegisterTypes addToList(System.Data.IDataReader results)
        {
            return new RegisterTypes { name = results["name"], phone = results["telephone"], address = results["address"]};
        }

        private static bool updateTwitter (string request)
        {
            try
            {
            string sendUpdate = $"https://api.twitter.com/1.1/statuses/update.json?status={request}";
            var client = new RestClient(sendUpdate);
            client.Timeout = -1;
            var arequest = new RestRequest(Method.POST);
            arequest.AddHeader("Authorization", "OAuth oauth_consumer_key=\"bRgFxYJXqKNOAkOp0vBVc9JBB\",oauth_token=\"1218675251472715776-gU2WslZxdKksP6DjvLez1ZdboOMStV\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"1601179380\",oauth_nonce=\"378Kqr4TI2Z\",oauth_version=\"1.0\",oauth_signature=\"pzhT0N0XeUd%2BI3OezKC53N%2F3utA%3D\"");
            IRestResponse response = client.Execute(arequest);
            Console.WriteLine(response.Content);
            return true;
            }
            catch(Exception e)
            {
               Console.WriteLine("man what are you trying to do fool");
               throw e;
            }
        }
    }
}
