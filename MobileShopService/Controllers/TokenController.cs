using MobileShopService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MobileShopService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class TokenController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();

        private string CreateToken(string username)
        {
            Random ran = new Random();
            String keySource = username + DateTime.Now + ran.Next(9999);
            byte[] tokenByte = System.Text.Encoding.UTF8.GetBytes(keySource);
            return System.Convert.ToBase64String(tokenByte);
        }

        [HttpPost]
        [Route("api/token/get")]
        public HttpResponseMessage GetToken([FromBody]RequestObj request)
        {
            List<Account> list = ctx.Accounts.ToList();

            Account acc = list.Where(t => t.Username == request.username).FirstOrDefault();

            if (acc == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "No data???");

            if (String.Compare(request.password, acc.Password) == 0)
            {
                string token = CreateToken(acc.Username);

                acc.Token = token;
                ctx.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, token);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [Route("api/token/attachTokenFB")]
        public HttpResponseMessage AttachTokenFB([FromUri]string id)
        {
            var headers = Request.Headers;
            if (!headers.Contains("TokenFB"))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            string name = "_" + id;
            List<Account> list = ctx.Accounts.ToList();

            Account acc = list.Where(t => t.Username == name).FirstOrDefault();

            if (acc == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "No data???");

            acc.Token = headers.GetValues("TokenFB").First();
            ctx.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("api/token/delete")]
        public HttpResponseMessage RemoveToken()
        {
            // Check token
            var headers = Request.Headers;
            string token = headers.GetValues("Token").First();

            if (headers.Contains("Token"))
            {
                if (!GLOBAL.CHECK_TOKEN(token))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            // End

            List<Account> list = ctx.Accounts.ToList();

            Account acc = list.Where(t => t.Token == token).FirstOrDefault();

            if (acc == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            acc.Token = null;
            int result = ctx.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
