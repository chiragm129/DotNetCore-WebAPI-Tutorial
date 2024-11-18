using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;

namespace WebAPIProject.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContext context;

        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;
        public CustomerService(LearndataContext context, IMapper mapper, ILogger<CustomerService> logger) { 
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<APIResponse> Create(Customermodel data)
        {
            APIResponse response = new APIResponse();

            try
            {
                this.logger.LogInformation("Create Begins");
                Customer _customer = this.mapper.Map<Customermodel, Customer>(data);
                await this.context.Customers.AddAsync(_customer);
                await this.context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = data.Code.ToString();
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Errormessage = ex.Message;
                this.logger.LogError(ex.Message,ex);
            }
            return response;
        }

        public async Task<List<Customermodel>> Getall()
        {
            List<Customermodel> _response = new List<Customermodel>();
            var _data =  await this.context.Customers.ToListAsync();
            if(_data != null)
            {
                _response = this.mapper.Map<List<Customer>,List<Customermodel>>(_data);
            }
            return _response;
        }

        public async Task<Customermodel> Getbycode(int code)
        {
            Customermodel _response = new Customermodel();
            var _data = await this.context.Customers.FindAsync(code);
            if (_data != null)
            {
                _response = this.mapper.Map<Customer, Customermodel>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> Remove(int code)
        {
            APIResponse response = new APIResponse();

            try
            {
                var _customer = await this.context.Customers.FindAsync(code);
                if(_customer != null )
                {
                    this.context.Customers.Remove( _customer );
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code.ToString();
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Errormessage = "Data not found";
                }
                
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Errormessage = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse> Update(Customermodel data, int code)
        {
            APIResponse response = new APIResponse();

            try
            {
                var _customer = await this.context.Customers.FindAsync(code);
                if (_customer != null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.Creditlimit = data.Creditlimit;
                    _customer.Address = data.Address;
                    _customer.IsActive = data.IsActive;
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code.ToString();
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Errormessage = "Data not found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Errormessage = ex.Message;
            }
            return response;
        }
    }
}
