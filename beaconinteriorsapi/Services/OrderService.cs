using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using Microsoft.EntityFrameworkCore;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class OrderService
    {
        private readonly IMapper _mapper;
        private readonly BeaconInteriorsDBContext _dbContext;
        public OrderService(BeaconInteriorsDBContext dbContext,IMapper mapper)
        {
          _dbContext = dbContext;
          _mapper = mapper;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).Include(o=>o.User).ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string id)
        {
            return await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).Where(o=>o.UserId==id).ToListAsync();
        }
        public async Task<Order> GetSingleOrderAsync(Guid id)
        {
            var order=await _dbContext.Orders.Include(o=>o.Addresses).Include(o=>o.Items).FirstOrDefaultAsync(o=>o.Id==id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            return order!;
        }
        public async Task<Order> TrackOrderAsync(string id)
        {
            var order = await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).FirstOrDefaultAsync(o => o.TrackingID == id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            return order!;
        }
        public async Task<bool> CheckIfOrderExpiredAsync(string id)
        {
            var order = await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).FirstOrDefaultAsync(o => o.TrackingID == id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            if(order!.HasExpired) return true;
            else
            {
                order.PaymentStatus=PaymentStatusType.Processing;
                await _dbContext.SaveChangesAsync();
                return false;
            }
        }
        public async Task DeleteOrderAsync(Guid id,IEnumerable<string> roles,string userId)
        {
            var order=await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            if (roles.Any(r => r.Equals(UserRoleType.SuperAdmin.ToString())) || order!.UserId == userId)
            {
                _dbContext.Orders.Remove(order!);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                ThrowUnauthorizedError("unauthorized");
            }
        }
    }
}
