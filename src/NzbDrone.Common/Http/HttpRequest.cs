using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Extensions;

namespace NzbDrone.Common.Http
{
    public class HttpRequest
    {
        public HttpRequest(string url, HttpAccept httpAccept = null)
        {
            Url = new HttpUri(url);
            Headers = new HttpHeader();
            ConnectionKeepAlive = true;
            AllowAutoRedirect = true;
            Cookies = new Dictionary<string, string>();

            if (!RuntimeInfo.IsProduction)
            {
                AllowAutoRedirect = false;
            }

            if (httpAccept != null)
            {
                Headers.Accept = httpAccept.Value;
            }
        }

        public HttpUri Url { get; set; }
        public HttpMethod Method { get; set; }
        public HttpHeader Headers { get; set; }
        public Encoding Encoding { get; set; }
        public IWebProxy Proxy { get; set; }
        public byte[] ContentData { get; set; }
        public string ContentSummary { get; set; }
        public bool SuppressHttpError { get; set; }
        public bool UseSimplifiedUserAgent { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public bool ConnectionKeepAlive { get; set; }
        public bool LogResponseContent { get; set; }
        public Dictionary<string, string> Cookies { get; private set; }
        public bool StoreRequestCookie { get; set; }
        public bool StoreResponseCookie { get; set; }
        public TimeSpan RequestTimeout { get; set; }
        public TimeSpan RateLimit { get; set; }

        public override string ToString()
        {
            return ToString();
        }

        public string ToString(bool includeMethod = true, bool includeSummary = true)
        {
            var builder = new StringBuilder();

            if (includeMethod)
            {
                builder.AppendFormat("Req: [{0}] ", Method);
            }

            builder.Append(Url);

            if (includeSummary && ContentSummary.IsNotNullOrWhiteSpace())
            {
                builder.Append(": ");
                builder.Append(ContentSummary);
            }

            return builder.ToString();
        }

        public void SetContent(byte[] data)
        {
            ContentData = data;
        }

        public void SetContent(string data)
        {
            if (Encoding != null)
            {
                ContentData = Encoding.GetBytes(data);
            }
            else
            {
                var encoding = HttpHeader.GetEncodingFromContentType(Headers.ContentType);
                ContentData = encoding.GetBytes(data);
            }
        }

        public string GetContent()
        {
            if (Encoding != null)
            {
                return Encoding.GetString(ContentData);
            }
            else
            {
                var encoding = HttpHeader.GetEncodingFromContentType(Headers.ContentType);
                return encoding.GetString(ContentData);
            }
        }

        public void AddBasicAuthentication(string username, string password)
        {
            var authInfo = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"));

            Headers.Set("Authorization", "Basic " + authInfo);
        }
    }
}
