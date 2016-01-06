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
    public class ManufacturerController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();

        // Admin only
        [HttpPost]
        [Route("api/manufacturer/add")]
        public HttpResponseMessage Add([FromBody]ManufacturerModel req)
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

            string username = headers.GetValues("Username").First();

            if (username != "admin")
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Manufacturer brand = ctx.Manufacturers.Where(t => t.Name == req.Name).FirstOrDefault();
            if (brand != null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Mapper.CreateMap<ManufacturerModel, Manufacturer>();
            var data = Mapper.Map<ManufacturerModel, Manufacturer>(req);

            ctx.Manufacturers.Add(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [Route("api/manufacturer/get")]
        public HttpResponseMessage Get([FromUri]string id)
        {
            int idInt = int.Parse(id);
            var data = ctx.Manufacturers.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Manufacturer, ManufacturerModel>();
            var result = Mapper.Map<Manufacturer, ManufacturerModel>(data);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/manufacturer/getAll")]
        public HttpResponseMessage GetAll()
        {            
            List<Manufacturer> list = ctx.Manufacturers.ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Manufacturer, ManufacturerModel>();
            var result = Mapper.Map<List<Manufacturer>, List<ManufacturerModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        
        // Admin only
        [HttpDelete]
        [Route("api/manufacturer/delete")]
        public HttpResponseMessage Delete([FromUri]string id)
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

            int idInt = int.Parse(id);
            var data = ctx.Manufacturers.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            ctx.Manufacturers.Remove(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // Admin only
        [HttpPut]
        [Route("api/manufacturer/update")]
        public HttpResponseMessage Update([FromUri]string id, [FromBody]ManufacturerModel req)
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

            int idInt = int.Parse(id);
            var data = ctx.Manufacturers.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            data.Name = req.Name;

            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
