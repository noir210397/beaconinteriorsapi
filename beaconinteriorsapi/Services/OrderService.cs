using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class OrderService
    {
        private readonly IMapper _mapper;
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly ClaimsPrincipal _principal;
        public OrderService(BeaconInteriorsDBContext dbContext,IMapper mapper,ClaimsPrincipal principal)
        {
          _dbContext = dbContext;
          _mapper = mapper;
            _principal = principal;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).Include(o=>o.User).ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string id)
        {
            var isAdmin = _principal.FindAll(ClaimTypes.Role).Select(r=>r.Value).Any(r=>r==UserRoleType.SuperAdmin.ToString()||r==UserRoleType.Admin.ToString());
            var userId = _principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
            if(!isAdmin && userId!=id)
            {
                ThrowUnauthorizedError("Unauthorized");
            }
            return await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).Where(o=>o.UserId==id).ToListAsync();
        }
        public async Task<Order> GetSingleOrderAsync(Guid id)
        {
            var isAdmin = _principal.FindAll(ClaimTypes.Role).Select(r => r.Value).Any(r => r == UserRoleType.SuperAdmin.ToString() || r == UserRoleType.Admin.ToString());
            var userId = _principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
            var order=await _dbContext.Orders.Include(o=>o.Addresses).Include(o=>o.Items).FirstOrDefaultAsync(o=>o.Id==id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            if (!isAdmin && userId != order!.Id.ToString())
            {
                ThrowUnauthorizedError("Unauthorized");
            }
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
        public async Task DeleteOrderAsync(Guid id)
        {
            var order=await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
             else
                {
                var userId = _principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

                var roles = _principal.FindAll(ClaimTypes.Role)
                                      .Select(r => r.Value);

                var isAdmin = roles.Any(r =>
                    r == UserRoleType.Admin.ToString() ||
                    r == UserRoleType.SuperAdmin.ToString());

                var isOwner = order.Id.ToString() == userId;

                if (!isAdmin && !isOwner)
                {
                    ThrowUnauthorizedError("Unauthorized");
                }
                _dbContext.Orders.Remove(order!);
                await _dbContext.SaveChangesAsync();
                }
        }

        
        

    }
}
