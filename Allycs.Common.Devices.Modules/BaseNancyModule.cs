using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Modules
{
    using Allycs.Core;
    using Nancy;
    using Nancy.Json;
    using Newtonsoft.Json;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class BaseNancyModule : NancyModule
    {
        private readonly Stopwatch st = new Stopwatch();
        //private readonly IMemberTokenService _memberTokenService;

        public BaseNancyModule() : this("base", "api", "")
        {
        }

        public BaseNancyModule(string baseUrl, string type, string modulePath) : base("/allycs/server/" + baseUrl + "/v1/" + type + modulePath)
        {
            CurrentRequestId = ObjectId.NewId();
            Before += ShowRequest;
            Before += CheckClientIP;
            After += WithRequestId;
            OnError.AddItemToEndOfPipeline((ctx, ex) =>
            {
                if (ex is Nancy.ModelBinding.ModelBindingException)
                {
                    var mbEx = ex as Nancy.ModelBinding.ModelBindingException;
                    Log.Logger?.Warning(mbEx, "发生数据参数不符合格式要求的情况");
                    return BadRequest("数据参数不符合格式要求");
                }

                if (ex is ArgumentNullException)
                {
                    var anEx = ex as System.ArgumentNullException;
                    Log.Logger?.Warning(anEx, $"{anEx.ParamName} 不能为空");

                    return BadRequest($"{anEx.ParamName} 不能为空");
                }
                Log.Logger?.Warning(ex, "服务端发生异常");
                return ServerError("服务端发生异常");
                //return null;
            });
        }

        private Response ShowRequest(NancyContext ctx)
        {
            var sb = new StringBuilder();
            sb.Append("\n------------ Request:").Append(CurrentRequestId).Append("------------\n");
            sb.Append("Request Url:").Append(Request.Url).Append("\n");
            foreach (var h in Request.Headers)
            {
                sb.Append("Header ").Append(h.Key).Append(":\t").Append(h.Value.FirstOrDefault()).Append("\n");
            }
            if (ctx.Request.Headers.ContentType != null && Json.IsJsonContentType(ctx.Request.Headers.ContentType.ToString()))
            {
                Stream bodyStream = new MemoryStream();
                ctx.Request.Body.CopyTo(bodyStream);
                bodyStream.Position = 0;
                string bodyText;
                using (var bodyReader = new StreamReader(bodyStream))
                {
                    bodyText = bodyReader.ReadToEnd();
                }
                sb.Append("^^^^^^^^^^^^^^^^ JSON ^^^^^^^^^^^^^^^^\n");
                sb.Append(bodyText);
            }
            else
            if (ctx.Request.Files.Count() > 0)
            {
                sb.Append("^^^^^^^^^^^^^^^^ files ^^^^^^^^^^^^^^^^\n");
                foreach (var item in ctx.Request.Files)
                    sb.Append(item.Name);
            }
            else
            if (ctx.Request.Body != null && ctx.Request.Body.Length != 0)
            {
                Stream bodyStream = new MemoryStream();
                ctx.Request.Body.CopyTo(bodyStream);
                bodyStream.Position = 0;
                string bodyText;
                using (var bodyReader = new StreamReader(bodyStream))
                {
                    bodyText = bodyReader.ReadToEnd();
                }
                sb.Append("^^^^^^^^^^^^^^^^ body ^^^^^^^^^^^^^^^^\n");
                sb.Append(bodyText);
            }
            var logContent = sb.ToString();
            Console.Write(logContent);
            ctx.Items.Add("request-log", logContent);
            return null;
        }

        /// <summary>
        /// 当前请求的唯一Id
        /// </summary>
        protected string CurrentRequestId
        {
            get;
        }

        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        protected string ClientIP
        {
            get;
            private set;
        }

        /// <summary>
        /// 为响应附加当前请求的Id
        /// </summary>
        /// <param name="ctx"></param>
        private void WithRequestId(NancyContext ctx)
        {
            ctx.Response.Headers.Add("x-request-id", CurrentRequestId);
            if (st.IsRunning)
            {
                st.Stop();
                if (st.ElapsedMilliseconds > 2000)
                {
                    Log.Logger?.Warning("the request executes for {microSeconds} milliseconds: {method} {request}", st.ElapsedMilliseconds, Request.Method.ToUpper(), Request.Url.ToString());
                }
                else if (st.ElapsedMilliseconds > 500)
                {
                    Log.Logger?.Information("the request executes for {microSeconds} milliseconds: {method} {request}", st.ElapsedMilliseconds, Request.Method.ToUpper(), Request.Url.ToString());
                }
            }
        }

        /// <summary>
        /// 检查客户端的IP地址，并写入属性中
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private Response CheckClientIP(NancyContext ctx)
        {
            var clientIP = ctx.Request.UserHostAddress;
            //TODO: 客户端的IP不一定是该值，因为Server的前端可能有反向代理服务器。需要根据不同的场景处理

            var hasRealIP = ctx.Request.Headers.Keys.Contains("X-Real-IP");
            if (hasRealIP)
            {
                clientIP = ctx.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            ctx.Parameters.ClientIP = ClientIP = clientIP;
            return null;
        }

        /// <summary>
        /// 200 - 正常
        /// </summary>
        /// <param name="model">返回的数据</param>
        /// <returns>响应</returns>
        protected Response Ok(object model)
        {
            return Response.AsJson(model);
        }

        /// <summary>
        /// 201 - 创建成功
        /// </summary>
        /// <param name="createdModel">创建成功的对象数据</param>
        /// <param name="url">创建的资源访问地址</param>
        /// <returns>响应</returns>
        protected Response Created(object createdModel, string url)
        {
            return Response.AsJson(createdModel, HttpStatusCode.Created).WithHeader("Location", url);
        }

        /// <summary>
        /// 202 - 已接受，异步处理中
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response Accepted(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.Accepted);
        }

        /// <summary>
        /// 204 - 无内容，资源有空表示
        /// </summary>
        /// <returns>响应</returns>
        protected Response NoContent()
        {
            return new Response { StatusCode = HttpStatusCode.NoContent };
        }

        /// <summary>
        /// 301 - 永久跳转到新地址
        /// </summary>
        /// <param name="url">新的地址</param>
        /// <returns>响应</returns>
        protected Response PermanentRedirect(string url)
        {
            return Response.AsRedirect(url, Nancy.Responses.RedirectResponse.RedirectType.Permanent);
        }

        /// <summary>
        /// 303 - 跳转到新地址
        /// </summary>
        /// <param name="url">要跳转的到地址</param>
        /// <returns>响应</returns>
        protected Response Redirect(string url)
        {
            return Response.AsRedirect(url, Nancy.Responses.RedirectResponse.RedirectType.SeeOther);
        }

        /// <summary>
        /// 400 - 请求无效
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response BadRequest(string message = "请提供正确的数据")
        {
            return Response.AsJson(new { message, errors = ModelValidationResult.FormattedErrors }, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 401 - 请求需要用户验证
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response Unauthorized(string message = "")
        {
            return Response.AsJson(new { message }, HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// 403 - 已经理解了请求，但是拒绝服务或这种请求的访问是不允许的
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response Forbidden(string message = "")
        {
            return Response.AsJson(new { message }, HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// 404 - 没有发现该资源
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response NotFound(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// 406 - 服务端不支持所需表示
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response NotAcceptable(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.NotAcceptable);
        }

        /// <summary>
        /// 409 - 冲突
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response Conflict(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.Conflict);
        }

        /// <summary>
        /// 412 - 前置条件失败（如执行条件更新时的冲突）
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response PreconditionFailed(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.PreconditionFailed);
        }

        /// <summary>
        /// 415 - 不支持的媒体类型
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response UnsupportedMediaType(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.UnsupportedMediaType);
        }

        /// <summary>
        /// 422 - 服务器端无法处理
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response Unprocessable(string message)
        {
            return Response.AsJson(new { message }, HttpStatusCode.UnprocessableEntity);
        }

        /// <summary>
        /// 500 - 服务器端发生异常，开发者应该尽一切可能避免这个错误
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response ServerError(string message = "")
        {
            return Response.AsJson(new { message }, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        ///
        /// 503 - 服务端不可用
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>响应</returns>
        protected Response ServiceUnavailable(string message = "")
        {
            return Response.AsJson(new { message }, HttpStatusCode.ServiceUnavailable);
        }
    }
}
