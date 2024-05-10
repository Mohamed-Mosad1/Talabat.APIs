using AdminDashboard.ViewModels;
using AutoMapper;
using Talabat.Core.Entities.Product;

namespace AdminDashboard.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductViewModel>().ReverseMap();


        }
    }
}
