using Ljuss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Persistence.ModelBuilderConfigurations
{
    public class ModelBuilderConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            #region VerificationCode and User Relations

                modelBuilder.Entity<VerificationCode>()
                .HasOne(vc => vc.User)
                .WithMany(u => u.VerificationCodes)
                .HasForeignKey(vc => vc.UserId);

            #endregion

            #region RefleshToken and User Relations

            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");

            modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

            #endregion

            #region ShippingAddress and User Relations

            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.User)
                .WithMany(u => u.ShippingAddresses)
                .HasForeignKey(sa => sa.UserId);

            #endregion

            #region Category and Product Relations

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            #endregion

            #region ProductAttribute and Product Relations

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(pa => pa.ProductId);

            #endregion

            #region AttributeValue and Attribute Relations

            modelBuilder.Entity<AttributeValue>()
                .HasOne(av => av.Attribute)
                .WithMany(a => a.Values)
                .HasForeignKey(av => av.AttributeId);

            #endregion

            #region ProductAttribute and Attribute Relations

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.Attribute)
                .WithMany()
                .HasForeignKey(pa => pa.AttributeId);

            #endregion

            #region ProductAttribute and AttributeValue Relations

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.AttributeValue)
                .WithMany()
                .HasForeignKey(pa => pa.AttributeValueId);

            #endregion

            #region Comment and Product Relations

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ProductId);

            #endregion

            #region Comment and User Relations

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            #endregion

            #region Favorite and Product Relations

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProductId);

            #endregion

            #region Favorite and User Relations

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            #endregion

            #region Order and User Relations

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            #endregion

            #region Order and ShippingAddress Relations

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId);

            #endregion

            #region OrderItem and Order Relations

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            #endregion

            #region OrderItem and Product Relations

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            #endregion

            #region Payment and Order Relations

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId);

            #endregion
        }
    }
}
