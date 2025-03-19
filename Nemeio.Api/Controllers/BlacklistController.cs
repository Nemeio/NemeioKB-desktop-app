using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.Out.Blacklists;
using Nemeio.Core.Errors;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Services.Blacklist;

namespace Nemeio.Api.Controllers
{

    [Route("api/blacklists")]
    [ApiController]
    public class BlacklistController : DefaultController
    {
        private readonly ILogger _logger;
        private readonly IBlacklistDbRepository _blacklistDbRepository;
        private readonly IApplicationLayoutManager _applicationLayoutManager;

        public BlacklistController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<LayoutController>();
            _blacklistDbRepository = Mvx.Resolve<IBlacklistDbRepository>();
            _applicationLayoutManager = Mvx.Resolve<IApplicationLayoutManager>();
        }

        /// <summary>
        /// Allow user to get every blacklisted applications. Returns system's applications automatically blacklisted and user's application.
        /// </summary>
        [HttpGet]
        public IActionResult GetBlacklists()
        {
            var blacklist = _blacklistDbRepository.ReadSystemBlacklists().ToList();
            blacklist.AddRange(_blacklistDbRepository.ReadBlacklists());

            var outDto = new BlacklistsApiOutDto()
            {
                Blacklists = blacklist.Select(x => BlacklistApiOutDto.FromModel(x)).ToList()
            };

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Add new application on blacklist
        /// </summary>
        /// <param name="name">Application's name</param>
        /// <returns>BlacklistApiOutDto</returns>
        [HttpPost("{name}")]
        public IActionResult PostBlacklist(string name)
        {
            try
            {
                var blacklist = _blacklistDbRepository.SaveBlacklist(name);
                if (blacklist == null)
                {
                    return ErrorResponse(ErrorCode.ApiPostBlacklistNotFound);
                }

                return SuccessResponse(BlacklistApiOutDto.FromModel(blacklist));
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError($"Putblacklist : {exception.Message}");

                return ErrorResponse(ErrorCode.ApiPostBlacklistNotFound);
            }
        }

        /// <summary>
        /// Remove application from blacklist. User can't delete system's application.
        /// </summary>
        /// <param name="id">Application's id</param>
        [HttpDelete("{id}")]
        public IActionResult Deleteblacklist(int id)
        {
            try
            {
                var blacklist = _blacklistDbRepository.FindBlacklistById(id);
                if (blacklist == null)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteBlacklistIdNotFound);
                }

                //  System blacklist can't be deleted
                if (blacklist.IsSystem)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteBlacklistSystemForbidden);
                }

                //  Delete blacklist
                _blacklistDbRepository.DeleteBlacklist(id);

                //  Return all categories
                var results = _blacklistDbRepository.ReadSystemBlacklists().ToList();
                results.AddRange(_blacklistDbRepository.ReadBlacklists());

                var outDto = new BlacklistsApiOutDto()
                {
                    Blacklists = results.Select(x => BlacklistApiOutDto.FromModel(x)).ToList()
                };

                return SuccessResponse(outDto);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError($"DeleteBlacklist : {exception.Message}");

                return ErrorResponse(ErrorCode.ApiDeleteBlacklistIdNotFound);
            }
        }
    }
}
