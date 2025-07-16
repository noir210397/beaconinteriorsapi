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
            return await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).ToListAsync();
        }
        public async Task<Order> GetSingleOrderAsync(Guid id)
        {
            var order=await _dbContext.Orders.Include(o=>o.Addresses).Include(o=>o.Items).FirstOrDefaultAsync(o=>o.Id==id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            return order!;
        }
        public async Task DeleteOrderAsync(Guid id)
        {
            var order=await _dbContext.Orders.Include(o => o.Addresses).Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) ThrowNotFound($"no order found with id:{id.ToString()}");
            _dbContext.Orders.Remove(order!);
            await _dbContext.SaveChangesAsync();
        }
    }
}
