USE [BlockChain]
GO

/****** Object:  Table [dbo].[BankList]    Script Date: 2017/5/14 17:32:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BankList](
	[IID] [uniqueidentifier] NULL,
	[BankNo] [nvarchar](50) NULL,
	[BankName] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BankReceipt](
	[IID] [uniqueidentifier] NULL,
	[ReceiptNO] [nvarchar](50) NULL,
	[BillDate] [smalldatetime] NULL,
	[CreateUserId] [nvarchar](50) NULL,
	[FromCompanyId] [nvarchar](50) NULL,
	[FromBankAccount] [nvarchar](50) NULL,
	[ToCompanyId] [nvarchar](50) NULL,
	[ToBankAccount] [nvarchar](50) NULL,
	[Amount] [decimal](27, 2) NULL,
	[AcceptDate] [smalldatetime] NULL,
	[AcceptBankNo] [nvarchar](50) NULL,
	[Protocol] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[ContractAddress] [nvarchar](100) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BankReceipt] ADD  CONSTRAINT [DF_BankReceipt_IID]  DEFAULT (newid()) FOR [IID]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BankReceiptChain](
	[IID] [nvarchar](50) NULL,
	[ReceiptNO] [nvarchar](50) NULL,
	[FromCompanyId] [nvarchar](50) NULL,
	[ToCompanyId] [nvarchar](50) NULL,
	[ToBankAccount] [nvarchar](50) NULL,
	[Amount] [decimal](27, 2) NULL,
	[CreateUserId] [nvarchar](50) NULL,
	[CreateDate] [smalldatetime] NULL,
	[BalanceAmount] [decimal](27, 2) NULL,
	[Status] [nvarchar](50) NULL,
	[UpstreamReceiptId] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyBankAccount]    Script Date: 2017/5/14 17:32:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CompanyBankAccount](
	[CompanyId] [nvarchar](50) NULL,
	[BankAccount] [nvarchar](50) NULL,
	[BankNo] [nvarchar](50) NULL,
	[Protocols] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyList]    Script Date: 2017/5/14 17:32:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CompanyList](
	[IID] [uniqueidentifier] NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[FullName] [nvarchar](50) NULL,
	[CompanyType] [nvarchar](50) NULL,
	[BCUser] [nvarchar](50) NULL,
	[BCPublicKey] [nvarchar](100) NULL
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[FavoriteIndicator]    Script Date: 2017/5/14 17:33:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FavoriteIndicator](
	[IID] [int] NULL,
	[Name] [nvarchar](50) NULL,
	[HtmlPage] [nvarchar](50) NULL,
	[UserId] [nvarchar](50) NULL
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[FavoriteReport]    Script Date: 2017/5/14 17:33:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FavoriteReport](
	[ReportId] [nvarchar](50) NULL,
	[UserId] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[DocumentType] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FinanceCompanyList](
	[IID] [int] NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FinancingReceipt]    Script Date: 2017/5/14 17:33:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FinancingReceipt](
	[IID] [uniqueidentifier] NULL,
	[ReceiptChainId] [nvarchar](50) NULL,
	[BankId] [nvarchar](50) NULL,
	[Discount] [decimal](27, 2) NULL,
	[FinancingValue] [decimal](27, 2) NULL,
	[CreateDate] [smalldatetime] NULL
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FunctionList](
	[IID] [uniqueidentifier] NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](100) NULL,
	[URL] [nvarchar](50) NULL,
	[ShowFunctionTree] [bit] NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FunctionPurview](
	[HolderId] [nvarchar](50) NULL,
	[FunctionId] [nvarchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[HistoryReport](
	[ReportId] [nvarchar](50) NULL,
	[UserId] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NewReportFolder](
	[PKID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](64) NULL,
	[Property] [nvarchar](512) NULL,
	[ParentID] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](64) NULL,
	[IsFavorite] [bit] NULL,
	[SupportDashboard] [bit] NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NewReportList](
	[IID] [uniqueidentifier] NULL,
	[Name] [nvarchar](128) NULL,
	[Prompt] [nvarchar](1024) NULL,
	[Description] [nvarchar](1024) NULL,
	[OpenRefresh] [bit] NULL,
	[ModifyDate] [timestamp] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[FolderId] [nvarchar](100) NULL,
	[ReportFilter] [nvarchar](4000) NULL,
	[ReportType] [nvarchar](50) NULL,
	[AdhocReport] [bit] NULL,
	[HtmlPage] [nvarchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NextContract](
	[ContractAddress] [nvarchar](100) NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RoleList](
	[IID] [uniqueidentifier] NOT NULL,
	[RoleCode] [nvarchar](32) NULL,
	[RoleName] [nvarchar](32) NULL,
	[Description] [nvarchar](128) NULL,
	[CreateDate] [datetime] NULL,
	[IsSystem] [bit] NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserList](
	[IID] [uniqueidentifier] NULL,
	[UserCode] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[PasswordMD5] [nvarchar](50) NULL,
	[PublicKey] [nvarchar](1000) NULL,
	[CompanyId] [nvarchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRoles](
	[UserId] [nvarchar](50) NULL,
	[RoleId] [nvarchar](50) NULL,
	[Access] [timestamp] NULL
) ON [PRIMARY]

GO
