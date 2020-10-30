using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Results
{
    public class FileFromStreamResult : IHttpActionResult
    {
        private readonly MemoryStream _filePath;
        private readonly string _contentType;

        public FileFromStreamResult(MemoryStream stream, string contentType)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _filePath = stream;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(_filePath)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);

            return Task.FromResult(response);
        }
    }
}
