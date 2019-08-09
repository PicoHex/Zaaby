﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zaabee.Extensions;
using Zaabee.NewtonsoftJson;
using Zaaby.Abstractions;

namespace Zaaby.Client
{
    public class ZaabyClient
    {
        /// <summary>
        /// Key is interface's NameSpace
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<HttpClient>> HttpClients =
            new ConcurrentDictionary<string, List<HttpClient>>();

        /// <summary>
        /// Key is interface's NameSpace
        /// </summary>
        /// <param name="baseUrls"></param>
        public ZaabyClient(Dictionary<string, List<string>> baseUrls)
        {
            if (baseUrls.Any(kv =>
                kv.Value?.Count == 0 || (kv.Value ?? throw new Exception()).Any(string.IsNullOrWhiteSpace)))
                throw new ArgumentException(nameof(baseUrls));

            var urls = baseUrls
                .SelectMany(kv => kv.Value.Select(v => new {Url = v.Trim(), Namespace = kv.Key}))
                .GroupBy(p => p.Url);

            foreach (var datas in urls)
            {
                foreach (var data in datas)
                {
                    var httpClients = HttpClients.GetOrAdd(data.Namespace, key => new List<HttpClient>());
                    httpClients.Add(new HttpClient {BaseAddress = new Uri(datas.Key)});
                }
            }
        }

        public T GetService<T>()
        {
            return DispatchProxy.Create<T, InvokeProxy<T>>();
        }

        public class InvokeProxy<T> : DispatchProxy
        {
            private readonly Type _type;
            private readonly HttpClient _client;

            public InvokeProxy()
            {
                _type = typeof(T);
                if (string.IsNullOrEmpty(_type.Namespace))
                    throw new ZaabyException($"{_type}'s namespace is null or empty.");
                if (!HttpClients.ContainsKey(_type.Namespace))
                    throw new ZaabyException($"{_type} has not set the url.");

                var clients = HttpClients[_type.Namespace];
                var random = new Random();
                _client = HttpClients[_type.Namespace][random.Next(0, clients.Count - 1)];
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
//                var stream = new MemoryStream();
//                if (args.Any())
//                    ProtobufHelper.Serialize(stream, args[0]);
//                stream.Seek(0, SeekOrigin.Begin);
//                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
//                    $"/{_type.FullName.Replace('.', '/')}/{targetMethod.Name}")
//                {
//                    Content = new StreamContent(stream)
//                };
//                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
//                httpRequestMessage.Headers.Add("Accept", "application/x-protobuf");
//                var httpResponseMessage = _client.SendAsync(httpRequestMessage).Result;
//                var result = httpResponseMessage.Content.ReadAsStreamAsync().Result;
//                if (httpResponseMessage.IsSuccessStatusCode)
//                    return ProtobufHelper.Deserialize(result, targetMethod.ReturnType);

                if (string.IsNullOrEmpty(_type.FullName))
                    throw new ZaabyException($"{_type}'s full name is null or empty.");
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
                    $"/{_type.FullName.Replace('.', '/')}/{targetMethod.Name}")
                {
                    Content = new StringContent(args.Any() ? args[0].ToJson() : "", Encoding.UTF8,
                        "application/json"),
                    Headers = {{"Accept", "application/json"}}
                };

                var httpResponseMessage = _client.SendAsync(httpRequestMessage).RunSync();
                var result = httpResponseMessage.Content.ReadAsStringAsync().RunSync();
                if (httpResponseMessage.IsSuccessStatusCode)
                    return string.IsNullOrWhiteSpace(result)
                        ? null
                        : result.FromJson(targetMethod.ReturnType);

                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                    throw result.FromJson<ZaabyException>();
                throw result.FromJson<Exception>();
            }

            private async Task<object> InvokeAsync(MethodInfo targetMethod, IReadOnlyList<object> args)
            {
                if (string.IsNullOrEmpty(_type.FullName))
                    throw new ZaabyException($"{_type}'s full name is null or empty.");
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
                    $"/{_type.FullName.Replace('.', '/')}/{targetMethod.Name}")
                {
                    Content = new StringContent(args.Any() ? args[0].ToJson() : "", Encoding.UTF8,
                        "application/json"),
                    Headers = {{"Accept", "application/json"}}
                };

                var httpResponseMessage = await _client.SendAsync(httpRequestMessage);
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                    return string.IsNullOrWhiteSpace(result)
                        ? null
                        : result.FromJson(targetMethod.ReturnType);

                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                    throw result.FromJson<ZaabyException>();
                throw result.FromJson<Exception>();
            }
        }
    }
}