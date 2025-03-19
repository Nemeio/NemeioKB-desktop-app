using System;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Test.Fakes;

namespace Nemeio.Api.Test.Dummy
{
    public class DummyCategoryDbRepository : FakeCategoryDbRepository
    {
        /// <summary>
        /// Goal: simulate no category found by database
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>In all case this methods will throw an exception : InvalidOperationException</returns>
        public override Category FindCategoryById(int id)
        {
            throw new InvalidOperationException();
        }
    }
}
