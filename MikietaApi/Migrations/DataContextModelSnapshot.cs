﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MikietaApi.Data;

#nullable disable

namespace MikietaApi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.18");

            modelBuilder.Entity("IngredientEntityProductEntity", b =>
                {
                    b.Property<Guid>("IngredientsId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductsId")
                        .HasColumnType("TEXT");

                    b.HasKey("IngredientsId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("ProductIngredient", (string)null);
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.IngredientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("PriceLarge")
                        .HasColumnType("REAL");

                    b.Property<double>("PriceMedium")
                        .HasColumnType("REAL");

                    b.Property<double>("PriceSmall")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("CanClearBasket")
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Comments")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeliveryMethod")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("DeliveryPrice")
                        .HasColumnType("REAL");

                    b.Property<bool?>("DeliveryRightAway")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DeliveryTiming")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FlatNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Floor")
                        .HasColumnType("TEXT");

                    b.Property<string>("HomeNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nip")
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .IsUnicode(true)
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Paid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool?>("ProcessingPersonalDataByEmail")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("ProcessingPersonalDataBySms")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SessionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Visible")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderOrderedProductEntity", b =>
                {
                    b.Property<Guid>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderedProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Ready")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderId", "OrderedProductId");

                    b.HasIndex("OrderedProductId");

                    b.ToTable("OrderOrderedProducts");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedIngredientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("PriceLarge")
                        .HasColumnType("REAL");

                    b.Property<double>("PriceMedium")
                        .HasColumnType("REAL");

                    b.Property<double>("PriceSmall")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("OrderedIngredients");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OrderedIngredientEntityId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PizzaType")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OrderedIngredientEntityId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderedProducts");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedProductOrderedIngredientEntity", b =>
                {
                    b.Property<Guid>("OrderedProductId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderedIngredientId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAdditionalIngredient")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsIngredientRemoved")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ReplacedIngredientId")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderedProductId", "OrderedIngredientId");

                    b.HasIndex("OrderedIngredientId");

                    b.HasIndex("ReplacedIngredientId");

                    b.ToTable("OrderedProductOrderedIngredients");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PizzaType")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<int>("ProductType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.ReservationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comments")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailSent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MessageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .IsUnicode(true)
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.SettingEntity", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("IngredientEntityProductEntity", b =>
                {
                    b.HasOne("MikietaApi.Data.Entities.IngredientEntity", null)
                        .WithMany()
                        .HasForeignKey("IngredientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MikietaApi.Data.Entities.ProductEntity", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderOrderedProductEntity", b =>
                {
                    b.HasOne("MikietaApi.Data.Entities.OrderEntity", "Order")
                        .WithMany("OrderOrderedProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MikietaApi.Data.Entities.OrderedProductEntity", "OrderedProduct")
                        .WithMany("OrderOrderedProducts")
                        .HasForeignKey("OrderedProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("OrderedProduct");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedIngredientEntity", b =>
                {
                    b.HasOne("MikietaApi.Data.Entities.IngredientEntity", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedProductEntity", b =>
                {
                    b.HasOne("MikietaApi.Data.Entities.OrderedIngredientEntity", null)
                        .WithMany("OrderedProducts")
                        .HasForeignKey("OrderedIngredientEntityId");

                    b.HasOne("MikietaApi.Data.Entities.ProductEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedProductOrderedIngredientEntity", b =>
                {
                    b.HasOne("MikietaApi.Data.Entities.OrderedIngredientEntity", "OrderedIngredient")
                        .WithMany("OrderedProductOrderedIngredients")
                        .HasForeignKey("OrderedIngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MikietaApi.Data.Entities.OrderedProductEntity", "OrderedProduct")
                        .WithMany("OrderedProductOrderedIngredients")
                        .HasForeignKey("OrderedProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MikietaApi.Data.Entities.OrderedIngredientEntity", "ReplacedIngredient")
                        .WithMany()
                        .HasForeignKey("ReplacedIngredientId");

                    b.Navigation("OrderedIngredient");

                    b.Navigation("OrderedProduct");

                    b.Navigation("ReplacedIngredient");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderEntity", b =>
                {
                    b.Navigation("OrderOrderedProducts");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedIngredientEntity", b =>
                {
                    b.Navigation("OrderedProductOrderedIngredients");

                    b.Navigation("OrderedProducts");
                });

            modelBuilder.Entity("MikietaApi.Data.Entities.OrderedProductEntity", b =>
                {
                    b.Navigation("OrderOrderedProducts");

                    b.Navigation("OrderedProductOrderedIngredients");
                });
#pragma warning restore 612, 618
        }
    }
}
