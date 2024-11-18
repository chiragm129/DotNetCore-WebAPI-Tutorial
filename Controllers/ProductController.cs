using Microsoft.AspNetCore.Mvc;
using WebAPIProject.Helper;
using WebAPIProject.Repos;

namespace WebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly LearndataContext context;
        public ProductController(IWebHostEnvironment  environment, LearndataContext context)
        {
            this.environment = environment;
            this.context = context;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilepath(productcode);
                if(!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\" + productcode + ".png";
                if(!System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using(FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
             
            }
            catch (Exception ex)
            {
                response.Errormessage = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("MultiUploadImages")]
        public async Task<IActionResult> MultiUploadImages(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;

            try
            {
                string Filepath = GetFilepath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                foreach(var file in filecollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;
                    if (!System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorcount++;
                response.Errormessage = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded & " + errorcount + " files failed";
            return Ok(response);
        }

        [HttpGet("GetImage")]
        public IActionResult GetImage(string productcode)
        {
            string Imageurl = string.Empty;
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if(System.IO.File.Exists(imagepath))
                {
                    Imageurl =  hosturl + "/Upload/product/" + productcode+ "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }          
            }
            catch (Exception)
            {
            }
            return Ok(Imageurl);
        }

        [HttpGet("GetMultiImage")]
        public  IActionResult GetMultiImage(string productcode)
        {
            List<string> Imageurl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string Filepath = GetFilepath(productcode);
                
                if(System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;

                        if (System.IO.File.Exists(imagepath))
                        {
                            Imageurl.Add(hosturl + "/Upload/product/" + productcode + "/" + filename);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return Ok(Imageurl);
        }

        [HttpGet("Download")]
        public async Task<IActionResult> Download(string productcode)
        {
            
            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new MemoryStream();
                    using(FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("Remove")]
        public IActionResult Remove(string productcode)
        {

            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("MultiRemove")]
        public IActionResult MultiRemove(string productcode)
        {

            try
            {
                string Filepath = GetFilepath(productcode);
                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    return Ok("Pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut("DBMultiUploadImages")]
        public async Task<IActionResult> DBMultiUploadImages(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;

            try
            {
                foreach (var file in filecollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        this.context.Productimages.Add(new Repos.Models.Productimage()
                        {
                            Productcode = productcode,
                            Productimage1 = stream.ToArray()
                        });
                        await this.context.SaveChangesAsync();
                        passcount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorcount++;
                response.Errormessage = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded & " + errorcount + " files failed";
            return Ok(response);
        }

        [HttpGet("GetDBMultiImage")]
        public IActionResult GetDBMultiImage(string productcode)
        {
            List<string> Imageurl = new List<string>();
            
            try
            {
                var _productimage =  this.context.Productimages.Where(item => item.Productcode == productcode).ToList();
                if(_productimage !=null && _productimage.Count>0)
                {
                    _productimage.ForEach(item =>
                    {
                        Imageurl.Add(Convert.ToBase64String(item.Productimage1));
                    });
                }
                else
                {
                    return NotFound();
                }
            
            }
            catch (Exception)
            {
            }
            return Ok(Imageurl);
        }

        [HttpGet("DBDownload")]
        public IActionResult DBDownload(string productcode)
        {

            try
            {
                var _productimage =  this.context.Productimages.FirstOrDefault(item => item.Productcode == productcode);
                if (_productimage != null)
                {
                    return File(_productimage.Productimage1, "image/png", productcode + ".png");
                }

                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [NonAction]
        private string GetFilepath(string productcode)
        {
            return this.environment.WebRootPath + "\\Upload\\product\\" + productcode;
        }


    }
}
