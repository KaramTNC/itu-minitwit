using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostgresSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
        ALTER TABLE cheeps
            ALTER COLUMN timestamp TYPE timestamp with time zone USING timestamp::timestamp with time zone,
            ALTER COLUMN peoplelikes TYPE integer[] USING '{}'::integer[];

        ALTER TABLE authors
            ALTER COLUMN follows TYPE integer[] USING '{}'::integer[],
            ALTER COLUMN cheeplikes TYPE integer[] USING '{}'::integer[];

        ALTER TABLE aspnetusers
            ALTER COLUMN emailconfirmed TYPE boolean USING CASE WHEN emailconfirmed = 1 THEN true ELSE false END,
            ALTER COLUMN phonenumberconfirmed TYPE boolean USING CASE WHEN phonenumberconfirmed = 1 THEN true ELSE false END,
            ALTER COLUMN twofactorenabled TYPE boolean USING CASE WHEN twofactorenabled = 1 THEN true ELSE false END,
            ALTER COLUMN lockoutenabled TYPE boolean USING CASE WHEN lockoutenabled = 1 THEN true ELSE false END,
            ALTER COLUMN lockoutend TYPE timestamp with time zone USING lockoutend::text::timestamp with time zone;
    "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aspnetroleclaims_aspnetroles_RoleId",
                table: "aspnetroleclaims"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetuserclaims_AspNetUsers_UserId",
                table: "aspnetuserclaims"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetuserlogins_AspNetUsers_UserId",
                table: "aspnetuserlogins"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetuserroles_AspNetUsers_UserId",
                table: "aspnetuserroles"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetuserroles_aspnetroles_RoleId",
                table: "aspnetuserroles"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetusertokens_AspNetUsers_UserId",
                table: "aspnetusertokens"
            );

            migrationBuilder.DropForeignKey(name: "FK_cheeps_authors_authorid", table: "cheeps");

            migrationBuilder.DropTable(name: "aspnetusers");

            migrationBuilder.DropPrimaryKey(name: "PK_cheeps", table: "cheeps");

            migrationBuilder.DropPrimaryKey(name: "PK_authors", table: "authors");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetusertokens", table: "aspnetusertokens");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetuserroles", table: "aspnetuserroles");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetuserlogins", table: "aspnetuserlogins");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetuserclaims", table: "aspnetuserclaims");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetroles", table: "aspnetroles");

            migrationBuilder.DropPrimaryKey(name: "PK_aspnetroleclaims", table: "aspnetroleclaims");

            migrationBuilder.RenameTable(name: "cheeps", newName: "Cheeps");

            migrationBuilder.RenameTable(name: "authors", newName: "Authors");

            migrationBuilder.RenameTable(name: "aspnetusertokens", newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(name: "aspnetuserroles", newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(name: "aspnetuserlogins", newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(name: "aspnetuserclaims", newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(name: "aspnetroles", newName: "AspNetRoles");

            migrationBuilder.RenameTable(name: "aspnetroleclaims", newName: "AspNetRoleClaims");

            migrationBuilder.RenameColumn(name: "timestamp", table: "Cheeps", newName: "TimeStamp");

            migrationBuilder.RenameColumn(name: "text", table: "Cheeps", newName: "Text");

            migrationBuilder.RenameColumn(
                name: "peoplelikes",
                table: "Cheeps",
                newName: "PeopleLikes"
            );

            migrationBuilder.RenameColumn(name: "authorid", table: "Cheeps", newName: "AuthorId");

            migrationBuilder.RenameColumn(name: "cheepid", table: "Cheeps", newName: "CheepId");

            migrationBuilder.RenameIndex(
                name: "IX_cheeps_authorid",
                table: "Cheeps",
                newName: "IX_Cheeps_AuthorId"
            );

            migrationBuilder.RenameColumn(name: "name", table: "Authors", newName: "Name");

            migrationBuilder.RenameColumn(name: "follows", table: "Authors", newName: "Follows");

            migrationBuilder.RenameColumn(name: "email", table: "Authors", newName: "Email");

            migrationBuilder.RenameColumn(
                name: "cheeplikes",
                table: "Authors",
                newName: "CheepLikes"
            );

            migrationBuilder.RenameColumn(name: "authorid", table: "Authors", newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_aspnetuserroles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_aspnetuserlogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_aspnetuserclaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_aspnetroleclaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "Cheeps",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Cheeps",
                type: "TEXT",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160
            );

            migrationBuilder.AlterColumn<string>(
                name: "PeopleLikes",
                table: "Cheeps",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(List<int>),
                oldType: "integer[]"
            );

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Cheeps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder
                .AlterColumn<int>(
                    name: "CheepId",
                    table: "Cheeps",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "integer"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.AlterColumn<string>(
                name: "Follows",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(List<int>),
                oldType: "integer[]"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.AlterColumn<string>(
                name: "CheepLikes",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(List<int>),
                oldType: "integer[]"
            );

            migrationBuilder
                .AlterColumn<int>(
                    name: "AuthorId",
                    table: "Authors",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "integer"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<int>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder
                .AlterColumn<int>(
                    name: "Id",
                    table: "AspNetUserClaims",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "integer"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder
                .AlterColumn<int>(
                    name: "Id",
                    table: "AspNetRoleClaims",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "integer"
                )
                .OldAnnotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AddPrimaryKey(name: "PK_Cheeps", table: "Cheeps", column: "CheepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "AuthorId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" }
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" }
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" }
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
