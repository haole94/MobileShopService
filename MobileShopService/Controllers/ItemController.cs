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
    public class ItemController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();

        private Cart GetValidCart(string username)
        {
            Cart cart = ctx.Carts.Where(t => t.Username == username).ToList().LastOrDefault();

            if (cart != null)
            {
                if (cart.Checked == 0)
                    return cart;
            }

            return null;
        }

        // Cần username trong header
        [HttpPost]
        [Route("api/item/add")]
        public HttpResponseMessage Add([FromBody]ItemModel req)
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

            if (username == "admin")
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Cart cart = GetValidCart(username);

            if (cart == null)
            {
                Cart newCart = new Cart()
                {
                    Checked = 0,
                    ConfirmedByAdmin = 0,
                    Username = username
                };

                ctx.Carts.Add(newCart);
                ctx.SaveChanges();

                cart = ctx.Carts.Where(t => t.Username == username).ToList().LastOrDefault();
            }
            else
            {
                Item temp = ctx.Items.Where(t => t.ID_Cart == cart.ID && t.ID_Product == req.ID_Product).FirstOrDefault();

                if (temp != null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest); 
            }

            Item data = new Item()
            {
                ID_Product = req.ID_Product,
                Quantity = req.Quantity,
                ID_Cart = cart.ID
            };

            ctx.Items.Add(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /*
        [HttpGet]
        [Route("api/item/get")]
        public HttpResponseMessage Get([FromUri]string idProduct, [FromUri]string idCart)
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

            int idProductInt = int.Parse(idProduct);
            int idCartInt = int.Parse(idCart);
            var data = ctx.Items.Where(t => t.ID_Product == idProductInt && t.ID_Cart == idCartInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Item, ItemModel>();
            var result = Mapper.Map<Item, ItemModel>(data);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/item/getAll")]
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

            List<Item> list = ctx.Items.ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Item, ItemModel>();
            var result = Mapper.Map<List<Item>, List<ItemModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }*/


        // Xóa sản phẩm khỏi giỏ hàng, nếu còn sản phẩm trong giỏ thì trả về 0, nếu không còn sản phẩm (giỏ trống) thì trả về 1
        [HttpDelete]
        [Route("api/item/delete")]
        public HttpResponseMessage Delete([FromUri]string idProduct, [FromUri]string idCart)
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

            int idProductInt = int.Parse(idProduct);
            int idCartInt = int.Parse(idCart);
            var data = ctx.Items.Where(t => t.ID_Product == idProductInt && t.ID_Cart == idCartInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Cart cart = ctx.Carts.Where(t => t.ID == idCartInt).FirstOrDefault();
            int nItem = ctx.Items.Where(t => t.ID_Cart == idCartInt).ToList().Count();

            ctx.Items.Remove(data);
            int result = ctx.SaveChanges();

            if (nItem == 1)
            {
                ctx.Carts.Remove(cart);
                ctx.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK, 0);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [Route("api/item/update")]
        public HttpResponseMessage Update([FromBody]ItemModel req)
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

            int idProductInt = req.ID_Product;
            int idCartInt = req.ID_Cart;
            var data = ctx.Items.Where(t => t.ID_Product == idProductInt && t.ID_Cart == idCartInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            data.Quantity = req.Quantity;

            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
