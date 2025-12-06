using Discount.Grpc.Data;
using Grpc.Core;
using Mapster;

namespace Discount.Grpc.Services
{
  public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
  {

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
      var coupon = await dbContext
        .Coupons
        .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

      if (coupon is null)
      {
        coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
      }

      logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

      return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
      var coupon = request.Coupon.Adapt<Coupon>();
      if (coupon is null)
      {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
      }

      dbContext.Coupons.Add(coupon);
      await dbContext.SaveChangesAsync();

      logger.LogInformation("Discount is successfully created.");

      return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
      var coupon = request.Coupon.Adapt<Coupon>();
      if (coupon is null)
      {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
      }

      dbContext.Coupons.Update(coupon);
      await dbContext.SaveChangesAsync();

      logger.LogInformation("Discount is successfully created.");

      return coupon.Adapt<CouponModel>();
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
      var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

      if (coupon is null)
      {
        throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));
      }

      dbContext.Coupons.Remove(coupon);
      await dbContext.SaveChangesAsync();

      logger.LogInformation("Discount succesfully deleted.");

      return new DeleteDiscountResponse { Success = true };
    }
  }
}
