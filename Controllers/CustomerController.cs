using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;
using WebAPIProject.Model;
using WebAPIProject.Service;

namespace WebAPIProject.Controllers
{

    // [DisableCors]//that mean the particular controller with action method cannot be accessed for app or any domain 

    [Authorize] //enable authenticat for controller
    [EnableRateLimiting("fixedwindow")] //enabling rate limiter
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerService service;

        private readonly IWebHostEnvironment env;

        public CustomerController(ICustomerService service, IWebHostEnvironment env)
        {
            this.service = service;
            this.env = env;
        }


        [AllowAnonymous]//thia measn auntication not work for this method
        //enable cors
       // [EnableCors("corspolicy1")]//we can do it for whole controller also
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await this.service.Getall();
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);

        }
       
        [DisableRateLimiting]//disable rate limit for this getbycode method
        [HttpGet("Getbycode")]
        public async Task<IActionResult> Getbycode(int code)
        {
            var data = await this.service.Getbycode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);

        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Customermodel _data)
        {
            var data = await this.service.Create(_data);
            return Ok(data);

        }
        
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Customermodel _data, int code)
        {
            var data = await this.service.Update(_data,code);
            return Ok(data);

        }
        
        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove( int code)
        {
            var data = await this.service.Remove( code);
            return Ok(data);

        }

        [AllowAnonymous]//no need of authentication
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string Filepath = GetFilepath();
                string excelpath = Filepath + "\\customerinfo.xlsx";
                DataTable dt = new DataTable();
                dt.Columns.Add("Code",typeof(string));
                dt.Columns.Add("Name",typeof(string));
                dt.Columns.Add("Email",typeof(string));
                dt.Columns.Add("Phone",typeof(int));
                dt.Columns.Add("CreditLimit",typeof(int));
                var data = await this.service.Getall();
                if (data != null && data.Count>0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.Creditlimit);
                    });
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(dt,"Customer Info");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        if(System.IO.File.Exists(excelpath))
                        {
                            System.IO.File.Delete(excelpath);
                        }
                        wb.SaveAs(excelpath);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Customer.xlsx");
                    }
                }
            }
            catch (Exception)
            {

                return NotFound();
            }

        }

        [NonAction]
        private string GetFilepath()
        {
            return this.env.WebRootPath + "\\Export";
        }
    }
}
