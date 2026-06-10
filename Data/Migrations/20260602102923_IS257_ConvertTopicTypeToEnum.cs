using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace szpont.Data.Migrations
{
    /// <inheritdoc />
    public partial class IS257_ConvertTopicTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Topics
                SET Type = CASE Type
                    WHEN 'Licencjacka' THEN '0'
                    WHEN 'Inżynierska' THEN '1'
                    WHEN 'Magisterska' THEN '2'
                    WHEN 'Doktorska' THEN '3'
                    ELSE '0'
                END;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Topics",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Topics",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.Sql("""
                UPDATE Topics
                SET Type = CASE Type
                    WHEN '0' THEN 'Licencjacka'
                    WHEN '1' THEN 'Inżynierska'
                    WHEN '2' THEN 'Magisterska'
                    WHEN '3' THEN 'Doktorska'
                    ELSE 'Licencjacka'
                END;
                """);
        }
    }
}
