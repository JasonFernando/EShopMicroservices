using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon is null)
                coupon = new Models.Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid rqeuest"));

            dbContext.Coupons.Add(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object"));

            var exists = await dbContext.Coupons.AnyAsync(x => x.ProductName == coupon.ProductName);

            if (!exists)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.Coupon.ProductName} is not found."));

            dbContext.Coupons.Update(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}", coupon.ProductName);

            // 5. Retorno
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            // 1. Búsqueda: Primero debemos encontrar lo que vamos a borrar.
            // Usamos FirstOrDefaultAsync porque buscamos por Nombre, no por ID (PK).
            var coupon = await dbContext.Coupons
                .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            // 2. Validación (Fail Fast): Si no existe, lanzamos excepción gRPC.
            // Esto se traduce en un error HTTP estándar en el cliente.
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} is not found."));
            }

            // 3. Eliminación: Marcamos la entidad como "Deleted" en el ChangeTracker de EF Core.
            dbContext.Coupons.Remove(coupon);

            // 4. Persistencia: Se genera el DELETE SQL y se envía a la DB.
            await dbContext.SaveChangesAsync();

            // 5. Logging: Importante para auditoría.
            logger.LogInformation("Discount is successfully deleted. ProductName: {ProductName}", request.ProductName);

            // 6. Respuesta: Construimos el mensaje de éxito definido en el proto.
            return new DeleteDiscountResponse { Success = true };
        }
    }
}
