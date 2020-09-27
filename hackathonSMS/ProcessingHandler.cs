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
using Flurl;
using Flurl.Http;


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
            var resultList = new List<ReturnTypes>();
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
                    await updateTwitter(JsonConvert.SerializeObject(resultList));
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

        private static ReturnTypes addToList(System.Data.IDataReader results)
        {
            return new ReturnTypes { theUser = results["name"], areaOnFile = results["address"]};
        }

        private async Task<bool> updateTwitter (string request)
        {
            try
            {
            // string completeResponse = ;
            dynamic allRepos = await "http://52.91.152.144:3000/sendUpdate".PostJsonAsync(new {message = $"SHELL HACKS EXAMPLE ONLY - User Has Requested Response {request}"});
            return true;
            }
            catch(Exception e)
            {
               Console.WriteLine("thats an error");
               throw e;
            }
        }
    }
}
