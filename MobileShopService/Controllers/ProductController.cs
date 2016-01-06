using AutoMapper;
using MobileShopService.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MobileShopService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProductController : ApiController
    {
        DienTuEntities ctx = new DienTuEntities();
        Random ran = new Random();
        int nItem = 9;

        // Admin only
        [HttpPost]
        [Route("api/product/add")]
        public HttpResponseMessage Add([FromBody]ProductModel req)
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

            Mapper.CreateMap<ProductModel, Product>();
            var data = Mapper.Map<ProductModel, Product>(req);

            ctx.Products.Add(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [Route("api/product/get")]
        public HttpResponseMessage Get([FromUri]string id)
        {
            int idInt = int.Parse(id);
            var data = ctx.Products.Where(t => t.ID == idInt && t.IsVisible == 1).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Product, ProductModel>();
            var result = Mapper.Map<Product, ProductModel>(data);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }



        // Cần username "admin" trong header
        [HttpGet]
        [Route("api/product/getProductByBrand")]
        public HttpResponseMessage GetProductByBrand([FromUri]string id)
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
            List<Product> list = ctx.Products.Where(t => t.ID_Manufacturer == intId).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Product, ProductModel>();
            var result = Mapper.Map<List<Product>, List<ProductModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        /*private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }*/

        [HttpPut]
        [Route("api/product/uploadImage")]
        public HttpResponseMessage UploadImage([FromUri]int id, [FromBody]ImageModel image)
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

            Product product = ctx.Products.Where(t => t.ID == id).FirstOrDefault();

            if (product == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            byte[] data = Convert.FromBase64String(image.Context);

            product.Image = data;

            ctx.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /*
        // Cần username "admin" trong header
        [HttpGet]
        [Route("api/product/getAll")]
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

            string username = headers.GetValues("Username").First();

            if (username != "admin")
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<Product> list = ctx.Products.ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            Mapper.CreateMap<Product, ProductModel>();
            var result = Mapper.Map<List<Product>, List<ProductModel>>(list);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        */

        private MiniProduct CreateMiniProduct(Product product)
        {
            MiniProduct result = new MiniProduct()
            {
                ID = product.ID,
                Name = product.Name,
                ID_Manufacturer = product.ID_Manufacturer,
                Price = product.Price,
                Image = product.Image
            };

            return result;
        }

        [HttpGet]
        [Route("api/product/get9Random")]
        public HttpResponseMessage Get6Random()
        {
            List<Product> tempList = ctx.Products.Where(t => t.IsVisible == 1).ToList();
            List<Product> list = new List<Product>();
            int count = tempList.Count();

            for (int i = 0; i < 9; i++)
            {
                list.Add(tempList[ran.Next(count)]);
            }

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            List<MiniProduct> result = new List<MiniProduct>();
            foreach (var p in list)
            {
                result.Add(CreateMiniProduct(p));
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/product/getSomeProduct")]
        public HttpResponseMessage GetSomeProduct([FromUri]string p)
        {
            List<Product> list = ctx.Products.Where(t => t.IsVisible == 1).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            int index = int.Parse(p);
            int count = ctx.Products.Where(t => t.IsVisible == 1).Count();
            int nPage = count / nItem;

            if (count % nItem != 0)
                nPage = nPage + 1;

            if (index > nPage || index <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<MiniProduct> result = new List<MiniProduct>();

            if (index == 1)
            {
                for (int i = index; i <= nItem; i++)
                {
                    result.Add(CreateMiniProduct(list[i - 1]));
                }
            }
            else if (index == nPage)
            {
                for (int i = nItem * (index - 1) + 1; i <= count; i++)
                {
                    result.Add(CreateMiniProduct(list[i - 1]));
                }
            }
            else
            {
                for (int i = nItem * (index - 1) + 1; i <= nItem * index; i++)
                {
                    result.Add(CreateMiniProduct(list[i - 1]));
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/product/getSomeProductByBrand")]
        public HttpResponseMessage GetSomeProductByBrand([FromUri]string p, [FromUri]int brandId)
        {
            List<Product> list = ctx.Products.Where(t => t.IsVisible == 1 && t.ID_Manufacturer == brandId).ToList();

            if (list == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            int index = int.Parse(p);
            int count = ctx.Products.Where(t => t.IsVisible == 1 && t.ID_Manufacturer == brandId).Count();
            int nPage = count / nItem;

            if (count % nItem != 0)
                nPage = nPage + 1;

            if (index > nPage || index <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            List<MiniProduct> result = new List<MiniProduct>();

            if (index == 1)
            {
                if (nPage == 1)
                {
                    for (int i = index; i <= count; i++)
                    {
                        result.Add(CreateMiniProduct(list[i - 1]));
                    }
                }
                else
                {
                    for (int i = index; i <= nItem; i++)
                    {
                        result.Add(CreateMiniProduct(list[i - 1]));
                    }
                }
            }
            else if (index == nPage)
            {
                for (int i = nItem * (index - 1) + 1; i <= count; i++)
                {
                    result.Add(CreateMiniProduct(list[i - 1]));
                }
            }
            else
            {
                for (int i = nItem * (index - 1) + 1; i <= nItem * index; i++)
                {
                    result.Add(CreateMiniProduct(list[i - 1]));
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/product/getNumberOfPages")]
        public HttpResponseMessage GetNumberOfPages()
        {
            int count = ctx.Products.Where(t => t.IsVisible == 1).Count();
            int nPage = count / nItem;

            if (count % nItem != 0)
                nPage = nPage + 1;

            List<int> result = new List<int>();
            for (int i = 1; i <= nPage; i++)
                result.Add(i);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/product/getNumberOfPages2")]
        public HttpResponseMessage GetNumberOfPages2([FromUri]int brandId)
        {
            int count = ctx.Products.Where(t => t.IsVisible == 1 && t.ID_Manufacturer == brandId).Count();
            int nPage = count / nItem;

            if (count % nItem != 0)
                nPage = nPage + 1;

            List<int> result = new List<int>();
            for (int i = 1; i <= nPage; i++)
                result.Add(i);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /*
        // Admin only
        [HttpDelete]
        [Route("api/product/delete")]
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

            string username = headers.GetValues("Username").First();

            if (username != "admin")
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            int idInt = int.Parse(id);
            var data = ctx.Products.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            ctx.Products.Remove(data);
            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        */

        // Admin only
        [HttpPut]
        [Route("api/product/update")]
        public HttpResponseMessage Update([FromUri]string id, [FromBody]ProductModel req)
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

            int idInt = int.Parse(id);
            var data = ctx.Products.Where(t => t.ID == idInt).FirstOrDefault();

            if (data == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            data.Name = req.Name;
            data.ID_Manufacturer = req.ID_Manufacturer;
            data.Price = req.Price;
            data.Screen = req.Screen;
            data.PrimaryCamera = req.PrimaryCamera;
            data.SecondaryCamera = req.SecondaryCamera;
            data.OS = req.OS;
            data.CPU = req.CPU;
            data.GraphicChip = req.GraphicChip;
            data.RAM = req.RAM;
            data.Storage = req.Storage;
            data.SD = req.SD;
            data.SIM = req.SIM;
            data.Battery = req.Battery;
            data.Comms = req.Comms;
            data.IsVisible = req.IsVisible;

            int result = ctx.SaveChanges();

            if (result == 1)
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
