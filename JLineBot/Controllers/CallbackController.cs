using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;

namespace JLineBot.Controllers
{
    /// <summary>
    /// 當有LineBot接收到訊息時 Line 呼叫這個 CallbackController
    /// 這個Controllert把接收到訊息在傳遞給別人
    /// </summary>
    public class CallbackController : ApiController
    {
        private void WriteDataBase(string message)
        {
            string sql = "INSERT INTO Message(Content) VALUES ('{0}')";

            try
            {
                string connection =
                    ConfigurationManager.ConnectionStrings["JLineBot"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open(); 
                    sql = string.Format(sql,message);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery(); 
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {            
              
            }          
        }
        private async void SenMessage(string message, string mid)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders
               .Add("X-Line-ChannelID", "1472523739");                                   //ChannelID
            client.DefaultRequestHeaders
              .Add("X-Line-ChannelSecret", "1fc2a219956bc4ceb440f536cd77f175");         //ChannelSecret
            client.DefaultRequestHeaders
              .Add("X-Line-Trusted-User-With-ACL", "u00930c040fefa011a04d701967919604");//MID
            
            var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",//Line 固定
            new
            {
                to = new[] { mid },
                toChannel = "1383378250",                       //Line 固定
                eventType = "138311608800106203",               //Line 固定
                content = new
                {
                    contentType = 1,
                    toType = 1,
                    text = string.Format("安安「{0}:{1}」你好", mid, message)
                }
            });        
        }

        public async Task<HttpResponseMessage> Post()
        {
            var contentString = await Request.Content.ReadAsStringAsync();

            //寫到資料庫
            WriteDataBase(contentString);
            //return new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            dynamic contentObj = JsonConvert.DeserializeObject(contentString);
            var result = contentObj.result[0];

            var client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders
                  .Add("X-Line-ChannelID", "1472523739");                                   //ChannelID
                client.DefaultRequestHeaders
                  .Add("X-Line-ChannelSecret", "1fc2a219956bc4ceb440f536cd77f175");         //ChannelSecret
                client.DefaultRequestHeaders
                  .Add("X-Line-Trusted-User-With-ACL", "u00930c040fefa011a04d701967919604");//MID

                //原始程式from http://note.jhpeng.com/Aspnet_LineBot/
                //var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",
                //    new {
                //      to = new[] { result.content.from },
                //      toChannel = "1383378250",
                //      eventType = "138311608800106203",
                //      content = new {
                //        contentType = 1,
                //        toType = 1,
                //        text = $"安安「{result.content.text}」你好"
                //      }
                //    });
                // System.Diagnostics.Debug.WriteLine(await res.Content.ReadAsStringAsync());

                var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",//Line 固定
                  new
                  {
                      to = new[] { result.content.from },
                      toChannel = "1383378250",                       //Line 固定
                      eventType = "138311608800106203",               //Line 固定
                      content = new
                      {
                          contentType = 1,
                          toType = 1,
                          text = string.Format("安安「{0}:{1}」你好", result.content.from,result.content.text)
                      }
                  });

                //if success => {"failed":[],"messageId":"1468348626465","timestamp":1468348626465,"version":1}
                System.Diagnostics.Debug.WriteLine(await res.Content.ReadAsStringAsync());
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        //public async Task<HttpResponseMessage> Post()
        //{
        //    var contentString = await Request.Content.ReadAsStringAsync();

        //    //寫到資料庫
        //    //WriteDataBase(contentString);

        //    dynamic contentObj = JsonConvert.DeserializeObject(contentString);
        //    var result = contentObj.result[0];
            
        //    try
        //    {
        //        SenMessage(result.content.text, result.content.from);
        //        //System.Diagnostics.Debug.WriteLine(await res.Content.ReadAsStringAsync());
        //        return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(e);
        //        return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        //    }
        //}
    }
}
