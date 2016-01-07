using AutoMapper;
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
    public class AccountController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();

        private bool IsValid(string username)
        {
            var acc = ctx.Accounts.Where(t => t.Username == username).FirstOrDefault();
            if (acc != null)
                return false;
            return true;
        }

        [HttpPost]
        [Route("api/account/add")]
        public HttpResponseMessage Add([FromBody]AccountModel req)
        {
            if (!IsValid(req.Username))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Account acc = new Account()
            {
                Username = req.Username,
                Password = req.Password,
                Name = req.Name,
                Birthday = req.Birthday,
                Address = req.Address,
                Phone = req.Phone
            };

            ctx.Accounts.Add(acc);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [Route("api/account/get")]
        public HttpResponseMessage Get([FromUri]string username)
        {
            // Check token
            var headers = Request.Headers;
            if (headers.Contains("Token"))
            {
                if (!GLOBAL.CHECK_TOKEN(headers.GetValues("Token").First()))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            // End

            var data = ctx.Accounts.Where(t => t.Username == username).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Account, AccountModel>();
            AccountModel result = Mapper.Map<Account, AccountModel>(data);
            result.Password = null;
            result.Token = null;

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Route("api/account/addAccountFB")]
        public HttpResponseMessage AddAccountFB()
        {
            var headers = Request.Headers;
            var name = "_" + headers.GetValues("id").First();

            if (!IsValid(name))
                return Request.CreateResponse(HttpStatusCode.OK);

            Account acc = new Account();
            acc.Username = name;

            ctx.Accounts.Add(acc);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /*
        // Admin only
        [HttpGet]
        [Route("api/account/getAll")]
        public HttpResponseMessage GetAll()
        {
            // Check token
            var headers = Request.Headers;
            if (headers.Contains("Token"))
            {
                if (!GLOBAL.CHECK_TOKEN(headers.GetValues("Token").First()))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            // End

            List<Account> list = ctx.Accounts.ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Account, AccountModel>();
            var result = Mapper.Map<List<Account>, List<AccountModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // Admin only
        [HttpDelete]
        [Route("api/account/delete")]
        public HttpResponseMessage Delete([FromUri]string username)
        {
            // Check token
            var headers = Request.Headers;
            if (headers.Contains("Token"))
            {
                if (!GLOBAL.CHECK_TOKEN(headers.GetValues("Token").First()))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            // End

            var data = ctx.Accounts.Where(t => t.Username == username).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            ctx.Accounts.Remove(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }*/

        [HttpPut]
        [Route("api/account/update")]
        public HttpResponseMessage Update([FromUri]string username, [FromBody]AccountModel req)
        {
            // Check token
            var headers = Request.Headers;
            if (headers.Contains("Token"))
            {
                if (!GLOBAL.CHECK_TOKEN(headers.GetValues("Token").First()))
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            // End

            var data = ctx.Accounts.Where(t => t.Username == username).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            data.Password = req.Password;
            data.Name = req.Name;
            data.Birthday = req.Birthday;
            data.Address = req.Address;
            data.Phone = req.Phone;

            int result = ctx.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
