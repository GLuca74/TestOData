using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using TestODataModels;

namespace TestODataClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            City city = new City() { CityID=Guid.NewGuid(), CityName="Madrid"};

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = new TimeSpan(1, 0, 0);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost.fiddler:7200/$batch"))
            {
                request.Headers.Add("OData-Version", "4.0");
                request.Headers.Add("OData-MaxVersion", "4.0");
                request.Headers.Add("Accept", "multipart/mixed");
                request.Headers.Add("Accept-Charset", "UTF-8");
                request.Headers.Add("Connection", "Keep-Alive");

                string batchBoundary = "batch_" + Guid.NewGuid().ToString();
                var multipartContent = new MultipartContent("mixed", batchBoundary);

                multipartContent.Headers.Remove("Content-Type");
                multipartContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/mixed; boundary=" + batchBoundary);

                string changesetBoundary = "changeset_" + Guid.NewGuid().ToString();
                var curMultipartContent = new MultipartContent("mixed", changesetBoundary);
                curMultipartContent.Headers.Remove("Content-Type");
                curMultipartContent.Headers.Remove("Content-Length");
                curMultipartContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/mixed; boundary=" + changesetBoundary);

                int contentID = 1;
                var obj = JsonSerializer.Serialize(city);
                var objFull = "POST /Cities HTTP/1.1" + Environment.NewLine;
                objFull = objFull + "OData-Version: 4.0" + Environment.NewLine;
                objFull = objFull + "OData-MaxVersion: 4.0" + Environment.NewLine;
                objFull = objFull + "Content-Type: application/json; odata.metadata=minimal" + Environment.NewLine;
                objFull = objFull + "Accept: application/json;odata.metadata=minimal" + Environment.NewLine;
                objFull = objFull + "Accept-Charset: UTF-8" + Environment.NewLine;
                objFull = objFull + Environment.NewLine;
                objFull = objFull + obj;

                var stringContent = new StringContent(objFull);
                stringContent.Headers.Remove("Content-Type");
                stringContent.Headers.TryAddWithoutValidation("Content-Type", "application/http");

                stringContent.Headers.Add("Content-Transfer-Encoding", "binary");
                stringContent.Headers.Add("Content-ID", contentID.ToString());
                contentID++;
                curMultipartContent.Add(stringContent);

                multipartContent.Add(curMultipartContent);

                request.Content = multipartContent;
                using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        HttpRequestException ex = new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);
                        throw ex;
                    }

                    var multipart = await response.Content.ReadAsMultipartAsync(); 
                    foreach (var c in multipart.Contents)
                    {
                        var responses = await c.ReadAsMultipartAsync();
                        foreach (var r in responses.Contents)
                        {
                            var xx1 = await r.ReadAsHttpResponseMessageAsync();
                        }

                    }
                }
            }
        }
    }
}
