using AdminDashboard.Helpers;
using AdminDashboard.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Error;
using Talabat.Core;
using Talabat.Core.Entities.Product;
using Talabat.Core.Specifications.ProductSpecs;

namespace AdminDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // GET All Products
            var spec = new ProductWithBrandAndCategorySpecifications();

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var mappedProduct = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductViewModel>>(products);

            return View(mappedProduct);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Image is not null)
                {
                    viewModel.PictureUrl = PictureSetting.UploadFile(viewModel.Image, "products");
                }
                else
                {
                    viewModel.PictureUrl = "images/products/blueberry-cheesecake.png";
                }

                var product = _mapper.Map<ProductViewModel, Product>(viewModel);
                await _unitOfWork.Repository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found."));

            var mappedProduct = _mapper.Map<Product, ProductViewModel>(product);

            return View(mappedProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound(new ApiResponse(404));

            if (ModelState.IsValid)
            {
                if (viewModel.Image is not null)
                {
                    if (!string.IsNullOrEmpty(viewModel.PictureUrl))
                    {
                        PictureSetting.DeleteFile(viewModel.PictureUrl, "products");

                        viewModel.PictureUrl = PictureSetting.UploadFile(viewModel.Image, "products");
                    }
                    else
                    {
                        viewModel.PictureUrl = PictureSetting.UploadFile(viewModel.Image, "products");
                    }

                    var product = _mapper.Map<ProductViewModel, Product>(viewModel);

                    _unitOfWork.Repository<Product>().Update(product);
                    var result = await _unitOfWork.CompleteAsync();
                    if (result > 0)
                        return RedirectToAction(nameof(Index));
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found."));

            var mappedProduct = _mapper.Map<Product, ProductViewModel>(product);

            return View(mappedProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound(new ApiResponse(404));

            try
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

                if (product == null)
                    return NotFound(new ApiResponse(404, "Product not found."));

                if (product.PictureUrl != null)
                    PictureSetting.DeleteFile(product.PictureUrl, "products");

                _unitOfWork.Repository<Product>().Delete(product);

                await _unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));

            }
            catch (Exception )
            {
                return View(viewModel);
            }

        }

    }
}
