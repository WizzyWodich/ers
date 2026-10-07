using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bell_schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sectors = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bell_schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Course = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Building = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "calendar_days",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsShortened = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedWeekday = table.Column<int>(type: "integer", nullable: true),
                    BellScheduleId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsHoliday = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_calendar_days_bell_schedules_BellScheduleId",
                        column: x => x.BellScheduleId,
                        principalTable: "bell_schedules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "base_schedule_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Weekday = table.Column<int>(type: "integer", nullable: false),
                    PairNumber = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    BellScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_base_schedule_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_base_schedule_entries_bell_schedules_BellScheduleId",
                        column: x => x.BellScheduleId,
                        principalTable: "bell_schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_base_schedule_entries_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_base_schedule_entries_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_base_schedule_entries_subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_base_schedule_entries_teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schedule_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PairNumber = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    NewSubjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewTeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewRoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NewPairNumber = table.Column<int>(type: "integer", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule_overrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_schedule_overrides_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_schedule_overrides_rooms_NewRoomId",
                        column: x => x.NewRoomId,
                        principalTable: "rooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_schedule_overrides_subjects_NewSubjectId",
                        column: x => x.NewSubjectId,
                        principalTable: "subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_schedule_overrides_teachers_NewTeacherId",
                        column: x => x.NewTeacherId,
                        principalTable: "teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_base_schedule_entries_BellScheduleId",
                table: "base_schedule_entries",
                column: "BellScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_base_schedule_entries_GroupId_Weekday_PairNumber_ValidFrom",
                table: "base_schedule_entries",
                columns: new[] { "GroupId", "Weekday", "PairNumber", "ValidFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_base_schedule_entries_RoomId",
                table: "base_schedule_entries",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_base_schedule_entries_SubjectId",
                table: "base_schedule_entries",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_base_schedule_entries_TeacherId",
                table: "base_schedule_entries",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_BellScheduleId",
                table: "calendar_days",
                column: "BellScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_Date",
                table: "calendar_days",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_DisplayName",
                table: "groups",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedule_overrides_GroupId",
                table: "schedule_overrides",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_overrides_NewRoomId",
                table: "schedule_overrides",
                column: "NewRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_overrides_NewSubjectId",
                table: "schedule_overrides",
                column: "NewSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_overrides_NewTeacherId",
                table: "schedule_overrides",
                column: "NewTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_overrides_TargetDate_GroupId_PairNumber",
                table: "schedule_overrides",
                columns: new[] { "TargetDate", "GroupId", "PairNumber" },
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "base_schedule_entries");

            migrationBuilder.DropTable(
                name: "calendar_days");

            migrationBuilder.DropTable(
                name: "schedule_overrides");

            migrationBuilder.DropTable(
                name: "bell_schedules");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "teachers");
        }
    }
}
