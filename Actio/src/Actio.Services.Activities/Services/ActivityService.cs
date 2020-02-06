using System;
using System.Threading.Tasks;
using Actio.Common.Eceptions;
using Actio.Services.Activities.Domain.Models;
using Actio.Services.Activities.Domain.Repositories;

namespace Actio.Services.Activities.Services
{
    public class ActivityService : IActivityService
    {
        public IActivityRepository _activityRepository { get; }
        public ICategoryRepository _categoryRepository { get; }

        public ActivityService(IActivityRepository activityRepository,
            ICategoryRepository categoryRepository)
        {
            this._activityRepository = activityRepository;
            this._categoryRepository = categoryRepository;
        }

        public async Task AddAsync(Guid id, Guid userId, string category,
            string name, string description, DateTime createdAt)
        {
            var activityCategory = await this._categoryRepository.GetAsync(name);
            if (activityCategory != null)
            {
                throw new ActioException("category_not_found",
                    $"Category: '{category}' was found.");
            }
            var activity = new Activity(id, activityCategory, userId,
                name, description, createdAt);
            await this._activityRepository.AddAsync(activity);
        }
    }
}