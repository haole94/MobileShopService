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
    public class CartController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();

        // Cần username trong header
        [HttpGet]
        [Route("api/cart/getNewestCart")]
        public HttpResponseMessage GetNewestCart()
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

            Cart cart = ctx.Carts.Where(t => t.Username == username).ToList().LastOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (cart.Checked == 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            List<Item> list = ctx.Items.Where(t => t.ID_Cart == cart.ID).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<ItemModel> result = new List<ItemModel>();

            foreach (Item i in list)
            {
                Product p = ctx.Products.Where(t => t.ID == i.ID_Product).FirstOrDefault();

                ItemModel item = new ItemModel()
                {
                    ID_Product = i.ID_Product,
                    ID_Cart = i.ID_Cart,
                    Quantity = i.Quantity,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image
                };

                result.Add(item);
            }

            if (result.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // Đặt hàng
        // Cần username trong header
        [HttpPost]
        [Route("api/cart/checkout")]
        public HttpResponseMessage CheckOut([FromBody]CartModel req)
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

            Cart cart = ctx.Carts.Where(t => t.Username == username).ToList().LastOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (cart.Checked == 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            cart.PurchasedDate = req.PurchasedDate;
            cart.Receiver = req.Receiver;
            cart.Address = req.Address;
            cart.Phone = req.Phone;
            cart.Email = req.Email;
            cart.Checked = 1;

            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // Ngày đặt hàng
        // Cần username trong header
        // 0: admin chưa xác nhận, -1: admin hủy đơn hàng, 1: admin đồng ý đặt hàng
        [HttpGet]
        [Route("api/cart/getHistoryCart")]
        public HttpResponseMessage GetHistoryCart()
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

            List<Cart> list = ctx.Carts.Where(t => t.Username == username && t.Checked == 1).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<List<Cart>, List<CartModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // Lịch sử mua hàng
        // Cần username trong header
        [HttpGet]
        [Route("api/cart/getHistoryItem")]
        public HttpResponseMessage GetHistoryItem([FromUri]string date)
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

            Cart cart = ctx.Carts.Where(t => t.Username == username && t.PurchasedDate == date).FirstOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<Item> list = ctx.Items.Where(t => t.ID_Cart == cart.ID).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<ItemModel> result = new List<ItemModel>();

            foreach (Item i in list)
            {
                Product p = ctx.Products.Where(t => t.ID == i.ID_Product).FirstOrDefault();

                ItemModel item = new ItemModel()
                {
                    ID_Product = i.ID_Product,
                    ID_Cart = i.ID_Cart,
                    Quantity = i.Quantity,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image
                };

                result.Add(item);
            }

            if (result.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // Admin only 
        // Lịch sử mua hàng
        // Cần username trong header
        [HttpGet]
        [Route("api/cart/getHistoryItemAdmin")]
        public HttpResponseMessage GetHistoryItemAdmin([FromUri]string date, [FromUri]string user)
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

            Cart cart = ctx.Carts.Where(t => t.Username == user && t.PurchasedDate == date).FirstOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<Item> list = ctx.Items.Where(t => t.ID_Cart == cart.ID).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<ItemModel> result = new List<ItemModel>();

            foreach (Item i in list)
            {
                Product p = ctx.Products.Where(t => t.ID == i.ID_Product).FirstOrDefault();

                ItemModel item = new ItemModel()
                {
                    ID_Product = i.ID_Product,
                    ID_Cart = i.ID_Cart,
                    Quantity = i.Quantity,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image
                };

                result.Add(item);
            }

            if (result.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // Admin only
        [HttpGet]
        [Route("api/cart/getAll0")]
        public HttpResponseMessage GetAll0()
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

            List<Cart> list = ctx.Carts.Where(t => t.ConfirmedByAdmin == 0 && t.Checked == 1).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<List<Cart>, List<CartModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // Admin only
        [HttpGet]
        [Route("api/cart/getAll1")]
        public HttpResponseMessage GetAll1()
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

            List<Cart> list = ctx.Carts.Where(t => (t.ConfirmedByAdmin == 1 || t.ConfirmedByAdmin == -1) && t.Checked == 1).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<List<Cart>, List<CartModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // Admin only
        [HttpGet]
        [Route("api/cart/getCart")]
        public HttpResponseMessage GetCart([FromUri]string id)
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

            int intId = int.Parse(id);

            Cart cart = ctx.Carts.Where(t => t.ID == intId && t.Checked == 1).FirstOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<Cart, CartModel>(cart);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // Admin only
        [HttpPut]
        [Route("api/cart/updateStatus")]
        public HttpResponseMessage GetCart([FromUri]string date, [FromUri]string user, [FromBody]int status)
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

            Cart cart = ctx.Carts.Where(t => t.Username == user && t.PurchasedDate == date && t.Checked == 1).FirstOrDefault();

            if (cart == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            cart.ConfirmedByAdmin = status;

            ctx.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /*
        // Dummy code
        [HttpPost]
        [Route("api/cart/add")]
        public HttpResponseMessage Add([FromBody]CartModel req)
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

            Mapper.CreateMap<CartModel, Cart>();
            var data = Mapper.Map<CartModel, Cart>(req);

            ctx.Carts.Add(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [Route("api/cart/get")]
        public HttpResponseMessage Get([FromUri]string id)
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
            var data = ctx.Carts.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<Cart, CartModel>(data);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/cart/getAll")]
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

            List<Cart> list = ctx.Carts.ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Cart, CartModel>();
            var result = Mapper.Map<List<Cart>, List<CartModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpDelete]
        [Route("api/cart/delete")]
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
            var data = ctx.Carts.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            ctx.Carts.Remove(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [Route("api/cart/update")]
        public HttpResponseMessage Update([FromUri]string id, [FromBody]CartModel req)
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
            var data = ctx.Carts.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            //data.PurchasedDate = data.PurchasedDate;
            data.Receiver = req.Receiver;
            data.Address = req.Address;
            data.Phone = req.Phone;
            data.Email = req.Email;
            //data.Confirmed = req.Confirmed;
            //data.Username = req.Username;

            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        */
    }
}
