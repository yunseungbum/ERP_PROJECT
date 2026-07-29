CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `members` (
    `member_id` bigint NOT NULL AUTO_INCREMENT,
    `member_name` varchar(50) NOT NULL,
    `primary_position` varchar(30) NOT NULL,
    `secondary_position` varchar(30) NULL,
    `phone_number` varchar(20) NOT NULL,
    `birth_year` int NOT NULL,
    `notes` varchar(1000) NOT NULL,
    `is_active` tinyint(1) NOT NULL DEFAULT TRUE,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`member_id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260723050536_InitialMemberSchema', '10.0.7');

CREATE TABLE `users` (
    `user_id` bigint NOT NULL AUTO_INCREMENT,
    `member_id` bigint NULL,
    `login_id` varchar(50) NOT NULL,
    `password_hash` varchar(500) NOT NULL,
    `display_name` varchar(50) NOT NULL,
    `role_code` varchar(30) NOT NULL,
    `is_active` tinyint(1) NOT NULL DEFAULT TRUE,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`user_id`),
    CONSTRAINT `FK_users_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`member_id`) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX `IX_users_login_id` ON `users` (`login_id`);

CREATE UNIQUE INDEX `IX_users_member_id` ON `users` (`member_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260723061041_AddUserAccounts', '10.0.7');

ALTER TABLE `members` ADD `member_status` varchar(20) NOT NULL DEFAULT 'Active';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260724035155_AddMemberStatus', '10.0.7');

CREATE TABLE `match_schedules` (
    `schedule_id` bigint NOT NULL AUTO_INCREMENT,
    `venue_name` varchar(100) NOT NULL,
    `opponent_name` varchar(100) NOT NULL,
    `starts_at` datetime(6) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`schedule_id`)
);

CREATE TABLE `match_participants` (
    `participant_id` bigint NOT NULL AUTO_INCREMENT,
    `schedule_id` bigint NOT NULL,
    `member_id` bigint NULL,
    `guest_name` varchar(50) NULL,
    `is_guest` tinyint(1) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    PRIMARY KEY (`participant_id`),
    CONSTRAINT `FK_match_participants_match_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `match_schedules` (`schedule_id`) ON DELETE CASCADE,
    CONSTRAINT `FK_match_participants_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`member_id`) ON DELETE RESTRICT
);

CREATE TABLE `quarter_formations` (
    `quarter_formation_id` bigint NOT NULL AUTO_INCREMENT,
    `schedule_id` bigint NOT NULL,
    `quarter_number` int NOT NULL,
    `formation_code` varchar(20) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`quarter_formation_id`),
    CONSTRAINT `FK_quarter_formations_match_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `match_schedules` (`schedule_id`) ON DELETE CASCADE
);

CREATE TABLE `quarter_lineup_players` (
    `lineup_player_id` bigint NOT NULL AUTO_INCREMENT,
    `quarter_formation_id` bigint NOT NULL,
    `participant_id` bigint NOT NULL,
    `slot_code` varchar(50) NOT NULL,
    `position_order` int NOT NULL,
    PRIMARY KEY (`lineup_player_id`),
    CONSTRAINT `FK_quarter_lineup_players_match_participants_participant_id` FOREIGN KEY (`participant_id`) REFERENCES `match_participants` (`participant_id`) ON DELETE CASCADE,
    CONSTRAINT `FK_quarter_lineup_players_quarter_formations_quarter_formation_~` FOREIGN KEY (`quarter_formation_id`) REFERENCES `quarter_formations` (`quarter_formation_id`) ON DELETE CASCADE
);

INSERT INTO `match_schedules` (`schedule_id`, `created_at`, `opponent_name`, `starts_at`, `updated_at`, `venue_name`)
VALUES (1, '2026-07-24 00:00:00.000000', '신풍 FC', '2026-08-20 20:00:00.000000', '2026-07-24 00:00:00.000000', '신트리 공원');
SELECT ROW_COUNT();


CREATE INDEX `IX_match_participants_member_id` ON `match_participants` (`member_id`);

CREATE UNIQUE INDEX `IX_match_participants_schedule_id_guest_name` ON `match_participants` (`schedule_id`, `guest_name`);

CREATE UNIQUE INDEX `IX_match_participants_schedule_id_member_id` ON `match_participants` (`schedule_id`, `member_id`);

CREATE UNIQUE INDEX `IX_quarter_formations_schedule_id_quarter_number` ON `quarter_formations` (`schedule_id`, `quarter_number`);

CREATE INDEX `IX_quarter_lineup_players_participant_id` ON `quarter_lineup_players` (`participant_id`);

CREATE UNIQUE INDEX `IX_quarter_lineup_players_quarter_formation_id_participant_id` ON `quarter_lineup_players` (`quarter_formation_id`, `participant_id`);

