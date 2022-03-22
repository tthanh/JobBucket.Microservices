using JB.Organization.Data;
using JB.Organization.Models;
using JB.Organization.Models.Organization;
using JB.Organization.Models.Review;
using JB.Organization.Models.User;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Organization.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ReviewDbContext _reviewDbContext;
        private readonly ILogger<ReviewService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IUserManagementService _userService;
        private readonly IOrganizationService _orgService;
        //private readonly IreviewService _reviewService;

        public ReviewService(
            ReviewDbContext reviewDbContext,
            ILogger<ReviewService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IOrganizationService orgService
            //IreviewService reviewService
            )
        {
            _reviewDbContext = reviewDbContext;
            _claims = claims;
            _userService = userService;
            _orgService = orgService;
            //_reviewService = reviewService;
        }


        public async Task<Status> Add(ReviewModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (entity == null)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                var organization = _orgService.GetById(entity.OrganizationId);
                if(organization == null)
                {
                    result.ErrorCode = ErrorCode.OrganizationNull;
                    break;
                }
                entity.UserId = userId;

                try
                {
                    bool isReviewed = await _reviewDbContext.Reviews.AnyAsync(x => x.OrganizationId == entity.OrganizationId && x.UserId == userId);
                    if (isReviewed)
                    {
                        result.ErrorCode = ErrorCode.ReviewAlreadyExist;
                        break;
                    }

                    await _reviewDbContext.Reviews.AddAsync(entity);
                    await _reviewDbContext.SaveChangesAsync();

                    _reviewDbContext.Entry(entity).State = EntityState.Detached;
                    entity = await _reviewDbContext.Reviews.FindAsync(entity.Id);

                    if (entity.UserId > 0)
                    {
                        UserModel reviewer = _userService.GetUser(entity.UserId).Result.Item2;
                        if (reviewer != null)
                        {
                            entity.User = reviewer;
                        }
                    }

                    await UpdateOrganizationRating(entity.OrganizationId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, long)> Count(Expression<Func<ReviewModel, bool>> predicate)
        {
            Status result = new Status();
            long count = 0;
            do
            {
                try
                {
                    if (predicate == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    count = await _reviewDbContext.Reviews.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, count);
        }

        public async Task<Status> Delete(int reviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (reviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.ReviewNull;
                    break;
                }


                try
                {
                    var review = await _reviewDbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
                    if (review == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }
                    
                    if (userId != review.UserId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                 

                    _reviewDbContext.Reviews.Remove(review);
                    await _reviewDbContext.SaveChangesAsync();

                    await UpdateOrganizationRating(review.OrganizationId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, ReviewModel)> GetById(int reviewId)
        {
            Status result = new Status();
            ReviewModel review = null;
            int userId = _claims?.Id ?? 0;
            do
            {

                try
                {
                    review = await _reviewDbContext.Reviews.Where(x => x.Id == reviewId).FirstOrDefaultAsync();
                    if (review == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }

                    if (userId > 0)
                    {
                        review.IsInterested = review.Interests.Any(i => i.UserId == userId);
                    }

                    if (review.UserId > 0)
                    {
                        UserModel reviewer = _userService.GetUser(review.UserId).Result.Item2;
                        if (reviewer != null)
                        {
                            review.User = reviewer;
                        }
                    }

                    if (review.OrganizationId > 0)
                    {
                        OrganizationModel organization = _orgService.GetById(review.OrganizationId).Result.Item2;
                        if (organization != null)
                        {
                            review.Organization = organization;
                        }
                    }

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, review);
        }

        public async Task<(Status, List<ReviewModel>)> List(Expression<Func<ReviewModel, bool>> filter, Expression<Func<ReviewModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var reviews = new List<ReviewModel>();
            int userId = _claims?.Id ?? 0;

            do
            {

                try
                {

                    var reviewQuery = _reviewDbContext.Reviews.Where(filter);
                    reviewQuery = isDescending ? reviewQuery.OrderByDescending(x => x.UserId == userId).ThenByDescending(sort) : reviewQuery.OrderByDescending(x => x.UserId == userId).ThenBy(sort);
                    reviews = await reviewQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (reviews == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }

                    if (userId > 0)
                    {
                        reviews.ForEach(j =>
                        {
                            j.IsInterested = j.Interests.Any(i => i.UserId == userId);
                        });
                    }



                    foreach (var review in reviews)
                    {
                        if (review.UserId > 0)
                        {
                            UserModel reviewer = _userService.GetUser(review.UserId).Result.Item2;
                            review.User = reviewer ?? review.User;
                        }

                        if (review.OrganizationId > 0)
                        {
                            OrganizationModel organization = _orgService.GetById(review.OrganizationId).Result.Item2;
                            review.Organization = organization ?? review.Organization;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, reviews);
        }

        public async Task<Status> Update(ReviewModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                   

                    var review = await _reviewDbContext.Reviews.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                    if (review == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }
                    if (userId != review.UserId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    PropertyHelper.InjectNonNull<ReviewModel>(review, entity);
                    _reviewDbContext.Update(review);
                    await _reviewDbContext.SaveChangesAsync();

                    if (review.UserId > 0)
                    {
                        UserModel reviewer = _userService.GetUser(review.UserId).Result.Item2;
                        if (reviewer != null)
                        {
                            review.User = reviewer;
                        }
                    }

                    await UpdateOrganizationRating(review.OrganizationId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, ReviewModel)> Interest(int reviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            ReviewModel review = null;
            do
            {
                try
                {
                    if (reviewId <= 0)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }
                    if (userId <= 0)    
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    review = await _reviewDbContext.Reviews.Where(x => x.Id == reviewId).FirstOrDefaultAsync();
                    if (review == null)
                    {
                        result.ErrorCode = ErrorCode.ReviewNull;
                        break;
                    }
                    review.IsInterested = review.Interests.Any(i => i.UserId == userId);
                    if (review.IsInterested)
                    {
                        var interest = review.Interests.Where(i => i.UserId == userId).FirstOrDefault();
                        review.Interests.Remove(interest);
                        
                    }
                    else
                    {
                        ReviewInterestModel interest = new ReviewInterestModel {
                            UserId = userId,
                            ReviewId = reviewId
                        
                        };
                        review.Interests.Add(interest);
                        
                    }
                    review.IsInterested = !review.IsInterested;

                    if (review.UserId > 0)
                    {
                        UserModel reviewer = _userService.GetUser(review.UserId).Result.Item2;
                        if (reviewer != null)
                        {
                            review.User = reviewer;
                        }
                    }

                    if (review.OrganizationId > 0)
                    {
                        OrganizationModel organization = _orgService.GetById(review.OrganizationId).Result.Item2;
                        if (organization != null)
                        {
                            review.Organization = organization;
                        }
                    }

                    _reviewDbContext.Update(review);
                    await _reviewDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, review);
        }

        private async Task UpdateOrganizationRating(int organizationId)
        {
            var averageResult = await _reviewDbContext.Reviews.Where(x => x.OrganizationId == organizationId).GroupBy(x => true).Select(x => new
            {
                Rating = x.Average(r => r.Rating),
                RatingBenefit = x.Average(r => r.RatingBenefit),
                RatingCulture = x.Average(r => r.RatingCulture),
                RatingLearning = x.Average(r => r.RatingLearning),
                RatingWorkspace = x.Average(r => r.RatingWorkspace),
            }).FirstOrDefaultAsync();

            await _orgService.UpdateRating(organizationId, averageResult.Rating, averageResult.RatingBenefit, averageResult.RatingLearning, averageResult.RatingCulture, averageResult.RatingWorkspace);
        }
    }
}
