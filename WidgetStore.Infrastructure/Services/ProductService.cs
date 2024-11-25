using AutoMapper;
using Microsoft.Extensions.Options;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.DTOs.Product;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.Infrastructure.Services
{
    /// <summary>
    /// Implementation of product service
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        private readonly AzureStorageConfig _storageConfig;

        /// <summary>
        /// Initializes a new instance of ProductService
        /// </summary>
        /// <param name="productRepository">Product repository instance</param>
        /// <param name="blobStorageService">Blob storage service instance</param>
        /// <param name="mapper">AutoMapper instance</param>
        /// <param name="storageConfig">Storage configuration</param>
        public ProductService(
            IProductRepository productRepository,
            IBlobStorageService blobStorageService,
            IMapper mapper,
            IOptions<AzureStorageConfig> storageConfig)
        {
            _productRepository = productRepository;
            _blobStorageService = blobStorageService;
            _mapper = mapper;
            _storageConfig = storageConfig.Value;
        }


        /// <inheritdoc/>
        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            ValidateProduct(createProductDto);

            var product = _mapper.Map<Product>(createProductDto);

            product.id = Guid.NewGuid().ToString("N");
            product.type = "Product";
            product.createdAt = DateTime.UtcNow;
            product.isAvailable = true;

            var createdProduct = await _productRepository.CreateAsync(product);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found");

            // Delete image if exists
            if (!string.IsNullOrEmpty(product.imageUrl))
            {
                var fileName = Path.GetFileName(new Uri(product.imageUrl).LocalPath);
                await _blobStorageService.DeleteAsync(_storageConfig.ProductImagesContainer, fileName);
            }

            // Soft delete the product
            product.isAvailable = false;
            product.modifiedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var activeProducts = products.Where(p => p.isAvailable);
            return _mapper.Map<IEnumerable<ProductDto>>(activeProducts);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new BadRequestException("Category cannot be empty");

            var products = await _productRepository.GetByCategoryAsync(category);
            var activeProducts = products.Where(p => p.isAvailable);
            return _mapper.Map<IEnumerable<ProductDto>>(activeProducts);
        }

        /// <inheritdoc/>
        public async Task<ProductDto?> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null || !product.isAvailable)
                return null;

            return _mapper.Map<ProductDto>(product);
        }

        /// <inheritdoc/>
        public async Task<ProductDto> UpdateAsync(string id, UpdateProductDto updateProductDto, string? imageUrl = null)
        {
            ValidateProduct(updateProductDto);

            var existingProduct = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found");

            _mapper.Map(updateProductDto, existingProduct);

            if (imageUrl != null)
            {
                existingProduct.imageUrl = imageUrl;
            }

            existingProduct.modifiedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        /// <inheritdoc/>
        public async Task<ProductDto> UpdateStockAsync(string id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found");

            var newStockQuantity = product.stockQuantity + quantity;

            if (newStockQuantity < 0)
                throw new BadRequestException("Cannot reduce stock below zero");

            product.stockQuantity = newStockQuantity;
            product.modifiedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        /// <inheritdoc/>
        public async Task<ProductDto> UploadImageAsync(string id, string fileName, string contentType, Stream imageStream)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with ID {id} not found");

            // Delete existing image if any
            if (!string.IsNullOrEmpty(product.imageUrl))
            {
                var existingFileName = Path.GetFileName(new Uri(product.imageUrl).LocalPath);
                await _blobStorageService.DeleteAsync(_storageConfig.ProductImagesContainer, existingFileName);
            }

            // Upload new image
            var imageUrl = await _blobStorageService.UploadAsync(
                _storageConfig.ProductImagesContainer,
                fileName,
                contentType,
                imageStream);

            // Update product with new image URL
            product.imageUrl = imageUrl;
            product.modifiedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        #region Private Methods

        private void ValidateProduct(CreateProductDto productDto)
        {
            if (string.IsNullOrWhiteSpace(productDto.Name))
                throw new BadRequestException("Product name is required");

            if (string.IsNullOrWhiteSpace(productDto.Description))
                throw new BadRequestException("Product description is required");

            if (productDto.Price < 0)
                throw new BadRequestException("Price cannot be negative");

            if (productDto.StockQuantity < 0)
                throw new BadRequestException("Stock quantity cannot be negative");

            if (string.IsNullOrWhiteSpace(productDto.Category))
                throw new BadRequestException("Category is required");

            if (string.IsNullOrWhiteSpace(productDto.Sku))
                throw new BadRequestException("SKU is required");
        }

        private void ValidateProduct(UpdateProductDto productDto)
        {
            if (string.IsNullOrWhiteSpace(productDto.Name))
                throw new BadRequestException("Product name is required");

            if (string.IsNullOrWhiteSpace(productDto.Description))
                throw new BadRequestException("Product description is required");

            if (productDto.Price < 0)
                throw new BadRequestException("Price cannot be negative");

            if (productDto.StockQuantity < 0)
                throw new BadRequestException("Stock quantity cannot be negative");

            if (string.IsNullOrWhiteSpace(productDto.Category))
                throw new BadRequestException("Category is required");

            if (string.IsNullOrWhiteSpace(productDto.Sku))
                throw new BadRequestException("SKU is required");
        }

        #endregion
    }
}