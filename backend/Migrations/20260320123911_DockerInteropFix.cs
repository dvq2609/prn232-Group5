using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DockerInteropFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__3213E83FCC4FE7F1", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SellerToBuyerReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    SellerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    BuyerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SellerTo__3214EC07E90DC1BD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    avatarURL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3213E83F84975739", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    isDefault = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Address__3213E83FBF640A2C", x => x.id);
                    table.ForeignKey(
                        name: "FK__Address__userId__3A81B327",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderId = table.Column<int>(type: "int", nullable: true),
                    receiverId = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message__3213E83F75D10DC6", x => x.id);
                    table.ForeignKey(
                        name: "FK__Message__receive__70DDC3D8",
                        column: x => x.receiverId,
                        principalTable: "User",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Message__senderI__6FE99F9F",
                        column: x => x.senderId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    images = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: true),
                    sellerId = table.Column<int>(type: "int", nullable: true),
                    isAuction = table.Column<bool>(type: "bit", nullable: true),
                    auctionEndTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__3213E83FC81F5D33", x => x.id);
                    table.ForeignKey(
                        name: "FK__Product__categor__3F466844",
                        column: x => x.categoryId,
                        principalTable: "Category",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Product__sellerI__403A8C7D",
                        column: x => x.sellerId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sellerId = table.Column<int>(type: "int", nullable: true),
                    storeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bannerImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Store__3213E83F17192FEE", x => x.id);
                    table.ForeignKey(
                        name: "FK__Store__sellerId__00200768",
                        column: x => x.sellerId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "OrderTable",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buyerId = table.Column<int>(type: "int", nullable: true),
                    addressId = table.Column<int>(type: "int", nullable: true),
                    orderDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    totalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    isCommented = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderTab__3213E83F8B3EC05A", x => x.id);
                    table.ForeignKey(
                        name: "FK__OrderTabl__addre__440B1D61",
                        column: x => x.addressId,
                        principalTable: "Address",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__OrderTabl__buyer__4316F928",
                        column: x => x.buyerId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Bid",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: true),
                    bidderId = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    bidTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bid__3213E83FD5A76A67", x => x.id);
                    table.ForeignKey(
                        name: "FK__Bid__bidderId__5629CD9C",
                        column: x => x.bidderId,
                        principalTable: "User",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Bid__productId__5535A963",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    discountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    maxUsage = table.Column<int>(type: "int", nullable: true),
                    productId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coupon__3213E83F115128B3", x => x.id);
                    table.ForeignKey(
                        name: "FK__Coupon__productI__60A75C0F",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    lastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__3213E83F1F7B52C3", x => x.id);
                    table.ForeignKey(
                        name: "FK__Inventory__produ__6383C8BA",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: true),
                    reviewerId = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__3213E83FADA5985E", x => x.id);
                    table.ForeignKey(
                        name: "FK__Review__productI__59063A47",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Review__reviewer__59FA5E80",
                        column: x => x.reviewerId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Dispute",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    raisedBy = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    submittedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    solvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    sellerResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    buyerResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    adminResponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dispute__3213E83FE44ADA69", x => x.id);
                    table.ForeignKey(
                        name: "FK__Dispute__orderId__693CA210",
                        column: x => x.orderId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Dispute__raisedB__6A30C649",
                        column: x => x.raisedBy,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sellerId = table.Column<int>(type: "int", nullable: true),
                    averageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    totalReviews = table.Column<int>(type: "int", nullable: true),
                    positiveRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OrdersId = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__3213E83FC9707D49", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedback_OrderTable_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Feedback__seller__66603565",
                        column: x => x.sellerId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    productId = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    unitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderIte__3213E83FCAAB6CC6", x => x.id);
                    table.ForeignKey(
                        name: "FK__OrderItem__order__46E78A0C",
                        column: x => x.orderId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__OrderItem__produ__47DBAE45",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    paidAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__3213E83FEDA138AD", x => x.id);
                    table.ForeignKey(
                        name: "FK__Payment__orderId__4AB81AF0",
                        column: x => x.orderId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Payment__userId__4BAC3F29",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnRe__3213E83F6961CABA", x => x.id);
                    table.ForeignKey(
                        name: "FK__ReturnReq__order__5165187F",
                        column: x => x.orderId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__ReturnReq__userI__52593CB8",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ShippingInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    trackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    estimatedArrival = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shipping__3213E83F797BF293", x => x.id);
                    table.ForeignKey(
                        name: "FK_ShippingInfo_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__ShippingI__order__4E88ABD4",
                        column: x => x.orderId,
                        principalTable: "OrderTable",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "DisputeImage",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    disputeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DisputeI__3213E83F94272785", x => x.id);
                    table.ForeignKey(
                        name: "FK_Dispute_Images",
                        column: x => x.disputeId,
                        principalTable: "Dispute",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetailFeedback",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryOnTime = table.Column<int>(type: "int", nullable: true),
                    ExactSame = table.Column<int>(type: "int", nullable: true),
                    Communication = table.Column<int>(type: "int", nullable: true),
                    feedbackId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetailFe__3213E83F46C8EB97", x => x.id);
                    table.ForeignKey(
                        name: "FK_DetailFeedback_Feedback",
                        column: x => x.feedbackId,
                        principalTable: "Feedback",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_userId",
                table: "Address",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_bidderId",
                table: "Bid",
                column: "bidderId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_productId",
                table: "Bid",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_productId",
                table: "Coupon",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailFeedback_feedbackId",
                table: "DetailFeedback",
                column: "feedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_orderId",
                table: "Dispute",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_raisedBy",
                table: "Dispute",
                column: "raisedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeImage_disputeId",
                table: "DisputeImage",
                column: "disputeId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_OrdersId",
                table: "Feedback",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_sellerId",
                table: "Feedback",
                column: "sellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_productId",
                table: "Inventory",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_receiverId",
                table: "Message",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_senderId",
                table: "Message",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_orderId",
                table: "OrderItem",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_productId",
                table: "OrderItem",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_addressId",
                table: "OrderTable",
                column: "addressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_buyerId",
                table: "OrderTable",
                column: "buyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_orderId",
                table: "Payment",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_userId",
                table: "Payment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_categoryId",
                table: "Product",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_sellerId",
                table: "Product",
                column: "sellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequest_orderId",
                table: "ReturnRequest",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequest_userId",
                table: "ReturnRequest",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_productId",
                table: "Review",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_reviewerId",
                table: "Review",
                column: "reviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingInfo_orderId",
                table: "ShippingInfo",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingInfo_UserId",
                table: "ShippingInfo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_sellerId",
                table: "Store",
                column: "sellerId");

            migrationBuilder.CreateIndex(
                name: "UQ__User__AB6E616473BAD3B4",
                table: "User",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__User__AB6E6164741FE703",
                table: "User",
                column: "email",
                unique: true,
                filter: "([email] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bid");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "DetailFeedback");

            migrationBuilder.DropTable(
                name: "DisputeImage");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ReturnRequest");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "SellerToBuyerReview");

            migrationBuilder.DropTable(
                name: "ShippingInfo");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Dispute");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "OrderTable");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
