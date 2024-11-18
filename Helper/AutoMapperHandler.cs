using AutoMapper;
using WebAPIProject.Model;
using WebAPIProject.Repos.Models;

namespace WebAPIProject.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler() {
            CreateMap<Customer, Customermodel>().ForMember(item=>item.Statusname,opt=>opt.MapFrom(
                item=> (item.IsActive.GetValueOrDefault()) ? "Active" : "In active")).ReverseMap();
        }
    }
}
