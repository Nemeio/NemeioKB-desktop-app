using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.In.Category;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Dto.Out.Category;
using Nemeio.Core;
using Nemeio.Core.Errors;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Api.Controllers
{
    /// <summary>
    /// Layout display option.
    /// All = With layouts
    /// None = Without layouts
    /// </summary>
    public enum LayoutOption
    {
        All,
        None
    }

    [Route("api/categories")]
    [ApiController]
    public class CategoryController : DefaultController
    {
        public readonly ILogger _logger;
        public readonly ICategoryDbRepository _categoryDbRepository;
        public readonly ILayoutDbRepository _layoutDbRepository;

        public CategoryController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<CategoryController>();
            _categoryDbRepository = Mvx.Resolve<ICategoryDbRepository>();
            _layoutDbRepository = Mvx.Resolve<ILayoutDbRepository>();
        }

        /// <summary>
        /// Allows to retrieve all the categories of the database (without layouts). OptionLayout allows you to retrieve all the layouts associated with a category or not. The possible values are all or none. The value is mandatory.
        /// </summary>
        /// <param name="optionLayout">Layout options : All (with layouts) or None (without layouts)</param>
        [HttpGet]
        public IActionResult GetCategories([FromQuery] LayoutOption optionLayout)
        {
            var results = GetAllCategories();

            var responseContent = optionLayout == LayoutOption.All
                ? GetCategoriesJsonWithLayouts(results)
                : GetCategoriesJsonWithoutLayouts(results);

            return SuccessResponse(responseContent);
        }

        private IEnumerable<Category> GetAllCategories()
        {
            var results = new List<Category>();
            var categories = _categoryDbRepository.ReadAllCategories();

            foreach (var category in categories)
            {
                var layouts = _layoutDbRepository.ReadAllLayoutsWhereCategoryId(category.Id);

                var newCategory = new Category(
                    category.Id,
                    category.Index,
                    category.Title,
                    layouts
                );

                results.Add(newCategory);
            }

            return results;
        }

        private IEnumerable<BaseOutDto> GetCategoriesJsonWithLayouts(IEnumerable<Category> categories, bool withKeys = true)
        {
            if (withKeys)
            {
                return categories.Select((x) => CategoryApiOutDto.FromModel(x));
            }

            return categories.Select((x) => CategoryApiOutDto.FromModelWithoutKeys(x));
        }

        private IEnumerable<BaseOutDto> GetCategoriesJsonWithoutLayouts(IEnumerable<Category> categories) => categories.Select((x) => SimpleCategoryApiOutDto.FromModel(x));

        /// <summary>
        /// Retrieve a category by is identifier.
        /// </summary>
        /// <param name="id">Category's id</param>
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                var category = _categoryDbRepository.FindCategoryById(id);

                var dtoResponse = SimpleCategoryApiOutDto.FromModel(category);

                return SuccessResponse(dtoResponse);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(exception, $"GetCategoryById");

                return ErrorResponse(ErrorCode.ApiGetCategoryIdNotFound);
            }
        }

        /// <summary>
        /// Allows you to add a new category to the database. The data must be in JSON format.
        /// </summary>
        /// <param name="content">Category's information</param>
        [HttpPost]
        public IActionResult PostCategory([FromBody] PostCategoryApiInDto content)
        {
            _categoryDbRepository.CreateCategory(content.ToDomainModel());

            return SuccessResponse();
        }

        /// <summary>
        /// Update a category.
        /// </summary>
        /// <param name="id">Category's id</param>
        /// <param name="inDto">Category's updated information</param>
        [HttpPut("{id}")]
        public IActionResult PutCategory(int id, [FromBody] PutCategoryApiInDto inDto)
        {
            try
            {
                var category = _categoryDbRepository.FindCategoryById(id);
                if (category == null)
                {
                    return ErrorResponse(ErrorCode.ApiPutCategoryNotFound);
                }

                _categoryDbRepository.UpdateCategory(inDto.ToDomainModel(id));

                return SuccessResponse();
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError($"PutCategory : {exception.Message}");

                return ErrorResponse(ErrorCode.ApiPutCategoryNotFound);
            }
        }

        /// <summary>
        /// Allows you to permanently delete a category from the database. If the category isn't empty, all layouts will be transfered to the default category.
        /// </summary>
        /// <param name="id">Category's id</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var layoutRepository = Mvx.Resolve<ILayoutDbRepository>();

                var category = _categoryDbRepository.FindCategoryById(id);
                if (category == null)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteCategoryIdNotFound);
                }

                //  Default category can't be deleted
                if (category.IsDefault)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteDefaultCategoryForbidden);
                }

                //  Update very linked layout to Default category
                _layoutDbRepository.TransferLayoutOwnership(category.Id, NemeioConstants.DefaultCategoryId);

                //  Delete category
                _categoryDbRepository.DeleteCategory(id);

                //  Return all categories
                var results = GetAllCategories();
                var response = GetCategoriesJsonWithLayouts(results, false);

                return SuccessResponse(response);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError($"DeleteCategory : {exception.Message}");

                return ErrorResponse(ErrorCode.ApiDeleteCategoryIdNotFound);
            }
        }
    }
}