CREATE UNIQUE INDEX `IX_quarter_lineup_players_quarter_formation_id_slot_code` ON `quarter_lineup_players` (`quarter_formation_id`, `slot_code`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260724064325_AddFormationManagement', '10.0.7');

ALTER TABLE `match_schedules` ADD `is_completed` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `match_schedules` ADD `is_match_fee_paid` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `match_schedules` ADD `match_fee` decimal(12,0) NOT NULL DEFAULT 0.0;

ALTER TABLE `match_schedules` ADD `notes` varchar(1000) NOT NULL DEFAULT '';

ALTER TABLE `match_schedules` ADD `opponent_contact` varchar(30) NULL;

UPDATE `match_schedules` SET `match_fee` = 0.0, `notes` = '', `opponent_contact` = NULL
WHERE `schedule_id` = 1;
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727022306_AddScheduleDetails', '10.0.7');

CREATE TABLE `match_attendances` (
    `attendance_id` bigint NOT NULL AUTO_INCREMENT,
    `schedule_id` bigint NOT NULL,
    `member_id` bigint NOT NULL,
    `status` varchar(1) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`attendance_id`),
    CONSTRAINT `FK_match_attendances_match_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `match_schedules` (`schedule_id`) ON DELETE CASCADE,
    CONSTRAINT `FK_match_attendances_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`member_id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_match_attendances_member_id` ON `match_attendances` (`member_id`);

CREATE UNIQUE INDEX `IX_match_attendances_schedule_id_member_id` ON `match_attendances` (`schedule_id`, `member_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727032352_AddMatchAttendanceOverrides', '10.0.7');

CREATE TABLE `inventory_items` (
    `inventory_item_id` bigint NOT NULL AUTO_INCREMENT,
    `item_name` varchar(100) NOT NULL,
    `quantity` int NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`inventory_item_id`)
);

CREATE TABLE `inventory_purchases` (
    `purchase_id` bigint NOT NULL AUTO_INCREMENT,
    `item_name` varchar(100) NOT NULL,
    `quantity` int NOT NULL,
    `amount` decimal(12,0) NOT NULL,
    `is_purchased` tinyint(1) NOT NULL DEFAULT FALSE,
    `purchased_at` datetime(6) NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`purchase_id`)
);

CREATE UNIQUE INDEX `IX_inventory_items_item_name` ON `inventory_items` (`item_name`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727060545_AddInventoryManagement', '10.0.7');

ALTER TABLE `members` ADD `has_uniform` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `members` ADD `uniform_number` int NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727064001_AddMemberUniformInfo', '10.0.7');

ALTER TABLE `match_schedules` ADD `payer_name` varchar(50) NOT NULL DEFAULT '윤승범';

CREATE TABLE `expenses` (
    `expense_id` bigint NOT NULL AUTO_INCREMENT,
    `schedule_id` bigint NULL,
    `expense_item` varchar(100) NOT NULL,
    `amount` decimal(12,0) NOT NULL,
    `payment_date` datetime(6) NOT NULL,
    `notes` varchar(1000) NOT NULL,
    `payer_name` varchar(50) NOT NULL,
    `is_settled` tinyint(1) NOT NULL DEFAULT FALSE,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`expense_id`),
    CONSTRAINT `FK_expenses_match_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `match_schedules` (`schedule_id`) ON DELETE CASCADE
);

UPDATE `match_schedules` SET `payer_name` = '윤승범'
WHERE `schedule_id` = 1;
SELECT ROW_COUNT();


CREATE UNIQUE INDEX `IX_expenses_schedule_id` ON `expenses` (`schedule_id`);

UPDATE match_schedules
SET payer_name = '윤승범'
WHERE payer_name = '';

INSERT INTO expenses (
    schedule_id,
    expense_item,
    amount,
    payment_date,
    notes,
    payer_name,
    is_settled,
    created_at,
    updated_at
)
SELECT
    schedule_id,
    '구장비',
    match_fee,
    starts_at,
    CONCAT(
        DATE_FORMAT(starts_at, '%Y.%m.%d'),
        ' ',
        venue_name,
        ' ',
        DATE_FORMAT(starts_at, '%H:%i')
    ),
    payer_name,
    is_match_fee_paid,
    created_at,
    updated_at
FROM match_schedules;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727072614_AddExpensesAndSchedulePayer', '10.0.7');

CREATE TABLE `announcements` (
    `announcement_id` bigint NOT NULL AUTO_INCREMENT,
    `title` varchar(100) NOT NULL,
    `content` varchar(1000) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`announcement_id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727074518_AddAnnouncementManagement', '10.0.7');

ALTER TABLE `announcements` ADD `author_name` varchar(50) NOT NULL DEFAULT '회장';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260727075101_AddAnnouncementAuthor', '10.0.7');

CREATE TABLE `member_dues` (
    `member_due_id` bigint NOT NULL AUTO_INCREMENT,
    `member_id` bigint NOT NULL,
    `due_year` int NOT NULL,
    `due_month` int NOT NULL,
    `amount` decimal(12,0) NOT NULL,
    `payment_status` varchar(20) NOT NULL,
    `paid_at` datetime(6) NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`member_due_id`),
    CONSTRAINT `FK_member_dues_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`member_id`) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX `IX_member_dues_member_id_due_year_due_month` ON `member_dues` (`member_id`, `due_year`, `due_month`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260728054331_AddMemberDuesManagement', '10.0.7');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260729015855_AddDuesExecutionAndYearSummary', '10.0.7');

CREATE TABLE `member_due_notes` (
    `member_due_note_id` bigint NOT NULL AUTO_INCREMENT,
    `member_id` bigint NOT NULL,
    `due_year` int NOT NULL,
    `execution_amount` decimal(12,0) NOT NULL DEFAULT 0.0,
    `content` varchar(1000) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`member_due_note_id`),
    CONSTRAINT `FK_member_due_notes_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`member_id`) ON DELETE RESTRICT
);

CREATE TABLE `dues_year_summaries` (
    `dues_year_summary_id` bigint NOT NULL AUTO_INCREMENT,
    `due_year` int NOT NULL,
    `notes` varchar(1000) NOT NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    PRIMARY KEY (`dues_year_summary_id`)
);

CREATE UNIQUE INDEX `IX_dues_year_summaries_due_year` ON `dues_year_summaries` (`due_year`);

CREATE UNIQUE INDEX `IX_member_due_notes_member_id_due_year` ON `member_due_notes` (`member_id`, `due_year`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260729020406_AddDuesExecutionAndYearSummaryV2', '10.0.7');

COMMIT;

