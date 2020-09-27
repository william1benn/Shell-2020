using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Hackathon.Base;
using shortid;
using Hackathon.Types;
using Amazon.Lambda.APIGatewayEvents;



namespace Hackathon
{
    public class RegisterHandler
    {
        public async Task<APIGatewayProxyResponse> RegisterUser(LambdaRequest request)
        {
            try
            {
                var userInfo = JsonConvert.DeserializeObject<RegisterTypes>(request.body);
                await insertData(userInfo.name,userInfo.phone,userInfo.address);
                return new APIGatewayProxyResponse
                    {
                        StatusCode = 500,
                        Body = "Thank you for registering",
                        Headers = new Dictionary<string, string>
                { 
                  { "Content-Type", "application/json" }, 
                  { "Access-Control-Allow-Origin", "*" },
                  {"Access-Control-Allow-Methods","OPTIONS,POST"}
                }
                    };

            } catch (Exception ex)
            {
                throw ex; 
            }
        }
         private async Task insertData(object name, object tele, object address)
        {
                string uuidGen = ShortId.Generate(true);
                var conn = new Connection();
                Console.WriteLine($"INSERT INTO CustomerI_Info SET id= '{uuidGen}', name= '{name}', address ='{address}', telephone = '{tele}'");
                conn.getData($"INSERT INTO CustomerI_Info SET id= '{uuidGen}', name= '{name}', address ='{address}', telephone = '{tele}'");
                await Task.CompletedTask;

        }
    }
}
